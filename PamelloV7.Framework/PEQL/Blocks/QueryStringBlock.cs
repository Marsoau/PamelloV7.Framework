namespace PamelloV7.Framework.PEQL.Blocks;

public enum QueryStringBlockKind
{
    Text,
    Operator,
    InParentheses,
    InBrackets,
    InBraces,
    InAngleBrackets,
    InDoubleQuotes,
    InSingleQuotes,
}

public record QueryStringBlock(
    int Position,
    string Text,
    char? Operator,
    QueryStringBlockKind Kind
)
{
    public override string ToString() => $"{Kind}[{Position}]{Text}";
};

public static class QueryStringBlockExtensions
{
    public static QueryStringBlockKind GetKind(this char c) => c switch {
        '(' or ')' => QueryStringBlockKind.InParentheses,
        '[' or ']' => QueryStringBlockKind.InBrackets,
        '{' or '}' => QueryStringBlockKind.InBraces,
        '<' or '>' => QueryStringBlockKind.InAngleBrackets,
        '"' => QueryStringBlockKind.InDoubleQuotes,
        '\'' => QueryStringBlockKind.InSingleQuotes,
        _ => QueryStringBlockKind.Text
    };
}