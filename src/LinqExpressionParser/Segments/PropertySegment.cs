using System.Diagnostics;
using System.Text;

namespace LinqExpressionParser.Segments
{
    [DebuggerDisplay("{DebuggerView,nq}")]
    public class PropertySegment(string name) : ValueSegment
    {
        public string Name { get; } = name.TrimStart('\\');
        public PropertySegment? Child { get; }

        private string DebuggerView
        {
            get
            {
                PropertySegment? property = this;
                StringBuilder sb = new();
                while (property is not null)
                {
                    sb.Append(property.Name);
                    sb.Append('.');
                    property = property.Child;
                }
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString();
            }
        }

        public PropertySegment(string name, PropertySegment child) : this(name)
        {
            Child = child;
        }

        public override bool Equals(SegmentBase? other) => other is PropertySegment p && Name == p.Name && Equals(Child, p.Child);

        public override int GetHashCode() => HashCode.Combine(Name, Child);
    }
}