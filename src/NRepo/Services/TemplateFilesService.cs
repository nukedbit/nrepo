using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NRepo.Services
{
    public class TemplateFilesService : ITemplateFilesService
    {
        private readonly IFileService _fileService;
        private readonly IConsoleService _consoleService;

        public TemplateFilesService(IFileService fileService, IConsoleService consoleService)
        {
            _fileService = fileService;
            _consoleService = consoleService;
        }

        static readonly List<(string filename, string url)> files = new List<(string filename, string url)>()
        {
            (".editorconfig", "https://raw.githubusercontent.com/aspnet/AspNetCore/master/.editorconfig"),
            (".gitattributes","https://raw.githubusercontent.com/aspnet/AspNetCore/master/.gitattributes"),
            (".gitignore","https://raw.githubusercontent.com/aspnet/AspNetCore/master/.gitignore")
        };

        public  async Task<List<string>> DownloadAsync()
        {
            _consoleService.WriteLine();
            var client = new WebClient();
            foreach (var (name , url) in files)
            {
                var filePath = Path.Combine(_fileService.GetCurrentDirectory(), name);
                if (_fileService.FileExists(filePath))
                {
                    _consoleService.WriteLine("File {0} already exists! skipping...", name);
                    continue;
                }
                _consoleService.WriteLine("Downloading {0} ...", name);
                await client.DownloadFileTaskAsync(new Uri(url), filePath);
            }
            _consoleService.WriteLine();
            return files.Select(f => f.filename).ToList();
        }
    }
}