using ProtoBuf;

namespace EasyNow.Dto.Query
{
    /// <summary>
    /// 查询所有Dto
    /// </summary>
    [ProtoContract]
    public class QueryAllDto
    {
        /// <summary>
        /// 表达式
        /// </summary>
        [ProtoMember(1,Name = "expression")]
        public Expression Expression { get; set; }

        /// <summary>
        /// 排序条件
        /// </summary>
        [ProtoMember(2,Name = "orders")]
        public OrderCondition[] Orders { get; set; }
    }
}