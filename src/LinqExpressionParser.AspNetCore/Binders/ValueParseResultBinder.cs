using LinqExpressionParser.AspNetCore.Results;
using LinqExpressionParser.Expressions.Exceptions;
using LinqExpressionParser.Expressions.Parser;
using LinqExpressionParser.Segments;
using LinqExpressionParser.Segments.Exceptions;
using LinqExpressionParser.Segments.Parser;

using Microsoft.AspNetCore.Mvc.ModelBinding;

using System.Linq.Expressions;

namespace LinqExpressionParser.AspNetCore.Binders
{
    public class ValueParseResultBinder<T, V>(ISegmentParser segmentParser, IExpressionParser expressionParser) : IModelBinder where T : class
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            string? value = valueProviderResult.FirstValue;
            if (string.IsNullOrWhiteSpace(value))
            {
                return Task.CompletedTask;
            }

            try
            {
                ValueSegment segment = segmentParser.ParseValue(value);
                Expression<Func<T, V>> exp = expressionParser.ParseValueExpression<T, V>(segment);
                ValueParseResult<T, V> result = new(exp);
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            catch (SegmentParseExceptionBase e)
            {
                bindingContext.ModelState.TryAddModelException("Invalid text", e);
            }
            catch (ExpressionParseExceptionBase e)
            {
                bindingContext.ModelState.TryAddModelException("Invalid segments", e);
            }

            return Task.CompletedTask;
        }
    }
}