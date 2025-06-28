using System;

namespace PrimeGeneratorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Prime numbers less than 100:");
            for (int i = 2; i < 100; i++)
            {
                if (IsPrime(i))
                {
                    Console.WriteLine(i);
                }
            }
        }

        static bool IsPrime(int number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false; // Optimization for even numbers

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (int i = 3; i <= boundary; i += 2) // Check only odd factors
            {
                if (number % i == 0)
                    return false;
            }

            return true;
        }
    }
}
