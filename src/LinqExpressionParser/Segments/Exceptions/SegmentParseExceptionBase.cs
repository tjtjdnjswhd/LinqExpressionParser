namespace LinqExpressionParser.Segments.Exceptions
{
    public abstract class SegmentParseExceptionBase : Exception
    {
        protected SegmentParseExceptionBase()
        {
        }

        protected SegmentParseExceptionBase(string message) : base(message)
        {
        }

        protected SegmentParseExceptionBase(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}