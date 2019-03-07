using System;
using System.IO;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace NRepo
{
    public class NewRepoCommandHandler
    {
        private readonly RemoteGithubCommandHandler _remoteGithubCommandHandler;
        private readonly LicensePicker _licensePicker;

        public NewRepoCommandHandler(RemoteGithubCommandHandler remoteGithubCommandHandler, LicensePicker licensePicker)
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
            var filename = Path.Combine(Environment.CurrentDirectory, repoName, "./README.md");
            File.WriteAllText(filename, "# " + name);
            return "README.md";
        }

        public async Task ExecuteAsync(string repoName)
        {
            repoName = FixRepoName(repoName);

            var repoPath = Path.Combine(Environment.CurrentDirectory, repoName);

            if (Directory.Exists(repoPath))
            {
                Console.WriteLine("A folder with the same already exists!");
                return;
            }

            var gitPath = Repository.Init(repoPath);

            await _licensePicker.PickLicenseAsync();

            var filesToAdd = await RepoFiles.DownloadAsync();
            filesToAdd.Add(CreateReadMe(repoName));
            filesToAdd.Add("LICENSE");

            using (var repository = new Repository(gitPath, new RepositoryOptions()))
            {
                foreach (var file in filesToAdd)
                {
                    Console.WriteLine("Adding file {0}", file);
                    repository.Index.Add(file);
                    repository.Index.Write();
                }

                var url = await _remoteGithubCommandHandler.HandleAsync(new NewGitHubRepoCommand(repoName));
                repository.Network.Remotes.Update("origin", r => r.Url = url);
            }
        }
    }
}