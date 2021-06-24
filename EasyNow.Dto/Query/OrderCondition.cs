using ProtoBuf;

namespace EasyNow.Dto.Query
{
    /// <summary>
    /// 排序条件
    /// </summary>
    [ProtoContract]
    public class OrderCondition
    {
        /// <summary>
        /// 名称
        /// </summary>
        [ProtoMember(1,Name = "name")]
        public string Name { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [ProtoMember(2,Name = "order")]
        public EOrder Order { get; set; }
    }
}