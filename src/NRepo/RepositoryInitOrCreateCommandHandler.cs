using System;
using System.IO;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace NRepo
{
    public class RepositoryInitOrCreateCommandHandler : ICommandHandler<RepositoryInitOrCreateCommand>
    {
        private readonly IFileService _fileService;
        private readonly IConsoleService _consoleService;

        public RepositoryInitOrCreateCommandHandler(IFileService fileService, IConsoleService consoleService)
        {
            _fileService = fileService;
            _consoleService = consoleService;
        }

        public void Handle(RepositoryInitOrCreateCommand command)
        {
            command.RepoPath = string.IsNullOrEmpty(command.RepoPath) ? _fileService.GetCurrentDirectory() : Path.GetFullPath(Path.Combine(_fileService.GetCurrentDirectory(), command.RepoPath));

            var repoAlreadyInitialized = _fileService.DirectoryExists(Path.Combine(command.RepoPath, ".git"));

            if (!repoAlreadyInitialized)
            {
                _consoleService.WriteLine("Initializing repo..");
                Repository.Init(command.RepoPath);
            }

            _fileService.SetCurrentDirectory(command.RepoPath);
        }
    }
}