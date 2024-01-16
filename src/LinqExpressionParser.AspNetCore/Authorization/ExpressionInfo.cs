using System.Linq.Expressions;
using System.Reflection;

namespace LinqExpressionParser.AspNetCore.Authorization
{
    public class ExpressionInfo
    {
        private static readonly Factory _factory = new();

        public Expression Expression { get; }
        public HashSet<PropertyInfo> UsedProperties { get; }
        public HashSet<Type> UsedTypes { get; }
        public HashSet<MethodInfo> UsedMethods { get; }

        public ExpressionInfo(Expression expression) : this(_factory.Create(expression)) { }

        private ExpressionInfo((Expression expression, HashSet<PropertyInfo> properties, HashSet<Type> types, HashSet<MethodInfo> methods) tuple)
        {
            Expression = tuple.expression;
            UsedProperties = tuple.properties;
            UsedTypes = tuple.types;
            UsedMethods = tuple.methods;
        }

        private class Factory : ExpressionVisitor
        {
#pragma warning disable CS8618 // 생성자를 종료할 때 null을 허용하지 않는 필드에 null이 아닌 값을 포함해야 합니다. null 허용으로 선언해 보세요.
            private HashSet<PropertyInfo> _properties;
            private HashSet<Type> _types;
            private HashSet<MethodInfo> _methods;
#pragma warning restore CS8618 // 생성자를 종료할 때 null을 허용하지 않는 필드에 null이 아닌 값을 포함해야 합니다. null 허용으로 선언해 보세요.

            public (Expression expression, HashSet<PropertyInfo> properties, HashSet<Type> types, HashSet<MethodInfo> methods) Create(Expression expression)
            {
                _properties = [];
                _types = [];
                _methods = [];

                Visit(expression);
                return (expression, _properties, _types, _methods);
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                AddType(node.ReturnType);
                foreach (var parameter in node.Parameters)
                {
                    AddType(parameter.Type);
                }
                return base.VisitLambda(node);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Member is PropertyInfo property)
                {
                    _properties.Add(property);
                    AddType(property.PropertyType);
                }
                return base.VisitMember(node);
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                _methods.Add(node.Method);
                return base.VisitMethodCall(node);
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                AddType(node.Type);
                return base.VisitConstant(node);
            }

            private void AddType(Type type)
            {
                _types.Add(type);
                Type? ienumerableInterface = type.GetInterface("IEnumerable`1");
                if (ienumerableInterface is not null)
                {
                    AddType(ienumerableInterface.GenericTypeArguments[0]);
                }
            }
        }
    }
}