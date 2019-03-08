using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NRepo.Services;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Octokit;
using Xunit;

namespace NRepo.Tests
{
    public class RemoteGithubCommandHandlerAsyncTess
    {
        [Fact(DisplayName = "No remote was chosen")]
        public async Task ChoiceExit()
        {
            var gitHubClient = Substitute.For<IGitHubClient>();
            var consoleService = Substitute.For<IConsoleService>();
            var handler = new RemoteGithubCommandHandlerAsync(gitHubClient, consoleService);

            consoleService.ReadInputNumber(min: 1, max: 3).Returns((int?)null);

            var repository = await handler.HandleAsync(new RemoteGithubCommand("arepo"));

            Assert.Null(repository);
        }

        [Fact(DisplayName = "Create New GitHub Repository")]
        public async Task CreateNew()
        {
            var gitHubClient = Substitute.For<IGitHubClient>();
            var repositoriesClient = Substitute.For<IRepositoriesClient>();
            gitHubClient.Repository.Returns(repositoriesClient);

            repositoriesClient.Create(Arg.Is<NewRepository>(n => n.Name == "arepo")).Returns(new Repository(1));

            var consoleService = Substitute.For<IConsoleService>();
            var handler = new RemoteGithubCommandHandlerAsync(gitHubClient, consoleService);

            consoleService.ReadInputNumber(min: 1, max: 3).Returns(1);

            var repository = await handler.HandleAsync(new RemoteGithubCommand("arepo"));

            Assert.NotNull(repository);
        }

        [Fact(DisplayName = "Create Remote Repo Already Exists")]
        public async Task CreateNewAlreadyExist()
        {
            var gitHubClient = Substitute.For<IGitHubClient>();
            var repositoriesClient = Substitute.For<IRepositoriesClient>();
            gitHubClient.Repository.Returns(repositoriesClient);

            var userClient = Substitute.For<IUsersClient>();
            userClient.Current().Returns(EmptyUser());

            gitHubClient.User.Returns(userClient);

            repositoriesClient.Get("login", "arepo").Returns(new Repository(0));
            repositoriesClient.Create(Arg.Is<NewRepository>(n => n.Name == "arepo"))
                .Throws(new RepositoryExistsException("arepo", new ApiValidationException()));

            var consoleService = Substitute.For<IConsoleService>();
            var handler = new RemoteGithubCommandHandlerAsync(gitHubClient, consoleService);

            consoleService.AskForConfirmation().Returns(true);

            consoleService.ReadInputNumber(min: 1, max: 3).Returns(1);

            var repository = await handler.HandleAsync(new RemoteGithubCommand("arepo"));

            consoleService.Received(1).AskForConfirmation();
        }


        [Fact(DisplayName = "Search an existing repo")]
        public async Task SearchExisting()
        {
            var gitHubClient = Substitute.For<IGitHubClient>();
            var repositoriesClient = Substitute.For<IRepositoriesClient>();
            gitHubClient.Repository.Returns(repositoriesClient);
            repositoriesClient.GetAllForCurrent().Returns(new List<Repository>()
            {
                new Repository(0),
                new Repository(1)
            });

            var consoleService = Substitute.For<IConsoleService>();
            consoleService.ReadLine().Returns("");
            consoleService.ReadInputNumber(min: 1, max: 3).Returns(2);
            consoleService.ReadInputNumber(min: 1, max: 2).Returns(1);

            var handler = new RemoteGithubCommandHandlerAsync(gitHubClient, consoleService);             

            var repository = await handler.HandleAsync(new RemoteGithubCommand("arepo"));

            Assert.Equal(0, repository.Id);
        }

        [Fact(DisplayName = "Choose it later")]
        public async Task ChooseItLater()
        {
            var gitHubClient = Substitute.For<IGitHubClient>();
            var repositoriesClient = Substitute.For<IRepositoriesClient>();
            gitHubClient.Repository.Returns(repositoriesClient);
            repositoriesClient.GetAllForCurrent().Returns(new List<Repository>()
            {
                new Repository(0),
                new Repository(1)
            });

            var consoleService = Substitute.For<IConsoleService>();
            consoleService.ReadLine().Returns("");
            consoleService.ReadInputNumber(min: 1, max: 3).Returns(3);

            var handler = new RemoteGithubCommandHandlerAsync(gitHubClient, consoleService);

            var repository = await handler.HandleAsync(new RemoteGithubCommand("arepo"));

            Assert.Null(repository);
        }


        private static User EmptyUser()
        {
            return new User(null, null, null,
                0, null, DateTimeOffset.MaxValue,
                DateTimeOffset.MaxValue, 0, "", 0,
                0, false, "", 0, 0, "",
                "login", "", "", 0, null, 0, 0, 0, "", null,
                false, "", null);
        }
    }
}
