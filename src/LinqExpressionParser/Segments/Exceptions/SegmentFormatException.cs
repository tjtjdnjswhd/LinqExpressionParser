namespace LinqExpressionParser.Segments.Exceptions
{
    /// <summary>
    /// The exception that throw when segment text format is wrong. see <see cref="Error"/>
    /// </summary>
    public class SegmentFormatException : SegmentParseExceptionBase
    {
        /// <summary>
        /// Error of segment format
        /// </summary>
        public ESegmentFormatError Error { get; }

        /// <summary>
        /// Wrong format text
        /// </summary>
        public string? Text { get; }

        public SegmentFormatException(ESegmentFormatError error, string text) : base($"Wrong segment format. text: {text}, error: {error}")
        {
            Error = error;
            Text = text;
        }

        public SegmentFormatException(ESegmentFormatError error) : base($"Wrong segment format. error: {error}")
        {
            Error = error;
        }
    }

    public enum ESegmentFormatError
    {
        InvalidOperationSegmentCount,
        InvalidContinuous,
        InvalidText,
        InvalidOperatorIndex,
        UnnamedSelectItem
    }
}