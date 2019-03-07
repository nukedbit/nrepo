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

        public async Task<string> HandleAsync(NewGitHubRepoCommand cmd)
        {
            Repository result = null;

            Console.WriteLine("Time to pick a remote:");
            Console.WriteLine("1: Create New");
            Console.WriteLine("2: Search Existing");

            var choice = ConsoleUtils.ReadInputNumber();

            if (choice is null)
            {
                return null;
            }
            else if (choice == 1)
            {
                result = await _client.Repository.Create(new NewRepository(cmd.RepoName));
            }
            else if (choice == 2)
            {
                Console.WriteLine("You can type a search string:");
                var search = Console.ReadLine();
                Console.WriteLine("Retrieving repo list...");
                var repoList = await _client.Repository.GetAllForCurrent(new RepositoryRequest());
                if (!string.IsNullOrEmpty(search))
                {
                    repoList = repoList.Where(r => r.Name.Contains(search.Trim())).ToList();
                }

                Console.WriteLine("Pick a remote:");

                for (var i = 0; i < repoList.Count; i++)
                {
                    Console.WriteLine("{0}: {1}",i, repoList[i].Name);    
                }

                while (result is null)
                {
                    choice = ConsoleUtils.ReadInputNumber();
                    if (choice is null)
                    {
                        break;
                    }
                    if (choice >= 0 && choice < repoList.Count)
                    {
                        result = repoList[choice.Value];
                    }
                    Console.WriteLine("Try again...");
                }
            }
            
            return result?.CloneUrl;
        }
    }
}