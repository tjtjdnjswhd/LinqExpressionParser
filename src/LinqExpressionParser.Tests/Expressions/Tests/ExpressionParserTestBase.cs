using LinqExpressionParser.Expressions.Maps.Methods;
using LinqExpressionParser.Expressions.Maps.Operators;
using LinqExpressionParser.Expressions.Parser;

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using System.Runtime.CompilerServices;

using ValueParserTestGenerator.Codes;

namespace LinqExpressionParser.Tests.Expressions.Tests
{
    public abstract class ExpressionParserTestBase : VerifyBase
    {
        protected static readonly ExpressionParser _parser = new(NullLogger<ExpressionParser>.Instance, Options.Create(new ExpressionParserOptions() { MethodMapOptions = MethodMapOptions.Default, OperatorMapOptions = OperatorMapOptions.Default }));

        protected abstract VerifySettings VerifySettings { get; }

        public Task TestAssert<T, V>(ValueParserTestData<T, V> testData, [CallerArgumentExpression(nameof(testData))] string dataName = "")
        {
            var actualLambda = _parser.ParseValueExpression<T, V>(testData.Segment);
            return Verify(actualLambda, VerifySettings).UseTextForParameters(dataName);
        }
    }
}