using LinqExpressionParser.Expressions.Maps.Methods;
using LinqExpressionParser.Expressions.Maps.Operators;

namespace LinqExpressionParser.Expressions
{
    public class ExpressionParserOptions
    {
        public required MethodMapOptions MethodMapOptions { get; set; }
        public required OperatorMapOptions OperatorMapOptions { get; set; }
    }
}