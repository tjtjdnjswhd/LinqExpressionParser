using System.Linq.Expressions;

namespace LinqExpressionParser.Expressions.Exceptions
{
    /// <summary>
    /// The exception that throw when prefix not have child
    /// </summary>
    public class PrefixChildNotExistException(string prefix, ParameterExpression parameterExp) : ExpressionParseExceptionBase($"Prefix has no child property. prefix: {prefix}, parameter type: {parameterExp.Type}")
    {
        public string Prefix { get; } = prefix;
        public ParameterExpression ParameterExp { get; } = parameterExp;
    }
}