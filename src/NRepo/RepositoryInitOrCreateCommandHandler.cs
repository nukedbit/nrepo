using System.IO;
using NukedBit.NRepo.Services;

namespace NukedBit.NRepo
{
    public class RepositoryInitOrCreateCommandHandler : ICommandHandler<RepositoryInitOrCreateCommand>
    {
        private readonly IFileService _fileService;
        private readonly IConsoleService _consoleService;
        private readonly IRepositoryService _repositoryService;

        public RepositoryInitOrCreateCommandHandler(IFileService fileService, IConsoleService consoleService, IRepositoryService repositoryService)
        {
            _fileService = fileService;
            _consoleService = consoleService;
            _repositoryService = repositoryService;
        }

        public void Handle(RepositoryInitOrCreateCommand command)
        {
            var repoPath = string.IsNullOrEmpty(command.RepoPath) ? _fileService.GetCurrentDirectory() : Path.GetFullPath(Path.Combine(_fileService.GetCurrentDirectory(), command.RepoPath));

            var gitFolderPath = Path.Combine(repoPath, ".git");
            var repoAlreadyInitialized = _fileService.DirectoryExists(gitFolderPath);

            if (!repoAlreadyInitialized)
            {
                _consoleService.WriteLine("Initializing repo..");
                _repositoryService.Init(repoPath);
            }

            _fileService.SetCurrentDirectory(repoPath);
        }
    }
}