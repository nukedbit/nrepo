using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;

namespace NRepo
{
    public class GitHubLicenseApi : IGitHubLicenseApi
    {
        private readonly IGitHubClient _client;

        public GitHubLicenseApi(IGitHubClient client)
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