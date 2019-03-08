using System.Threading.Tasks;

namespace NukedBit.NRepo
{
    public interface ICommandHandler
    {
        Task<TResult> HandleAsync<TCommand, TResult>(TCommand command);
        Task HandleAsync<TCommand>(TCommand command);
        void Handle<TCommand>(TCommand command);
    }
}