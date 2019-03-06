namespace NRepo
{
    public class NewGitHubRepoCommand
    {
        public string RepoName { get; }

        public NewGitHubRepoCommand(string repoName)
        {
            RepoName = repoName;
        }
    }
}