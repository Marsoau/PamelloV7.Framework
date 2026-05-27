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

    public static implicit operator PamelloQueryRange(string s) => Parse(s);
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
    
    public int StartIndex(int count, bool includeLastEmpty = false) => IndexOfRangePoint(count, StartValue, includeLastEmpty);
    public int EndIndex(int count, bool includeLastEmpty = false) => IndexOfRangePoint(count, EndValue, includeLastEmpty);
    
    public static int IndexOfRangePoint(int count, string rangePoint, bool includeLastEmpty = false) {
        if (int.TryParse(rangePoint, out var index)) {
            return index switch {
                0 => 0,
                < 0 => count - (-index - 1) % count,
                _ => (index - 1) % count
            };
        }
        
        return rangePoint switch {
            "first" or "start" => 0,
            "last" or "end" => includeLastEmpty ? count : count - 1,
            "random" => Random.Shared.Next(count),
            "current" => GetCurrent(),
            "next" => (GetCurrent() + 1) % count,
            "prev" or "previous" => GetCurrent() is var current && current != 0
                ? current - 1
                : count - 1,
            _ => throw new NotImplementedException()
        };
        
        int GetCurrent() => throw new NotImplementedException();
    }
    
    public override string ToString() => $"{StartValue}-{EndValue}";
}
