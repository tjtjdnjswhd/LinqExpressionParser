using System.Linq.Expressions;
using System.Reflection;

using static LinqExpressionParser.Expressions.Maps.Methods.MethodMapOptions;

namespace LinqExpressionParser.Expressions.Maps.Methods
{
    public static class StringMethods
    {
        private static readonly MethodInfo CompareMethod;
        private static readonly MethodInfo ConcatMethod;
        private static readonly MethodInfo IndexOfMethod;
        private static readonly MethodInfo IndexOfStartIndexMethod;
        private static readonly MethodInfo ContainsMethod;
        private static readonly MethodInfo StartsWithMethod;
        private static readonly MethodInfo EndsWithMethod;
        private static readonly MethodInfo SubstringMethod;
        private static readonly MethodInfo SubstringLengthMethod;
        private static readonly MethodInfo TrimMethod;
        private static readonly MethodInfo TrimStartMethod;
        private static readonly MethodInfo TrimEndMethod;
        private static readonly MethodInfo ToLowerMethod;
        private static readonly MethodInfo ToUpperMethod;

        static StringMethods()
        {
            const BindingFlags staticFlags = BindingFlags.Public | BindingFlags.Static;
            const BindingFlags instanceFlags = BindingFlags.Public | BindingFlags.Instance;

            CompareMethod = typeof(string).GetMethod(nameof(string.Compare), staticFlags, [typeof(string), typeof(string)]) ?? throw new MissingMethodException(nameof(String), nameof(string.Compare));
            ConcatMethod = typeof(string).GetMethod(nameof(string.Concat), staticFlags, [typeof(string), typeof(string)]) ?? throw new MissingMethodException(nameof(String), nameof(string.Concat));
            IndexOfMethod = typeof(string).GetMethod(nameof(string.IndexOf), instanceFlags, [typeof(string)]) ?? throw new MissingMethodException(nameof(String), nameof(string.IndexOf));
            IndexOfStartIndexMethod = typeof(string).GetMethod(nameof(string.IndexOf), instanceFlags, [typeof(string), typeof(int)]) ?? throw new MissingMethodException(nameof(String), nameof(string.IndexOf));
            ContainsMethod = typeof(string).GetMethod(nameof(string.Contains), instanceFlags, [typeof(string)]) ?? throw new MissingMethodException(nameof(String), nameof(string.Contains));
            StartsWithMethod = typeof(string).GetMethod(nameof(string.StartsWith), instanceFlags, [typeof(string)]) ?? throw new MissingMethodException(nameof(String), nameof(string.StartsWith));
            EndsWithMethod = typeof(string).GetMethod(nameof(string.EndsWith), instanceFlags, [typeof(string)]) ?? throw new MissingMethodException(nameof(String), nameof(string.EndsWith));
            SubstringMethod = typeof(string).GetMethod(nameof(string.Substring), instanceFlags, [typeof(int)]) ?? throw new MissingMethodException(nameof(String), nameof(string.Substring));
            SubstringLengthMethod = typeof(string).GetMethod(nameof(string.Substring), instanceFlags, [typeof(int), typeof(int)]) ?? throw new MissingMethodException(nameof(String), nameof(string.Substring));
            TrimMethod = typeof(string).GetMethod(nameof(string.Trim), instanceFlags, Type.EmptyTypes) ?? throw new MissingMethodException(nameof(String), nameof(string.Trim));
            TrimStartMethod = typeof(string).GetMethod(nameof(string.TrimStart), instanceFlags, Type.EmptyTypes) ?? throw new MissingMethodException(nameof(String), nameof(string.TrimStart));
            TrimEndMethod = typeof(string).GetMethod(nameof(string.TrimEnd), instanceFlags, Type.EmptyTypes) ?? throw new MissingMethodException(nameof(String), nameof(string.TrimEnd));
            ToLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), instanceFlags, Type.EmptyTypes) ?? throw new MissingMethodException(nameof(String), nameof(string.ToLower));
            ToUpperMethod = typeof(string).GetMethod(nameof(string.ToUpper), instanceFlags, Type.EmptyTypes) ?? throw new MissingMethodException(nameof(String), nameof(string.ToUpper));
        }

        public static GetMethodCallExpressionDelegate CallCompareMethod => args => args switch
        {
            [Expression arg0, Expression arg1] when arg0.Type == typeof(string) && arg1.Type == typeof(string) => Expression.Call(CompareMethod, arg0, arg1),
            _ => null
        };

        public static GetMethodCallExpressionDelegate CallConcatMethod => args => args switch
        {
            [Expression arg0, Expression arg1] when arg0.Type == typeof(string) && arg1.Type == typeof(string) => Expression.Call(ConcatMethod, arg0, arg1),
            _ => null
        };

        public static GetMethodCallExpressionDelegate CallIndexOfMethod => args =>
        {
            switch (args)
            {
                case [Expression instance, Expression arg, ..] when instance.Type == typeof(string) && arg.Type == typeof(string):
                    if (args.Count == 2)
                    {
                        return Expression.Call(instance, IndexOfMethod, arg);
                    }
                    if (args.Count == 3 && args[2].Type == typeof(int))
                    {
                        return Expression.Call(instance, IndexOfStartIndexMethod, arg, args[2]);
                    }
                    break;
            }

            return null;
        };

        public static GetMethodCallExpressionDelegate CallContainsMethod => args => args switch
        {
            [Expression instance, Expression arg] when instance.Type == typeof(string) && arg.Type == typeof(string) => Expression.Call(instance, ContainsMethod, arg),
            _ => null
        };
        public static GetMethodCallExpressionDelegate CallStartsWithMethod => args => args switch
        {
            [Expression instance, Expression arg] when instance.Type == typeof(string) && arg.Type == typeof(string) => Expression.Call(instance, StartsWithMethod, arg),
            _ => null
        };

        public static GetMethodCallExpressionDelegate CallEndsWithMethod => args => args switch
        {
            [Expression instance, Expression arg] when instance.Type == typeof(string) && arg.Type == typeof(string) => Expression.Call(instance, EndsWithMethod, arg),
            _ => null
        };

        public static GetMethodCallExpressionDelegate CallSubstringMethod => args =>
        {
            switch (args)
            {
                case [Expression instance, Expression arg, ..] when instance.Type == typeof(string) && arg.Type == typeof(int):
                    if (args.Count == 2)
                    {
                        return Expression.Call(instance, SubstringMethod, arg);
                    }
                    if (args.Count == 3 && args[2].Type == typeof(int))
                    {
                        return Expression.Call(instance, SubstringLengthMethod, arg, args[2]);
                    }
                    break;
            }
            return null;
        };

        public static GetMethodCallExpressionDelegate CallTrimMethod => args => args switch
        {
            [Expression instance] when instance.Type == typeof(string) => Expression.Call(instance, TrimMethod),
            _ => null
        };

        public static GetMethodCallExpressionDelegate CallTrimStartMethod => args => args switch
        {
            [Expression instance] when instance.Type == typeof(string) => Expression.Call(instance, TrimStartMethod),
            _ => null
        };

        public static GetMethodCallExpressionDelegate CallTrimEndMethod => args => args switch
        {
            [Expression instance] when instance.Type == typeof(string) => Expression.Call(instance, TrimEndMethod),
            _ => null
        };

        public static GetMethodCallExpressionDelegate CallToLowerMethod => args => args switch
        {
            [Expression instance] when instance.Type == typeof(string) => Expression.Call(instance, ToLowerMethod),
            _ => null
        };

        public static GetMethodCallExpressionDelegate CallToUpperMethod => args => args switch
        {
            [Expression instance] when instance.Type == typeof(string) => Expression.Call(instance, ToUpperMethod),
            _ => null
        };
    }
}