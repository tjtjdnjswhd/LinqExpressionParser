using LinqExpressionParser.Segments.Comparer;
using LinqExpressionParser.Segments.Exceptions;

using System.Collections.ObjectModel;

namespace LinqExpressionParser.Segments
{
    public class OperationSegment : ValueSegment
    {
        public ReadOnlyCollection<SegmentBase> Segments { get; }

        public OperationSegment(List<SegmentBase> segments)
        {
            Segments = segments.AsReadOnly();
        }

        public OperationSegment(params SegmentBase[] segments)
        {
            Segments = segments.AsReadOnly();
        }

        public override bool Equals(SegmentBase? other) => other is OperationSegment o && Segments.SequenceEqual(o.Segments, SegmentComparer.GetInstance());

        public override int GetHashCode() => Segments.Aggregate(7, (seed, segment) => seed * segment.GetHashCode() + seed);

        internal static class Factory
        {
            internal static (ESegmentFormatError error, OperationSegment? segment) Create(List<SegmentBase> segments)
            {
                int count = segments.Count;
                if (count == 1)
                {
                    throw new ArgumentException("Segments must contain more than 1", nameof(segments));
                }

                if ((count & 1) == 0)
                {
                    return (ESegmentFormatError.InvalidOperationSegmentCount, null);
                }

                if (segments is [OperatorSegment, ..] or [.., OperatorSegment])
                {
                    return (ESegmentFormatError.InvalidOperatorIndex, null);
                }

                bool isValue = true;
                foreach (var segment in segments)
                {
                    if (segment is ValueSegment != isValue)
                    {
                        return (ESegmentFormatError.InvalidContinuous, null);
                    }
                    isValue = !isValue;
                }

                OperationSegment result = new(segments);
                return (0, result);
            }
        }
    }
}