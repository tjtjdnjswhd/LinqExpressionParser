using LinqExpressionParser.Segments;

namespace LinqExpressionParser.Expressions.Exceptions
{
    /// <summary>
    /// The exception that throw when segments is invalid
    /// </summary>
    public class InvalidOperationSegmentsException(IEnumerable<SegmentBase> segments, EInvalidSegmentError error) : ExpressionParseExceptionBase($"Can not parse invalid segments. erorr: {error}")
    {
        public IEnumerable<SegmentBase> Segments { get; } = segments;
        public EInvalidSegmentError Error { get; } = error;
    }

    public enum EInvalidSegmentError
    {
        WrongOperatorCount,
        WrongOperandCount,
        WrongSegmentType
    }
}
