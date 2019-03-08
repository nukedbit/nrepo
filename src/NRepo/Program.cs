using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NRepo.Services;
using Octokit;

namespace NRepo
{
    public interface ICommandHandler
    {
        Task<TResult> HandleAsync<TCommand, TResult>(TCommand command);
        Task HandleAsync<TCommand>(TCommand command);
        void Handle<TCommand>(TCommand command);
    }

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

    class Program
    {

        public static async Task Main(string[] args) =>
            await new HostBuilder()
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddConsole();
                })
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddSingleton<ILicenseService, LicenseService>()
                        .AddSingleton<ICommandHandler<RepositoryInitOrCreateCommand>, RepositoryInitOrCreateCommandHandler>()
                        .AddSingleton<ICommandHandler<FinishRepoSetupCommand>, FinishRepoSetupCommandHandler>()
                        .AddSingleton<IGitHubLicenseApi, GitHubLicenseApi>()
                        .AddSingleton<IFileService, FileService>()
                        .AddSingleton<IConsoleService, ConsoleService>()
                        .AddSingleton<ITemplateFilesService, TemplateFilesService>()
                        .AddSingleton<ICommandHandlerAsync<NewGitHubRepoCommand, Repository>, RemoteGithubCommandHandlerAsync>()
                        .AddSingleton<IHttpService, HttpService>()
                        .AddSingleton<ICommandHandlerAsync<DownloadTemplateFilesCommand, IEnumerable<string>>, DownloadTemplateFilesCommandHandlerAsync>()
                        .AddSingleton<ICommandHandler, CommandHandler>()
                        .AddSingleton((_) =>
                        {
                            var client = new GitHubClient(new ProductHeaderValue("github-tools"))
                            {
                                Credentials = new Credentials(Environment.GetEnvironmentVariable("NREPO_GITHUB_TOKEN"))
                            };
                            return client;
                        })
                        .AddSingleton(PhysicalConsole.Singleton);
                })
                .RunCommandLineApplicationAsync<App>(args);
    }
}
