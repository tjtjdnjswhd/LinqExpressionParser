using LinqExpressionParser.Segments;

using System.Linq.Expressions;

namespace LinqExpressionParser.Expressions.Parser
{
    public interface IExpressionParser
    {
        Expression<Func<T, V>> ParseValueExpression<T, V>(ValueSegment value);
        LambdaExpression ParseSelectorExpression<T>(SelectorSegment selector);
    }
}