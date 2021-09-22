namespace EasyNow.Collection
{
    /// <summary>
    /// 分页接口
    /// </summary>
    public interface IPagination
    {
        /// <summary>
        /// 页码
        /// </summary>
        int PageNumber { get; set; }
        /// <summary>
        /// 分页大小
        /// </summary>
        int PageSize { get; set; }
    }
}