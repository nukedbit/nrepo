using System;
using System.IO;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace NRepo
{
    public class NewRepoCommandHandler
    {
        private readonly NewGitHubRepoCommandHandler _newGitHubRepoCommandHandler;
        private readonly LicensePicker _licensePicker;

        public NewRepoCommandHandler(NewGitHubRepoCommandHandler newGitHubRepoCommandHandler, LicensePicker licensePicker)
        {
            _newGitHubRepoCommandHandler = newGitHubRepoCommandHandler;
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
            var gitPath = Repository.Init(repoPath);

            await _licensePicker.PickLicenseAsync(repoPath);

            var filesToAdd = await RepoFiles.DownloadAsync(repoPath);
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

                var url = await _newGitHubRepoCommandHandler.HandleAsync(new NewGitHubRepoCommand(repoName));
                repository.Network.Remotes.Update("origin", r => r.Url = url);
            }
        }
    }
}