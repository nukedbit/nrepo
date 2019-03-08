using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;

namespace NukedBit.NRepo
{
    public interface IGitHubLicenseApi
    {
        Task<IReadOnlyList<LicenseMetadata>> ListAsync();
        Task<string> DownloadLicenseContentAsync(LicenseMetadata metadata);
    }
}