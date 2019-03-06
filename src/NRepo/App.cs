using System;
using System.Reflection;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace NRepo
{
    [HelpOption]
    public class App
    {
        private readonly NewRepoCommandHandler _newRepoCommandHandler;

        public App(NewRepoCommandHandler newRepoCommandHandler)
        {
            _newRepoCommandHandler = newRepoCommandHandler;
        }

        [Option(Description = "Create a new Repository with specified Name", ShortName = "n")]
        public string NewRepoName { get; set; }

        [Option(Description = "Initialize a repository in current folder, if already exists just add the ignore files and license", ShortName = "i")]
        public bool Init { get; set; }

        private async Task OnExecuteAsync()
        {
            if (!string.IsNullOrEmpty(NewRepoName))
            {
                await _newRepoCommandHandler.ExecuteAsync(NewRepoName);
            }
            else
            {
                var versionString = Assembly.GetEntryAssembly()
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion
                    .ToString();
                Console.WriteLine($"nrepo v{versionString}");
                Console.WriteLine(".NET GitHub Repo Setup");
            }
        }
    }
}