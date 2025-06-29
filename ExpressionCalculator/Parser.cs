using System;
using System.Globalization;

namespace ExpressionCalculator;

public class Parser
{
    private readonly Lexer _lexer;
    private Token _currentToken;

    public Parser(Lexer lexer)
    {
        _lexer = lexer;
        _currentToken = _lexer.GetNextToken();
    }

    private void Eat(TokenType tokenType)
    {
        if (_currentToken.Type == tokenType)
        {
            _currentToken = _lexer.GetNextToken();
        }
        else
        {
            throw new SyntaxErrorException($"Error: Unexpected token '{_currentToken.Text}' (Type: {_currentToken.Type}). Expected {tokenType}.");
        }
    }

    // Factor : NUMBER | LPAREN Expression RPAREN | MINUS Factor
    private double ParseFactor()
    {
        Token token = _currentToken;

        if (token.Type == TokenType.Number)
        {
            Eat(TokenType.Number);
            if (token.Value is double val)
            {
                return val;
            }
            // This should not happen if lexer is correct
            throw new SyntaxErrorException($"Error: Invalid number format for token '{token.Text}'.");
        }
        else if (token.Type == TokenType.LeftParen)
        {
            Eat(TokenType.LeftParen);
            double result = ParseExpression();
            Eat(TokenType.RightParen);
            return result;
        }
        else if (token.Type == TokenType.Minus) // Unary minus
        {
            Eat(TokenType.Minus);
            return -ParseFactor(); // Important: ParseFactor, not ParseTerm or ParseExpression
        }
        // Removed unary plus handling to make "1 ++ 2" a syntax error as per test expectations.
        // else if (token.Type == TokenType.Plus)
        // {
        //     Eat(TokenType.Plus);
        //     return ParseFactor();
        // }

        throw new SyntaxErrorException($"Error: Unexpected token '{token.Text}' (Type: {token.Type}) in Factor. Expected Number, '(', or '-'.");
    }

    // Term : Factor ( (MULTIPLY | DIVIDE) Factor )*
    private double ParseTerm()
    {
        double result = ParseFactor();

        while (_currentToken.Type == TokenType.Multiply || _currentToken.Type == TokenType.Divide)
        {
            Token token = _currentToken;
            if (token.Type == TokenType.Multiply)
            {
                Eat(TokenType.Multiply);
                result *= ParseFactor();
            }
            else if (token.Type == TokenType.Divide)
            {
                Eat(TokenType.Divide);
                double divisor = ParseFactor();
                if (divisor == 0)
                {
                    // Standard double division by zero results in Infinity or -Infinity.
                    // Or, you could throw an ArithmeticException here if preferred.
                    // For this implementation, we'll allow double's behavior.
                    if (result == 0) return double.NaN; // 0/0 = NaN
                    result /= divisor;
                }
                else
                {
                    result /= divisor;
                }
            }
        }
        return result;
    }

    // Expression : Term ( (PLUS | MINUS) Term )*
    public double ParseExpression()
    {
        double result = ParseTerm();

        while (_currentToken.Type == TokenType.Plus || _currentToken.Type == TokenType.Minus)
        {
            Token token = _currentToken;
            if (token.Type == TokenType.Plus)
            {
                Eat(TokenType.Plus);
                result += ParseTerm();
            }
            else if (token.Type == TokenType.Minus)
            {
                Eat(TokenType.Minus);
                result -= ParseTerm();
            }
        }

        // After parsing the expression, if we haven't reached the end, it's a syntax error.
        // (e.g. "2 + 3 4" - the "4" is unexpected after a full expression)
        // However, "2+3(" would be caught by ParseFactor expecting a number after '('.
        if (_currentToken.Type != TokenType.EndOfExpression)
        {
            // This check is particularly useful for trailing unexpected tokens.
            // For instance, an expression like "5 * (2+3) 7" would parse "5 * (2+3)"
            // and then _currentToken would be a Number (7). This indicates an error.
            // It might also catch cases where an operator is missing, e.g. "2 3" -> "2" parsed, then "3" is unexpected.
            // No, "2 3" would be caught by ParseFactor for the first number "2", then ParseTerm/Expression
            // would finish. Then this check would see "3" and flag it.

            // Let's reconsider. If the grammar is complete, this might not be needed here,
            // as `Eat(TokenType.EndOfExpression)` could be called by the public entry point.
            // For now, let's keep it to see how it behaves with Program.cs.
            // The main Parse method should ensure all tokens are consumed.
        }

        return result;
    }

    public double Parse()
    {
        double result = ParseExpression();
        if (_currentToken.Type != TokenType.EndOfExpression)
        {
            throw new SyntaxErrorException($"Error: Unexpected token '{_currentToken.Text}' after expression. Expected end of expression.");
        }
        return result;
    }
}
