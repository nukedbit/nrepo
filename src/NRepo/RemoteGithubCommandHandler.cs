using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octokit;

namespace NRepo
{
    public class RemoteGithubCommandHandler
    {
        private readonly GitHubClient _client;

        public RemoteGithubCommandHandler(GitHubClient client)
        {
            _client = client;
        }

        public async Task<Repository> HandleAsync(NewGitHubRepoCommand cmd)
        {
            Repository result = null;

            Console.WriteLine();
            Console.WriteLine("Time to pick a remote:");
            Console.WriteLine("1: Create New");
            Console.WriteLine("2: Search Existing");

            var choice = ConsoleUtils.ReadInputNumber(min: 1, max: 2);

            if (choice is null)
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
                    Console.WriteLine("A repository with the same name already exist on GitHub.");
                    Console.WriteLine("Repository Name: {0}", repositoryExistsException.RepositoryName);
                    Console.WriteLine("Do you want to use this one?");
                    if (ConsoleUtils.AskForConfirmation())
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
                Console.WriteLine("You can type a search string:");
                var search = Console.ReadLine();
                Console.WriteLine("Retrieving repo list...");
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

                Console.WriteLine("Pick a remote:");

                for (var i = 0; i < repoList.Count; i++)
                {
                    Console.WriteLine("{0}: {1}", i + 1, repoList[i].Name);
                }


                choice = ConsoleUtils.ReadInputNumber(min: 1, max: repoList.Count);
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