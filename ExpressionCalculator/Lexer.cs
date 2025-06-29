using System;
using System.Collections.Generic;
using System.Globalization;

namespace ExpressionCalculator;

public class Lexer
{
    private readonly string _text;
    private int _position;
    private char CurrentChar => _position < _text.Length ? _text[_position] : '\0';

    public Lexer(string text)
    {
        _text = text;
        _position = 0;
    }

    private void Advance()
    {
        _position++;
    }

    private void SkipWhitespace()
    {
        while (CurrentChar != '\0' && char.IsWhiteSpace(CurrentChar))
        {
            Advance();
        }
    }

    private Token Number()
    {
        var start = _position;
        while (CurrentChar != '\0' && (char.IsDigit(CurrentChar) || CurrentChar == '.'))
        {
            Advance();
        }
        var length = _position - start;
        var text = _text.Substring(start, length);
        if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
        {
            return new Token(TokenType.Number, text, value);
        }
        return new Token(TokenType.Unknown, text, null); // Should ideally not happen if parsing is correct
    }

    public Token GetNextToken()
    {
        while (CurrentChar != '\0')
        {
            if (char.IsWhiteSpace(CurrentChar))
            {
                SkipWhitespace();
                continue;
            }

            if (char.IsDigit(CurrentChar) || CurrentChar == '.')
            {
                return Number();
            }

            if (CurrentChar == '+')
            {
                Advance();
                return new Token(TokenType.Plus, "+", null);
            }

            if (CurrentChar == '-')
            {
                Advance();
                return new Token(TokenType.Minus, "-", null);
            }

            if (CurrentChar == '*')
            {
                Advance();
                return new Token(TokenType.Multiply, "*", null);
            }

            if (CurrentChar == '/')
            {
                Advance();
                return new Token(TokenType.Divide, "/", null);
            }

            if (CurrentChar == '(')
            {
                Advance();
                return new Token(TokenType.LeftParen, "(", null);
            }

            if (CurrentChar == ')')
            {
                Advance();
                return new Token(TokenType.RightParen, ")", null);
            }

            // If no token is matched, return Unknown
            var unknownChar = CurrentChar;
            Advance();
            return new Token(TokenType.Unknown, unknownChar.ToString(), null);
        }

        return new Token(TokenType.EndOfExpression, "\0", null);
    }
}
