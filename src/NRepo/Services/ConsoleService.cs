using System;
using JetBrains.Annotations;

namespace NRepo.Services
{
    public class ConsoleService : IConsoleService
    {
        public void WriteLine(string format, [NotNull] params object[] args) => Console.WriteLine(format, args);

        public void WriteLine(string str) => Console.WriteLine(str);

        public void WriteLine() => Console.WriteLine();

        public void WriteLineColored(ConsoleColor color, string format, [NotNull] params object[] args)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(format, args);
            Console.ForegroundColor = currentColor;
        }

        public string ReadLine() => Console.ReadLine();

        public bool AskForConfirmation()
        {
            Console.Write("Yes/No:");
            while (true)
            {
                var line = Console.ReadLine().ToLowerInvariant().Trim();
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

        public int? ReadInputNumber(int min, int max, string exitKey = "exit")
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
                    return result;
                }

                if (line.Trim() == exitKey)
                {
                    break;
                }
            }

            return null;
        }
    }
}
