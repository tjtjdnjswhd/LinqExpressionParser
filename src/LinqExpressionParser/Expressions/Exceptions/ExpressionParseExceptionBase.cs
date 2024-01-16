namespace LinqExpressionParser.Expressions.Exceptions
{
    public abstract class ExpressionParseExceptionBase : Exception
    {
        protected ExpressionParseExceptionBase()
        {
        }

        protected ExpressionParseExceptionBase(string message) : base(message)
        {
        }

        protected ExpressionParseExceptionBase(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}