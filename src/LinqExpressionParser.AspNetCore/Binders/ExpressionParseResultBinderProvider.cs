using LinqExpressionParser.AspNetCore.Results;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace LinqExpressionParser.AspNetCore.Binders
{
    public class ExpressionParseResultBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            Type modelType = context.Metadata.ModelType;

            if (!modelType.IsGenericType)
            {
                return null;
            }

            Type binderType;
            Type genericTypeDefinition = modelType.GetGenericTypeDefinition();
            if (genericTypeDefinition == typeof(ValueParseResult<,>))
            {
                binderType = typeof(ValueParseResultBinder<,>).MakeGenericType(modelType.GenericTypeArguments);
            }
            else if (genericTypeDefinition == typeof(SelectorParseResult<>))
            {
                binderType = typeof(SelectorParseResultBinder<>).MakeGenericType(modelType.GenericTypeArguments);
            }
            else
            {
                return null;
            }

            IModelBinder binder = (IModelBinder)ActivatorUtilities.CreateInstance(context.Services, binderType);
            return binder;
        }
    }
}