using System.Linq.Expressions;

namespace LinqExpressionParser.AspNetCore.Results
{
    public interface IExpressionParseResult<out TExpression> where TExpression : Expression
    {
        public TExpression GetExpression();
    }
}