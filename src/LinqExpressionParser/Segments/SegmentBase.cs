namespace LinqExpressionParser.Segments;

public abstract class SegmentBase : IEquatable<SegmentBase>
{
    public abstract bool Equals(SegmentBase? other);
    public override bool Equals(object? obj) => Equals(obj as SegmentBase);
    public abstract override int GetHashCode();
}