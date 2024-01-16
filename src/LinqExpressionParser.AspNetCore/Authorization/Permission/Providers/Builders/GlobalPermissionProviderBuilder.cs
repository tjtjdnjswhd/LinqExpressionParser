namespace LinqExpressionParser.AspNetCore.Authorization.Permission.Providers.Builders;

public class GlobalPermissionProviderBuilder : IPermissionProviderBuilder
{
    public List<string> Permissions { get; }

    public GlobalPermissionProviderBuilder(IEnumerable<string> permissions)
    {
        if (!permissions.Any())
        {
            throw new ArgumentException("Empty permissions is not allowed", nameof(permissions));
        }
        Permissions = permissions.ToList();
    }

    public IPermissionProvider Build()
    {
        return new GlobalPermissionProvider(Permissions);
    }
}