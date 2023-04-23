namespace EasyNow.Dto.Query
{
    /// <summary>
    /// 操作
    /// </summary>
    public enum EOperator
    {
        /// <summary>
        /// 相等
        /// </summary>
        Eq=0,
        /// <summary>
        /// 不等
        /// </summary>
        Neq,
        /// <summary>
        /// 包含
        /// </summary>
        Contain,
        /// <summary>
        /// 不包含
        /// </summary>
        NContain,
        /// <summary>
        /// 包含
        /// </summary>
        In,
        /// <summary>
        /// 不包含
        /// </summary>
        Nin,
        /// <summary>
        /// 大于
        /// </summary>
        Gt,
        /// <summary>
        /// 大于等于
        /// </summary>
        Gte,
        /// <summary>
        /// 小于
        /// </summary>
        Lt,
        /// <summary>
        /// 小于等于
        /// </summary>
        Lte,
        /// <summary>
        /// 为空
        /// </summary>
        Null,
        /// <summary>
        /// 不为空
        /// </summary>
        NNull
    }
}