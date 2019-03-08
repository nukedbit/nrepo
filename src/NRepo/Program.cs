using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Octokit;

namespace NRepo
{
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
                        .AddSingleton<ILicensePicker, LicensePicker>()
                        .AddSingleton<IRepositoryInitOrCreateCommandHandler, RepositoryInitOrCreateCommandHandler>()
                        .AddSingleton<IGitHubLicenseApi, GitHubLicenseApi>()
                        .AddSingleton<IFileService, FileService>()
                        .AddSingleton<IConsoleService, ConsoleService>()
                        .AddSingleton<ITemplateFilesService, TemplateFilesService>()
                        .AddSingleton<ICommandHandler<NewGitHubRepoCommand, Repository>, RemoteGithubCommandHandler>()
                        .AddSingleton<IHttpService, HttpService>()
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
