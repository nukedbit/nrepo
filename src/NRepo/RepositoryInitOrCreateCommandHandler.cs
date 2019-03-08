using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace NRepo
{
    public class RepositoryInitOrCreateCommandHandler : IRepositoryInitOrCreateCommandHandler
    {
        private readonly ICommandHandler<NewGitHubRepoCommand, Octokit.Repository> _remoteGithubCommandHandler;
        private readonly ILicensePicker _licensePicker;
        private readonly ITemplateFilesService _templateFilesService;
        private readonly IFileService _fileService;
        private readonly IConsoleService _consoleService;

        public RepositoryInitOrCreateCommandHandler(ICommandHandler<NewGitHubRepoCommand, Octokit.Repository> remoteGithubCommandHandler, ILicensePicker licensePicker,
            ITemplateFilesService templateFilesService, IFileService fileService, IConsoleService consoleService)
        {
            _remoteGithubCommandHandler = remoteGithubCommandHandler;
            _licensePicker = licensePicker;
            _templateFilesService = templateFilesService;
            _fileService = fileService;
            _consoleService = consoleService;
        }

        public string CreateReadMe()
        {
            var repoName = _fileService.GetCurrentDirectoryName();
            var name = repoName.Replace("-", " ");
            var filename = Path.Combine(_fileService.GetCurrentDirectory(), "./README.md");
            _fileService.WriteAllText(filename, "# " + name);
            return "README.md";
        }

        public async Task ExecuteAsync(RepositoryInitOrCreateCommand command)
        {
            command.RepoPath = string.IsNullOrEmpty(command.RepoPath) ? _fileService.GetCurrentDirectory() : Path.GetFullPath(Path.Combine(_fileService.GetCurrentDirectory(), command.RepoPath));

            var repoAlreadyInitialized = _fileService.DirectoryExists(Path.Combine(command.RepoPath, ".git"));

            if (!repoAlreadyInitialized)
            {
                _consoleService.WriteLine("Initializing repo..");
                Repository.Init(command.RepoPath);
            }

            _fileService.SetCurrentDirectory(command.RepoPath);

            var filesToAdd = await _templateFilesService.DownloadAsync();

            if (await _licensePicker.PickLicenseAsync() is var licenseFile)
            {
                filesToAdd.Add(licenseFile);
            }

            filesToAdd.Add(CreateReadMe());

            using (var repository = new Repository(command.RepoPath, new RepositoryOptions()))
            {
                AddFilesToGitRepository(filesToAdd, repository);
                var repoName = _fileService.GetCurrentDirectoryName();
                var githubRepository = await _remoteGithubCommandHandler.HandleAsync(new NewGitHubRepoCommand(repoName));

                SetOriginRemote(repository, githubRepository);

                _consoleService.WriteLine();
                _consoleService.WriteLine("Remote Url: {0}", githubRepository != null ? githubRepository.Url : "None");
            }

            _consoleService.WriteLine();
            _consoleService.WriteLine("Done.");
        }

        private static void SetOriginRemote(Repository repository, Octokit.Repository githubRepository)
        {
            if (githubRepository is null)
            {
                return;
            }
            if (repository.Network.Remotes.Any(r => r.Name == "origin"))
            {
                repository.Network.Remotes.Update("origin", r => r.Url = githubRepository.CloneUrl);
            }
            else
            {
                repository.Network.Remotes.Add("origin", githubRepository.CloneUrl);
            }
        }

        private void AddFilesToGitRepository(List<string> filesToAdd, Repository repository)
        {
            foreach (var file in filesToAdd)
            {
                _consoleService.WriteLine("Adding file {0}", file);
                repository.Index.Add(file);
                repository.Index.Write();
            }
        }
    }
}