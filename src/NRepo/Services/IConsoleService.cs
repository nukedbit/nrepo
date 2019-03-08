using System;
using JetBrains.Annotations;

namespace NRepo.Services
{
    public interface IConsoleService
    {
        void WriteLine(string format, [NotNull] params object[] args);
        void WriteLine(string str);
        void WriteLine();
        bool AskForConfirmation();
        int? ReadInputNumber(int min, int max, string exitKey = "exit");
        void WriteLineColored(ConsoleColor color, string format, [NotNull] params object[] args);
        string ReadLine();
    }
}