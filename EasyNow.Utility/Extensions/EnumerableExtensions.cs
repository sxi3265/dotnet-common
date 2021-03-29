using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace EasyNow.Utility.Extensions
{
    /// <summary>
    /// The enumerable extensions.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// the Foreach
        /// </summary>
        /// <param name="input">the input</param>
        /// <param name="action">the action</param>
        /// <typeparam name="T">the T</typeparam>
        public static void Foreach<T>(this IEnumerable<T> input, Action<T> action)
        {
            foreach (var item in input)
            {
                action(item);
            }
        }

        public static string Join<T>(this IEnumerable<T> source, string separator)
        {
            return string.Join(separator, source);
        }

        public static string JoinStrings<TItem>(this IEnumerable<TItem> sequence, string separator)
        {
            return sequence.JoinStrings(separator, item => item.ToString());
        }

        public static string JoinStrings<TItem>(this IEnumerable<TItem> sequence, string separator, Func<TItem, string> converter)
        {
            StringBuilder seed = new StringBuilder();
            sequence.Aggregate(seed, (builder, item) =>
                {
                    if (builder.Length > 0)
                        builder.Append(separator);
                    builder.Append(converter(item));
                    return builder;
                });
            return seed.ToString();
        }

        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> enumerable, bool cond,
                                                Expression<Func<T, bool>> predicate)
        {
            return cond ? enumerable.Where(predicate.Compile()) : enumerable;
        }
    }
}