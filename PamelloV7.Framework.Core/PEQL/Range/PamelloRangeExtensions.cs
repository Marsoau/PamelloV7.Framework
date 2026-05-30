namespace PamelloV7.Framework.Core.PEQL.Range;

public static class PamelloRangeExtensions
{
    public static IEnumerable<T> GetRange<T>(this ICollection<T> source, PamelloQueryRange range) {
        return source.GetRange(source.Count, range);
    }

    public static IEnumerable<T> GetRange<T>(this IEnumerable<T> source, int count, PamelloQueryRange range) {
        return source.ToAsyncEnumerable().GetRange(count, range).ToBlockingEnumerable();
    }
    public static IAsyncEnumerable<T> GetRange<T>(this IAsyncEnumerable<T> source, int count, PamelloQueryRange range) {
        var startIndex = range.StartIndex(count);
        var endIndex = range.EndIndex(count);

        if (range is { EndValue: "random", IsEndJustCopied: true }) {
            endIndex = startIndex;
        }
        
        var isReversed = startIndex > endIndex;
        
        if (isReversed) {
            (startIndex, endIndex) = (endIndex, startIndex);
        }
        
        source = source.Skip(startIndex).Take(endIndex - startIndex + 1);
        
        if (isReversed) source = source.Reverse();

        return source;
    }
}
