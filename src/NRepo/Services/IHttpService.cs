using System.Threading.Tasks;

namespace NukedBit.NRepo.Services
{
    public interface IHttpService
    {
        Task DownloadFileTaskAsync(string url, string filePath);
    }
}