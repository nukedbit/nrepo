using System.Threading.Tasks;
using Optional;

namespace NukedBit.NRepo.Services
{
    public interface ILicenseService
    {
        Task<Option<string>> PickLicenseAsync();
    }
}