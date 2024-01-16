using LinqExpressionParser.AspNetCore.Authorization.Permission.Providers;
using LinqExpressionParser.AspNetCore.Authorization.Permission.Providers.Builders;

namespace LinqExpressionParser.AspNetCore.Authorization.Permission;

public class PermissionBuilder
{
    public List<IPermissionProviderBuilder> ProviderBuilders { get; } = [];

    public GlobalPermissionProviderBuilder Global(params string[] permissions)
        => Global(permissions as IEnumerable<string>);

    public GlobalPermissionProviderBuilder Global(IEnumerable<string> permissions)
    {
        GlobalPermissionProviderBuilder globalBuilder = new(permissions);
        ProviderBuilders.Add(globalBuilder);
        return globalBuilder;
    }

    public EntityPermissionProviderBuilder<T> Entity<T>(params string[] permissions) where T : class
        => Entity<T>(permissions as IEnumerable<string>);

    public EntityPermissionProviderBuilder<T> Entity<T>(IEnumerable<string> permissions) where T : class
    {
        if (ProviderBuilders.Find(p => p is EntityPermissionProviderBuilder<T>) is EntityPermissionProviderBuilder<T> provider)
        {
            return provider;
        }

        EntityPermissionProviderBuilder<T> typePermissionBuilder = new(permissions);
        ProviderBuilders.Add(typePermissionBuilder);
        return typePermissionBuilder;
    }

    public IEnumerable<IPermissionProvider> Build()
    {
        foreach (var builder in ProviderBuilders)
        {
            yield return builder.Build();
        }
    }
}