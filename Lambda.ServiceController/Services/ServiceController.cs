using Lambda.ServiceController.Contracts.Services;
using Lambda.ServiceController.Helpers;
using Microsoft.Extensions.Logging;

namespace Lambda.ServiceController.Services;

public class ServiceController : IServiceController
{
    private readonly ILogger _logger;
    private readonly ILambdaServiceControllerConfiguration _configuration;

    public ServiceController(
        ILogger<ServiceController> logger,
        ILambdaServiceControllerConfiguration lambdaServiceControllerConfiguration)
    {
        _logger = logger;
        _configuration = lambdaServiceControllerConfiguration;
    }

    private string ToString(ServiceControllerAction action)
    {
        return action.ToString().ToLower();
    }

    public async Task DispatchAction(ServiceControllerAction action)
    {
        var binDirectory = AppDomain.CurrentDomain.BaseDirectory;

        var serviceControllerBin = $"{typeof(Program).Assembly.GetName().Name}.exe";
        var serviceControllerPath = Path.Combine(binDirectory, serviceControllerBin);

        var serviceName = _configuration.GetAppName();

        if (serviceName is null)
        {
            var errorMessage = $"Failed to get app name from calling assembly assebmly.";
            _logger.LogError(errorMessage);
            throw new Exception(errorMessage);
        }

        var servicePath = Path.Combine(binDirectory, $"{serviceName}.exe");

        var arguments = new List<string>()
        {
            serviceName,
            servicePath,
            ToString(action),
        };

        var process = ProcessHelper.RunAsAdministrator(serviceControllerPath, arguments);

        if (process is null)
        {
            var errorMessage = $"Failed to run service controller with admin privileges by \"{serviceControllerPath}\" path.";
            _logger.LogError(errorMessage);
            throw new Exception(errorMessage);
        }

        await process.WaitForExitAsync();
    }
}
