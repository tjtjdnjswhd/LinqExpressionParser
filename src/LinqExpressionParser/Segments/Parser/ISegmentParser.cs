using LinqExpressionParser.Segments.Exceptions;

using System.Diagnostics.CodeAnalysis;

namespace LinqExpressionParser.Segments.Parser
{
    public interface ISegmentParser
    {
        /// <exception cref="SegmentFormatException"></exception>
        ValueSegment ParseValue(string text);
        bool TryParseValue(string text, [NotNullWhen(true)] out ValueSegment? result);

        /// <exception cref="SegmentFormatException"></exception>
        SelectorSegment ParseSelector(string text);
        bool TryParseSelector(string text, [NotNullWhen(true)] out SelectorSegment? result);
    }
}