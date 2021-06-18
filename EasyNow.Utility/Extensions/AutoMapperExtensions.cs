using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace EasyNow.Utility.Extensions
{
    public static class AutoMapperExtensions
    {
        public static IMappingExpression<TSource, TDestination> IgnoreAllDestinationVirtual<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            var desType = typeof(TDestination);
            foreach (var property in desType.GetProperties().Where(p => p.GetGetMethod().IsVirtual && !p.GetGetMethod().IsFinal))
            {
                expression.ForMember(property.Name, opt => opt.Ignore());
            }

            return expression;
        }

        public static IMappingExpression<TSource, TDestination> IgnoreAllSourceVirtual<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            var srcType = typeof(TSource);
            foreach (var property in srcType.GetProperties().Where(p => p.GetGetMethod().IsVirtual && !p.GetGetMethod().IsFinal))
            {
                expression.ForSourceMember(property.Name, opt => opt.DoNotValidate());
            }

            return expression;
        }

        public static IMappingExpression IgnoreAllDestinationVirtual(this IMappingExpression expression, Type desType)
        {
            foreach (var property in desType.GetProperties().Where(p => p.GetGetMethod().IsVirtual && !p.GetGetMethod().IsFinal))
            {
                expression.ForMember(property.Name, opt => opt.Ignore());
            }

            return expression;
        }

        public static IMappingExpression IgnoreAllSourceVirtual(this IMappingExpression expression, Type srcType)
        {
            foreach (var property in srcType.GetProperties().Where(p => p.GetGetMethod().IsVirtual && !p.GetGetMethod().IsFinal))
            {
                expression.ForSourceMember(property.Name, opt => opt.DoNotValidate());
            }

            return expression;
        }

        public static IMappingExpression IgnoreVirtual(this IMappingExpression expression, Type srcType, Type desType)
        {
            foreach (var property in desType.GetProperties().Where(p => p.GetGetMethod().IsVirtual && !p.GetGetMethod().IsFinal))
            {
                expression.ForMember(property.Name, opt => opt.Ignore());
            }

            foreach (var property in srcType.GetProperties().Where(p => p.GetGetMethod().IsVirtual && !p.GetGetMethod().IsFinal))
            {
                expression.ForSourceMember(property.Name, opt => opt.DoNotValidate());
            }

            return expression;
        }

        /// <summary>
        /// 注册AutoMapper
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterAutoMapper(this IServiceCollection services,params Assembly[] assemblies)
        {
            var types=assemblies.SelectMany(e =>
            {
                try
                {
                    return e.GetTypes();
                }
                catch
                {
                    return new Type[0];
                }
            }).Where(e => typeof(Profile).GetTypeInfo().IsAssignableFrom(e.GetTypeInfo())).ToArray();
            UtilitySetup.profileTypes = types;
            services.AddSingleton(UtilitySetup.Mapper);
            return services;
        }

        /// <summary>
        /// 注册AutoMapper
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static ContainerBuilder RegisterAutoMapper(this ContainerBuilder builder,params Assembly[] assemblies)
        {
            assemblies = assemblies as Assembly[] ?? assemblies.ToArray();

            var allTypes = assemblies
                .Where(a => !a.IsDynamic && a.GetName().Name != nameof(AutoMapper))
                .Distinct() // avoid AutoMapper.DuplicateTypeMapConfigurationException
                .SelectMany(a => a.DefinedTypes)
                .ToArray();

            var openTypes = new[]
            {
                typeof(IValueResolver<,,>),
                typeof(IMemberValueResolver<,,,>),
                typeof(ITypeConverter<,>),
                typeof(IValueConverter<,>),
                typeof(IMappingAction<,>)
            };
            foreach (var type in openTypes.SelectMany(openType => allTypes
                .Where(t => t.IsClass 
                            && !t.IsAbstract 
                            && ImplementsGenericInterface(t.AsType(),openType))))
            {
                builder.RegisterGeneric(type.AsType());
            }
            builder.RegisterAssemblyTypes(assemblies).AssignableTo(typeof(Profile)).AsSelf().As<Profile>().SingleInstance();

            builder.RegisterAssemblyTypes(assemblies).AssignableTo(typeof(Profile)).AsSelf().As<Profile>().SingleInstance();

            // 注册automapper的所有profile
            builder
                .Register(componentContext => new MapperConfiguration(config =>
                {
                    config.AddProfiles(componentContext.Resolve<IEnumerable<Profile>>());
                }))
                .AsSelf()
                .SingleInstance();

            // 通过MapperConfiguration，注册IMapper
            builder
                .Register(componentContext => componentContext
                    .Resolve<MapperConfiguration>()
                    .CreateMapper(componentContext.Resolve<IComponentContext>().Resolve))
                .As<IMapper>();

            builder.RegisterBuildCallback(container =>
            {
                UtilitySetup.Init(container.Resolve<IMapper>());
            });

            return builder;
        }
        
        private static bool ImplementsGenericInterface(Type type, Type interfaceType)
            => IsGenericType(type, interfaceType) || type.GetTypeInfo().ImplementedInterfaces.Any(@interface => IsGenericType(@interface, interfaceType));
        
        private static bool IsGenericType(Type type, Type genericType)
            => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType;
    }
}
