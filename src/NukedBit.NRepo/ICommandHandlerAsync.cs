using System.Threading.Tasks;

namespace NukedBit.NRepo
{
    public interface ICommandHandlerAsync<in TCommand, TResult>
    {
        Task<TResult> HandleAsync(TCommand command);
    }

    public interface ICommandHandlerAsync<in TCommand>
    {
        Task HandleAsync(TCommand command);
    }

    public interface ICommandHandler<in TCommand>
    {
        void Handle(TCommand command);
    }
}