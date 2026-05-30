namespace PamelloV7.Framework.Core.PEQL.Blocks;

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
    
    public static IEnumerable<PamelloQueryBlock> EnumerateStringBlocks(
        this string query,
        char[]? operators = null,
        bool ignoreDelimiters = false
    ) {
        operators ??= [];
        
        PamelloQueryBlock? lastBlock = null;

        List<DelimiterPosition> openedDelimiters = [];
        
        for (var i = 0; i <= query.Length; i++) {
            if (i == query.Length) {
                if (openedDelimiters.Count == 0) break;

                i = openedDelimiters.Last().Position;
                openedDelimiters.RemoveAt(openedDelimiters.Count - 1);
                
                continue;
            }

            if (TryDelimiterBlockAt(i) is { } blockAtDelimiter) {
                if (TryCreateTextBlockAt(blockAtDelimiter.Position - 1) is { } blockBeforeDelimiter) yield return blockBeforeDelimiter;
                
                yield return lastBlock = blockAtDelimiter;
                
                continue;
            }
            
            if (openedDelimiters.Count > 0 || !operators.Contains(query[i])) continue;
            
            if (TryCreateTextBlockAt(i - 1) is { } blockBeforeOperator) yield return blockBeforeOperator;
                
            yield return lastBlock = new PamelloQueryBlock(
                i,
                $"{query[i]}",
                QueryStringBlockKind.Operator
            );
        }
        
        if (TryCreateTextBlockAt(query.Length - 1) is { } endBlock) yield return endBlock;

        yield break;

        int LastBlockEndPosition() => lastBlock is not null
            ? lastBlock.Position + lastBlock.Text.Length + (lastBlock.Kind > QueryStringBlockKind.Operator ? 2 : 0)
            : 0;

        PamelloQueryBlock? TryCreateTextBlockAt(int position) {
            if (openedDelimiters.Count > 0) return null;
            
            var lastPosition = LastBlockEndPosition();
            if (position < lastPosition) return null;
            
            return lastBlock = new PamelloQueryBlock(
                lastPosition,
                query[lastPosition..(position + 1)],
                QueryStringBlockKind.Text
            );
        }

        PamelloQueryBlock? TryDelimiterBlockAt(int i) {
            if (ignoreDelimiters) return null;

            if (openedDelimiters.LastOrDefault() is { } lastOpenedDelimiter
                && lastOpenedDelimiter.Delimiter.Right == query[i]
            ) {
                openedDelimiters.RemoveAt(openedDelimiters.Count - 1);
                if (openedDelimiters.Count != 0) return null;
                
                return new PamelloQueryBlock(
                    lastOpenedDelimiter.Position,
                    query[(lastOpenedDelimiter.Position + 1)..i],
                    query[i].GetKind()
                );
            }
            if (Delimiters.FirstOrDefault(d => d.Left == query[i]) is { } newOpeningDelimiter) {
                openedDelimiters.Add(new DelimiterPosition(i, newOpeningDelimiter));
            }
            
            return null;
        }
    }

    public static (
        PamelloQueryBlock? Target,
        PamelloQueryBlock? Left,
        PamelloQueryBlock? Right
    ) ToBlocksPairAround(
        this IEnumerable<PamelloQueryBlock> blocks,
        Predicate<PamelloQueryBlock> predicate
    ) {
        PamelloQueryBlock? target = null;
        var targetSeparatedBlocks = blocks.ToSingleBlocksAround(block => {
            if (!predicate(block)) return false;
            
            target = block;
            
            return true;
        }, 2, true).Reverse().ToList();

        return (
            target,
            targetSeparatedBlocks.ElementAtOrDefault(0),
            targetSeparatedBlocks.ElementAtOrDefault(1)
        );
    }

    public static IEnumerable<PamelloQueryBlock> ToSingleBlocksAround(
        this IEnumerable<PamelloQueryBlock> blocks,
        Predicate<PamelloQueryBlock> predicate,
        int maxItems = int.MaxValue,
        bool isBackward = false
    ) {
        List<PamelloQueryBlock> currentBlocks = [];

        var leftToReturn = maxItems;
        
        blocks = isBackward ? blocks.Reverse() : blocks;

        foreach (var block in blocks) {
            if (predicate(block) && leftToReturn - 1 > 0) {
                if (currentBlocks.ToSingleBlock() is { } singleBlock) yield return singleBlock;
                leftToReturn--;
                
                currentBlocks.Clear();
                
                continue;
            }

            if (isBackward) {
                currentBlocks.Insert(0, block);
            }
            else {
                currentBlocks.Add(block);
            }
        }
        
        if (leftToReturn > 0 && currentBlocks.ToSingleBlock() is { } lastBlock) yield return lastBlock;
    }
    
    public static PamelloQueryBlock? ToSingleBlock(this IEnumerable<PamelloQueryBlock> blocks, bool dedupe = false) {
        var blocksList = blocks.ToList();
        
        switch (blocksList.Count) {
            case 0: return null;
            case > 1: return new PamelloQueryBlock(
                blocksList.First().Position,
                string.Join("", blocksList.Select(b => b.ToOriginalString())),
                QueryStringBlockKind.Text
            );
            case 1 when !dedupe: return blocksList.First();
        }

        var singleBlock = blocksList.First();
        
        if (singleBlock.Kind is QueryStringBlockKind.Text or QueryStringBlockKind.Operator) return singleBlock;

        var innerBlocks = singleBlock.Text.EnumerateStringBlocks().ToList();
        if (innerBlocks.Count == 1 && innerBlocks.First().Kind == singleBlock.Kind) return innerBlocks.ToSingleBlock();

        return singleBlock;
    }
}
