using System;
using ExpressionCalculator;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter a mathematical expression (or type 'exit' to quit):");

        while (true)
        {
            Console.Write("> ");
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            if (input.Trim().ToLowerInvariant() == "exit")
            {
                break;
            }

            try
            {
                Lexer lexer = new Lexer(input);
                Parser parser = new Parser(lexer);
                double result = parser.Parse();
                Console.WriteLine($"Result: {result}");
            }
            catch (SyntaxErrorException ex)
            {
                Console.WriteLine($"Syntax Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
