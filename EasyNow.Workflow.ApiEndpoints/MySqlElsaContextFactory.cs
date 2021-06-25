using System.IO;
using Elsa.Persistence.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace EasyNow.Workflow.ApiEndpoints
{
    public class MySqlElsaContextFactory: IDesignTimeDbContextFactory<ElsaContext>
    {
        public ElsaContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddCommandLine(args)
                .Build();

            var dbContextBuilder = new DbContextOptionsBuilder();
            var connectionString = "xxx";

            dbContextBuilder.UseMySql(connectionString,ServerVersion.AutoDetect(connectionString), mysql =>
            {
                mysql.MigrationsAssembly(typeof(Elsa.Persistence.EntityFramework.MySql.MySqlElsaContextFactory)
                    .Assembly.FullName);
                mysql.SchemaBehavior(MySqlSchemaBehavior.Translate, (schema, entity) => entity);
            });

            return new ElsaContext(dbContextBuilder.Options);
        }
    }
}