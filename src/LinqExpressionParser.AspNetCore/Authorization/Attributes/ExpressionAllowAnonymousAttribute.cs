namespace LinqExpressionParser.AspNetCore.Authorization.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public class ExpressionAllowAnonymousAttribute : Attribute { }