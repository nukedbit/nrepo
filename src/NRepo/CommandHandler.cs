using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace NukedBit.NRepo
{
    public class CommandHandler : ICommandHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<TResult> HandleAsync<TCommand, TResult>(TCommand command)
        {
            var handlerAsync = _serviceProvider.GetService<ICommandHandlerAsync<TCommand, TResult>>();
            return handlerAsync.HandleAsync(command);
        }

        public Task HandleAsync<TCommand>(TCommand command)
        {
            var handlerAsync = _serviceProvider.GetService<ICommandHandlerAsync<TCommand>>();
            return handlerAsync.HandleAsync(command);
        }

        public void Handle<TCommand>(TCommand command)
        {
            var handlerAsync = _serviceProvider.GetService<ICommandHandler<TCommand>>();
            handlerAsync.Handle(command);
        }
    }
}