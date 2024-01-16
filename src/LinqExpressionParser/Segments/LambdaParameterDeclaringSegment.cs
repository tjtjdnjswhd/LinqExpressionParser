using System.Diagnostics;

namespace LinqExpressionParser.Segments
{
    [DebuggerDisplay("{Prefix}.{Property}")]
    public class LambdaParameterDeclaringSegment(string prefix, PropertySegment property) : ValueSegment
    {
        public string Prefix { get; } = prefix;
        public PropertySegment Property { get; } = property;

        public override bool Equals(SegmentBase? other) => other is LambdaParameterDeclaringSegment p && Prefix == p.Prefix && Property.Equals(p.Property);

        public override int GetHashCode() => HashCode.Combine(Prefix, Property);
    }
}