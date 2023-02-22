using System.Globalization;
using System.Resources;
using Lambda.Core.Contracts.Services;
using Lambda.ServiceController.Contracts.Services;

namespace Helios.Services;

public enum HeliosSettingsKeys
{
    Counter
}

public class HeliosConfiguration : ILambdaCoreConfiguration, ILambdaServiceControllerConfiguration
{
    public string GetAppName() => "Helios";

    public string GetRootDirectory() =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), GetAppName());

    public string GetLogsDirectory() => Path.Combine(GetRootDirectory(), "Logs");

    public string GetSettingsDirectory() => GetRootDirectory();
    public string GetSettingsFile() => "Settings.json";

    public CultureInfo? GetLocalizationCulture() => null;
    public ResourceManager? GetLocalizationResourceManager() => null;
}
