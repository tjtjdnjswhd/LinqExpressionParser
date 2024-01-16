using LinqExpressionParser.AspNetCore.Authorization.Permission;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace LinqExpressionParser.AspNetCore.Authorization.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddExpressionAuthorization(this IServiceCollection services, Action<ExpressionAuthorizationOptions> options, Action<PermissionBuilder> permissionBuilderAction)
    {
        return services.AddExpressionAuthorization(ExpressionAuthorizationDefaults.AUTHORIZE_POLICY, options, permissionBuilderAction);
    }

    public static IServiceCollection AddExpressionAuthorization(this IServiceCollection services, string policyName, Action<ExpressionAuthorizationOptions> options, Action<PermissionBuilder> permissionBuilderAction)
    {
        services.Configure(options);
        services.AddSingleton<IAuthorizationHandler, ExpressionAuthorizationHandler>();
        services.Configure<MvcOptions>(o =>
        {
            o.Conventions.Add(new ExpressionAllowAnonymousModelConvention());
            o.Filters.Add(new ExpressionAuthorizationFilter(policyName));
        });

        services.AddAuthorizationBuilder().AddPolicy(policyName, pb =>
        {
            PermissionBuilder permission = new();
            permissionBuilderAction(permission);
            ExpressionAuthorizationRequirement requirement = new(permission.Build());
            pb.AddRequirements(requirement);
        });

        return services;
    }
}