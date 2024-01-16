using System.Linq.Expressions;

namespace LinqExpressionParser.Expressions.Exceptions
{
    /// <summary>
    /// The exception that throw if prefix duplicated./>
    /// </summary>
    public class PrefixDuplicateException(string prefix, ParameterExpression firstParameter, ParameterExpression secondParameter) : ExpressionParseExceptionBase($"Prefix duplicated. prefix: {prefix}, first parameter type: {firstParameter.Type}, second parameter type: {secondParameter.Type}")
    {
        public string Prefix { get; } = prefix;
        public ParameterExpression FirstParameter { get; } = firstParameter;
        public ParameterExpression SecondParameter { get; } = secondParameter;
    }
}