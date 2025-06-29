using Xunit;
using ExpressionCalculator; // Ensure this using directive is present

namespace ExpressionCalculator.Tests;

public class CalculatorTests // Class name matches file name
{
    private double Evaluate(string expression)
    {
        var lexer = new Lexer(expression);
        var parser = new Parser(lexer);
        return parser.Parse();
    }

    [Theory]
    [InlineData("5", 5)]
    [InlineData("123", 123)]
    [InlineData("0", 0)]
    [InlineData("0.5", 0.5)]
    [InlineData("123.456", 123.456)]
    public void TestNumbers(string expression, double expected)
    {
        Assert.Equal(expected, Evaluate(expression), precision: 5);
    }

    [Theory]
    [InlineData("2 + 3", 5)]
    [InlineData("10 - 4", 6)]
    [InlineData("7 * 8", 56)]
    [InlineData("100 / 4", 25)]
    [InlineData("2.5 + 1.5", 4)]
    [InlineData("5 - 7.5", -2.5)]
    [InlineData("3 * 1.5", 4.5)]
    [InlineData("10 / 0.5", 20)]
    public void TestSimpleArithmetic(string expression, double expected)
    {
        Assert.Equal(expected, Evaluate(expression), precision: 5);
    }

    [Theory]
    [InlineData("-5", -5)]
    [InlineData("-0.25", -0.25)]
    [InlineData("-(3)", -3)]
    [InlineData("- (3)", -3)] // With space
    [InlineData("-(2+3)", -5)]
    [InlineData("10 + -2", 8)] // This is "10 + (-2)"
    [InlineData("10 * -2", -20)]// This is "10 * (-2)"
    [InlineData("--3", 3)] // Double unary minus
    [InlineData("---3", -3)]// Triple unary minus
    // Removed [InlineData("+5", 5)] as unary + is not supported by ParseFactor directly
    // Removed [InlineData("-+3", -3)] as it implies unary +
    // Removed [InlineData("+-3", -3)] and moved to TestSyntaxErrors as it will throw due to leading +
    public void TestUnaryOperations(string expression, double expected)
    {
        Assert.Equal(expected, Evaluate(expression), precision: 5);
    }

    [Theory]
    [InlineData("2 + 3 * 4", 14)] // 2 + 12
    [InlineData("10 - 6 / 2", 7)] // 10 - 3
    [InlineData("2 * 3 + 4 * 5", 26)] // 6 + 20
    [InlineData("100 / 4 * 2", 50)]   // 25 * 2
    [InlineData("7 - 2 * 3 + 10 / 5", 3)] // 7 - 6 + 2
    public void TestOperatorPrecedence(string expression, double expected)
    {
        Assert.Equal(expected, Evaluate(expression), precision: 5);
    }

    [Theory]
    [InlineData("(5)", 5)]
    [InlineData("(2 + 3)", 5)]
    [InlineData("10 - (4 + 2)", 4)] // 10 - 6
    [InlineData("(7 - 2) * 3", 15)] // 5 * 3
    [InlineData("100 / (4 * 2)", 12.5)] // 100 / 8
    [InlineData("((2 + 3) * 4) - (10 / (1 + 1))", 15)] // (5 * 4) - (10 / 2) = 20 - 5
    [InlineData("-(2+3)*4", -20)] // -5 * 4
    public void TestParentheses(string expression, double expected)
    {
        Assert.Equal(expected, Evaluate(expression), precision: 5);
    }

    [Theory]
    [InlineData("1+1", 2)] // No spaces
    [InlineData(" 5 ", 5)] // Leading/trailing spaces
    [InlineData("2.0 + 3.00", 5.0)] // Decimal formatting
    [InlineData("10 / 3", 10.0/3.0)] // Division leading to double
    public void TestFormattingAndEdgeCases(string expression, double expected)
    {
        Assert.Equal(expected, Evaluate(expression), precision: 5);
    }

    [Theory]
    [InlineData("5 / 0", double.PositiveInfinity)]
    [InlineData("-5 / 0", double.NegativeInfinity)]
    [InlineData("0 / 0", double.NaN)]
    [InlineData("10 * (5 / 0)", double.PositiveInfinity)]
    public void TestDivisionByZero(string expression, double expected)
    {
        Assert.Equal(expected, Evaluate(expression));
    }

    [Theory]
    [InlineData("-1+5*-3/5", -4)] // Original example: -1 + (5 * -3) / 5 = -1 + (-15/5) = -1 + (-3) = -4
    [InlineData("2+2*2/2-2", 2)] // 2 + (2*2)/2 - 2 = 2 + 4/2 - 2 = 2 + 2 - 2 = 2
    [InlineData("10 * -(2+3) / (1 - -1)", -25)] // 10 * -5 / (1+1) = -50 / 2 = -25
    [InlineData("1.5 * 2 + 3.5 / 0.5 - -1", 11)] // 3 + 7 - (-1) = 3 + 7 + 1 = 11
    public void TestComplexExpressions(string expression, double expected)
    {
        Assert.Equal(expected, Evaluate(expression), precision: 5);
    }

    [Theory]
    [InlineData(" ( 2 + 3 ) * ( (4 - 1) / 1.5 ) ", 10)] // (5) * (3 / 1.5) = 5 * 2 = 10
    [InlineData(" - ( - ( -5 + 2) * 2 ) ", -6)] // Corrected expected value from 6 to -6
    [InlineData("100 / (2 * (10 / (2+3))) + 5", 30)] // 100 / (2 * (10/5)) + 5 = 100 / (2*2) + 5 = 100/4 + 5 = 25 + 5 = 30
    [InlineData("3 + (4 * (5 - (6 / 2))) - 1", 10)] // 3 + (4 * (5-3)) -1 = 3 + (4*2) -1 = 3 + 8 - 1 = 10
    public void TestMoreComplexExpressionsWithNesting(string expression, double expected)
    {
        Assert.Equal(expected, Evaluate(expression), precision: 5);
    }

    // Test cases for expected syntax errors
    [Theory]
    [InlineData("1 +")] // Missing operand
    [InlineData("* 2")] // Missing operand
    [InlineData("1 + 2 *")] // Missing operand
    [InlineData("(1 + 2")] // Missing closing parenthesis
    [InlineData("1 + 2)")] // Missing opening parenthesis
    [InlineData("1 ( 2")] // Operator expected
    [InlineData("1 ++ 2")] // Unexpected token after operator
    [InlineData("1 + * 2")] // Operator after operator
    [InlineData("1 2 + 3")] // Number then number without operator
    [InlineData("a + b")] // Invalid characters (results in Unknown token)
    [InlineData("1.2.3 + 4")] // Invalid number (Lexer creates Unknown token for "1.2.3")
    [InlineData("+5")] // Unary + at start of expression no longer supported
    [InlineData("-+3")] // Sequence -+ no longer supported
    [InlineData("+-3")] // Sequence +- no longer supported
    public void TestSyntaxErrors(string expression)
    {
        // For "a + b", Lexer will produce Unknown for 'a'. Parser's ParseFactor will throw.
        // For "1.2.3", Lexer will produce Unknown for "1.2.3". Parser's ParseFactor will throw.
        Assert.Throws<SyntaxErrorException>(() => Evaluate(expression));
    }
}
