using System.Diagnostics.CodeAnalysis;

namespace LinqExpressionParser.AspNetCore.Authorization.Permission.Providers;

public class GlobalPermissionProvider(IEnumerable<string> permissions) : IPermissionProvider
{
    private readonly IEnumerable<string> _permissions = permissions;

    [return: NotNull]
    public IEnumerable<string>? GetPermissions(IServiceProvider services, ExpressionInfo expressionInfo) => _permissions;
}