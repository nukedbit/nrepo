namespace NRepo
{
    public class RemoteGithubCommand
    {
        public string RepoName { get; }

        public RemoteGithubCommand(string repoName)
        {
            RepoName = repoName;
        }
    }
}