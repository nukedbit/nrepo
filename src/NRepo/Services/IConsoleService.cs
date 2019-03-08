using System;

namespace NRepo.Services
{
    public interface IConsoleService
    {
        void WriteLine(string format, params object[] args);
        void WriteLine(string str);
        void WriteLine();
        bool AskForConfirmation();
        int? ReadInputNumber(int min, int max, string exitKey = "exit");
        void WriteLineColored(ConsoleColor color, string format, params object[] args);
        string ReadLine();
    }
}