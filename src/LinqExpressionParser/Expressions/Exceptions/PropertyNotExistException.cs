namespace LinqExpressionParser.Expressions.Exceptions
{
    public class PropertyNotExistException(Type type, string propertyName) : ExpressionParseExceptionBase($"{type} not have property '{propertyName}'")
    {
        public Type Type { get; } = type;
        public string PropertyName { get; } = propertyName;
    }
}