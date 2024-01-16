namespace LinqExpressionParser.Extensions;

internal static class ReadOnlySpanExtensions
{
    public static ReadOnlySpan<T> SliceWhile<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate)
    {
        int length = span.GetLengthWhile(predicate);
        return span[..length];
    }

    public static string GetStringWhile(this ReadOnlySpan<char> span, Func<char, bool> predicate)
    {
        return span.SliceWhile(predicate).ToString();
    }

    public static int GetLengthWhile<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate)
    {
        int length = 0;
        while (span.Length > length && predicate(span[length]))
        {
            ++length;
        }
        return length;
    }
}