using System.Linq;
using System.Threading.Tasks;
using NukedBit.NRepo.Services;
using Octokit;
using Optional;
using Optional.Unsafe;

namespace NukedBit.NRepo
{
    public class RemoteGithubCommandHandlerAsync : ICommandHandlerAsync<RemoteGithubCommand, Option<Repository>>
    {
        private readonly IGitHubClient _client;
        private readonly IConsoleService _consoleService;

        public RemoteGithubCommandHandlerAsync(IGitHubClient client, IConsoleService consoleService)
        {
            _client = client;
            _consoleService = consoleService;
        }

        public async Task<Option<Repository>> HandleAsync(RemoteGithubCommand cmd)
        {
            _consoleService.WriteLine();
            _consoleService.WriteLine("Time to pick a remote:");
            _consoleService.WriteLine("1: Create New.");
            _consoleService.WriteLine("2: Search Existing.");
            _consoleService.WriteLine("3: I'll do it later.");
            var choice = _consoleService.ReadInputNumber(min: 1, max: 3);

            if (!choice.HasValue || choice.ValueOrFailure() == 3)
            {
                return Option.None<Repository>();
            }
            else if (choice.ValueOrFailure() == 1)
            {
                try
                {

                    _consoleService.WriteLine("Your repository should be?");
                    _consoleService.WriteLine("If you input \"exit\", will skip creating the repository.");
                    _consoleService.WriteLine("1: Private");
                    _consoleService.WriteLine("2: Public");
                    var choiceOption = _consoleService.ReadInputNumber(1,2);
                    var privateOption = choiceOption.Map((value) => value == 1 ? true : false);
                    if (privateOption.ValueOrDefault() is bool visibility)
                    {
                        return Option.Some(await _client.Repository.Create(new NewRepository(cmd.RepoName){ Private = visibility}));
                    }

                    return Option.None<Repository>();
                }
                catch (Octokit.RepositoryExistsException repositoryExistsException)
                {
                    _consoleService.WriteLine("A repository with the same name already exist on GitHub.");
                    _consoleService.WriteLine($"Repository Name: {repositoryExistsException.RepositoryName}");
                    _consoleService.WriteLine("Do you want to use this one?");
                    if (_consoleService.AskForConfirmation())
                    {
                        var user = await _client.User.Current();
                        return Option.Some(await _client.Repository.Get(user.Login, repositoryExistsException.RepositoryName));
                    }
                    else
                    {
                        return await HandleAsync(cmd);
                    }
                }
            }
            else if (choice.ValueOrFailure() == 2)
            {
                _consoleService.WriteLine("You can type a search string:");
                var search = _consoleService.ReadLine();
                _consoleService.WriteLine("Retrieving repo list...");
                var repoList = await _client.Repository.GetAllForCurrent();
                if (!string.IsNullOrEmpty(search))
                {
                    repoList = repoList.Where(r => r.Name.Contains(search.Trim())).Select(repository =>
                        {
                            var sort = repository.Name.StartsWith(search.Trim()) ? 1 : 0;
                            return (sort, repository);
                        })
                        .OrderByDescending(tuple => tuple.sort)
                        .Select(tuple => tuple.repository)
                        .ToList();
                }

                _consoleService.WriteLine("Pick a remote:");

                for (var i = 0; i < repoList.Count; i++)
                {
                    _consoleService.WriteLine($"{i + 1}: {repoList[i].Name}");
                }

                choice = _consoleService.ReadInputNumber(min: 1, max: repoList.Count);
                if (!choice.HasValue)
                {
                    return Option.None<Repository>();
                }

                return Option.Some(repoList[choice.ValueOrFailure() - 1]);
            }

            return Option.None<Repository>();
        }
    }
}