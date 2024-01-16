namespace LinqExpressionParser.AspNetCore.Authorization.Permission.Providers;

public interface IPermissionProvider
{
    public IEnumerable<string> GetPermissions(IServiceProvider services, ExpressionInfo expressionInfo);
}