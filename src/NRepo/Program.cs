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
        private static string Token = "";

        public static async Task Main(string[] args) =>
            await new HostBuilder()
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddConsole();
                })
                .ConfigureServices((context, services) => {
                    services
                        .AddSingleton<NewGitHubRepoCommandHandler>()
                        .AddSingleton<NewRepoCommandHandler>()
                        .AddSingleton<LicensePicker>()
                        .AddSingleton<LicenseApi>()
                        .AddSingleton((_) =>
                        {
                            var client = new GitHubClient(new ProductHeaderValue("github-tools"))
                            {
                                Credentials = new Credentials(Token)
                            };
                            return client;
                        })
                        .AddSingleton<IConsole>(PhysicalConsole.Singleton);
                })
                .RunCommandLineApplicationAsync<App>(args);
    }
}
