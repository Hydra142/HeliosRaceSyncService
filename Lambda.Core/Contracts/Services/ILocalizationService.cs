namespace Lambda.Core.Contracts.Services;

public interface ILocalizationService
{
    string TryToLocalize(string resourceKey);

    string TryToLocalize(string resourceKey, string[] formatArgs);
}
