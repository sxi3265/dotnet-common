using System.Linq;
using EasyNow.Utility.Extensions;

namespace EasyNow.Collection.Extensions
{
    public static class QueryableExtensions
    {
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> allItems, int? pageNumber, int pageSize)
        {
            var truePageNumber = pageNumber ?? 1;
            var itemIndex = (truePageNumber - 1) * pageSize;
            var pageOfItems = allItems.Skip(itemIndex).Take(pageSize);
            return new PagedList<T>(pageOfItems.ToArray(), new Pagination{PageNumber = truePageNumber, PageSize = pageSize}, allItems.Count());
        }

        public static PagedList<T> ToPagedList<T>(this IQueryable<T> allItems, IPagination pagination)
        {
            var itemIndex = (pagination.PageNumber - 1) * pagination.PageSize;
            var pageOfItems = allItems.Skip(itemIndex).Take(pagination.PageSize);
            return new PagedList<T>(pageOfItems.ToArray(), pagination, allItems.Count());
        }

        /// <summary>
        /// 分页数据查询,并转换数据类型
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="allItems"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public static PagedList<TDestination> ToPagedList<TSource,TDestination>(this IQueryable<TSource> allItems, IPagination pagination)
        {
            var itemIndex = (pagination.PageNumber - 1) * pagination.PageSize;
            var count = allItems.Count();
            var pageOfItems = (itemIndex > 0 ? allItems.Skip(itemIndex) : allItems).Take(pagination.PageSize).AsEnumerable().Select(e=>e.To<TDestination>()).ToArray();
            return new PagedList<TDestination>(pageOfItems, pagination,
                count);
        }
    }
}