namespace EasyNow.Dto
{
    /// <summary>
    /// 软删
    /// </summary>
    public interface ISoftDeleteDto : IDto
    {
        /// <summary>
        /// 已删除
        /// </summary>
        bool IsDeleted{ get; set; }
    }
}