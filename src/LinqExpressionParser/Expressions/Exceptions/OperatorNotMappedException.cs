using LinqExpressionParser.Segments.Enums;

using System.Diagnostics.CodeAnalysis;

namespace LinqExpressionParser.Expressions.Exceptions
{
    public class OperatorNotMappedException : ExpressionParseExceptionBase
    {
        public EOperator Operator { get; }

        [NotNullIfNotNull(nameof(RightType))]
        public Type? LeftType { get; }

        [NotNullIfNotNull(nameof(LeftType))]
        public Type? RightType { get; }

        public OperatorNotMappedException(EOperator @operator) : base($"Operator not mapped. operator: {@operator}")
        {
            Operator = @operator;
        }

        public OperatorNotMappedException(EOperator @operator, Type leftType, Type rightType) : base($"Operator not mapped. operator: {@operator}, left type: {leftType}, right type: {rightType}")
        {
            Operator = @operator;
            LeftType = leftType;
            RightType = rightType;
        }
    }
}