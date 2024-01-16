using System.Security.Claims;

namespace LinqExpressionParser.AspNetCore.Authorization;

public class ExpressionAuthorizationOptions
{
    public StringComparison PermissionComparsion { get; set; } = StringComparison.OrdinalIgnoreCase;
    public required Func<ClaimsPrincipal, IEnumerable<string>> PermissionFinder { get; set; }
}