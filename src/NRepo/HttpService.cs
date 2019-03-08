using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NRepo
{
    public interface IHttpService
    {
        Task DownloadFileTaskAsync(string url, string filePath);
    }

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
