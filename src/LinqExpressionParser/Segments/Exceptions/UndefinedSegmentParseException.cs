namespace LinqExpressionParser.Segments.Exceptions
{
    public class UndefinedSegmentParseException(Exception innerException) : SegmentParseExceptionBase($"Exception throwed while parsing. see {nameof(InnerException)}", innerException)
    {
    }
}