using LinqExpressionParser.Expressions.Exceptions;
using LinqExpressionParser.Expressions.Maps.Methods;
using LinqExpressionParser.Expressions.Maps.Operators;
using LinqExpressionParser.Expressions.Parser;
using LinqExpressionParser.Segments.Enums;

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace LinqExpressionParser.Tests.Expressions.Tests
{
    [TestClass]
    public class ExceptionTests
    {
        private static readonly ExpressionParser _defaultParser = new(NullLogger<ExpressionParser>.Instance, Options.Create(new ExpressionParserOptions() { MethodMapOptions = MethodMapOptions.Default, OperatorMapOptions = OperatorMapOptions.Default }));

        private static IEnumerable<object[]> InvalidBodyTypeData => new[]
        {
            new object[] { new PropertySegment(nameof(User.Id)) },
            [new PropertySegment(nameof(User.LastLoginAt))]
        };

        [DataTestMethod]
        [DynamicData(nameof(InvalidBodyTypeData))]
        public void InvalidBodyTypeException(ValueSegment segment)
        {
            Assert.ThrowsException<InvalidBodyTypeException>(() => _defaultParser.ParseValueExpression<User, string>(segment));
        }

        private static IEnumerable<object[]> PropertyNotExistData => new[]
        {
            new object[] { new PropertySegment("Invalid") }
        };

        [DataTestMethod]
        [DynamicData(nameof(PropertyNotExistData))]
        public void PropertyNotExistException(ValueSegment segment)
        {
            Assert.ThrowsException<PropertyNotExistException>(() => _defaultParser.ParseValueExpression<User, string>(segment));
        }

        private static IEnumerable<object[]> InvalidLambdaSourceData => new[]
{
            new object[] { new LambdaMethodSegment("Any", new LambdaParameterDeclaringSegment("r", new PropertySegment(nameof(User.Id)))) }
        };

        [DataTestMethod]
        [DynamicData(nameof(InvalidLambdaSourceData))]
        public void InvalidLambdaSourceException(ValueSegment segment)
        {
            Assert.ThrowsException<InvalidLambdaSourceException>(() => _defaultParser.ParseValueExpression<User, string>(segment));
        }

        private static IEnumerable<object[]> InvalidOperationSegmentsData => new[]
{
            new object[] { new OperationSegment(new OperatorSegment(EOperator.Multiply)) },
            [new OperationSegment(new PropertySegment(nameof(User.Id)), new OperatorSegment(EOperator.EQ), new OperatorSegment(EOperator.And))],
            [new OperationSegment(new PropertySegment(nameof(User.Id)), new OperatorSegment(EOperator.EQ), new OperatorSegment(EOperator.And), new PropertySegment(nameof(User.Id)))]
        };

        [DataTestMethod]
        [DynamicData(nameof(InvalidOperationSegmentsData))]
        public void InvalidOperationSegmentsException(ValueSegment segment)
        {
            var exception = Assert.ThrowsException<InvalidOperationSegmentsException>(() => _defaultParser.ParseValueExpression<User, string>(segment));
        }

        private static IEnumerable<object[]> LambdaMethodNotMappedData => new[]
{
            new object[] { new LambdaMethodSegment("Notmapped", new LambdaParameterDeclaringSegment("r", new PropertySegment(nameof(User.Reviews)))) }
        };

        [DataTestMethod]
        [DynamicData(nameof(LambdaMethodNotMappedData))]
        public void LambdaMethodNotMappedException(ValueSegment segment)
        {
            Assert.ThrowsException<LambdaMethodNotMappedException>(() => _defaultParser.ParseValueExpression<User, string>(segment));
        }

        private static IEnumerable<object[]> MethodNotMappedData => new[]
{
            new object[] { new MethodSegment("Notmapped") },
            [new MethodSegment("Notmapped", new PropertySegment(nameof(User.Id)))]
        };

        [DataTestMethod]
        [DynamicData(nameof(MethodNotMappedData))]
        public void MethodNotMappedException(ValueSegment segment)
        {
            Assert.ThrowsException<MethodNotMappedException>(() => _defaultParser.ParseValueExpression<User, string>(segment));
        }

        private static IEnumerable<object[]> OperatorNotMappedData => new[]
        {
            new object[] { new OperationSegment(new PropertySegment(nameof(User.Id)), new OperatorSegment(EOperator.Add), new PropertySegment(nameof(User.Reviews))) }
        };

        [DataTestMethod]
        [DynamicData(nameof(OperatorNotMappedData))]
        public void OperatorNotMappedException(ValueSegment segment)
        {
            Assert.ThrowsException<OperatorNotMappedException>(() => _defaultParser.ParseValueExpression<User, string>(segment));
        }

        private static IEnumerable<object[]> PrefixDuplicateData => new[]
{
            new object[] {
                new LambdaMethodSegment(
                    name: "Any",
                    parameterDeclearingSegment: new LambdaParameterDeclaringSegment("o", new PropertySegment(nameof(User.OrderSets))),
                    arguments: new LambdaMethodSegment(
                        name: "Any",
                        parameterDeclearingSegment: new LambdaParameterDeclaringSegment("o", new PropertySegment("o", new PropertySegment(nameof(OrderSet.Orders))))))
            }
        };

        [DataTestMethod]
        [DynamicData(nameof(PrefixDuplicateData))]
        public void PrefixDuplicateException(ValueSegment segment)
        {
            Assert.ThrowsException<PrefixDuplicateException>(() => _defaultParser.ParseValueExpression<User, string>(segment));
        }
    }
}