using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NRepo
{
    public static class RepoFiles
    {
        static readonly List<(string filename, string url)> files = new List<(string filename, string url)>()
        {
            (".editorconfig", "https://raw.githubusercontent.com/aspnet/AspNetCore/master/.editorconfig"),
            (".gitattributes","https://raw.githubusercontent.com/aspnet/AspNetCore/master/.gitattributes"),
            (".gitignore","https://raw.githubusercontent.com/aspnet/AspNetCore/master/.gitignore")
        };

        public static async Task<List<string>> DownloadAsync()
        {
            var client = new WebClient();
            foreach (var (name , url) in files)
            {
                var filePath = Path.Combine(Environment.CurrentDirectory, name);
                if (File.Exists(filePath))
                {
                    Console.WriteLine("File {0} already exists! skipping...", name);
                    continue;
                }
                Console.WriteLine("Downloading {0} ...", name);
                await client.DownloadFileTaskAsync(new Uri(url), filePath);
            }
            return files.Select(f => f.filename).ToList();
        }
    }
}