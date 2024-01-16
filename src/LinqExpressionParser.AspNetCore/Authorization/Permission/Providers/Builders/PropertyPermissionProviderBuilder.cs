using System.Linq.Expressions;
using System.Reflection;

namespace LinqExpressionParser.AspNetCore.Authorization.Permission.Providers.Builders;

public class PropertyPermissionProviderBuilder<TEntity> : IPermissionProviderBuilder
{
    public PropertyInfo PropertyInfo { get; }
    public List<string> Permissions { get; }

    private PropertyPermissionProviderBuilder(PropertyInfo propertyInfo, IEnumerable<string> permissions)
    {
        PropertyInfo = propertyInfo;
        Permissions = permissions.ToList();
    }

    public static PropertyPermissionProviderBuilder<TEntity> Create<TProperty>(Expression<Func<TEntity, TProperty>> propertyExp, IEnumerable<string> permissions)
    {
        if (propertyExp.Body is not MemberExpression member || member.Member is not PropertyInfo propertyInfo)
        {
            throw new ArgumentException("Body must be property access expression", nameof(propertyExp));
        }

        return new PropertyPermissionProviderBuilder<TEntity>(propertyInfo, permissions);
    }

    public IPermissionProvider Build()
    {
        return new PropertyPermissionProvider(PropertyInfo, Permissions);
    }
}