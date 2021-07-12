using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using EasyNow.Workflow.Mqtt;
using Elsa;
using Elsa.Attributes;
using Elsa.Caching.Rebus.Extensions;
using Elsa.Extensions;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Rebus.RabbitMq;
using Hangfire;
using Hangfire.Storage.MySql;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Rebus.Config;

namespace EasyNow.Workflow.ApiEndpoints
{
    public class Startup
    {
        private IWebHostEnvironment Environment { get; }
        private IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            var elsaSection = Configuration.GetSection("Elsa");
            services.AddRedis("103.127.126.204:30003,password=KwVRqDFTjZ7k60iT,abortConnect=false");
            // Elsa services.
            services
                .AddElsa(elsa => elsa
                    .UseRabbitMq("amqp://admin:tDVpZs4fEEwabekn@103.127.126.204:30002/")
                    .UseServiceBus(context =>
                    {
                        context.Configurer.Transport(t => t.UseRabbitMq("amqp://admin:tDVpZs4fEEwabekn@103.127.126.204:30002/", "rebus"));
                    })
                    .UseRebusCacheSignal()
                    .UseEntityFrameworkPersistence(ef => ef.UseMySql(Configuration.GetConnectionString("Workflow"),
                        ServerVersion.AutoDetect(Configuration.GetConnectionString("Workflow"))))
                    .ConfigureDistributedLockProvider(options=>options.UseRedisLockProvider(string.Empty))
                    .AddConsoleActivities()
                    .AddHttpActivities(elsaSection.GetSection("Server").Bind)
                    .AddHangfireTemporalActivities(hangfire =>
                    {
                        hangfire.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                            .UseSimpleAssemblyNameTypeSerializer().UseRecommendedSerializerSettings();
                        var storage = new MySqlStorage(Configuration.GetConnectionString("Hangfire"),
                            new MySqlStorageOptions
                            {
                                TransactionIsolationLevel = IsolationLevel.ReadCommitted,
                                QueuePollInterval = TimeSpan.FromSeconds(15),
                                JobExpirationCheckInterval = TimeSpan.FromHours(1),
                                CountersAggregateInterval = TimeSpan.FromMinutes(5),
                                PrepareSchemaIfNecessary = true,
                                DashboardJobListLimit = 50000,
                                TransactionTimeout = TimeSpan.FromMinutes(1)
                            });
                        hangfire.UseStorage(storage);
                    })
                    .AddJavaScriptActivities()
                    .AddRebusActivities()
                    .AddMqttActivities()
                    .AddWorkflowsFrom<Startup>()
                );

            // Elsa API endpoints.
            services.AddElsaApiEndpoints();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.All;
            });

            // Allow arbitrary client browser apps to access the API.
            // In a production environment, make sure to allow only origins you trust.
            services.AddCors(cors => cors.AddDefaultPolicy(policy => policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .WithExposedHeaders("Content-Disposition"))
            );
            // For Dashboard.
            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseForwardedHeaders()
                .UseStaticFiles()
                .UseCors()
                .UseHttpActivities()
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    // Elsa API Endpoints are implemented as regular ASP.NET Core API controllers.
                    endpoints.MapControllers();

                    // For Dashboard.
                    endpoints.MapFallbackToPage("/_Host");
                });
        }
    }
}
