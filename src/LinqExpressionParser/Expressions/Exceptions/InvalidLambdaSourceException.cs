using LinqExpressionParser.Segments;

using System.Linq.Expressions;

namespace LinqExpressionParser.Expressions.Exceptions
{
    /// <summary>
    /// The exception that throw when source is not derived <see cref="IEnumerable{T}"/>
    /// </summary>
    public class InvalidLambdaSourceException(Expression sourceExp, LambdaParameterDeclaringSegment parameterSegment) : ExpressionParseExceptionBase($"Source is not derived of IEmumerable<>. source: {sourceExp}")
    {
        public Expression SourceExp { get; } = sourceExp;
        public LambdaParameterDeclaringSegment ParameterSegment { get; } = parameterSegment;
    }
}