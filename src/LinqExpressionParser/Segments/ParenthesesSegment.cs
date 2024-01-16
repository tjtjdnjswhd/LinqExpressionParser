using System.Diagnostics;

namespace LinqExpressionParser.Segments;

[DebuggerDisplay("{Value}")]
public class ParenthesesSegment(ValueSegment value) : ValueSegment
{
    public ValueSegment Value { get; } = value;

    public override bool Equals(SegmentBase? other) => other is ParenthesesSegment p && Value.Equals(p.Value);

    public override int GetHashCode() => Value.GetHashCode();
}