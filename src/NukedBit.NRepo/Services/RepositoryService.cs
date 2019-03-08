using LibGit2Sharp;

namespace NukedBit.NRepo.Services
{ 
    public class RepositoryService : IRepositoryService
    {
        public string Init(string path) => Repository.Init(path);
    }
}
