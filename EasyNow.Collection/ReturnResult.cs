namespace EasyNow.Collection
{
    /// <summary>
    /// 返回结果
    /// </summary>
    /// <typeparam name="TCode"></typeparam>
    public class ReturnResult<TCode>
    {
        /// <summary>
        /// 结果
        /// </summary>
        public TCode Code { get; set; }
        
        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; set; }
    }

    /// <summary>
    /// 带数据返回结果
    /// </summary>
    /// <typeparam name="TCode"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public class ReturnResult<TCode, TData> : ReturnResult<TCode>
    {
        /// <summary>
        /// 数据
        /// </summary>
        public TData Data { get; set; }
    }
}