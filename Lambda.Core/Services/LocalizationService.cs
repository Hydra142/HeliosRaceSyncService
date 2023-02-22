using System.Diagnostics;
using System.Globalization;
using System.Resources;
using Lambda.Core.Contracts.Services;

namespace Lambda.Core.Services;

public class LocalizationService : ILocalizationService
{
    private readonly ResourceManager? resourceManager;
    private readonly CultureInfo? culture;

    public LocalizationService(ILambdaCoreConfiguration lambdaCoreConfiguration)
    {
        resourceManager = lambdaCoreConfiguration.GetLocalizationResourceManager();
        culture = lambdaCoreConfiguration.GetLocalizationCulture();
    }

    public string TryToLocalize(string resourceKey)
    {
        return GetResource(resourceKey);
    }

    public string TryToLocalize(string resourceKey, string[] formatArgs)
    {
        return string.Format(GetResource(resourceKey), formatArgs);
    }

    private string GetResource(string resourceKey)
    {
        if (resourceManager is null || culture is null)
        {
            return resourceKey;
        }

        var localizedString = resourceManager.GetString(resourceKey, culture);

        if (localizedString is null)
        {
            Debug.Fail($"Failed to localize \"{resourceKey}\" resource key with \"{culture.Name}\" locale.");
            return resourceKey;
        }

        return localizedString;
    }
}
