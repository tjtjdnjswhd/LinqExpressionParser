using LinqExpressionParser.AspNetCore.Authorization.Permission.Providers;

using Microsoft.AspNetCore.Authorization;

namespace LinqExpressionParser.AspNetCore.Authorization;

internal class ExpressionAuthorizationRequirement(IEnumerable<IPermissionProvider> permissionProviders) : IAuthorizationRequirement
{
    public IEnumerable<IPermissionProvider> PermissionProviders { get; } = permissionProviders;

    public IEnumerable<string> GetPermissions(IServiceProvider services, ExpressionInfo expressionInfo)
    {
        return PermissionProviders.SelectMany(p => p.GetPermissions(services, expressionInfo)).Distinct();
    }
}