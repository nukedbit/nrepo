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
        private readonly IRepositoryInitOrCreateCommandHandler _repositoryHandler;
        private readonly IConsoleService _consoleService;

        public App(IRepositoryInitOrCreateCommandHandler repositoryHandler, IConsoleService consoleService)
        {
            _repositoryHandler = repositoryHandler;
            _consoleService = consoleService;
        }

        [Option(Description = "Create a new Repository at the specified path, could be a new folder or an existing one.", ShortName = "n")]
        public (bool create, string path) RepoPath { get; set; }

        private async Task OnExecuteAsync()
        {
            var (create, path) = RepoPath;
            if (create)
            {
                if (string.IsNullOrEmpty(path))
                {
                    _consoleService.WriteLine("Repository will be created on the current directory.");
                    if (!_consoleService.AskForConfirmation())
                    {
                        _consoleService.WriteLine("Exit");
                        return;
                    }
                }
                await _repositoryHandler.ExecuteAsync(new RepositoryInitOrCreateCommand(path));
            }
            else
            {
                var versionString = Assembly.GetEntryAssembly()
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion;
                _consoleService.WriteLine($"nrepo v{versionString}");
                _consoleService.WriteLine(".NET GitHub Repo Setup");
                _consoleService.WriteLineColored(ConsoleColor.Red, "This tool is on alpha state! be safe with your data!");
            }
        }
    }
}