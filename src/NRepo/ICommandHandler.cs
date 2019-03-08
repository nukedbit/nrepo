using System.Threading.Tasks;

namespace NRepo
{
    public interface ICommandHandler<in TCommand, TResult>
    {
        Task<TResult> HandleAsync(TCommand command);
    }
}