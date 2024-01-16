namespace LinqExpressionParser.AspNetCore.Authorization.Permission.Providers.Builders
{
    public class AssertionPermissionProviderBuilder(Func<IServiceProvider, ExpressionInfo, bool> assertion, IEnumerable<string> permissions) : IPermissionProviderBuilder
    {
        public Func<IServiceProvider, ExpressionInfo, bool> Assertion { get; set; } = assertion;
        public List<string> Permissions { get; set; } = permissions.ToList();

        public IPermissionProvider Build() => new AssertionPermissionProvider(Assertion, Permissions);
    }
}