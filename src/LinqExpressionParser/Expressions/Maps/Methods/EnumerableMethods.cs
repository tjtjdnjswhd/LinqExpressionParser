using System.Linq.Expressions;
using System.Reflection;

using static LinqExpressionParser.Expressions.Maps.Methods.MethodMapOptions;

namespace LinqExpressionParser.Expressions.Maps.Methods
{
    public static class EnumerableMethods
    {
        private static readonly MethodInfo AnyMethodInfo;
        private static readonly MethodInfo AnyPredicateMethodInfo;
        private static readonly MethodInfo AllMethodInfo;
        private static readonly MethodInfo CountMethodInfo;
        private static readonly MethodInfo CountPredicateMethodInfo;
        private static readonly MethodInfo WhereMethodInfo;

        private const BindingFlags DefaultBindingFlags = BindingFlags.Public | BindingFlags.Static;
        private static readonly Type EnumerableType = typeof(Enumerable);
        private static readonly Type GenericArgType = Type.MakeGenericMethodParameter(0);
        private static readonly Type SourceType = typeof(IEnumerable<>).MakeGenericType(GenericArgType);
        private static readonly Type PredicateType = typeof(Func<,>).MakeGenericType(GenericArgType, typeof(bool));
        private static readonly Type[] SourceTypeArr = [SourceType];
        private static readonly Type[] PredicateTypeArr = [SourceType, PredicateType];

        private static readonly Type[] NumberTypes = [typeof(int), typeof(int?), typeof(long), typeof(long?), typeof(float), typeof(float?), typeof(double), typeof(double?), typeof(decimal), typeof(decimal?)];

        static EnumerableMethods()
        {
            AnyMethodInfo = EnumerableType.GetMethod(nameof(Enumerable.Any), 1, DefaultBindingFlags, null, SourceTypeArr, null) ?? throw new MissingMethodException();
            AnyPredicateMethodInfo = EnumerableType.GetMethod(nameof(Enumerable.Any), 1, DefaultBindingFlags, null, PredicateTypeArr, null) ?? throw new MissingMethodException();
            AllMethodInfo = EnumerableType.GetMethod(nameof(Enumerable.All), 1, DefaultBindingFlags, null, PredicateTypeArr, null) ?? throw new MissingMethodException();
            CountMethodInfo = EnumerableType.GetMethod(nameof(Enumerable.Count), 1, DefaultBindingFlags, null, SourceTypeArr, null) ?? throw new MissingMethodException();
            CountPredicateMethodInfo = EnumerableType.GetMethod(nameof(Enumerable.Count), 1, DefaultBindingFlags, null, PredicateTypeArr, null) ?? throw new MissingMethodException();
            WhereMethodInfo = EnumerableType.GetMethod(nameof(Enumerable.Where), 1, DefaultBindingFlags, null, PredicateTypeArr, null) ?? throw new MissingMethodException();
        }

        public static GetLambdaMethodCallExpressionDelegate CallAnyMethod => (parameter, args) =>
        {
            if (args is [Expression source, ..] && GetIEnumerableElementTypeOrNull(source.Type) is Type elementType && elementType == parameter.Type)
            {
                switch (args.Count)
                {
                    case 1:
                        return Expression.Call(AnyMethodInfo.MakeGenericMethod(elementType), source);

                    case 2 when args[1].Type == typeof(bool):
                        LambdaExpression predicateExp = Expression.Lambda(args[1], parameter);
                        return Expression.Call(AnyPredicateMethodInfo.MakeGenericMethod(elementType), source, predicateExp);
                }
            }
            return null;
        };

        public static GetLambdaMethodCallExpressionDelegate CallAllMethod => (parameter, args) =>
        {
            if (args is [Expression source, Expression predicateBody] && GetIEnumerableElementTypeOrNull(source.Type) is Type elementType && elementType == parameter.Type && predicateBody.Type == typeof(bool))
            {
                LambdaExpression predicateExp = Expression.Lambda(predicateBody, parameter);
                return Expression.Call(AllMethodInfo.MakeGenericMethod(elementType), source, predicateExp);
            }
            return null;
        };

        public static GetLambdaMethodCallExpressionDelegate CallCountMethod => (parameter, args) =>
        {
            if (args is [Expression source, ..] && GetIEnumerableElementTypeOrNull(source.Type) is Type elementType && parameter.Type == elementType)
            {
                switch (args.Count)
                {
                    case 1:
                        return Expression.Call(CountMethodInfo.MakeGenericMethod(elementType), source);

                    case 2 when args[1].Type == typeof(bool):
                        LambdaExpression predicateExp = Expression.Lambda(args[1], parameter);
                        return Expression.Call(CountPredicateMethodInfo.MakeGenericMethod(elementType), source, predicateExp);
                }
            }
            return null;
        };

        public static GetLambdaMethodCallExpressionDelegate CallMaxMethod => (parameter, args) =>
        {
            if (args is [Expression source, Expression selectorBody] && GetIEnumerableElementTypeOrNull(source.Type) is Type elementType && parameter.Type == elementType && GetMaxMethodInfoOrNull(selectorBody.Type) is MethodInfo maxMethod)
            {
                LambdaExpression selectorExp = Expression.Lambda(selectorBody, parameter);
                return Expression.Call(maxMethod.MakeGenericMethod(elementType), source, selectorExp);
            }
            return null;
        };

        public static GetLambdaMethodCallExpressionDelegate CallMinMethod => (parameter, args) =>
        {
            if (args is [Expression source, Expression selectorBody] && GetIEnumerableElementTypeOrNull(source.Type) is Type elementType && parameter.Type == elementType && GetMinMethodInfoOrNull(selectorBody.Type) is MethodInfo minMethod)
            {
                LambdaExpression selectorExp = Expression.Lambda(selectorBody, parameter);
                return Expression.Call(minMethod.MakeGenericMethod(elementType), source, selectorExp);
            }
            return null;
        };

        public static GetLambdaMethodCallExpressionDelegate CallAverageMethod => (parameter, args) =>
        {
            if (args is [Expression source, Expression selectorBody] && GetIEnumerableElementTypeOrNull(source.Type) is Type elementType && parameter.Type == elementType && GetAverageMethodInfoOrNull(selectorBody.Type) is MethodInfo avgMethod)
            {
                LambdaExpression selectorExp = Expression.Lambda(selectorBody, parameter);
                return Expression.Call(avgMethod.MakeGenericMethod(elementType), source, selectorExp);
            }
            return null;
        };

        public static GetLambdaMethodCallExpressionDelegate CallSumMethod => (parameter, args) =>
        {
            if (args is [Expression source, Expression selectorBody] && GetIEnumerableElementTypeOrNull(source.Type) is Type elementType && parameter.Type == elementType && GetSumMethodInfoOrNull(selectorBody.Type) is MethodInfo sumMethod)
            {
                LambdaExpression selectorExp = Expression.Lambda(selectorBody, parameter);
                return Expression.Call(sumMethod.MakeGenericMethod(elementType), source, selectorExp);
            }
            return null;
        };

        public static MethodInfo? GetMaxMethodInfoOrNull(Type numberType)
        {
            if (Array.IndexOf(NumberTypes, numberType) < 0)
            {
                return null;
            }

            Type selectorType = typeof(Func<,>).MakeGenericType(GenericArgType, numberType);
            return EnumerableType.GetMethod(nameof(Enumerable.Max), 1, DefaultBindingFlags, null, [SourceType, selectorType], null) ?? throw new MissingMethodException();
        }

        public static MethodInfo? GetMinMethodInfoOrNull(Type numberType)
        {
            if (Array.IndexOf(NumberTypes, numberType) < 0)
            {
                return null;
            }

            Type selectorType = typeof(Func<,>).MakeGenericType(GenericArgType, numberType);
            return EnumerableType.GetMethod(nameof(Enumerable.Min), 1, DefaultBindingFlags, null, [SourceType, selectorType], null) ?? throw new MissingMethodException();
        }

        public static MethodInfo? GetAverageMethodInfoOrNull(Type numberType)
        {
            if (Array.IndexOf(NumberTypes, numberType) < 0)
            {
                return null;
            }

            Type selectorType = typeof(Func<,>).MakeGenericType(GenericArgType, numberType);
            return EnumerableType.GetMethod(nameof(Enumerable.Average), 1, DefaultBindingFlags, null, [SourceType, selectorType], null) ?? throw new MissingMethodException();
        }

        public static MethodInfo? GetSumMethodInfoOrNull(Type numberType)
        {
            if (Array.IndexOf(NumberTypes, numberType) < 0)
            {
                return null;
            }

            Type selectorType = typeof(Func<,>).MakeGenericType(GenericArgType, numberType);
            return EnumerableType.GetMethod(nameof(Enumerable.Sum), 1, DefaultBindingFlags, null, [SourceType, selectorType], null) ?? throw new MissingMethodException();
        }

        private static Type? GetIEnumerableElementTypeOrNull(Type type) => type.GetInterface("IEnumerable`1")?.GenericTypeArguments[0];
    }
}