using System.Collections.ObjectModel;

namespace LinqExpressionParser.Segments
{
    public class SelectorSegment(Dictionary<string, ValueSegment> valueByName) : SegmentBase
    {
        public ReadOnlyDictionary<string, ValueSegment> SelectedItems { get; } = valueByName.AsReadOnly();

        public override bool Equals(SegmentBase? other)
        {
            if (other is not SelectorSegment s || SelectedItems.Count != s.SelectedItems.Count)
            {
                return false;
            }

            foreach (var value in SelectedItems)
            {
                if (!s.SelectedItems.TryGetValue(value.Key, out ValueSegment? otherValue))
                {
                    return false;
                }

                if (!value.Value.Equals(otherValue))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode() => SelectedItems.Aggregate(7, (seed, set) => seed * (set.Key.GetHashCode() ^ set.Value.GetHashCode()) + seed);
    }
}