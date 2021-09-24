using ProtoBuf;

namespace EasyNow.Collection
{
    [ProtoContract]
    public class GrpcReq<T>
    {
        /// <summary>
        /// 数据
        /// </summary>
        [ProtoMember(1, Name = "data")]
        public T Data { get; set; }
    }
}