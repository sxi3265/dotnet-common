using ProtoBuf;

[assembly: CompatibilityLevel(CompatibilityLevel.Level300)]
namespace EasyNow.Collection
{
    /// <summary>
    /// GrpcResult
    /// </summary>
    [ProtoContract]
    public class GrpcResult
    {
        /// <summary>
        /// Code
        /// </summary>
        [ProtoMember(1,Name = "code")]
        public bool Code { get; set; }

        /// <summary>
        /// Msg
        /// </summary>
        [ProtoMember(2,Name = "msg")]
        public string Msg { get; set; }
    }

    /// <summary>
    /// GrpcResult
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    [ProtoContract]
    public class GrpcResult<TData> : GrpcResult
    {
        /// <summary>
        /// Code
        /// </summary>
        [ProtoMember(1,Name = "code")]
        public new bool Code
        {
            get => base.Code;
            set => base.Code = value;
        }

        /// <summary>
        /// Msg
        /// </summary>
        [ProtoMember(2,Name = "msg")]
        public new string Msg
        {
            get => base.Msg;
            set => base.Msg = value;
        }

        /// <summary>
        /// Data
        /// </summary>
        [ProtoMember(3,Name = "data")]
        public TData Data { get; set; }
    }
}