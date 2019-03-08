using System;
using System.IO;

namespace NukedBit.NRepo.Services
{
    public class FileService : IFileService
    {
        public string GetCurrentDirectory()
        {
            return Environment.CurrentDirectory;
        }

        public void WriteAllText(string filename, string contents)
        {
            File.WriteAllText(filename, contents);
        }

        public bool DirectoryExists(string dirPath)
        {
            return Directory.Exists(dirPath);
        }

        public bool FileExists(string filename)
        {
            return File.Exists(filename);
        }

        public string GetCurrentDirectoryName()
        {
            return new DirectoryInfo(Environment.CurrentDirectory).Name;
        }

        public void SetCurrentDirectory(string path) => Environment.CurrentDirectory = path;
    }
}
