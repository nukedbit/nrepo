using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public string CreateReadMe()
        {
            var repoName = new DirectoryInfo(Environment.CurrentDirectory).Name;
            var name = repoName.Replace("-", " ");
            var filename = Path.Combine(Environment.CurrentDirectory, "./README.md");
            File.WriteAllText(filename, "# " + name);
            return "README.md";
        }

        public async Task ExecuteAsync(bool isInit, string repoPath)
        {
            repoPath = isInit ? Environment.CurrentDirectory : Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, repoPath));

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
            
            filesToAdd.Add(CreateReadMe());

            using (var repository = new Repository(repoPath, new RepositoryOptions()))
            {
                AddFilesToGitRepository(filesToAdd, repository);
                var repoName = new DirectoryInfo(Environment.CurrentDirectory).Name;
                var githubRepository = await _remoteGithubCommandHandler.HandleAsync(new NewGitHubRepoCommand(repoName));

                SetOriginRemote(repository, githubRepository);

                Console.WriteLine();
                Console.WriteLine("Remote Url: {0}",githubRepository.Url);
            }
           
            Console.WriteLine();
            Console.WriteLine("Done.");
        }

        private static void SetOriginRemote(Repository repository, Octokit.Repository githubRepository)
        {
            if (repository.Network.Remotes.Any(r => r.Name == "origin"))
            {
                repository.Network.Remotes.Update("origin", r => r.Url = githubRepository.CloneUrl);
            }
            else
            {
                repository.Network.Remotes.Add("origin", githubRepository.CloneUrl);
            }
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