using System;
using System.IO;
using System.Threading.Tasks;

namespace NRepo
{
    public class LicensePicker
    {
        private readonly LicenseApi _licenseApi;

        public LicensePicker(LicenseApi licenseApi)
        {
            _licenseApi = licenseApi;
        }

        public async Task PickLicenseAsync(string repoPath)
        {
            Console.WriteLine("Downloading licenses list ...");
            var infos = await _licenseApi.ListAsync();
            Console.Clear();
            Console.WriteLine("Choose a License:");
            Console.WriteLine("Enter -1 to exit");
            Console.WriteLine();
            for (var i = 0; i < infos.Count; i++)
            {
                Console.WriteLine("{0}: {1}", i, infos[i].Name);
            }

            var licenseIndex = 0;
            while (licenseIndex != -1)
            {
                if (int.TryParse(Console.ReadLine(), out var index))
                {
                    licenseIndex = index;
                    break;
                }
            }
            if (licenseIndex == -1)
            {
                return;
            }

            var licenseBody = await _licenseApi.DownloadLicenseContentAsync(infos[licenseIndex]);
            var licenseFile = Path.Combine(repoPath, "LICENSE");
            File.WriteAllText(licenseFile, licenseBody);
        }
    }
}