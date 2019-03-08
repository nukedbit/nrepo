using System.Threading.Tasks;

namespace NRepo.Services
{
    public interface ILicenseService
    {
        Task<string> PickLicenseAsync();
    }
}