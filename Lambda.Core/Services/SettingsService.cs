using System.Diagnostics;
using System.Security.Cryptography;
using Lambda.Core.Contracts.Services;
using Lambda.Core.Helpers;

namespace Lambda.Core.Services;

public class SettingsService<E> : ISettingsService<E> where E : Enum
{
    private readonly IFileService _fileService;
    private readonly string _directory;
    private readonly string _file;

    private bool _isInitialized;
    private static readonly SemaphoreSlim _semaphoreSlim = new(1);

    private Dictionary<string, object?> _settings = new();

    public SettingsService(IFileService fileService, ILambdaCoreConfiguration lambdaCoreConfiguration)
    {
        _fileService = fileService;
        _directory = lambdaCoreConfiguration.GetSettingsDirectory();
        _file = lambdaCoreConfiguration.GetSettingsFile();
    }

    private async Task InitializeAsync()
    {
        if (!_isInitialized)
        {
            await _semaphoreSlim.WaitAsync();
            _fileService.EnsureFileExists(_directory, _file);
            var savedSettings = await _fileService.Read<Dictionary<string, object?>>(_directory, _file);
            if (savedSettings != null)
            {
                _settings = savedSettings;
            }
            _semaphoreSlim.Release();

            _isInitialized = true;
        }
    }

    private void AssertKeyIsInOptions(E key)
    {
        Debug.Assert(Enum.IsDefined(typeof(E), key), $"Attempted to read unknown \"{key}\" settings key.");
    }

    public async Task<T?> ReadAsync<T>(E key)
    {
        AssertKeyIsInOptions(key);

        await InitializeAsync();

        if (_settings.TryGetValue(key.ToString(), out var stringifiedObject))
        {
            return await JsonHelper.DeserializeAsync<T>((string?)stringifiedObject);
        }

        return default;
    }

    public async Task WriteAsync<T>(E key, T value)
    {
        AssertKeyIsInOptions(key);

        await InitializeAsync();

        _settings[key.ToString()] = await JsonHelper.SerializeAsync(value);

        await _semaphoreSlim.WaitAsync();
        await _fileService.Write(_directory, _file, _settings);
        _semaphoreSlim.Release();
    }

    public async Task<T?> ReadSecretAsync<T>(E key)
    {
        AssertKeyIsInOptions(key);

        var encryptedValue = await ReadAsync<byte[]>(key);
        if (encryptedValue is not null)
        {
            var decryptedValue = ProtectedData.Unprotect(encryptedValue, null, DataProtectionScope.LocalMachine);
            return ByteHelper.ToObject<T>(decryptedValue);
        }

        return default;
    }

    public async Task WriteSecretAsync<T>(E key, T value)
    {
        AssertKeyIsInOptions(key);

        if (value is not null)
        {
            var _out = ProtectedData.Protect(ByteHelper.ToByteArray(value), null, DataProtectionScope.LocalMachine);
            await WriteAsync(key, _out);
        }
        else
        {
            await WriteAsync(key, value);
        }
    }
}
