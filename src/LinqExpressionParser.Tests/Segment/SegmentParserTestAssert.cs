using Newtonsoft.Json;

namespace LinqExpressionParser.Tests.Segment
{
    public static class SegmentParserTestAssert
    {
        public static void AreEqual(SegmentBase expected, SegmentBase? actual, string text)
        {
            Type expectedType = expected.GetType();
            Type? actualType = actual?.GetType();
            if (expectedType != actualType)
            {
                Assert.Fail($"Type not match. expected type: {expectedType.Name}, actual type: {actualType?.Name}, text: {text}");
            }

            string expectedJson = JsonConvert.SerializeObject(expected);
            string actualJson = JsonConvert.SerializeObject(actual);

            if (expectedJson != actualJson)
            {
                throw new AssertFailedException(
$$"""
Text: {{text}}
Expected: 
{{expectedJson}}
Actual: 
{{actualJson}}
"""
                    );
            }
        }
    }
}