using LinqExpressionParser.Segments.Comparer;

using System.Collections.ObjectModel;
using System.Diagnostics;

namespace LinqExpressionParser.Segments
{
    [DebuggerDisplay("{Name}")]
    public class LambdaMethodSegment : ValueSegment
    {
        public string Name { get; }
        public LambdaParameterDeclaringSegment ParameterDeclaringSegment { get; }
        public ReadOnlyCollection<ValueSegment> Arguments { get; }

        public LambdaMethodSegment(string name, LambdaParameterDeclaringSegment parameterDeclaringSegment, List<ValueSegment> arguments)
        {
            Name = name;
            ParameterDeclaringSegment = parameterDeclaringSegment;
            Arguments = arguments.AsReadOnly();
        }

        public LambdaMethodSegment(string name, LambdaParameterDeclaringSegment parameterDeclearingSegment, params ValueSegment[] arguments)
        {
            Name = name;
            ParameterDeclaringSegment = parameterDeclearingSegment;
            Arguments = arguments.AsReadOnly();
        }

        public override bool Equals(SegmentBase? other) => other is LambdaMethodSegment l && Name == l.Name && ParameterDeclaringSegment.Equals(l.ParameterDeclaringSegment) && Arguments.SequenceEqual(l.Arguments, SegmentComparer.GetInstance());

        public override int GetHashCode() => HashCode.Combine(Name, ParameterDeclaringSegment, Arguments);
    }
}