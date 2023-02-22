using System.Text;
using Lambda.Core.Contracts.Services;
using Lambda.Core.Helpers;
using Microsoft.Extensions.Logging;

namespace Lambda.Core.Services;

public class FileService : IFileService
{
    private readonly ILogger _logger;

    public FileService(ILogger<FileService> logger)
    {
        _logger = logger;
    }

    public async Task<T?> Read<T>(string directory, string fileName)
    {
        var path = Path.Combine(directory, fileName);

        if (File.Exists(path))
        {
            string? fileContents;
            try
            {
                fileContents = File.ReadAllText(path);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to read contents of \"{path}\" file.\n{ex.Message}");
                throw;
            }

            T? deserializedObject;
            try
            {
                deserializedObject = await JsonHelper.DeserializeAsync<T>(fileContents);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to deserialize \"{nameof(T)}\" from \"{fileContents}\".\n{ex.Message}");
                throw;
            }

            return deserializedObject;
        }

        var fileNotFoundException = new FileNotFoundException($"\"{path}\" file is not found.");
        _logger.LogError(fileNotFoundException.Message);
        throw fileNotFoundException;
    }

    public void EnsureDirectoryExists(string directory)
    {
        if (Directory.Exists(directory))
        {
            return;
        }

        try
        {
            Directory.CreateDirectory(directory);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to create \"{directory}\" directory.\n{ex.Message}");
            throw;
        }
    }

    public void EnsureFileExists(string directory, string fileName)
    {
        EnsureDirectoryExists(directory);

        var path = Path.Combine(directory, fileName);

        if (File.Exists(path))
        {
            return;
        }

        try
        {
            var file = File.Create(path);
            file.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to create \"{path}\" file.\n{ex.Message}");
            throw;
        }
    }

    public async Task Write<T>(string directory, string fileName, T contents)
    {
        EnsureDirectoryExists(directory);

        var path = Path.Combine(directory, fileName);
        var serializedContents = await JsonHelper.SerializeAsync(contents);

        try
        {
            File.WriteAllText(path, serializedContents, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to write \"{serializedContents}\" to \"{path}\" file.\n{ex.Message}");
            throw;
        }
    }

    public void Delete(string directory, string fileName)
    {
        var path = Path.Combine(directory, fileName);

        if (File.Exists(path))
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to delete \"{path}\" file.\n{ex.Message}");
                throw;
            }
        }
    }
}
