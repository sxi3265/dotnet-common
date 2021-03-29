using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EasyNow.Utility.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> sequence, bool cond, Expression<Func<T, bool>> predicate)
        {
            return cond ? sequence.Where(predicate) : sequence;
        }

        /// <summary>
        /// 获取最大值，没有则返回空
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static TResult? MaxOrNull<TSource, TResult>(this IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> selector) where TResult:struct
        {
            return source.Max(e => (TResult?) selector.Compile()(e));
        }

        /// <summary>
        /// 获取最小值，没有则返回空
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static TResult? MinOrNull<TSource, TResult>(this IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> selector) where TResult:struct
        {
            return source.Min(e => (TResult?) selector.Compile()(e));
        }
    }
}