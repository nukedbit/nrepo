using System.Threading.Tasks;
using Octokit;

namespace NRepo
{
    public class NewGitHubRepoCommandHandler
    {
        private readonly GitHubClient _client;

        public NewGitHubRepoCommandHandler(GitHubClient client)
        {
            _client = client;
        }
        public async Task<string> HandleAsync(NewGitHubRepoCommand cmd)
        {          
            var result = await _client.Repository.Create(new NewRepository(cmd.RepoName));
            return result.CloneUrl;
        }
    }
}