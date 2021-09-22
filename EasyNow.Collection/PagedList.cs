using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ProtoBuf;

namespace EasyNow.Collection
{
    [ProtoContract]
    public class PagedList<T>:IPagination
    {
        [JsonConstructor]
        public PagedList(int pageNumber,int pageSize,IEnumerable<T> items,int totalItemCount):this(items,new Pagination{PageNumber = pageNumber,PageSize = pageSize},totalItemCount)
        {
            
        }
        
        public PagedList(IQueryable<T> queryable, IPagination page) : this(page, queryable?.Count() ?? 0)
        {
            if (TotalItemCount > 0)
            {
                this.Items=(page.PageNumber == 1 ? queryable.Take(page.PageSize) : queryable.Skip((page.PageNumber - 1) * page.PageSize).Take(page.PageSize)).ToArray();
            }
        }

        public PagedList(IPagination page, int totalItemCount)
        {
            TotalItemCount = totalItemCount;
            PageSize = page.PageSize;
            PageNumber = page.PageNumber;
        }

        public PagedList(IEnumerable<T> enumerable, IPagination page) : this(enumerable.AsQueryable<T>(), page)
        {

        }

        public PagedList(IEnumerable<T> enumerable, IPagination page, int totalItemCount) : this(page, totalItemCount)
        {
            if (enumerable.Any())
            {
                this.Items=enumerable.ToArray();
            }
        }

        public PagedList(PagedList<T> pagedList)
        {
            TotalItemCount = pagedList.TotalItemCount;
            PageSize = pagedList.PageSize;
            PageNumber = pagedList.PageNumber;

            this.Items=pagedList.Items;
        }

        /// <summary>
        /// 项
        /// </summary>
        [ProtoMember(4, Name = "items")]
        public IReadOnlyCollection<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount => this.TotalItemCount > 0 ? (int) Math.Ceiling(TotalItemCount / (double) PageSize) : 0;

        /// <summary>
        /// 总对象数
        /// </summary>
        [ProtoMember(1,Name = "totalItemCount")]
        public int TotalItemCount { get; protected set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        [ProtoMember(2,Name = "pageNumber")]
        public int PageNumber { get; set; }

        /// <summary>
        /// 页面最大对象数
        /// </summary>
        [ProtoMember(3,Name = "pageSize")]
        public int PageSize { get; set; }

        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool HasNextPage => PageNumber < PageCount;

        /// <summary>
        /// 是否第一页
        /// </summary>
        public bool IsFirstPage => PageNumber == 1;

        /// <summary>
        /// 是否最后一页
        /// </summary>
        public bool IsLastPage => PageNumber == PageCount;

        /// <summary>
        /// 当前页第一项所处索引
        /// </summary>
        public int FirstItemOnPage => (PageNumber - 1) * PageSize + 1;

        /// <summary>
        /// 当前页总数
        /// </summary>
        public int Count => Items.Count;

        /// <summary>
        /// 当前页最后一项所处索引
        /// </summary>
        public int LastItemOnPage
        {
            get
            {
                var numberOfLastItemOnPage = FirstItemOnPage + PageSize - 1;
                return numberOfLastItemOnPage > TotalItemCount ? TotalItemCount : numberOfLastItemOnPage;
            }
        }
    }
}
