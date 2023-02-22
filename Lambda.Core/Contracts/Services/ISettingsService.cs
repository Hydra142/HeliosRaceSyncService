namespace Lambda.Core.Contracts.Services;

public interface ISettingsService<E> where E : Enum
{
    Task<T?> ReadAsync<T>(E key);

    Task WriteAsync<T>(E key, T value);

    Task<T?> ReadSecretAsync<T>(E key);

    Task WriteSecretAsync<T>(E key, T value);
}
