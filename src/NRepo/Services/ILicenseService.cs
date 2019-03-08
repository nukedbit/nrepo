using System.Threading.Tasks;

namespace NukedBit.NRepo.Services
{
    public interface ILicenseService
    {
        Task<string> PickLicenseAsync();
    }
}