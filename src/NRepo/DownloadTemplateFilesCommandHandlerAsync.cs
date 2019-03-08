using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NRepo
{
    public  class DownloadTemplateFilesCommandHandlerAsync : ICommandHandlerAsync<DownloadTemplateFilesCommand, IEnumerable<string>>
    {
        private readonly ITemplateFilesService _templateFilesService;
        private readonly ILicensePicker _licensePicker;
        private readonly IFileService _fileService;

        public DownloadTemplateFilesCommandHandlerAsync(ITemplateFilesService templateFilesService, ILicensePicker licensePicker, IFileService fileService)
        {
            _templateFilesService = templateFilesService;
            _licensePicker = licensePicker;
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

            if (await _licensePicker.PickLicenseAsync() is var licenseFile)
            {
                filesToAdd.Add(licenseFile);
            }

            filesToAdd.Add(CreateReadMe());
            return filesToAdd;
        }
    }
}