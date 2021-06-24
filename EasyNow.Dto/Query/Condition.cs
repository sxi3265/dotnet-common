using ProtoBuf;

namespace EasyNow.Dto.Query
{
    /// <summary>
    /// 条件
    /// </summary>
    [ProtoContract]
    public class Condition
    {
        /// <summary>
        /// 名称
        /// </summary>
        [ProtoMember(1,Name = "name")]
        public string Name { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        [ProtoMember(2,Name="value")]
        public object Value { get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        [ProtoMember(3,Name="operator")]
        public EOperator Operator { get; set; }
    }
}