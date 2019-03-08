using System.Threading.Tasks;

namespace NRepo
{
    public interface IRepositoryInitOrCreateCommandHandler
    {
        Task ExecuteAsync(RepositoryInitOrCreateCommand command);
    }
}