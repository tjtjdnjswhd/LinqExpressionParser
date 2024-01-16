using AspNetCore.ExpressionParse.Segments.Parser;

using LinqExpressionParser.Segments.Enums;
using LinqExpressionParser.Segments.Exceptions;

using Microsoft.Extensions.Logging.Abstractions;

[assembly: TestDataSourceDiscovery(TestDataSourceDiscoveryOption.DuringExecution)]
namespace LinqExpressionParser.Tests.Segment;

[TestClass]
public class SegmentParserTests
{
    private readonly SegmentParser _parser = new(NullLogger<SegmentParser>.Instance);

    public static IEnumerable<object[]> SegmentFormatExceptionTestData => new[]
    {
        //text
        new object[] {"((Name)", ESegmentFormatError.InvalidText },
        ["'a EQ b", ESegmentFormatError.InvalidText],
        ["Id EQ 823 OR (Name EQ 6" , ESegmentFormatError.InvalidText],
        ["Any(Items:i, i.Id EQ EQ", ESegmentFormatError.InvalidText],
        ["(Id EQ 823 OR (Name EQ 6 (OR Name EQ 3)))" , ESegmentFormatError.InvalidText],
        ["'''", ESegmentFormatError.InvalidText],
        //operator
        ["EQ Id 3", ESegmentFormatError.InvalidOperatorIndex],
        ["Id EQ NEQ 3", ESegmentFormatError.InvalidOperationSegmentCount],
        ["Id EQ 3 OR", ESegmentFormatError.InvalidOperationSegmentCount],
    };

    [DataTestMethod]
    [DynamicData(nameof(SegmentFormatExceptionTestData))]
    public void SegmentFormatExceptionTest(string text, ESegmentFormatError error)
    {
        var exception = Assert.ThrowsException<SegmentFormatException>(() => _parser.ParseValue(text));
        Assert.AreEqual(error, exception.Error);
    }

    public static IEnumerable<object[]> OperationSegmentTestData => new[]
    {
        new object[] {
            "Id EQ 6 OR Name EQ '3'",
            new OperationSegment(
                new PropertySegment("Id"),
                new OperatorSegment(EOperator.EQ),
                new ConstantSegment(6),
                new OperatorSegment(EOperator.Or),
                new PropertySegment("Name"),
                new OperatorSegment(EOperator.EQ),
                new ConstantSegment("3")
            )
        },
        [
            " Name EQ '\\n\\tabc' And ( (Id GT 555 Or Item.Price LTE 100) And (Id LTE 665 And Name EQ 'asddsa') ) Or Max(Books:b, b.Id * (b.Id + 3) + Min(b.Items:i, i.Id)) GT 100  ",
            new OperationSegment(
                new PropertySegment("Name"),
                new OperatorSegment(EOperator.EQ),
                new ConstantSegment("\n\tabc"),
                new OperatorSegment(EOperator.And),
                new ParenthesesSegment(
                    new OperationSegment(
                        new ParenthesesSegment(
                            new OperationSegment(
                                new PropertySegment("Id"),
                                new OperatorSegment(EOperator.GT),
                                new ConstantSegment(555),
                                new OperatorSegment(EOperator.Or),
                                new PropertySegment("Item", new PropertySegment("Price")),
                                new OperatorSegment(EOperator.LTE),
                                new ConstantSegment(100)
                            )
                        ),
                        new OperatorSegment(EOperator.And),
                        new ParenthesesSegment(
                            new OperationSegment(
                                new PropertySegment("Id"),
                                new OperatorSegment(EOperator.LTE),
                                new ConstantSegment(665),
                                new OperatorSegment(EOperator.And),
                                new PropertySegment("Name"),
                                new OperatorSegment(EOperator.EQ),
                                new ConstantSegment("asddsa")
                            )
                        )
                    )
                ),
                new OperatorSegment(EOperator.Or),
                new LambdaMethodSegment("Max",
                    new LambdaParameterDeclaringSegment("b", new PropertySegment("Books")),
                    new List<ValueSegment>()
                    {
                        new OperationSegment(
                            new PropertySegment("b", new PropertySegment("Id")),
                            new OperatorSegment(EOperator.Multiply),
                            new ParenthesesSegment(
                                new OperationSegment(
                                    new PropertySegment("b", new PropertySegment("Id")),
                                    new OperatorSegment(EOperator.Add),
                                    new ConstantSegment(3)
                                )
                            ),
                            new OperatorSegment(EOperator.Add),
                            new LambdaMethodSegment("Min",
                                new LambdaParameterDeclaringSegment("i", new PropertySegment("b", new PropertySegment("Items"))),
                                new List<ValueSegment>()
                                {
                                    new PropertySegment("i", new PropertySegment("Id"))
                                }
                            )
                        )
                    }
                ),
                new OperatorSegment(EOperator.GT),
                new ConstantSegment(100)
            )
        ]
    };

    [DataTestMethod]
    [DynamicData(nameof(OperationSegmentTestData))]
    public void OperationSegmentTest(string text, ValueSegment expected)
    {
        //Act
        ValueSegment actual = _parser.ParseValue(text);

        //Assert
        SegmentParserTestAssert.AreEqual(expected, actual, text);
    }

    public static IEnumerable<object[]> PropertySegmentTestData => new[]
    {
        new object[] { "  \\EQ" , new PropertySegment("\\EQ") },
        ["  \\EQ.\\And" , new PropertySegment("\\EQ", new PropertySegment("\\And"))],
        ["  \\EQ.\\And.\\OR" , new PropertySegment("\\EQ", new PropertySegment("\\And", new PropertySegment("\\OR")))],
        ["Id  ", new PropertySegment("Id")],
        [" User.Name  ", new PropertySegment("User", new PropertySegment("Name"))],
        [" User.Item.Name  ", new PropertySegment("User", new PropertySegment("Item", new PropertySegment("Name")))]
    };

    [DataTestMethod]
    [DynamicData(nameof(PropertySegmentTestData))]
    public void PropertySegmentTest(string text, PropertySegment expected)
    {
        //Act
        ValueSegment actual = _parser.ParseValue(text);

        //Assert
        Assert.IsNotNull(actual);
        SegmentParserTestAssert.AreEqual(expected, actual, text);
    }

    public static IEnumerable<object[]> ConstantSegmentTestData => new[]
    {
        new object[] { "  -10", new ConstantSegment(-10) },
        [" 35  ", new ConstantSegment(35)],
        ["  12.345  ", new ConstantSegment(12.345)],
        [" -12.340", new ConstantSegment(-12.340)],
        ["  '  aa'" , new ConstantSegment("  aa")],
        ["'\\''", new ConstantSegment("'")],
        ["'\\a\\b\\' abc'", new ConstantSegment("\a\b' abc")],
        ["'(AA EQ 6)'", new ConstantSegment("(AA EQ 6)")],
        ["' ( )'", new ConstantSegment(" ( )")],
        ["' ('", new ConstantSegment(" (")]
    };

    [DataTestMethod]
    [DynamicData(nameof(ConstantSegmentTestData))]
    public void ConstantSegmentTest(string text, ConstantSegment expected)
    {
        //Act
        ValueSegment actual = _parser.ParseValue(text);

        //Assert
        SegmentParserTestAssert.AreEqual(expected, actual, text);
    }

    public static IEnumerable<object[]> MethodSegmentTestData => new[]
    {
        new object[] {
            "Max(  book:b  ,   b.Id + 3  )",
            new LambdaMethodSegment("Max",
                new LambdaParameterDeclaringSegment("b", new PropertySegment("book")),
                new List<ValueSegment>()
                {
                    new OperationSegment(
                        new PropertySegment("b", new PropertySegment("Id")),
                        new OperatorSegment(EOperator.Add),
                        new ConstantSegment(3))
                })
        },
        [
            "IndexOf(  Name, 'a,b,c\\a'  )",
            new MethodSegment("IndexOf",
                new List<ValueSegment>()
                {
                    new PropertySegment("Name"),
                    new ConstantSegment("a,b,c\a")
                }
            )
        ],
        [
            "DateNow()",
            new MethodSegment("DateNow")
        ]
    };

    [DataTestMethod]
    [DynamicData(nameof(MethodSegmentTestData))]
    public void MethodSegmentTest(string text, SegmentBase expected)
    {
        //Act
        var actual = _parser.ParseValue(text);

        SegmentParserTestAssert.AreEqual(expected, actual, text);
    }

    public static IEnumerable<object[]> SelectorSegmentTestData => new[]
    {
        new object[]
        {
            "  a, b,   ,c,d  ",
            new SelectorSegment(
                new Dictionary<string, ValueSegment>()
                {
                    { "a", new PropertySegment("a") }, { "b", new PropertySegment("b") }, { "c", new PropertySegment("c") }, { "d", new PropertySegment("d") }
                }
            )
        },

        [
            "  Id = A, MaxItemId = Max(Items:i, i.Id), C = Substring(Name, 6), D = Id * 6 + Price, Name  ",
            new SelectorSegment(
                new Dictionary<string, ValueSegment>()
                {
                    { "Id", new PropertySegment("A") },
                    { "MaxItemId", new LambdaMethodSegment("Max", new LambdaParameterDeclaringSegment("i", new PropertySegment("Items")), new List<ValueSegment>() { new PropertySegment("i", new PropertySegment("Id")) }) },
                    { "C", new MethodSegment("Substring", new List<ValueSegment>() { new PropertySegment("Name"), new ConstantSegment(6) }) },
                    { "D", new OperationSegment(new PropertySegment("Id"), new OperatorSegment(EOperator.Multiply), new ConstantSegment(6), new OperatorSegment(EOperator.Add), new PropertySegment("Price")) },
                    { "Name", new PropertySegment("Name") }
                }
            )
        ]
    };

    [DataTestMethod]
    [DynamicData(nameof(SelectorSegmentTestData))]
    public void SelectorSegmentTest(string text, SelectorSegment expected)
    {
        //Act
        var actual = _parser.ParseSelector(text);

        SegmentParserTestAssert.AreEqual(expected, actual, text);
    }

    public static IEnumerable<object[]> SelectorSegmentFormatExceptionTestData => new[]
    {
        new object[] { "Id == A", ESegmentFormatError.InvalidText },
        ["Id = = A", ESegmentFormatError.InvalidText],
        ["= Id = A", ESegmentFormatError.InvalidText],
        ["= Id A", ESegmentFormatError.InvalidText],
        ["Substring(Name, 6)", ESegmentFormatError.UnnamedSelectItem]
    };

    [DataTestMethod]
    [DynamicData(nameof(SelectorSegmentFormatExceptionTestData))]
    public void SelectorSegmentFormatExceptionTest(string text, ESegmentFormatError error)
    {
        var exception = Assert.ThrowsException<SegmentFormatException>(() => _parser.ParseSelector(text));
        Assert.AreEqual(error, exception.Error);
    }
}