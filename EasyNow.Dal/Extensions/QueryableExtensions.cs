using System.Linq;
using System.Threading.Tasks;
using EasyNow.Dto;
using EasyNow.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace EasyNow.Dal.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PagedList<TDestination>> ToPagedListAsync<TSource,TDestination>(this IQueryable<TSource> allItems, IPagination pagination)
        {
            var itemIndex = (pagination.PageNumber - 1) * pagination.PageSize;
            if (allItems.Provider is EntityQueryProvider)
            {
                return new PagedList<TDestination>(
                    (await (itemIndex > 0 ? allItems.Skip(itemIndex) : allItems).Take(pagination.PageSize)
                        .ToArrayAsync()).Select(e => e.To<TDestination>()).ToArray(), pagination,
                    await allItems.CountAsync());
            }

            var count = allItems.Count();
            var pageOfItems = (itemIndex > 0 ? allItems.Skip(itemIndex) : allItems).Take(pagination.PageSize).AsEnumerable().Select(e=>e.To<TDestination>()).ToArray();
            return new PagedList<TDestination>(pageOfItems, pagination,
                count);
        }
    }
}