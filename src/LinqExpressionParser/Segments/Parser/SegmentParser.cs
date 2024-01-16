using LinqExpressionParser.Extensions;
using LinqExpressionParser.Segments;
using LinqExpressionParser.Segments.Enums;
using LinqExpressionParser.Segments.Exceptions;
using LinqExpressionParser.Segments.Parser;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace AspNetCore.ExpressionParse.Segments.Parser;

public partial class SegmentParser(ILogger<SegmentParser> logger) : ISegmentParser
{
    private const char PARAMETER_PREFIX_CHARACTER = ':';

    [GeneratedRegex($@"^'(.*?(?<!\\))'", RegexOptions.Singleline)]
    private static partial Regex GetStringRegex();

    [GeneratedRegex(@"\\['\\abfnrtv]")]
    private static partial Regex GetEscapeCharacterRegex();

    /// <inheritdoc/>
    public SelectorSegment ParseSelector(string text)
    {
        ReadOnlySpan<char> textSpan = text.AsSpan().Trim();
        List<Range> argumentRanges = GetArgumentRanges(textSpan);
        Dictionary<string, ValueSegment> valueByName = new(argumentRanges.Count);

        foreach (var range in argumentRanges)
        {
            ReadOnlySpan<char> argumentSpan = textSpan[range];
            int assignmentIndex = argumentSpan.IndexOf('=');

            string name;
            ValueSegment value;
            if (assignmentIndex < 1)
            {
                try
                {
                    value = ParseValue(argumentSpan);
                }
                catch (Exception e) when (e is not SegmentParseExceptionBase)
                {
                    throw new UndefinedSegmentParseException(e);
                }

                if (value is not PropertySegment p)
                {
                    throw new SegmentFormatException(ESegmentFormatError.UnnamedSelectItem, text);
                }

                name = p.Name;
            }
            else
            {
                name = argumentSpan[..(assignmentIndex - 1)].Trim().ToString();
                var valueSpan = argumentSpan[(assignmentIndex + 1)..].Trim();

                try
                {
                    value = ParseValue(valueSpan);
                }
                catch (Exception e) when (e is not SegmentParseExceptionBase)
                {
                    throw new UndefinedSegmentParseException(e);
                }
            }

            valueByName.Add(name, value);
        }

        SelectorSegment selectorSegment = new(valueByName);
        return selectorSegment;
    }

    /// <inheritdoc/>
    public bool TryParseSelector(string text, [NotNullWhen(true)] out SelectorSegment? result)
    {
        try
        {
            result = ParseSelector(text);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }

    /// <inheritdoc/>
    public ValueSegment ParseValue(string text)
    {
        ReadOnlySpan<char> textSpan = text.AsSpan();
        try
        {
            return ParseValue(textSpan);
        }
        catch (Exception e) when (e is not SegmentParseExceptionBase)
        {
            throw new UndefinedSegmentParseException(e);
        }
    }

    /// <inheritdoc/>
    public bool TryParseValue(string text, [NotNullWhen(true)] out ValueSegment? result) => TryParseValue(text.AsSpan().Trim(), out result);

    private ValueSegment ParseValue(ReadOnlySpan<char> textSpan)
    {
        textSpan = textSpan.Trim();
        List<SegmentBase> segments = [];
        logger.LogDebug("Start parsing. text: '{text}'", textSpan.ToString());

        while (!textSpan.IsEmpty)
        {
            if (TryGetParentheses(textSpan, out (int length, SegmentBase? segment) result) || TryGetConstant(textSpan, out result) || TryGetMethod(textSpan, out result) || TryGetOperator(textSpan, out result) || TryGetProperty(textSpan, out result))
            {
                Debug.Assert(result.segment is not null);
            }
            else
            {
                Debug.Assert(result.segment is null);
                throw new SegmentFormatException(ESegmentFormatError.InvalidText, textSpan.ToString());
            }

            logger.LogDebug("Segment parsed: '{segment}', segment type: {type}", textSpan[..result.length].ToString(), result.segment.GetType().FullName);

            textSpan = textSpan[result.length..].TrimStart();
            segments.Add(result.segment);
        }

        logger.LogDebug("End parsing. text: '{text}'", textSpan.ToString());

        if (segments.Count == 1)
        {
            return segments[0] as ValueSegment ?? throw new SegmentFormatException(ESegmentFormatError.InvalidText, textSpan.ToString());
        }
        else
        {
            (ESegmentFormatError error, OperationSegment? result) = OperationSegment.Factory.Create(segments);
            if (result is null)
            {
                throw new SegmentFormatException(error, textSpan.ToString());
            }
            return result;
        }
    }

    private bool TryParseValue(ReadOnlySpan<char> textSpan, [NotNullWhen(true)] out ValueSegment? result)
    {
        try
        {
            result = ParseValue(textSpan);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }

    private bool TryGetParentheses(ReadOnlySpan<char> textSpan, [NotNullWhen(true)] out (int length, SegmentBase?) result)
    {
        int length = GetParenthesesLength(textSpan);
        if (length < 0)
        {
            result = (0, null);
            return false;
        }

        if (textSpan.IsEmpty)
        {
            result = (0, null);
            return false;
        }

        if (length < 3)
        {
            result = (0, null);
            return false;
        }

        if (!TryParseValue(textSpan[1..(length - 1)], out ValueSegment? value))
        {
            result = (0, null);
            return false;
        }

        result = (length, new ParenthesesSegment(value));
        return true;
    }

    private bool TryGetConstant(ReadOnlySpan<char> textSpan, [NotNullWhen(true)] out (int length, SegmentBase?) result)
    {
        string text = textSpan.ToString();
        Match match = GetStringRegex().Match(text);
        if (match.Success)
        {
            string parsedText = GetEscapeCharacterRegex().Replace(match.Groups[1].Value, match =>
            {
                if (match.Length != 2 || match.Value[0] != '\\')
                {
                    throw new ArgumentException("Escape character length must be 2 and start with '\\'", nameof(match));
                }

                return match.Value[1] switch
                {
                    '\'' => "\'",
                    '\\' => "\\",
                    'a' => "\a",
                    'b' => "\b",
                    'f' => "\f",
                    'n' => "\n",
                    'r' => "\r",
                    't' => "\t",
                    'v' => "\v",
                    _ => throw new ArgumentException($"'{match.Value}' is not escape character", nameof(match))
                };
            });
            result = (match.Length, new ConstantSegment(parsedText));
            return true;
        }

        if (text == "NULL")
        {
            result = (4, new ConstantSegment());
            return true;
        }

        if (text == "TRUE")
        {
            result = (4, new ConstantSegment(true));
            return true;
        }

        if (text == "FALSE")
        {
            result = (4, new ConstantSegment(false));
            return true;
        }

        string numberText = textSpan.GetStringWhile(c => !char.IsWhiteSpace(c));
        if (numberText.Length == 0)
        {
            result = (0, null);
            return false;
        }

        if (int.TryParse(numberText, out int intValue))
        {
            result = (numberText.Length, new ConstantSegment(intValue));
            return true;
        }

        if (double.TryParse(numberText, out double doubleValue))
        {
            result = (numberText.Length, new ConstantSegment(doubleValue));
            return true;
        }

        result = (0, null);
        return false;
    }

    private bool TryGetMethod(ReadOnlySpan<char> textSpan, [NotNullWhen(true)] out (int length, SegmentBase?) result)
    {
        if (!textSpan.Contains('('))
        {
            result = (0, null);
            return false;
        }

        int nameLength = textSpan.GetLengthWhile(c => !char.IsWhiteSpace(c) && c != '(');
        int argumentsLength = GetParenthesesLength(textSpan[nameLength..]);
        if (nameLength < 1 || argumentsLength < 2)
        {
            result = (0, null);
            return false;
        }

        string methodName = textSpan[..nameLength].ToString();
        if (string.IsNullOrEmpty(methodName))
        {
            result = (0, null);
            return false;
        }

        int segmentLength = nameLength + argumentsLength;
        ReadOnlySpan<char> argumentsSpan = textSpan[(nameLength + 1)..(segmentLength - 1)].Trim();
        if (argumentsSpan.IsWhiteSpace())
        {
            result = (segmentLength, new MethodSegment(methodName));
            return true;
        }

        IEnumerable<Range> argumentRanges = GetArgumentRanges(argumentsSpan);
        bool isParameterParsed = TryGetLambdaParameter(argumentsSpan, argumentRanges.First(), out LambdaParameterDeclaringSegment? parameterSegment);
        if (isParameterParsed)
        {
            argumentRanges = argumentRanges.Skip(1);
        }

        List<ValueSegment> argumentValues = new(argumentRanges.Count());
        foreach (var range in argumentRanges)
        {
            ReadOnlySpan<char> argumentSpan = argumentsSpan[range];
            ValueSegment value = ParseValue(argumentSpan);
            argumentValues.Add(value);
        }

        if (isParameterParsed)
        {
            result = (segmentLength, new LambdaMethodSegment(methodName, parameterSegment!, argumentValues));
        }
        else
        {
            result = (segmentLength, new MethodSegment(methodName, argumentValues));
        }

        return true;
    }

    private bool TryGetLambdaParameter(ReadOnlySpan<char> textSpan, Range range, [NotNullWhen(true)] out LambdaParameterDeclaringSegment? result)
    {
        ReadOnlySpan<char> lambdaSpan = textSpan[range];
        int prefixIndex = lambdaSpan.IndexOf(PARAMETER_PREFIX_CHARACTER);
        if (prefixIndex < 1)
        {
            result = null;
            return false;
        }

        ReadOnlySpan<char> propertyTextSpan = lambdaSpan[..prefixIndex];

        if (!TryGetProperty(propertyTextSpan, out (int length, SegmentBase? segment) propertySegment))
        {
            result = null;
            return false;
        }

        string prefixText = lambdaSpan[(prefixIndex + 1)..].Trim().ToString();
        result = new LambdaParameterDeclaringSegment(prefixText, (PropertySegment)propertySegment.segment!);
        return true;
    }

    private bool TryGetOperator(ReadOnlySpan<char> textSpan, out (int length, SegmentBase?) result)
    {
        string operatorName = textSpan.GetStringWhile(c => !char.IsWhiteSpace(c));

        OperatorSegment? segment;
        if (Enum.TryParse(operatorName, true, out EOperator @operator))
        {
            segment = @operator switch
            {
                EOperator.EQ => OperatorSegment.EQOperator,
                EOperator.NQ => OperatorSegment.NQOperator,
                EOperator.GT => OperatorSegment.GTOperator,
                EOperator.GTE => OperatorSegment.GTEOperator,
                EOperator.LT => OperatorSegment.LTOperator,
                EOperator.LTE => OperatorSegment.LTEOperator,
                EOperator.And => OperatorSegment.AndOperator,
                EOperator.Or => OperatorSegment.OrOperator,
                _ => throw new InvalidEnumArgumentException(nameof(@operator), (int)@operator, typeof(EOperator))
            };

            result = (operatorName.Length, segment);
            return true;
        }

        segment = operatorName switch
        {
            "+" => OperatorSegment.AddOperator,
            "-" => OperatorSegment.SubtractOperator,
            "*" => OperatorSegment.MultiplyOperator,
            "/" => OperatorSegment.DivideOperator,
            _ => null
        };

        if (segment is null)
        {
            result = (0, null);
            return false;
        }
        else
        {
            result = (operatorName.Length, segment);
            return true;
        }
    }

    private bool TryGetProperty(ReadOnlySpan<char> textSpan, out (int length, SegmentBase?) result)
    {
        string propertyChains = textSpan.GetStringWhile(c => !char.IsWhiteSpace(c) && (char.IsAsciiLetter(c) || c == '.' || c == '\\'));
        if (string.IsNullOrWhiteSpace(propertyChains))
        {
            result = (0, null);
            return false;
        }

        IEnumerable<StringSegment> tokens = new StringTokenizer(propertyChains, ['.']).Reverse();
        Debug.Assert(tokens.Any());

        PropertySegment propertySegment = new(tokens.First().Value!);
        foreach (var token in tokens.Skip(1))
        {
            propertySegment = new(token.Value!, propertySegment);
        }

        result = (propertyChains.Length, propertySegment);
        return true;
    }

    /// <summary>
    /// Get parentheses length include '(', ')'
    /// </summary>
    /// <param name="textSpan"></param>
    /// <returns>Length of parentheses that include '(', ')'. return -1 If <paramref name="textSpan"/> is empty or parentheses not closed</returns>
    private int GetParenthesesLength(ReadOnlySpan<char> textSpan)
    {
        if (textSpan.IsEmpty)
        {
            return -1;
        }

        if (textSpan[0] != '(')
        {
            return -1;
        }

        int length = 1;
        int openCount = 1;
        while (openCount > 0 && textSpan.Length > length)
        {
            char c = textSpan[length];

            switch (c)
            {
                case '(':
                    ++openCount;
                    break;
                case ')':
                    --openCount;
                    break;
                case '\'':
                    string stringSegmentStart = textSpan[length..].ToString();

                    Match stringSegmentMatch = GetStringRegex().Match(stringSegmentStart);
                    if (!stringSegmentMatch.Success)
                    {
                        throw new SegmentFormatException(ESegmentFormatError.InvalidText, textSpan.ToString());
                    }

                    int stringLength = stringSegmentMatch.Length;
                    length += stringLength;
                    continue;
            }

            ++length;
        }

        return openCount > 0 ? -1 : length;
    }

    private List<Range> GetArgumentRanges(ReadOnlySpan<char> argumentsSpan)
    {
        List<Range> ranges = [];
        int start = 0;
        int end = 0;

        while (end < argumentsSpan.Length)
        {
            char c = argumentsSpan[end];
            switch (c)
            {
                case '\'':
                    Match stringMatch = GetStringRegex().Match(argumentsSpan[end..].ToString());
                    if (stringMatch.Success)
                    {
                        end += GetStringRegex().Match(argumentsSpan[end..].ToString()).Length;
                    }
                    else
                    {
                        throw new SegmentFormatException(ESegmentFormatError.InvalidText, argumentsSpan.ToString());
                    }
                    break;
                case ',':
                    ranges.Add(start..end);
                    start = end + argumentsSpan[end..].GetLengthWhile(c => char.IsWhiteSpace(c) || c == ',');
                    end = start;
                    break;
                case '(':
                    int parenthesesLength = GetParenthesesLength(argumentsSpan[end..]);
                    if (parenthesesLength < 0)
                    {
                        throw new SegmentFormatException(ESegmentFormatError.InvalidText, argumentsSpan.ToString());
                    }
                    end += parenthesesLength;
                    break;
                default:
                    ++end;
                    break;
            }
        }

        if (start != end)
        {
            ranges.Add(start..end);
        }

        return ranges;
    }
}