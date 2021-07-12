using ProtoBuf;

namespace EasyNow.Dto.Query
{
    /// <summary>
    /// 查询Dto
    /// </summary>
    [ProtoContract]
    public class QueryDto:IPagination
    {
        /// <inheritdoc />
        [ProtoMember(1,Name = "pageNumber")]
        public int PageNumber { get; set; }

        /// <inheritdoc />
        [ProtoMember(2,Name = "pageSize")]
        public int PageSize { get; set; }

        /// <summary>
        /// 表达式
        /// </summary>
        [ProtoMember(3,Name = "expression")]
        public Expression Expression { get; set; }

        /// <summary>
        /// 排序条件
        /// </summary>
        [ProtoMember(4,Name = "orders")]
        public OrderCondition[] Orders { get; set; }
    }
}