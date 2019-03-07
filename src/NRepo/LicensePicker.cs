using System;
using System.IO;
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
            Console.Clear();
            Console.WriteLine("Choose a License:");
            Console.WriteLine("Enter exit to cancel");
            Console.WriteLine();
            for (var i = 0; i < infos.Count; i++)
            {
                Console.WriteLine("{0}: {1}", i, infos[i].Name);
            }

            var licenseIndex = -1;
            while (true)
            {
                var line = Console.ReadLine();
                if (int.TryParse(line, out var index))
                {
                    licenseIndex = index;
                    break;
                }

                if (line.Trim() == "exit")
                {
                    break;
                }
            }
            if (licenseIndex == -1)
            {
                return null;
            }

            var licenseBody = await _gitHubLicenseApi.DownloadLicenseContentAsync(infos[licenseIndex]);
            var licenseFile =  "LICENSE";
            var licenseFilePath = Path.Combine(Environment.CurrentDirectory, licenseFile);
            File.WriteAllText(licenseFilePath, licenseBody);
            return licenseFile;
        }
    }
}