using System.Linq.Expressions;
using System.Reflection;

using static LinqExpressionParser.Expressions.Maps.Methods.MethodMapOptions;

namespace LinqExpressionParser.Expressions.Maps.Methods
{
    public static class DateTimeMethods
    {
        private static readonly MethodInfo AddYearsMethod;
        private static readonly MethodInfo AddMonthsMethod;
        private static readonly MethodInfo AddDaysMethod;
        private static readonly MethodInfo AddHoursMethod;
        private static readonly MethodInfo AddMinutesMethod;
        private static readonly MethodInfo AddSecondsMethod;
        private static readonly MethodInfo AddMillisecondsMethod;


        static DateTimeMethods()
        {
            const BindingFlags DefaultBindingFlag = BindingFlags.Instance | BindingFlags.Public;
            AddYearsMethod = typeof(DateTime).GetMethod(nameof(DateTime.AddYears), DefaultBindingFlag) ?? throw new MissingMethodException();
            AddMonthsMethod = typeof(DateTime).GetMethod(nameof(DateTime.AddMonths), DefaultBindingFlag) ?? throw new MissingMethodException();
            AddDaysMethod = typeof(DateTime).GetMethod(nameof(DateTime.AddDays), DefaultBindingFlag) ?? throw new MissingMethodException();
            AddHoursMethod = typeof(DateTime).GetMethod(nameof(DateTime.AddHours), DefaultBindingFlag) ?? throw new MissingMethodException();
            AddMinutesMethod = typeof(DateTime).GetMethod(nameof(DateTime.AddMinutes), DefaultBindingFlag) ?? throw new MissingMethodException();
            AddSecondsMethod = typeof(DateTime).GetMethod(nameof(DateTime.AddSeconds), DefaultBindingFlag) ?? throw new MissingMethodException();
            AddMillisecondsMethod = typeof(DateTime).GetMethod(nameof(DateTime.AddMilliseconds), DefaultBindingFlag) ?? throw new MissingMethodException();
        }

        public static GetMethodCallExpressionDelegate CallAddYearsMethod => args => args switch
        {
            [Expression source, Expression arg] when source.Type == typeof(DateTime) && arg.Type == typeof(int) => Expression.Call(source, AddYearsMethod, arg),
            _ => null
        };

        public static GetMethodCallExpressionDelegate CallAddMonthsMethod => args => args switch
        {
            [Expression source, Expression arg] when source.Type == typeof(DateTime) && arg.Type == typeof(int) => Expression.Call(source, AddMonthsMethod, arg),
            _ => null
        };

        public static GetMethodCallExpressionDelegate CallAddDaysMethod => args => args switch
        {
            [Expression source, Expression arg] when source.Type == typeof(DateTime) && (arg.Type == typeof(int) || arg.Type == typeof(double)) => Expression.Call(source, AddDaysMethod, Expression.Convert(arg, typeof(double))),
            _ => null
        };

        public static GetMethodCallExpressionDelegate CallAddHoursMethod => args => args switch
        {
            [Expression source, Expression arg] when source.Type == typeof(DateTime) && (arg.Type == typeof(int) || arg.Type == typeof(double)) => Expression.Call(source, AddHoursMethod, Expression.Convert(arg, typeof(double))),
            _ => null
        };

        public static GetMethodCallExpressionDelegate CallAddMinutesMethod => args => args switch
        {
            [Expression source, Expression arg] when source.Type == typeof(DateTime) && (arg.Type == typeof(int) || arg.Type == typeof(double)) => Expression.Call(source, AddMinutesMethod, Expression.Convert(arg, typeof(double))),
            _ => null
        };

        public static GetMethodCallExpressionDelegate CallAddSecondsMethod => args => args switch
        {
            [Expression source, Expression arg] when source.Type == typeof(DateTime) && (arg.Type == typeof(int) || arg.Type == typeof(double)) => Expression.Call(source, AddSecondsMethod, Expression.Convert(arg, typeof(double))),
            _ => null
        };

        public static GetMethodCallExpressionDelegate CallAddMillisecondsMethod => args => args switch
        {
            [Expression source, Expression arg] when source.Type == typeof(DateTime) && (arg.Type == typeof(int) || arg.Type == typeof(double)) => Expression.Call(source, AddMillisecondsMethod, Expression.Convert(arg, typeof(double))),
            _ => null
        };
    }
}