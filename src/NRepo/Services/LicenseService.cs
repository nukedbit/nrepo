using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NRepo.Services
{
    public class LicenseService : ILicenseService
    {
        private readonly IGitHubLicenseApi _gitHubLicenseApi;
        private readonly IConsoleService _consoleService;
        private readonly IFileService _fileService;

        public LicenseService(IGitHubLicenseApi gitHubLicenseApi, IConsoleService consoleService, IFileService fileService)
        {
            _gitHubLicenseApi = gitHubLicenseApi;
            _consoleService = consoleService;
            _fileService = fileService;
        }

        public async Task<string> PickLicenseAsync()
        {
            _consoleService.WriteLine("Downloading licenses list ...");
            var infos = await _gitHubLicenseApi.ListAsync();
            infos = infos.OrderBy(l => l.Name).ToList();
            _consoleService.WriteLine("Choose a License:");
            _consoleService.WriteLine("Enter exit to cancel");
            _consoleService.WriteLine();
            for (var i = 0; i < infos.Count; i++)
            {
                _consoleService.WriteLine("{0}: {1}", i + 1, infos[i].Name);
            }

            var inputNumber = _consoleService.ReadInputNumber(min: 1, max: infos.Count);
            if (inputNumber is int index && index >= 1 && index <= infos.Count)
            {
                var licenseBody = await _gitHubLicenseApi.DownloadLicenseContentAsync(infos[index - 1]);
                var licenseFile = "LICENSE";
                var licenseFilePath = Path.Combine(_fileService.GetCurrentDirectory(), licenseFile);
                _fileService.WriteAllText(licenseFilePath, licenseBody);
                return licenseFile;
            }

            return null;
        }
    }
}