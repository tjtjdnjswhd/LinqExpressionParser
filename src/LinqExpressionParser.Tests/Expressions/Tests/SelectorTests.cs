using LinqExpressionParser.Expressions.Maps.Methods;
using LinqExpressionParser.Expressions.Maps.Operators;
using LinqExpressionParser.Expressions.Parser;

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using System.Linq.Expressions;

namespace LinqExpressionParser.Tests.Expressions.Tests
{
    [TestClass]
    public class SelectorTests : VerifyBase
    {
        private static readonly ExpressionParser _parser = new(NullLogger<ExpressionParser>.Instance, Options.Create(new ExpressionParserOptions() { MethodMapOptions = MethodMapOptions.Default, OperatorMapOptions = OperatorMapOptions.Default }));

        static SelectorTests()
        {
            SelectorTestDatas = new[]
            {
            new object[] {
                new SelectorSegment(new Dictionary<string, ValueSegment>()
                {
                    { "Id", new PropertySegment(nameof(User.Id)) },
                    { "NameSubstring", new MethodSegment("Substring", new PropertySegment(nameof(User.Name)), new ConstantSegment(1)) },
                    { "SignupAt", new PropertySegment(nameof(User.SignupAt)) },
                    { "LastLoginAddDay", new MethodSegment("AddDays", new PropertySegment(nameof(User.LastLoginAt)), new ConstantSegment(-2.6)) }
                })}
            };
        }

        private static IEnumerable<object[]> SelectorTestDatas { get; }

        [DataTestMethod]
        [DynamicData(nameof(SelectorTestDatas))]
        public Task SelectTest(SelectorSegment selector)
        {
            LambdaExpression result = _parser.ParseSelectorExpression<User>(selector);
            return Verify(result);
        }
    }
}