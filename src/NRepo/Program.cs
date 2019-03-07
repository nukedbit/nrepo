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
                .ConfigureServices((context, services) => {
                    services
                        .AddSingleton<RemoteGithubCommandHandler>()
                        .AddSingleton<LicensePicker>()
                        .AddSingleton<RepositoryInitOrCreateCommandHandler>()
                        .AddSingleton<IGitHubLicenseApi, GitHubLicenseApi>()
                        .AddSingleton((_) =>
                        {
                            var client = new GitHubClient(new ProductHeaderValue("github-tools"))
                            {
                                Credentials = new Credentials(Environment.GetEnvironmentVariable("NREPO_GITHUB_TOKEN"))
                            };
                            return client;
                        })
                        .AddSingleton<IConsole>(PhysicalConsole.Singleton);
                })
                .RunCommandLineApplicationAsync<App>(args);
    }
}
