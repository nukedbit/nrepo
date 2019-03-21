using System;
using Optional;

namespace NukedBit.NRepo.Services
{
    public interface IConsoleService
    {
        /// <summary>
        /// Write input string to the console
        /// </summary>
        /// <param name="inputString">Input string</param>
        void WriteLine(string inputString);

        /// <summary>
        /// Write blank line to the console
        /// </summary>
        void WriteLine();

        /// <summary>
        /// Asks for Yes/No confirmation in the console
        /// </summary>
        /// <returns><see langword="true"/> or <see langword="false"/>; <see langword="false"/> if the confirmation was Yes</returns>
        bool AskForConfirmation();

        /// <summary>
        /// Returns the input value if its within the minimum and maximum values
        /// </summary>
        /// <param name="min">Minimum input value</param>
        /// <param name="max">Maximum input value</param>
        /// <returns></returns>
        Option<int> ReadInputNumber(int min, int max);

        /// <summary>
        /// Write input string in specified console color to the console
        /// </summary>
        /// <param name="color"></param>
        /// <param name="inputString">Input string</param>
        void WriteLineColored(ConsoleColor color, string inputString);
        
        /// <summary>
        /// Reads the console input on the next line in the console
        /// </summary>
        /// <returns>Returns the input from the console</returns>
        string ReadLine();
    }
}