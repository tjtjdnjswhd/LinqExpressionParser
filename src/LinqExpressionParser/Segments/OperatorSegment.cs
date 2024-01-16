using LinqExpressionParser.Segments.Enums;

using System.Diagnostics;

namespace LinqExpressionParser.Segments;

[DebuggerDisplay("{Operator}")]
public class OperatorSegment(EOperator @operator) : SegmentBase
{
    public readonly static OperatorSegment MultiplyOperator = new(EOperator.Multiply);
    public readonly static OperatorSegment DivideOperator = new(EOperator.Divide);
    public readonly static OperatorSegment AddOperator = new(EOperator.Add);
    public readonly static OperatorSegment SubtractOperator = new(EOperator.Subtract);
    public readonly static OperatorSegment EQOperator = new(EOperator.EQ);
    public readonly static OperatorSegment NQOperator = new(EOperator.NQ);
    public readonly static OperatorSegment GTOperator = new(EOperator.GT);
    public readonly static OperatorSegment GTEOperator = new(EOperator.GTE);
    public readonly static OperatorSegment LTOperator = new(EOperator.LT);
    public readonly static OperatorSegment LTEOperator = new(EOperator.LTE);
    public readonly static OperatorSegment AndOperator = new(EOperator.And);
    public readonly static OperatorSegment OrOperator = new(EOperator.Or);

    public EOperator Operator { get; } = @operator;

    public override bool Equals(SegmentBase? other) => other is OperatorSegment o && Operator == o.Operator;

    public override int GetHashCode() => HashCode.Combine(Operator);
}