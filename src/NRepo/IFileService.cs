namespace NRepo
{
    public interface IFileService
    {
        string GetCurrentDirectory();
        void WriteAllText(string filename, string contents);
        bool DirectoryExists(string dirPath);
        bool FileExists(string filename);
        string GetCurrentDirectoryName();
        void SetCurrentDirectory(string path);
    }
}