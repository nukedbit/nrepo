using System;
using Optional;

namespace NukedBit.NRepo.Services
{
    public interface IConsoleService
    {
        void WriteLine(string format, params object[] args);
        void WriteLine(string str);
        void WriteLine();
        bool AskForConfirmation();
        Option<int> ReadInputNumber(int min, int max, string exitKey = "exit");
        void WriteLineColored(ConsoleColor color, string format, params object[] args);
        string ReadLine();
    }
}