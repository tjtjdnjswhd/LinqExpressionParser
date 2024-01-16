using System.Linq.Expressions;

namespace LinqExpressionParser.AspNetCore.Results
{
    public class ValueParseResult<T, V>(Expression<Func<T, V>> lambdaExp) : IExpressionParseResult<Expression<Func<T, V>>>
    {
        public Expression<Func<T, V>> GetExpression() => lambdaExp;
    }
}