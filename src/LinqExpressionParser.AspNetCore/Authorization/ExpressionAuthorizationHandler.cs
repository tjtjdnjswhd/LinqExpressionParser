using LinqExpressionParser.AspNetCore.Results;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Linq.Expressions;
using System.Security.Claims;

namespace LinqExpressionParser.AspNetCore.Authorization;

internal class ExpressionAuthorizationHandler(IServiceProvider services, ILogger<ExpressionAuthorizationHandler> logger, IOptions<ExpressionAuthorizationOptions> options) : AuthorizationHandler<ExpressionAuthorizationRequirement, IExpressionParseResult<Expression>>
{
    private readonly ExpressionAuthorizationOptions _options = options.Value;

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExpressionAuthorizationRequirement requirement, IExpressionParseResult<Expression> resource)
    {
        ExpressionInfo expressionInfo = new(resource.GetExpression());
        IEnumerable<string> requiredPermissions = requirement.GetPermissions(services, expressionInfo);

        Func<ClaimsPrincipal, IEnumerable<string>> permissionFinder = _options.PermissionFinder;
        IEnumerable<string> userPermissions = permissionFinder(context.User);

        StringComparer permissionComparer = StringComparer.FromComparison(_options.PermissionComparsion);
        IEnumerable<string> exceptedPermissions = requiredPermissions.Except(userPermissions, permissionComparer);

        logger.LogDebug("userPermissions: {userPermissions}, requiredPermissions: {requiredPermissions}", userPermissions, requiredPermissions);
        if (exceptedPermissions.Any())
        {
            string expectedPermissionsText = string.Join(", ", exceptedPermissions);
            logger.LogInformation("Expression authorize failed. excepted permissions: {exceptedPermissions}", expectedPermissionsText);
            context.Fail(new AuthorizationFailureReason(this, $"Require these permission: {expectedPermissionsText}"));
        }
        else
        {
            logger.LogInformation("Expression authorize successed");
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}