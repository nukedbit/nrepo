using System.Collections.Generic;
using System.Threading.Tasks;

namespace NukedBit.NRepo.Services
{
    public interface ITemplateFilesService
    {
        Task<List<string>> DownloadAsync();
    }
}