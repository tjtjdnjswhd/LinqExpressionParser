namespace LinqExpressionParser.AspNetCore.Authorization.Permission.Providers;

public class EntityPermissionProvider(Type type, IEnumerable<string> permissions, List<IPermissionProvider> propertyPermissionProviders) : IPermissionProvider
{
    private readonly Type _entityGenericDefinitionType = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
    private readonly IEnumerable<string> _permissions = permissions;

    public IEnumerable<string> GetPermissions(IServiceProvider services, ExpressionInfo expressionInfo)
    {
        List<string> permissions = [];
        if (expressionInfo.UsedTypes.Any(t => IsContainsType(t) || expressionInfo.UsedProperties.Any(p => IsContainsType(p.PropertyType))))
        {
            permissions.AddRange(_permissions);
        }

        permissions.AddRange(propertyPermissionProviders.SelectMany(p => p.GetPermissions(services, expressionInfo)));
        return permissions;
    }

    private bool IsContainsType(Type targetType)
    {
        if (targetType.GetInterface("IEnumerable`1")?.GenericTypeArguments[0] is Type elementType)
        {
            if (IsContainsType(elementType))
            {
                return true;
            }
        }

        Type? target = targetType.IsGenericType ? targetType.GetGenericTypeDefinition() : targetType;
        if (_entityGenericDefinitionType == target)
        {
            return true;
        }
        else if (_entityGenericDefinitionType.IsInterface)
        {
            if (targetType.GetInterfaces().Any(i => _entityGenericDefinitionType == (i.IsGenericType ? i.GetGenericTypeDefinition() : i)))
            {
                return true;
            }
        }
        else
        {
            while (target is not null)
            {
                if (_entityGenericDefinitionType == target)
                {
                    return true;
                }
                target = target.BaseType?.IsGenericType ?? false ? target.BaseType.GetGenericTypeDefinition() : target.BaseType;
            }
        }
        return false;
    }
}