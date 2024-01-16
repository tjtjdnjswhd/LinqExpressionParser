using LinqExpressionParser.Segments.Enums;

using System.Linq.Expressions;

namespace LinqExpressionParser.Expressions.Maps.Operators
{
    public class OperatorMapOptions
    {
        public static OperatorMapOptions Default => new() { OperatorMaps = new(DefaultOperatorMap.DefaultMaps) };

        public delegate Expression? GetOperatorExpressionDelegate(Expression left, Expression right);

        public required Dictionary<EOperator, IEnumerable<GetOperatorExpressionDelegate>> OperatorMaps { get; init; }
    }
}