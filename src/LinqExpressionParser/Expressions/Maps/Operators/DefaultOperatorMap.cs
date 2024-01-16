using LinqExpressionParser.Segments.Enums;

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

using static LinqExpressionParser.Expressions.Maps.Operators.OperatorMapOptions;

namespace LinqExpressionParser.Expressions.Maps.Operators
{
    public static class DefaultOperatorMap
    {
        public static ReadOnlyDictionary<EOperator, IEnumerable<GetOperatorExpressionDelegate>> DefaultMaps { get; }

        private static readonly Type[] NumberOperandTypePriority =
        [
            typeof(decimal?),
            typeof(decimal),
            typeof(double?),
            typeof(double),
            typeof(float?),
            typeof(float),
            typeof(long?),
            typeof(long),
            typeof(int?),
            typeof(int),
            typeof(short?),
            typeof(short),
            typeof(byte?),
            typeof(byte)
        ];

        static DefaultOperatorMap()
        {
            Dictionary<EOperator, IEnumerable<GetOperatorExpressionDelegate>> defaultMaps = new()
            {
                { EOperator.Multiply, new GetOperatorExpressionDelegate[] { (left, right) => TryMatchArithmeticOperandType(left, right, out Expression? matchedLeft, out Expression? matchedRight) ? Expression.Multiply(matchedLeft, matchedRight) : null } },
                { EOperator.Divide, new GetOperatorExpressionDelegate[] { (left, right) => TryMatchArithmeticOperandType(left, right, out Expression? matchedLeft, out Expression? matchedRight) ? Expression.Divide(matchedLeft, matchedRight) : null } },
                { EOperator.Add, new GetOperatorExpressionDelegate[] { (left, right) => TryMatchArithmeticOperandType(left, right, out Expression? matchedLeft, out Expression? matchedRight) ? Expression.Add(matchedLeft, matchedRight) : null } },
                { EOperator.Subtract, new GetOperatorExpressionDelegate[] { (left, right) => TryMatchArithmeticOperandType(left, right, out Expression? matchedLeft, out Expression? matchedRight) ? Expression.Subtract(matchedLeft, matchedRight) : null } },
                {
                    EOperator.EQ,
                    new GetOperatorExpressionDelegate[] {
                        (left, right) =>
                        {
                            (Expression matchedLeft, Expression matchedRight) = MatchEqualityOperandType(left, right);
                            return Expression.Equal(matchedLeft, matchedRight);
                        }
                    }
                },
                {
                    EOperator.NQ,
                    new GetOperatorExpressionDelegate[] {
                        (left, right) =>
                        {
                            (Expression matchedLeft, Expression matchedRight) = MatchEqualityOperandType(left, right);
                            return Expression.NotEqual(matchedLeft, matchedRight);
                        }
                    }
                },
                { EOperator.GT, new GetOperatorExpressionDelegate[] { (left, right) => TryMatchCompareOperandType(left, right, out Expression? matchedLeft, out Expression? matchedRight) ? Expression.GreaterThan(matchedLeft, matchedRight) : null } },
                { EOperator.GTE, new GetOperatorExpressionDelegate[] { (left, right) => TryMatchCompareOperandType(left, right, out Expression? matchedLeft, out Expression? matchedRight) ? Expression.GreaterThanOrEqual(matchedLeft, matchedRight) : null } },
                { EOperator.LT, new GetOperatorExpressionDelegate[] { (left, right) => TryMatchCompareOperandType(left, right, out Expression? matchedLeft, out Expression? matchedRight) ? Expression.LessThan(matchedLeft, matchedRight) : null } },
                { EOperator.LTE, new GetOperatorExpressionDelegate[] { (left, right) => TryMatchCompareOperandType(left, right, out Expression? matchedLeft, out Expression? matchedRight) ? Expression.LessThanOrEqual(matchedLeft, matchedRight) : null } },
                { EOperator.And, new GetOperatorExpressionDelegate[] { (left, right) => left.Type == typeof(bool) && right.Type == typeof(bool) ? Expression.AndAlso(left, right) : null } },
                { EOperator.Or, new GetOperatorExpressionDelegate[] { (left, right) => left.Type == typeof(bool) && right.Type == typeof(bool) ? Expression.OrElse(left, right) : null } }
            };

            DefaultMaps = new(defaultMaps);
        }

        private static bool TryMatchArithmeticOperandType(Expression left, Expression right, [NotNullWhen(true)] out Expression? matchedLeft, [NotNullWhen(true)] out Expression? matchedRight)
        {
            int leftTypeIndex = Array.IndexOf(NumberOperandTypePriority, left.Type);
            int rightTypeIndex = Array.IndexOf(NumberOperandTypePriority, right.Type);

            if (leftTypeIndex < 0 || rightTypeIndex < 0)
            {
                matchedLeft = null;
                matchedRight = null;
                return false;
            }

            if (leftTypeIndex == rightTypeIndex)
            {
                matchedLeft = left;
                matchedRight = right;
                return true;
            }

            if (leftTypeIndex > rightTypeIndex)
            {
                matchedLeft = Expression.Convert(left, right.Type);
                matchedRight = right;
            }
            else
            {
                matchedRight = Expression.Convert(right, left.Type);
                matchedLeft = left;
            }

            return true;
        }

        private static (Expression left, Expression right) MatchEqualityOperandType(Expression left, Expression right)
        {
            if (left.Type == right.Type)
            {
                return (left, right);
            }

            if (TryMatchArithmeticOperandType(left, right, out var matchedLeft, out var matchedRight))
            {
                return (matchedLeft, matchedRight);
            }

            if (left.Type == typeof(DateTime) && right.Type == typeof(DateTimeOffset))
            {
                return (left, Expression.Property(right, nameof(DateTimeOffset.Date)));
            }
            else if (left.Type == typeof(DateTimeOffset) && right.Type == typeof(DateTime))
            {
                return (Expression.Property(left, nameof(DateTimeOffset.Date)), right);
            }

            return (Expression.Convert(left, typeof(object)), Expression.Convert(right, typeof(object)));
        }


        private static bool TryMatchCompareOperandType(Expression left, Expression right, [NotNullWhen(true)] out Expression? matchedLeft, [NotNullWhen(true)] out Expression? matchedRight)
        {
            if (left.Type == right.Type)
            {
                matchedLeft = left;
                matchedRight = right;
                return true;
            }

            if (TryMatchArithmeticOperandType(left, right, out matchedLeft, out matchedRight))
            {
                return true;
            }

            if (left.Type == typeof(DateTime) && right.Type == typeof(DateTimeOffset))
            {
                matchedRight = Expression.Property(right, nameof(DateTimeOffset.Date));
                matchedLeft = left;
                return true;
            }

            if (left.Type == typeof(DateTimeOffset) && right.Type == typeof(DateTime))
            {
                matchedLeft = Expression.Property(left, nameof(DateTimeOffset.Date));
                matchedRight = right;
                return true;
            }

            matchedLeft = null;
            matchedRight = null;

            return false;
        }
    }
}
