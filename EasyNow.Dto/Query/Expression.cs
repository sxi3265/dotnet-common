using ProtoBuf;

namespace EasyNow.Dto.Query
{
    [ProtoContract]
    public class Expression
    {
        /// <summary>
        /// 表达式
        /// </summary>
        [ProtoMember(1,Name = "expressions")]
        public Expression[] Expressions { get; set; }

        /// <summary>
        /// 条件
        /// </summary>
        [ProtoMember(2,Name = "conditions")]
        public Condition[] Conditions { get; set; }

        /// <summary>
        /// 操作符
        /// </summary>
        [ProtoMember(3,Name = "operator")]
        public EExpressionOperator Operator { get; set; }
    }
}