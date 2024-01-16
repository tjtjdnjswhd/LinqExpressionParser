using System.Linq.Expressions;

namespace LinqExpressionParser.Expressions.Maps.Methods
{
    public class MethodMapOptions
    {
        public static MethodMapOptions Default => new()
        {
            MethodMap = new Dictionary<string, List<GetMethodCallExpressionDelegate>>(DefaultMethodMap.DefaultMaps.ToDictionary(set => set.Key, set => set.Value.ToList()), StringComparer.OrdinalIgnoreCase),
            LambdaMethodMap = new Dictionary<string, List<GetLambdaMethodCallExpressionDelegate>>(DefaultMethodMap.DefaultLambdaMaps.ToDictionary(set => set.Key, set => set.Value.ToList()), StringComparer.OrdinalIgnoreCase)
        };

        public delegate Expression? GetMethodCallExpressionDelegate(List<Expression> args);

        public delegate Expression? GetLambdaMethodCallExpressionDelegate(ParameterExpression parameter, List<Expression> args);

        public required Dictionary<string, List<GetMethodCallExpressionDelegate>> MethodMap { get; init; }
        public required Dictionary<string, List<GetLambdaMethodCallExpressionDelegate>> LambdaMethodMap { get; init; }
    }
}