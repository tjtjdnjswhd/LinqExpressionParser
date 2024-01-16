using System.Diagnostics.CodeAnalysis;

namespace LinqExpressionParser.Segments.Comparer
{
    public class SegmentComparer : IEqualityComparer<SegmentBase>
    {
        private static SegmentComparer? _instance;

        private SegmentComparer()
        {
        }

        [MemberNotNull(nameof(_instance))]
        public static SegmentComparer GetInstance()
        {
            _instance ??= new();
            return _instance;
        }

        public bool Equals(SegmentBase? x, SegmentBase? y) => x is null && y is null || (x?.Equals(y) ?? false);

        public int GetHashCode([DisallowNull] SegmentBase obj) => obj.GetHashCode();
    }
}