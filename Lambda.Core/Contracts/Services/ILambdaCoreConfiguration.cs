using System.Globalization;
using System.Resources;

namespace Lambda.Core.Contracts.Services;

public interface ILambdaCoreConfiguration
{
    string GetAppName();

    string GetRootDirectory();

    string GetLogsDirectory();

    string GetSettingsDirectory();
    string GetSettingsFile();

    ResourceManager? GetLocalizationResourceManager();
    CultureInfo? GetLocalizationCulture();
}
