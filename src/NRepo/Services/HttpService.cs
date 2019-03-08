using System.Net;
using System.Threading.Tasks;

namespace NukedBit.NRepo.Services
{
    public class HttpService : IHttpService
    {
        public async Task DownloadFileTaskAsync(string url, string filePath)
        {
            using (var client = new WebClient())
            {
                await client.DownloadFileTaskAsync(url, filePath);
            }
        }
    }
}
