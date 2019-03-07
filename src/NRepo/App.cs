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

        [Option(Description = "Create a new Repository with specified Name", ShortName = "n")]
        public string NewRepoName { get; set; }

        [Option("-i|--init", Description =
            "Initialize a repository in current folder, if already exists just add the ignore files and license")]
        public (bool init, string folder) Init { get; set; }

        private async Task OnExecuteAsync()
        {
            if (!string.IsNullOrEmpty(NewRepoName))
            {
                await _repositoryHandler.ExecuteAsync(false,NewRepoName);
            }
            else if (Init.init)
            {
                var folder = Init.folder;

                if (string.IsNullOrEmpty(folder))
                {
                    folder = Environment.CurrentDirectory;
                }
                else
                {
                    Environment.CurrentDirectory = Path.GetFullPath(folder);
                }

                var repoName = Path.GetDirectoryName(folder);

                await _repositoryHandler.ExecuteAsync(true, repoName);
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