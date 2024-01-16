using LinqExpressionParser.AspNetCore.Authorization.Attributes;

using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace LinqExpressionParser.AspNetCore.Authorization;

public class ExpressionAllowAnonymousModelConvention : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            if (controller.Attributes.Any(a => a is ExpressionAllowAnonymousAttribute))
            {
                controller.Filters.Add(new ExpressionAllowAnonymousFilter());
            }
            else
            {
                foreach (var action in controller.Actions.Where(action => action.Attributes.Any(a => a is ExpressionAllowAnonymousAttribute)))
                {
                    action.Filters.Add(new ExpressionAllowAnonymousFilter());
                }
            }
        }
    }
}