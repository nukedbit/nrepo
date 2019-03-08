using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using NRepo.Services;

namespace NRepo
{
    [HelpOption]
    public class App
    {
        private readonly IConsoleService _consoleService;
        private readonly ICommandHandler _commandHandler;
        private readonly IFileService _fileService;

        public App(IConsoleService consoleService, ICommandHandler commandHandler, IFileService fileService)
        {
            _consoleService = consoleService;
            _commandHandler = commandHandler;
            _fileService = fileService;
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
                _commandHandler.Handle(new RepositoryInitOrCreateCommand(path));
                var filesToAdd = await _commandHandler.HandleAsync<DownloadTemplateFilesCommand, IEnumerable<string>>(
                    new DownloadTemplateFilesCommand());
                var repoCommand = new RemoteGithubCommand(_fileService.GetCurrentDirectoryName());
                var githubRepository = 
                    await _commandHandler.HandleAsync<RemoteGithubCommand, Octokit.Repository>(repoCommand);
                _commandHandler.Handle(new FinishRepoSetupCommand(filesToAdd, githubRepository?.CloneUrl));
                Console.WriteLine();
                Console.WriteLine("Done, Bye.");
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