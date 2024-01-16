using System.Reflection;

namespace LinqExpressionParser.AspNetCore.Authorization.Permission.Providers;

public class PropertyPermissionProvider(PropertyInfo propertyInfo, IEnumerable<string> permissions) : IPermissionProvider
{
    public IEnumerable<string> GetPermissions(IServiceProvider services, ExpressionInfo expressionInfo)
    {
        bool isPropertyUsed = expressionInfo.UsedProperties.Any(p =>
        {
            if (p.Name != propertyInfo.Name)
            {
                return false;
            }

            Type usedPropertyDeclaringType = p.DeclaringType!;
            Type targetDeclaringType = propertyInfo.DeclaringType!.IsGenericType ? propertyInfo.DeclaringType.GetGenericTypeDefinition() : propertyInfo.DeclaringType;
            if (targetDeclaringType.IsInterface)
            {
                if (usedPropertyDeclaringType?.GetInterfaces().Any(i => targetDeclaringType == (i.IsGenericType ? i.GetGenericTypeDefinition() : i)) ?? false)
                {
                    return true;
                }
            }
            else
            {
                Type? baseType = usedPropertyDeclaringType.IsGenericType ? usedPropertyDeclaringType.GetGenericTypeDefinition() : usedPropertyDeclaringType;
                while (baseType is not null)
                {
                    if (baseType == targetDeclaringType)
                    {
                        return true;
                    }
                    baseType = baseType.BaseType?.IsGenericType ?? false ? baseType.BaseType.GetGenericTypeDefinition() : baseType.BaseType;
                }
            }

            return false;
        });

        return isPropertyUsed ? permissions : Array.Empty<string>();
    }
}