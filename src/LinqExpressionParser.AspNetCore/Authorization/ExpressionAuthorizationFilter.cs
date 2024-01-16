using LinqExpressionParser.AspNetCore.Authorization.Attributes;
using LinqExpressionParser.AspNetCore.Results;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

using System.Linq.Expressions;
using System.Reflection;

namespace LinqExpressionParser.AspNetCore.Authorization;

public class ExpressionAuthorizationFilter(string policy) : IAsyncActionFilter
{
    public string Policy { get; private set; } = policy;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.Filters.Any(f => f is ExpressionAllowAnonymousFilter))
        {
            await next();
            return;
        }

        IEnumerable<string> permissionRequiredResultNames =
            context.ActionDescriptor.Parameters
            .OfType<ControllerParameterDescriptor>()
            .Where(p => p.ParameterType.IsAssignableTo(typeof(IExpressionParseResult<Expression>)) && p.ParameterInfo.GetCustomAttribute<ExpressionAllowAnonymousAttribute>(true) is null)
            .Select(p => p.Name);

        IEnumerable<IExpressionParseResult<Expression>> permissionRequiredResults =
            context.ActionArguments.IntersectBy(permissionRequiredResultNames, set => set.Key)
            .Select(set => set.Value)
            .OfType<IExpressionParseResult<Expression>>();

        IAuthorizationService authorizationService = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
        foreach (var parseResult in permissionRequiredResults)
        {
            AuthorizationResult authorizationResult = await authorizationService.AuthorizeAsync(context.HttpContext.User, parseResult, Policy);
            if (!authorizationResult.Succeeded)
            {
                context.Result = context.HttpContext.User.Identity?.IsAuthenticated switch
                {
                    true => new ForbidResult(),
                    _ => new UnauthorizedResult()
                };

                return;
            }
        }

        await next();
    }
}