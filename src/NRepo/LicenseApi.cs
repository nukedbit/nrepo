using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;

namespace NRepo
{
    public class LicenseApi
    {
        private readonly GitHubClient _client;

        public LicenseApi(GitHubClient client)
        {
            _client = client;
        }

        public async Task<IReadOnlyList<LicenseMetadata>> ListAsync()
        {
            return await _client.Miscellaneous.GetAllLicenses();
        }

        public async Task<string> DownloadLicenseContentAsync(LicenseMetadata metadata)
        {
            var license = await _client.Miscellaneous.GetLicense(metadata.Key);
            return license?.Body;
        }
    }
}