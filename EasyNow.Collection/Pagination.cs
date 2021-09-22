using ProtoBuf;

namespace EasyNow.Collection
{
    /// <summary>
    /// 分页
    /// </summary>
    [ProtoContract]
    public class Pagination:IPagination
    {
        private int _pageNumber = 1;
        private int _pageSize = 20;

        /// <summary>
        /// 页码
        /// </summary>
        /// <remarks>页码从1开始，不允许小于1</remarks>
        [ProtoMember(1,Name = "pageNumber")]
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        /// <summary>
        /// 分页大小
        /// </summary>
        /// <remarks>分页大小不允许小于1</remarks>
        [ProtoMember(2,Name = "pageSize")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 ? 1 : value;
        }
    }
}