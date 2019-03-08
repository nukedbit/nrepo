using System.Threading.Tasks;

namespace NRepo.Services
{
    public interface IHttpService
    {
        Task DownloadFileTaskAsync(string url, string filePath);
    }
}