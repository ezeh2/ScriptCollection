using System;

namespace ExpressionCalculator;

public class SyntaxErrorException : Exception
{
    public SyntaxErrorException(string message) : base(message)
    {
    }
}
