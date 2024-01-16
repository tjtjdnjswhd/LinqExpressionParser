using System.Collections.ObjectModel;

using static LinqExpressionParser.Expressions.Maps.Methods.MethodMapOptions;

namespace LinqExpressionParser.Expressions.Maps.Methods
{
    public static class DefaultMethodMap
    {
        static DefaultMethodMap()
        {
            Dictionary<string, IEnumerable<GetMethodCallExpressionDelegate>> defaultMap = new()
            {
                { "Compare", new GetMethodCallExpressionDelegate[] { StringMethods.CallCompareMethod } },
                { "Concat", new GetMethodCallExpressionDelegate[] { StringMethods.CallConcatMethod } },
                { "IndexOf", new GetMethodCallExpressionDelegate[] { StringMethods.CallIndexOfMethod } },
                { "Contains", new GetMethodCallExpressionDelegate[] { StringMethods.CallContainsMethod } },
                { "StartsWith", new GetMethodCallExpressionDelegate[] { StringMethods.CallStartsWithMethod } },
                { "EndsWith", new GetMethodCallExpressionDelegate[] { StringMethods.CallEndsWithMethod } },
                { "Substring", new GetMethodCallExpressionDelegate[] { StringMethods.CallSubstringMethod } },
                { "Trim", new GetMethodCallExpressionDelegate[] { StringMethods.CallTrimMethod } },
                { "TrimStart", new GetMethodCallExpressionDelegate[] { StringMethods.CallTrimStartMethod } },
                { "TrimEnd", new GetMethodCallExpressionDelegate[] { StringMethods.CallTrimEndMethod } },
                { "ToLower", new GetMethodCallExpressionDelegate[] { StringMethods.CallToLowerMethod } },
                { "ToUpper", new GetMethodCallExpressionDelegate[] { StringMethods.CallToUpperMethod } },
                { "AddYears", new GetMethodCallExpressionDelegate[] { DateTimeMethods.CallAddYearsMethod, DateTimeOffsetMethods.CallAddYearsMethod } },
                { "AddMonths", new GetMethodCallExpressionDelegate[] { DateTimeMethods.CallAddMonthsMethod, DateTimeOffsetMethods.CallAddMonthsMethod } },
                { "AddDays", new GetMethodCallExpressionDelegate[] { DateTimeMethods.CallAddDaysMethod, DateTimeOffsetMethods.CallAddDaysMethod } },
                { "AddHours", new GetMethodCallExpressionDelegate[] { DateTimeMethods.CallAddHoursMethod, DateTimeOffsetMethods.CallAddHoursMethod } },
                { "AddMinutes", new GetMethodCallExpressionDelegate[] { DateTimeMethods.CallAddMinutesMethod, DateTimeOffsetMethods.CallAddMinutesMethod } },
                { "AddSeconds", new GetMethodCallExpressionDelegate[] { DateTimeMethods.CallAddSecondsMethod, DateTimeOffsetMethods.CallAddSecondsMethod } },
                { "AddMilliseconds", new GetMethodCallExpressionDelegate[] { DateTimeMethods.CallAddMillisecondsMethod, DateTimeOffsetMethods.CallAddMillisecondsMethod } }
            };

            Dictionary<string, IEnumerable<GetLambdaMethodCallExpressionDelegate>> defaultLambdaMap = new()
            {
                { "Any", new GetLambdaMethodCallExpressionDelegate[] { EnumerableMethods.CallAnyMethod } },
                { "All", new GetLambdaMethodCallExpressionDelegate[] { EnumerableMethods.CallAllMethod } },
                { "Count", new GetLambdaMethodCallExpressionDelegate[] { EnumerableMethods.CallCountMethod } },
                { "Max", new GetLambdaMethodCallExpressionDelegate[] { EnumerableMethods.CallMaxMethod } },
                { "Min", new GetLambdaMethodCallExpressionDelegate[] { EnumerableMethods.CallMinMethod } },
                { "Avg", new GetLambdaMethodCallExpressionDelegate[] { EnumerableMethods.CallAverageMethod } },
                { "Sum", new GetLambdaMethodCallExpressionDelegate[] { EnumerableMethods.CallSumMethod } }
            };

            DefaultMaps = new(defaultMap);
            DefaultLambdaMaps = new(defaultLambdaMap);
        }

        public static ReadOnlyDictionary<string, IEnumerable<GetMethodCallExpressionDelegate>> DefaultMaps { get; }
        public static ReadOnlyDictionary<string, IEnumerable<GetLambdaMethodCallExpressionDelegate>> DefaultLambdaMaps { get; }
    }
}