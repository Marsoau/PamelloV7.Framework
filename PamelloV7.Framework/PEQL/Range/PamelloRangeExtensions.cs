namespace PamelloV7.Framework.PEQL.Range;

public static class PamelloRangeExtensions
{
    public static IEnumerable<T> GetRange<T>(this ICollection<T> source, PamelloQueryRange range) {
        return source.GetRange(source.Count, range);
    }
    public static IEnumerable<T> GetRange<T>(this IEnumerable<T> source, int count, PamelloQueryRange range) {
        var startIndex = range.StartIndex(count);
        var endIndex = range.EndIndex(count);
        
        var isReversed = startIndex > endIndex;
        
        if (isReversed) {
            (startIndex, endIndex) = (endIndex, startIndex);
        }
        
        source = source.Skip(startIndex).Take(endIndex - startIndex + 1);
        
        if (isReversed) source = source.Reverse();

        return source;
    }
}
