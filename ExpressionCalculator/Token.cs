namespace ExpressionCalculator;

public enum TokenType
{
    Number,
    Plus,
    Minus,
    Multiply,
    Divide,
    LeftParen,
    RightParen,
    EndOfExpression,
    Unknown
}

public record Token(TokenType Type, string Text, object? Value);
