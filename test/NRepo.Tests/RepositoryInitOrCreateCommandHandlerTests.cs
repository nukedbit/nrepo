using System;
using System.IO;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NukedBit.NRepo;
using NukedBit.NRepo.Services;
using Xunit;

namespace NRepo.Tests
{
    public class RepositoryInitOrCreateCommandHandlerTests
    {
        [Fact(DisplayName = "If RepoName is empty use current folder")]
        public void RepoNameEmptyUseCurrentDirectory()
        {
            var fileService = Substitute.For<IFileService>();
            var consoleService = Substitute.For<IConsoleService>();
            var repositoryService = Substitute.For<IRepositoryService>();
            var handler = new RepositoryInitOrCreateCommandHandler(fileService, consoleService, repositoryService);

            var expectedRepoDirectory = "/dev/null";

            fileService.GetCurrentDirectory().Returns(expectedRepoDirectory);
            fileService.DirectoryExists($"/dev/null{Path.DirectorySeparatorChar}.git").Returns(true);


            handler.Handle(new RepositoryInitOrCreateCommand(null));

            fileService.Received(1).SetCurrentDirectory(expectedRepoDirectory);
        }

        [Fact(DisplayName = "Initialise Repository")]
        public void InitialiseRepository()
        {
            var fileService = Substitute.For<IFileService>();
            var consoleService = Substitute.For<IConsoleService>();
            var repositoryService = Substitute.For<IRepositoryService>();
            var handler = new RepositoryInitOrCreateCommandHandler(fileService, consoleService, repositoryService);

            var expectedRepoDirectory = "/dev/null";

            fileService.GetCurrentDirectory().Returns(expectedRepoDirectory);
            fileService.DirectoryExists($"/dev/null{Path.DirectorySeparatorChar}.git").Returns(false);


            handler.Handle(new RepositoryInitOrCreateCommand(null));

            repositoryService.Received(1).Init(expectedRepoDirectory);
        }

        [Fact(DisplayName = "Do Not Initialise Repository if a .git folder already exists")]
        public void DoNotInitialiseRepository()
        {
            var fileService = Substitute.For<IFileService>();
            var consoleService = Substitute.For<IConsoleService>();
            var repositoryService = Substitute.For<IRepositoryService>();
            var handler = new RepositoryInitOrCreateCommandHandler(fileService, consoleService, repositoryService);

            var expectedRepoDirectory = "/dev/null";

            fileService.GetCurrentDirectory().Returns(expectedRepoDirectory);
            fileService.DirectoryExists($"/dev/null{Path.DirectorySeparatorChar}.git").Returns(true);


            handler.Handle(new RepositoryInitOrCreateCommand(null));

            repositoryService.Received(0).Init(expectedRepoDirectory);
        }

        [Fact(DisplayName = "Set repository as current Directory")]
        public void SetRepositoryAsCurrentDirectory()
        {
            var fileService = Substitute.For<IFileService>();
            var consoleService = Substitute.For<IConsoleService>();
            var repositoryService = Substitute.For<IRepositoryService>();
            var handler = new RepositoryInitOrCreateCommandHandler(fileService, consoleService, repositoryService);

            var expectedRepoDirectory = "/dev/null";

            fileService.GetCurrentDirectory().Returns(expectedRepoDirectory);
            fileService.DirectoryExists($"/dev/null{Path.DirectorySeparatorChar}.git").Returns(true);

            handler.Handle(new RepositoryInitOrCreateCommand(null));

            fileService.Received(1).SetCurrentDirectory(expectedRepoDirectory);
        }

        [Fact(DisplayName = "Create Repository at specified path")]
        public void CreateRepositoryAtSpecifiedPath()
        {
            var fileService = Substitute.For<IFileService>();
            var consoleService = Substitute.For<IConsoleService>();
            var repositoryService = Substitute.For<IRepositoryService>();
            var handler = new RepositoryInitOrCreateCommandHandler(fileService, consoleService, repositoryService);

            var expectedRepoDirectory = Path.GetFullPath(Path.Combine(Path.GetPathRoot("/"), "dev", "null", "test"));
            var parentFolder = Path.GetFullPath(Path.Combine(Path.GetPathRoot("/"), "dev", "null"));

            fileService.GetCurrentDirectory().Returns(parentFolder);
            fileService.DirectoryExists($"{expectedRepoDirectory}{Path.DirectorySeparatorChar}.git").Returns(true);

            handler.Handle(new RepositoryInitOrCreateCommand(expectedRepoDirectory));

            fileService.Received(1).SetCurrentDirectory(expectedRepoDirectory);
        }
    }
}
