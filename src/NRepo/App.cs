using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace NRepo
{
    [HelpOption]
    public class App
    {
        private readonly RepositoryInitOrCreateCommandHandler _repositoryHandler;

        public App(RepositoryInitOrCreateCommandHandler repositoryHandler)
        {
            _repositoryHandler = repositoryHandler;
        }

        [Option(Description = "Create a new Repository at the specified path, could be a new folder or an existing one.", ShortName = "n")]
        public (bool create, string path) RepoPath { get; set; }

        private async Task OnExecuteAsync()
        {
            var (create, path ) = RepoPath;
            if (create)
            {
                if (string.IsNullOrEmpty(path))
                {
                    Console.WriteLine("Repository will be created on the current directory.");
                    if (!ConsoleUtils.AskForConfirmation())
                    {
                        Console.WriteLine("Exit");
                        return;
                    }
                }                                
                await _repositoryHandler.ExecuteAsync(path);
            }
            else
            {
                var versionString = Assembly.GetEntryAssembly()
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion
                    .ToString();
                Console.WriteLine($"nrepo v{versionString}");
                Console.WriteLine(".NET GitHub Repo Setup");
                var currentColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("This tool is on alpha state! be safe with your data!");
                Console.ForegroundColor = currentColor;
            }
        }
    }
}