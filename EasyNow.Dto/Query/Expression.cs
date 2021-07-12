namespace EasyNow.Dto.Query
{
    public class Expression
    {
        /// <summary>
        /// 表达式
        /// </summary>
        public Expression[] Expressions { get; set; }

        /// <summary>
        /// 条件
        /// </summary>
        public Condition[] Conditions { get; set; }

        /// <summary>
        /// 操作符
        /// </summary>
        public EExpressionOperator Operator { get; set; }
    }
}