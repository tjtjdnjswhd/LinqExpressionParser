namespace LinqExpressionParser.AspNetCore.Authorization.Permission.Providers
{
    public class AssertionPermissionProvider(Func<IServiceProvider, ExpressionInfo, bool> assertion, IEnumerable<string> permissions) : IPermissionProvider
    {
        public IEnumerable<string> GetPermissions(IServiceProvider services, ExpressionInfo expressionInfo) => assertion(services, expressionInfo) ? permissions : Array.Empty<string>();
    }
}