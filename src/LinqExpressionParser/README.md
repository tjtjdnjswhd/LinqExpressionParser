# LinqExpressionParser

**LinqExpressionParser** is class library project that you can use in your app to parse `System.Linq.Expressions.Expression` from string

## Dependencies

- .NET 7.0 ~ 8.0

## Getting start

```csharp
ISegmentParser segmentParser = new SegmentParser(NullLogger<SegmentParser>.Instance);
IExpressionParser expressionParser = new ExpressionParser(NullLogger<ExpressionParser>.Instance, Options.Create<ExpressionParser>(...));

ValueSegment segment = segmentParser.ParseValue("Substring(Name, 5)");

Expression<Func<User, string>> exp = expressionParser.ParseValueExpression<User, string>(segment);

public class User
{
    public string Name { get; set; }
}
```

First, SegmentParser parse segment from string.
And ExpressionParse parse LambdaExpression from parsed segment

## Version history

- 0.1
  - Initial release

## Parsing

### SegmentParser

A class that parse string to ValueSegment or SelectorSegment.

```csharp
ILogger<SegmentParser> logger = ...;
SegmentParser segmentParser = new(logger)

//ConstantSegment: "ABC\n\t"
ValueSegment stringSegment = segmentParser.ParseValue("ABC\\n\\t");

//ConstantSegment: 1.235
ValueSegment doubleSegment = segmentParser.ParseValue("1.235");

//PropertySegment
ValueSegment propertySegment = segmentParser.ParseValue("Id");

//MethodSegment: [Name: Substring, Arg: Name, 3]
ValueSegment methodSegment = segmentParser.ParseValue("Substring(Name, 3)");

//LambdaMethodSegment: [Name: Any, Parameter: Items:i, Arg: i.Price NEQ 6]
ValueSegment lambdaMethodSegment = segmentParser.ParseValue("Any(Items:i, i.Price NEQ 6)");

//OperationSegment: [Id, GTE, 6]
ValueSegment idGTESixSegment = segmentParser.ParseValue("Id GTE 6");

//OperationSegment: [IsExist EQ (Contains(Name, 'a'))]
ValueSegment complexSegment = segmentParser.ParseValue("IsExist EQ (Contains(Name, 'a'))");

//SelectorSegment
SelectorSegment selectorSegment = segmentParser.ParseSelector("IsIdGTFive = Id GT 5, Name, Items");
```

- Constant
  - A constant value
  - type: int, double, string, bool, null
  - bool, null value must be upper case
  - string value can contain escape character
    - allowed escape character: \\, \\', \a, \b, \f, \n, \r, \t, \v
  - ex) "56", "12.345", "'abc\r\n'", "TRUE", "FALSE", "NULL"

- Property
  - A name of property
  - if property name is 'NULL', 'TRUE', operator name, keyword or number, must start with '\\'
    - ex) "\\TRUE", "\\NULL", "\\2354", "\\123.456"

- Method
  - A method may contain arguments
  - argument can be constant, property, method, lambdaMethod, parantheses, operation
  - ex) "Contains(FirstName, Substring(LastName, '3'))"

- LambdaMethod:
  - A lambda method that declared parameter and may contain arguments
  - argument can be constant, property, method, lambdaMethod, parantheses, operation
  - parameter must declared by IEnumerable\<T> type property
  - ex) "Any(Items:i, i.Price GTE 5)"

- Operator:
  - A name of operator
  - Add, Subtract, Divide, Multiply: "+", "-", "/", "*"
  - EQ, NQ, GT, GTE, LT, LTE, And, Or
  - see [EOperator](Segments/Enums/EOperator.cs)

- Parantheses:
  - A value of single segment or operation
  - ex) "(Count(Items:i))", "Id GTE 6 AND (Price LTE 100 OR Price GTE 1000)"

- Operation:
  - ex) "123 + 456 - Name.Length"

see [SegmentTest](../LinqExpressionParser.Tests/Segment/SegmentParserTests.cs) for more sample.

### ExpressionParser

A class that parse Segment to LambdaExpression

```csharp
ILogger<ExpressionParser> logger = ...;
ExpressionParserOptions options = new()
{
    MethodMapOptions = MethodMapOptions.Default
    OperatorMapOptions = OperatorMapOptions.Default
}

ExpressionParser expressionParser = new(logger, options);

//u => u.Items.Any(i => i.Price != 6)
Expression<Func<User, bool>> itemsAnyExpression = expressionParser.ParseValueExpression<User, bool>(lambdaMethodSegment);

//u => u.Id >= 6
Expression<Func<User, bool>> idGTESixExpression = expressionParser.ParseValueExpression<User, bool>(idGTESixSegment);

//u => new { IsIdGTFive = u.Id > 5, Name, Items }
LambdaExpression selectorExpression = expressionParser.ParseSelectorExpression<User>(selectorSegment);
```

You can configure MethodMapOptions, OperatorMapOptions.
Throwing exception in delegate is not recommend

```csharp
MethodMapOptions methodMapOptions = MethodMapOptions.Default;

methodMapOptions.MethodMap.Remove("Substring");
methodMapOptions.MethodMap["AVG"].Add(args => {
    //Check args and return null or Expression
});

methodMapOptions.MethodMap.Add("SomeMethod", new List<GetMethodCallExpressionDelegate>()
{
    args => 
    {
        //Check args and return null or Expression
    },
    args =>
    {
        //Check args and return null or Expression
    }
});
```

```csharp
OperatorMapOptions operatorMapOptions = OperatorMapOptions.Default;
operatorMapOptions.OperatorMap[EOperator.EQ].Add((left, right) => {
    //Check args and return null or Expression
});
```
