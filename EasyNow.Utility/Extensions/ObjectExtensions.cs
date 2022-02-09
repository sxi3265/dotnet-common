using System;
using AutoMapper;
using EasyNow.Utility.Tools;
using Newtonsoft.Json;

namespace EasyNow.Utility.Extensions
{
    /// <summary>
    /// Object扩展方法
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// 对象转换为json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="jsonSerializerSettings"></param>
        /// <returns></returns>
        public static string ToJson<T>(this T obj, object jsonSerializerSettings)
        {
            if (jsonSerializerSettings != null && jsonSerializerSettings is JsonSerializerSettings settings)
            {
                return JsonConvert.SerializeObject(obj, settings);
            }

            return JsonConvert.SerializeObject(obj, JsonTool.JsonSerializerSettings);
        }

        /// <summary>
        /// 对象转换为json字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson<T>(this T obj)
        {
            return obj.ToJson(null);
        }

        /// <summary>
        /// 转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="opts"></param>
        /// <returns></returns>
        public static T To<T>(this object source,Action<IMappingOperationOptions> opts)
        {
            if (source == null)
            {
                return default;
            }
            return opts==null?UtilitySetup.Mapper.Map<T>(source):UtilitySetup.Mapper.Map<T>(source,opts);
        }

        /// <summary>
        /// 转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T To<T>(this object source)
        {
            return source.To<T>(null);
        }

        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        /// <param name="opts"></param>
        /// <returns></returns>
        public static object To(this object source,Type sourceType, Type targetType,Action<IMappingOperationOptions> opts)
        {
            return opts == null
                ? UtilitySetup.Mapper.Map(source, sourceType, targetType)
                : UtilitySetup.Mapper.Map(source, sourceType, targetType, opts);
        }

        /// <summary>
        /// 判断是否可以转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool CanTo<T>(this object source)
        {
            return source.CanTo<T>(source.GetType());
        }

        /// <summary>
        /// 判断是否可以转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public static bool CanTo<T>(this object source, Type sourceType)
        {
            return source.CanTo(sourceType, typeof(T));
        }

        /// <summary>
        /// 判断是否可以转换
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static bool CanTo(this object source, Type targetType)
        {
            return source.CanTo(source.GetType(), targetType);
        }

        /// <summary>
        /// 判断是否可以转换
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static bool CanTo(this object source, Type sourceType, Type targetType)
        {
            if (UtilitySetup.Mapper.ConfigurationProvider.BuildExecutionPlan(sourceType, targetType) == null)
            {
                if (sourceType == targetType || targetType.IsInstanceOfType(source))
                {
                    return true;
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object To(this object source,Type sourceType, Type targetType)
        {
            return source.To(sourceType, targetType, null);
        }

        /// <summary>
        /// 转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object To<T>(this T source,Type targetType)
        {
            return source.To(typeof(T), targetType, null);
        }

        /// <summary>
        /// 转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="targetType"></param>
        /// <param name="opts"></param>
        /// <returns></returns>
        public static object To<T>(this T source,Type targetType,Action<IMappingOperationOptions> opts)
        {
            return source.To(typeof(T), targetType, opts);
        }

        /// <summary>
        /// 转换
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <param name="opts"></param>
        /// <returns></returns>
        public static TDestination To<TSource,TDestination>(this TSource source,Action<IMappingOperationOptions<TSource,TDestination>> opts=null)
        {
            return opts==null?UtilitySetup.Mapper.Map<TSource,TDestination>(source):UtilitySetup.Mapper.Map(source,opts);
        }

        /// <summary>
        /// 数据转换
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="opts"></param>
        /// <returns></returns>
        public static TDestination CopyTo<TSource,TDestination>(this TSource source,TDestination destination,Action<IMappingOperationOptions<TSource,TDestination>> opts=null)
        {
            return opts==null?UtilitySetup.Mapper.Map(source,destination):UtilitySetup.Mapper.Map(source,destination,opts);
        }

        /// <summary>
        /// 从目标转换数据
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="destination"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TDestination CopyFrom<TSource,TDestination>(this TDestination destination, TSource source)
        {
            return source.CopyTo(destination);
        }
    }
}