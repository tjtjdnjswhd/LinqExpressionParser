using AspNetCore.ExpressionParse.Segments.Parser;

using LinqExpressionParser.AspNetCore.Binders;
using LinqExpressionParser.Expressions;
using LinqExpressionParser.Expressions.Maps.Methods;
using LinqExpressionParser.Expressions.Maps.Operators;
using LinqExpressionParser.Expressions.Parser;
using LinqExpressionParser.Segments.Parser;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace LinqExpressionParser.AspNetCore.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddExpressionParse(this IServiceCollection services)
    {
        return services.AddExpressionParse(options =>
        {
            options.MethodMapOptions = MethodMapOptions.Default;
            options.OperatorMapOptions = OperatorMapOptions.Default;
        });
    }

    public static IServiceCollection AddExpressionParse(this IServiceCollection services, Action<ExpressionParserOptions> options)
    {
        services.AddTransient<ISegmentParser, SegmentParser>();
        services.AddTransient<IExpressionParser, ExpressionParser>();
        services.Configure<MvcOptions>(o =>
        {
            o.ModelBinderProviders.Insert(0, new ExpressionParseResultBinderProvider());
        });
        services.Configure(options);
        return services;
    }
}