namespace LinqExpressionParser.Expressions.Exceptions
{
    public class UndefinedExpressionParseException(Exception innerException) : ExpressionParseExceptionBase($"Exception thrown while parsing. see {nameof(InnerException)}", innerException)
    {
    }
}