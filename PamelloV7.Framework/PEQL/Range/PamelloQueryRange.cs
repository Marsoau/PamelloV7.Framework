using PamelloV7.Framework.PEQL.Blocks;

namespace PamelloV7.Framework.PEQL.Range;

public class PamelloQueryRange
{
    public string StartValue { get; set; }
    public int StartNumber => int.Parse(StartValue);
    
    public string EndValue { get; set; }
    public int EndNumber => int.Parse(EndValue);
    
    public bool IsPurelyNumeric => int.TryParse(StartValue, out _) && int.TryParse(EndValue, out _);

    public PamelloQueryRange(string startValue, string endValue) {
        StartValue = startValue;
        EndValue = endValue;
    }

    public static PamelloQueryRange Parse(string range) {
        var blocks = range
            .EnumerateStringBlocks(['-'])
            .ToSingleBlocksAround(b => b.Kind == QueryStringBlockKind.Operator)
            .ToList();
        
        return new PamelloQueryRange(
            blocks[0].Text,
            blocks.Count == 1
                ? blocks[0].Text
                : blocks[1].Text
        );
    }

    public IEnumerable<int> EnumerateNumericRange() {
        var startNumber = StartNumber;
        var endNumber = EndNumber;

        if (startNumber > endNumber) {
            (startNumber, endNumber) = (endNumber, startNumber);
        }
        
        var range = Enumerable.Range(startNumber, endNumber - startNumber + 1);
        
        if (StartNumber > EndNumber) return range.Reverse();
        
        return range;
    }
    
    private static int IndexOfSingleRange<T>(IEnumerable<T> enumerable, string singleRange) {
        if (int.TryParse(singleRange, out var index)) {
            if (index == 0) return 0;

            var count = enumerable.Count();

            if (index < 0) {
                return count - (-index - 1) % count;
            }

            return index % count;
        }
        
        throw new NotSupportedException();
    }
    
    public override string ToString() => $"{StartValue}-{EndValue}";
}
