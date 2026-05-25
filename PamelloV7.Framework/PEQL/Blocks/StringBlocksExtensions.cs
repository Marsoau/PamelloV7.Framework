using PamelloV7.Framework.Shared.Variants.Attributes;

namespace PamelloV7.Framework.PEQL.Blocks;

public static class StringBlocksExtensions
{
    public record DelimiterPair(char Left, char Right);
    
    private static readonly DelimiterPair[] Delimiters = [
        new('(', ')'),
        new('[', ']'),
        new('{', '}'),
        new('<', '>'),
        new('"', '"'),
        new('\'', '\'')
    ];

    record DelimiterPosition(int Position, DelimiterPair Delimiter);
    
    public static IEnumerable<QueryStringBlock> EnumerateStringBlocks(
        this string query,
        char[]? operators = null
    ) {
        operators ??= [];
        
        QueryStringBlock? lastBlock = null;

        DelimiterPosition? openedDelimiter = null;
        
        for (var i = 0; i <= query.Length; i++) {
            if (i == query.Length) {
                if (openedDelimiter is null) break;

                i = openedDelimiter.Position;
                openedDelimiter = null;
                
                continue;
            }
            
            if (openedDelimiter is null && Delimiters.FirstOrDefault(d => d.Left == query[i]) is { } openingDelimiter) {
                openedDelimiter = new DelimiterPosition(i, openingDelimiter);
                
                continue;
            }
            if (Delimiters.FirstOrDefault(d => d.Right == query[i]) is { } closingDelimiter) {
                if (openedDelimiter is null || openedDelimiter.Delimiter != closingDelimiter) continue;
                
                var preservedDelimiter = openedDelimiter;
                openedDelimiter = null;
                
                if (TryCreateTextBlockAt(preservedDelimiter.Position - 1) is { } blockBeforeDelimiter) yield return blockBeforeDelimiter;
                
                yield return lastBlock = new QueryStringBlock(
                    preservedDelimiter.Position,
                    query[(preservedDelimiter.Position + 1)..i],
                    null,
                    query[i].GetKind()
                );
                continue;
            }

            if (openedDelimiter is not null || !operators.Contains(query[i])) continue;
            
            if (TryCreateTextBlockAt(i - 1) is { } blockBeforeOperator) yield return blockBeforeOperator;
                
            yield return lastBlock = new QueryStringBlock(
                i,
                $"{query[i]}",
                query[i],
                QueryStringBlockKind.Operator
            );
        }
        
        if (TryCreateTextBlockAt(query.Length - 1) is { } endBlock) yield return endBlock;

        yield break;

        int LastBlockEndPosition() => lastBlock is not null
            ? lastBlock.Position + lastBlock.Text.Length + (lastBlock.Kind > QueryStringBlockKind.Operator ? 2 : 0)
            : 0;

        QueryStringBlock? TryCreateTextBlockAt(int position) {
            if (openedDelimiter is not null) return null;
            
            var lastPosition = LastBlockEndPosition();
            if (position <= lastPosition) return null;
            
            return lastBlock = new QueryStringBlock(
                lastPosition,
                query[lastPosition..(position + 1)],
                null,
                QueryStringBlockKind.Text
            );
        }
    }
}
