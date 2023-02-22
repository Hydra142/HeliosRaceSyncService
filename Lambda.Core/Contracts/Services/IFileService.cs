namespace Lambda.Core.Contracts.Services;

public interface IFileService
{
    Task<T?> Read<T>(string directory, string fileName);

    void EnsureDirectoryExists(string directory);

    void EnsureFileExists(string directory, string fileName);

    Task Write<T>(string directory, string fileName, T contents);

    void Delete(string directory, string fileName);
}
