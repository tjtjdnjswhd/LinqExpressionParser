using LinqExpressionParser.Segments.Comparer;

using System.Collections.ObjectModel;
using System.Diagnostics;

namespace LinqExpressionParser.Segments;

[DebuggerDisplay("{Name}")]
public class MethodSegment : ValueSegment
{
    public string Name { get; }
    public ReadOnlyCollection<ValueSegment> Arguments { get; }

    public MethodSegment(string name)
    {
        Name = name;
        Arguments = Array.Empty<ValueSegment>().AsReadOnly();
    }

    public MethodSegment(string name, List<ValueSegment> arguments)
    {
        Name = name;
        Arguments = arguments.AsReadOnly();
    }

    public MethodSegment(string name, params ValueSegment[] arguments)
    {
        Name = name;
        Arguments = arguments.AsReadOnly();
    }

    public override bool Equals(SegmentBase? other) => other is MethodSegment f && Arguments.SequenceEqual(f.Arguments, SegmentComparer.GetInstance());

    public override int GetHashCode() => HashCode.Combine(Name, Arguments.Aggregate(7, (seed, segment) => seed * segment.GetHashCode() + seed));
}