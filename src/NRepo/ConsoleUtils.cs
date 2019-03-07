using System;
using System.Collections.Generic;
using System.Text;

namespace NRepo
{
    public static class ConsoleUtils
    {
        public static int? ReadInputNumber(string exitKey = "exit")
        {
            while (true)
            {
                var line = Console.ReadLine();
                if (int.TryParse(line, out var result))
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
