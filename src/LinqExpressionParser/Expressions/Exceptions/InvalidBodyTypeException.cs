using LinqExpressionParser.Segments;

using System.Linq.Expressions;

namespace LinqExpressionParser.Expressions.Exceptions
{
    /// <summary>
    /// The exception that throw if body expression type is invalid
    /// </summary>
    public class InvalidBodyTypeException(Type expectedType, Type actualType, SegmentBase segment, Expression bodyExp) : ExpressionParseExceptionBase($"Invalid body type. expected type: {expectedType}, actual type: {actualType}, body expression: {bodyExp}")
    {
        public Type ExpectedType { get; } = expectedType;
        public Type ActualType { get; } = actualType;
        public SegmentBase Segment { get; } = segment;
        public Expression BodyExp { get; } = bodyExp;
    }
}