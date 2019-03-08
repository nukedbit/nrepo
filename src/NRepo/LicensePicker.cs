using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NRepo
{
    public class LicensePicker
    {
        private readonly IGitHubLicenseApi _gitHubLicenseApi;

        public LicensePicker(IGitHubLicenseApi gitHubLicenseApi)
        {
            _gitHubLicenseApi = gitHubLicenseApi;
        }

        public async Task<string> PickLicenseAsync()
        {
            Console.WriteLine("Downloading licenses list ...");
            var infos = await _gitHubLicenseApi.ListAsync();
            infos = infos.OrderBy(l => l.Name).ToList();
            Console.Clear();
            Console.WriteLine("Choose a License:");
            Console.WriteLine("Enter exit to cancel");
            Console.WriteLine();
            for (var i = 0; i < infos.Count; i++)
            {
                Console.WriteLine("{0}: {1}", i, infos[i].Name);
            }

            var licenseIndex = ConsoleUtils.ReadInputNumber(min: 1, max: infos.Count);
            if (licenseIndex is int index)
            {
                var licenseBody = await _gitHubLicenseApi.DownloadLicenseContentAsync(infos[index - 1]);
                var licenseFile = "LICENSE";
                var licenseFilePath = Path.Combine(Environment.CurrentDirectory, licenseFile);
                File.WriteAllText(licenseFilePath, licenseBody);
                return licenseFile;
            }

            return null;
        }
    }
}