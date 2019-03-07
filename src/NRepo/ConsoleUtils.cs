using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace NRepo
{
    public static class ConsoleUtils
    {
        public static int? ReadInputNumber(int min, int max, string exitKey = "exit")
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

        public static bool AskForConfirmation()
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
    }
}
