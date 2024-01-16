using LinqExpressionParser.Segments.Enums;

using System.Diagnostics;

namespace LinqExpressionParser.Segments;

[DebuggerDisplay("{Value}")]
public class ConstantSegment : ValueSegment
{
    public object? Value { get; }
    public EConstantType ConstantType { get; }

    public ConstantSegment(int value)
    {
        Value = value;
        ConstantType = EConstantType.Int;
    }

    public ConstantSegment(double value)
    {
        Value = value;
        ConstantType = EConstantType.Double;
    }

    public ConstantSegment(string value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        Value = value;
        ConstantType = EConstantType.String;
    }

    public ConstantSegment(bool value)
    {
        Value = value;
        ConstantType = EConstantType.Bool;
    }

    public ConstantSegment()
    {
        Value = null;
        ConstantType = EConstantType.Null;
    }

    public override bool Equals(SegmentBase? other)
    {
        if (other is not ConstantSegment c || ConstantType != c.ConstantType)
        {
            return false;
        }

        return ConstantType switch
        {
            EConstantType.String => (string)Value! == (string)c.Value!,
            EConstantType.Int => (int)Value! == (int)c.Value!,
            EConstantType.Double => (double)Value! == (double)c.Value!,
            EConstantType.Bool => (bool)Value! == (bool)c.Value!,
            EConstantType.Null => Value is null && c.Value is null,
            _ => throw new NotImplementedException(),
        };
    }

    public override int GetHashCode() => HashCode.Combine(Value);

    public override string ToString() => Value?.ToString() ?? string.Empty;
}