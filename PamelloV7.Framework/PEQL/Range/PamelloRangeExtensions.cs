namespace PamelloV7.Framework.PEQL.Range;

public static class PamelloRangeExtensions
{
    public static IEnumerable<T> GetRange<T>(this ICollection<T> source, PamelloQueryRange range) {
        var startIndex = range.StartIndex(source.Count);
        var endIndex = range.EndIndex(source.Count);
        
        return source.Skip(startIndex).Take(endIndex - startIndex + 1);
    }
    public static IEnumerable<T> GetRange<T>(this IEnumerable<T> source, int count, PamelloQueryRange range) {
        var startIndex = range.StartIndex(count);
        var endIndex = range.EndIndex(count);
        
        return source.Skip(startIndex).Take(endIndex - startIndex + 1);
    }
}
