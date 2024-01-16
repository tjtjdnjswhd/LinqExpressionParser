using System.Linq.Expressions;

namespace LinqExpressionParser.AspNetCore.Results
{
    public class SelectorParseResult<T>(LambdaExpression selectorExp) : IExpressionParseResult<LambdaExpression>
    {
        public LambdaExpression GetExpression() => selectorExp;

        public IQueryable Select(IQueryable<T> query)
        {
            return query.Provider.CreateQuery(
                Expression.Call(typeof(Queryable), "Select", [query.ElementType, selectorExp.Body.Type], query.Expression, Expression.Quote(selectorExp)));
        }
    }
}