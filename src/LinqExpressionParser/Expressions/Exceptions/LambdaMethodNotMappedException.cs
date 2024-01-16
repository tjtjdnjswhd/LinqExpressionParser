namespace LinqExpressionParser.Expressions.Exceptions
{
    public class LambdaMethodNotMappedException : MethodNotMappedException
    {
        public LambdaMethodNotMappedException(string methodName) : base(methodName)
        {
        }

        public LambdaMethodNotMappedException(string methodName, IEnumerable<Type> argumentTypes) : base(methodName, argumentTypes)
        {
        }
    }
}