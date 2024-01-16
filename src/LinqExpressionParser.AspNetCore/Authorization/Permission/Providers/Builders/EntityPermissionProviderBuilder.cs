using System.Linq.Expressions;

namespace LinqExpressionParser.AspNetCore.Authorization.Permission.Providers.Builders;

public class EntityPermissionProviderBuilder<TEntity>(IEnumerable<string> permissions) : IPermissionProviderBuilder
{
    public List<string> Permissions { get; } = permissions.ToList();
    public List<PropertyPermissionProviderBuilder<TEntity>> PropertyPermissionProviderBuilders { get; } = [];

    public PropertyPermissionProviderBuilder<TEntity> Property<TProperty>(Expression<Func<TEntity, TProperty>> propertyExp, params string[] permissions)
        => Property(propertyExp, permissions as IEnumerable<string>);

    public PropertyPermissionProviderBuilder<TEntity> Property<TProperty>(Expression<Func<TEntity, TProperty>> propertyExp, IEnumerable<string> permissions)
    {
        PropertyPermissionProviderBuilder<TEntity> propertyPermissionProvider = PropertyPermissionProviderBuilder<TEntity>.Create(propertyExp, permissions);
        PropertyPermissionProviderBuilders.Add(propertyPermissionProvider);
        return propertyPermissionProvider;
    }

    public IPermissionProvider Build()
    {
        List<IPermissionProvider> propertyPermissionProviders = PropertyPermissionProviderBuilders.Select(p => p.Build()).ToList();
        return new EntityPermissionProvider(typeof(TEntity), Permissions, propertyPermissionProviders);
    }
}