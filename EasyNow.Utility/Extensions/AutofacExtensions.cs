using System;
using System.Linq;
using System.Reflection;
using AspectCore.Configuration;
using AspectCore.Extensions.Autofac;
using Autofac;
using Autofac.Core.Lifetime;
using EasyCaching.Core.Configurations;
using EasyCaching.Core.Interceptor;
using EasyCaching.Interceptor.AspectCore;
using Microsoft.Extensions.Options;

namespace EasyNow.Utility.Extensions
{
    public static class AutofacExtensions
    {
        /// <summary>
        /// Add the AspectCore interceptor.
        /// </summary>
        public static void AddEasyCachingAspectCoreInterceptor(this ContainerBuilder builder, Action<EasyCachingInterceptorOptions> action)
        {
            builder.RegisterType<DefaultEasyCachingKeyGenerator>().As<IEasyCachingKeyGenerator>();

            builder.RegisterType<EasyCachingInterceptor>();

            var config = new EasyCachingInterceptorOptions();

            action(config);

            var options = Options.Create(config);

            builder.Register(x => options);

            builder.RegisterDynamicProxy(configure =>
            {
                bool all(MethodInfo x) => x.CustomAttributes.Any(data => typeof(EasyCachingInterceptorAttribute).GetTypeInfo().IsAssignableFrom(data.AttributeType));

                configure.Interceptors.AddTyped<EasyCachingInterceptor>(all);
            });
        }

        public static ILifetimeScope GetRootScope(this ILifetimeScope lifetimeScope)
        {
            if(lifetimeScope is LifetimeScope scope)
            {
                return scope.RootLifetimeScope;
            }

            throw new NotImplementedException("未实现寻找root的方法");
        }

        /// <summary>
        /// 注册配置项
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <typeparam name="TOptions"></typeparam>
        /// <returns></returns>
        public static ContainerBuilder Configure<TOptions>(this ContainerBuilder builder,string name,Action<TOptions> action) where TOptions : class
        {
            return builder.Configure<TOptions>(name, (_, options) => action(options));
        }
        
        /// <summary>
        /// 注册配置项
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="action"></param>
        /// <typeparam name="TOptions"></typeparam>
        /// <returns></returns>
        public static ContainerBuilder Configure<TOptions>(this ContainerBuilder builder,Action<TOptions> action) where TOptions : class
        {
            return builder.Configure(string.Empty, action);
        }
        
        /// <summary>
        /// 注册配置项
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="action"></param>
        /// <typeparam name="TOptions"></typeparam>
        /// <returns></returns>
        public static ContainerBuilder Configure<TOptions>(this ContainerBuilder builder,Action<ILifetimeScope,TOptions> action) where TOptions : class
        {
            return builder.Configure(string.Empty, action);
        }
        
        /// <summary>
        /// 注册配置项
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <typeparam name="TOptions"></typeparam>
        /// <returns></returns>
        public static ContainerBuilder Configure<TOptions>(this ContainerBuilder builder,string name,Action<ILifetimeScope,TOptions> action) where TOptions : class
        {
            builder.Register(componentContext =>
                {
                    var serviceFactory = componentContext.Resolve<ILifetimeScope>();
                    return new Microsoft.Extensions.Options.ConfigureNamedOptions<TOptions>(name, options =>
                    {
                        using (var newServiceFactory = serviceFactory.BeginLifetimeScope())
                        {
                            action(newServiceFactory, options);
                        }
                    });
                })
                .As<IConfigureOptions<TOptions>>().SingleInstance();
            return builder;
        }
    }
}