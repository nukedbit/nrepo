using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octokit;

namespace NRepo
{
    public class RemoteGithubCommandHandlerAsync : ICommandHandlerAsync<NewGitHubRepoCommand, Repository>
    {
        private readonly GitHubClient _client;
        private readonly IConsoleService _consoleService;

        public RemoteGithubCommandHandlerAsync(GitHubClient client, IConsoleService consoleService)
        {
            _client = client;
            _consoleService = consoleService;
        }

        public async Task<Repository> HandleAsync(NewGitHubRepoCommand cmd)
        {
            Repository result = null;

            _consoleService.WriteLine();
            _consoleService.WriteLine("Time to pick a remote:");
            _consoleService.WriteLine("1: Create New.");
            _consoleService.WriteLine("2: Search Existing.");
            _consoleService.WriteLine("3: I'll do it later.");
            var choice = _consoleService.ReadInputNumber(min: 1, max: 3);

            if (choice is null || choice == 3)
            {
                return null;
            }
            else if (choice == 1)
            {
                try
                {
                    result = await _client.Repository.Create(new NewRepository(cmd.RepoName));
                }
                catch (Octokit.RepositoryExistsException repositoryExistsException)
                {
                    _consoleService.WriteLine("A repository with the same name already exist on GitHub.");
                    _consoleService.WriteLine("Repository Name: {0}", repositoryExistsException.RepositoryName);
                    _consoleService.WriteLine("Do you want to use this one?");
                    if (_consoleService.AskForConfirmation())
                    {
                        var user = await _client.User.Current();
                        result = await _client.Repository.Get(user.Login, repositoryExistsException.RepositoryName);
                    }
                    else
                    {
                        return await HandleAsync(cmd);
                    }
                }
            }
            else if (choice == 2)
            {
                _consoleService.WriteLine("You can type a search string:");
                var search = _consoleService.ReadLine();
                _consoleService.WriteLine("Retrieving repo list...");
                var repoList = await _client.Repository.GetAllForCurrent(new RepositoryRequest());
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
                    _consoleService.WriteLine("{0}: {1}", i + 1, repoList[i].Name);
                }

                choice = _consoleService.ReadInputNumber(min: 1, max: repoList.Count);
                if (choice is null)
                {
                    return null;
                }

                result = repoList[choice.Value - 1];

            }

            return result;
        }
    }
}