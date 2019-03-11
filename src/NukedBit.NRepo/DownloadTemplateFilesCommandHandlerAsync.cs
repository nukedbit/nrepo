using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NukedBit.NRepo.Services;
using Optional.Unsafe;

namespace NukedBit.NRepo
{
    public  class DownloadTemplateFilesCommandHandlerAsync : ICommandHandlerAsync<DownloadTemplateFilesCommand, IEnumerable<string>>
    {
        private readonly ITemplateFilesService _templateFilesService;
        private readonly ILicenseService _licenseService;
        private readonly IFileService _fileService;

        public DownloadTemplateFilesCommandHandlerAsync(ITemplateFilesService templateFilesService, ILicenseService licenseService, IFileService fileService)
        {
            _templateFilesService = templateFilesService;
            _licenseService = licenseService;
            _fileService = fileService;
        }

        public string CreateReadMe()
        {
            var repoName = _fileService.GetCurrentDirectoryName();
            var name = repoName.Replace("-", " ");
            var filename = Path.Combine(_fileService.GetCurrentDirectory(), "./README.md");
            _fileService.WriteAllText(filename, "# " + name);
            return "README.md";
        }


        public async Task<IEnumerable<string>> HandleAsync(DownloadTemplateFilesCommand command)
        {
            var filesToAdd = await _templateFilesService.DownloadAsync();
            var license = await _licenseService.PickLicenseAsync();
            if (license.ValueOrDefault() is string licenseFile)
            {
                filesToAdd.Add(licenseFile);
            }

            filesToAdd.Add(CreateReadMe());
            return filesToAdd;
        }
    }
}