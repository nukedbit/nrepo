using System;
using Optional;

namespace NukedBit.NRepo.Services
{
    public class ConsoleService : IConsoleService
    {
        public void WriteLine(string inputString) => Console.WriteLine(inputString);

        public void WriteLine() => Console.WriteLine();

        public void WriteLineColored(ConsoleColor color, string inputString)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(inputString);
            Console.ForegroundColor = currentColor;
        }

        public string ReadLine() => Console.ReadLine();

        public bool AskForConfirmation()
        {
            Console.Write("Yes/No:");
            while (true)
            {
                var line = Console.ReadLine()?.ToLowerInvariant()?.Trim();
                if (line == "yes")
                {
                    return true;
                }
                else if (line == "no")
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("What?");
                    Console.Write("Yes/No:");
                }
            }
        }

        public Option<int> ReadInputNumber(int min, int max)
        {
            var defaultColor = Console.ForegroundColor;
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(">");
                Console.ForegroundColor = defaultColor;
                var line = Console.ReadLine();
                if (int.TryParse(line, out var result) && result >= min && result <= max)
                {
                    return Option.Some(result);
                }

                if (line?.Trim() == "exit")
                {
                    break;
                }
            }

            return Option.None<int>();
        }
    }
}
