namespace LinqExpressionParser.Expressions.Exceptions
{
    public class MethodNotMappedException : ExpressionParseExceptionBase
    {
        public string MethodName { get; }
        public IEnumerable<Type>? ArgumentTypes { get; }

        public MethodNotMappedException(string methodName) : base($"Method not mapped. method: {methodName}")
        {
            MethodName = methodName;
        }

        public MethodNotMappedException(string methodName, IEnumerable<Type> argumentTypes) : base($"Method not mapped. method: {methodName}, argument types: {string.Join(", ", argumentTypes.Select(a => a.ToString()))}")
        {
            MethodName = methodName;
            ArgumentTypes = argumentTypes;
        }
    }
}