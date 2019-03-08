namespace NukedBit.NRepo
{
    public class RepositoryInitOrCreateCommand 
    {
        public RepositoryInitOrCreateCommand(string repoPath)
        {
            RepoPath = repoPath;
        }

        public string RepoPath { get; set; }
    }
}