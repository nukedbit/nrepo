using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace NRepo
{
    public class RepositoryInitOrCreateCommandHandler
    {
        private readonly RemoteGithubCommandHandler _remoteGithubCommandHandler;
        private readonly LicensePicker _licensePicker;

        public RepositoryInitOrCreateCommandHandler(RemoteGithubCommandHandler remoteGithubCommandHandler,LicensePicker licensePicker)
        {
            _remoteGithubCommandHandler = remoteGithubCommandHandler;
            _licensePicker = licensePicker;
        }

        private string FixRepoName(string name)
        {
            return name.Replace(" ", "-");
        }

        public string CreateReadMe(string repoName)
        {
            var name = repoName.Replace("-", " ");
            var filename = Path.Combine(Environment.CurrentDirectory, "./README.md");
            File.WriteAllText(filename, "# " + name);
            return "README.md";
        }

        public async Task ExecuteAsync(bool isInit, string repoName)
        {
            if (!isInit)
            {
                repoName = FixRepoName(repoName);
            }

            string repoPath = null;
            repoPath = isInit ? Environment.CurrentDirectory : Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, repoName));

            var repoAlreadyInitialized = Directory.Exists(Path.Combine(repoPath, ".git"));

            if(!repoAlreadyInitialized)
            {
                Console.WriteLine("Initializing repo..");
                Repository.Init(repoPath);
            }

            Environment.CurrentDirectory = repoPath;

            var filesToAdd = await RepoFiles.DownloadAsync();

            if (await _licensePicker.PickLicenseAsync() is var licenseFile)
            {
                filesToAdd.Add(licenseFile);
            }
            
            filesToAdd.Add(CreateReadMe(repoName));
            filesToAdd.Add("LICENSE");

            using (var repository = new Repository(repoPath, new RepositoryOptions()))
            {
                AddFilesToGitRepository(filesToAdd, repository);

                var newRemoteUrl = await _remoteGithubCommandHandler.HandleAsync(new NewGitHubRepoCommand(repoName));
                repository.Network.Remotes.Update("origin", r => r.Url = newRemoteUrl);
            }
            Console.WriteLine();
            Console.WriteLine("Done.");
        }

        private static void AddFilesToGitRepository(List<string> filesToAdd, Repository repository)
        {
            foreach (var file in filesToAdd)
            {
                Console.WriteLine("Adding file {0}", file);
                repository.Index.Add(file);
                repository.Index.Write();
            }
        }
    }
}