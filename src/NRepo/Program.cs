﻿using System;
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
                        .AddSingleton<IRepositoryService, RepositoryService>()
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
