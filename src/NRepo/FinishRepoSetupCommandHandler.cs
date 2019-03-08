using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;
using NRepo.Services;

namespace NRepo
{
    public class FinishRepoSetupCommandHandler: ICommandHandler<FinishRepoSetupCommand>
    {
        private readonly IFileService _fileService;
        private readonly IConsoleService _consoleService;

        public FinishRepoSetupCommandHandler(IFileService fileService, IConsoleService consoleService)
        {
            _fileService = fileService;
            _consoleService = consoleService;
        }

        private void AddFilesToGitRepository(IEnumerable<string> filesToAdd, Repository repository)
        {
            foreach (var file in filesToAdd)
            {
                _consoleService.WriteLine("Adding file {0}", file);
                repository.Index.Add(file);
                repository.Index.Write();
            }
        }

        private static void SetOriginRemote(Repository repository, string cloneUrl)
        {
            if (cloneUrl is null)
            {
                return;
            }
            if (repository.Network.Remotes.Any(r => r.Name == "origin"))
            {
                repository.Network.Remotes.Update("origin", r => r.Url = cloneUrl);
            }
            else
            {
                repository.Network.Remotes.Add("origin", cloneUrl);
            }
        }

        public void Handle(FinishRepoSetupCommand command)
        {
            using (var repository = new Repository(_fileService.GetCurrentDirectory(), new RepositoryOptions()))
            {
                AddFilesToGitRepository(command.FilesToAdd, repository); 

                SetOriginRemote(repository, command.CloneUrl);

                _consoleService.WriteLine();
                _consoleService.WriteLine("Remote Url: {0}", command.CloneUrl ?? "None");
            }
        }
    }
}