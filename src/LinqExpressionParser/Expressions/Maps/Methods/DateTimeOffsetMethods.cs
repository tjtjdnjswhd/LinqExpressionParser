using System.Linq.Expressions;
using System.Reflection;

using static LinqExpressionParser.Expressions.Maps.Methods.MethodMapOptions;

namespace LinqExpressionParser.Expressions.Maps.Methods
{
    public static class DateTimeOffsetMethods
    {
        private static readonly MethodInfo AddYearsMethod;
        private static readonly MethodInfo AddMonthsMethod;
        private static readonly MethodInfo AddDaysMethod;
        private static readonly MethodInfo AddHoursMethod;
        private static readonly MethodInfo AddMinutesMethod;
        private static readonly MethodInfo AddSecondsMethod;
        private static readonly MethodInfo AddMillisecondsMethod;

        static DateTimeOffsetMethods()
        {
            const BindingFlags DefaultBindingFlag = BindingFlags.Instance | BindingFlags.Public;
            AddYearsMethod = typeof(DateTimeOffset).GetMethod(nameof(DateTimeOffset.AddYears), DefaultBindingFlag) ?? throw new MissingMethodException();
            AddMonthsMethod = typeof(DateTimeOffset).GetMethod(nameof(DateTimeOffset.AddMonths), DefaultBindingFlag) ?? throw new MissingMethodException();
            AddDaysMethod = typeof(DateTimeOffset).GetMethod(nameof(DateTimeOffset.AddDays), DefaultBindingFlag) ?? throw new MissingMethodException();
            AddHoursMethod = typeof(DateTimeOffset).GetMethod(nameof(DateTimeOffset.AddHours), DefaultBindingFlag) ?? throw new MissingMethodException();
            AddMinutesMethod = typeof(DateTimeOffset).GetMethod(nameof(DateTimeOffset.AddMinutes), DefaultBindingFlag) ?? throw new MissingMethodException();
            AddSecondsMethod = typeof(DateTimeOffset).GetMethod(nameof(DateTimeOffset.AddSeconds), DefaultBindingFlag) ?? throw new MissingMethodException();
            AddMillisecondsMethod = typeof(DateTimeOffset).GetMethod(nameof(DateTimeOffset.AddMilliseconds), DefaultBindingFlag) ?? throw new MissingMethodException();
        }

        public static GetMethodCallExpressionDelegate CallAddYearsMethod => args => args switch
        {
            [Expression source, Expression arg] when source.Type == typeof(DateTimeOffset) && arg.Type == typeof(int) => Expression.Call(source, AddYearsMethod, arg),
            _ => null
        };

        public static GetMethodCallExpressionDelegate CallAddMonthsMethod => args => args switch
        {
            [Expression source, Expression arg] when source.Type == typeof(DateTimeOffset) && arg.Type == typeof(int) => Expression.Call(source, AddMonthsMethod, arg),
            _ => null
        };

        public static GetMethodCallExpressionDelegate CallAddDaysMethod => args => args switch
        {
            [Expression source, Expression arg] when source.Type == typeof(DateTimeOffset) && (arg.Type == typeof(int) || arg.Type == typeof(double)) => Expression.Call(source, AddDaysMethod, Expression.Convert(arg, typeof(double))),
            _ => null
        };

        public static GetMethodCallExpressionDelegate CallAddHoursMethod => args => args switch
        {
            [Expression source, Expression arg] when source.Type == typeof(DateTimeOffset) && (arg.Type == typeof(int) || arg.Type == typeof(double)) => Expression.Call(source, AddHoursMethod, Expression.Convert(arg, typeof(double))),
            _ => null
        };

        public static GetMethodCallExpressionDelegate CallAddMinutesMethod => args => args switch
        {
            [Expression source, Expression arg] when source.Type == typeof(DateTimeOffset) && (arg.Type == typeof(int) || arg.Type == typeof(double)) => Expression.Call(source, AddMinutesMethod, Expression.Convert(arg, typeof(double))),
            _ => null
        };

        public static GetMethodCallExpressionDelegate CallAddSecondsMethod => args => args switch
        {
            [Expression source, Expression arg] when source.Type == typeof(DateTimeOffset) && (arg.Type == typeof(int) || arg.Type == typeof(double)) => Expression.Call(source, AddSecondsMethod, Expression.Convert(arg, typeof(double))),
            _ => null
        };

        public static GetMethodCallExpressionDelegate CallAddMillisecondsMethod => args => args switch
        {
            [Expression source, Expression arg] when source.Type == typeof(DateTimeOffset) && (arg.Type == typeof(int) || arg.Type == typeof(double)) => Expression.Call(source, AddMillisecondsMethod, Expression.Convert(arg, typeof(double))),
            _ => null
        };
    }
}
