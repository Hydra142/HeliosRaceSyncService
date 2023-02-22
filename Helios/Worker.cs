using Helios.Services;
using Lambda.Core.Contracts.Services;
using Lambda.ServiceController.Contracts.Services;

namespace Helios;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ISettingsService<HeliosSettingsKeys> _settingsService;
    private readonly ILambdaCoreConfiguration _lambdaCoreConfiguration;
    private readonly IServiceController _serviceController;

    public Worker(
        ILogger<Worker> logger,
        ISettingsService<HeliosSettingsKeys> settingsService,
        ILambdaCoreConfiguration lambdaCoreConfiguration,
        IServiceController serviceController)
    {
        _logger = logger;
        _settingsService = settingsService;
        _lambdaCoreConfiguration = lambdaCoreConfiguration;
        _serviceController = serviceController;
    }

    // Demonstrational content
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _serviceController.DispatchAction(ServiceControllerAction.Start);

        var settingsPath = Path.Combine(
            _lambdaCoreConfiguration.GetSettingsDirectory(),
            _lambdaCoreConfiguration.GetSettingsFile()
        );

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker is running");

            var counter = await _settingsService.ReadAsync<int>(HeliosSettingsKeys.Counter);
            _logger.LogInformation(
                $"Value of \"{nameof(HeliosSettingsKeys.Counter)}\" key " +
                $"from \"{settingsPath}\" file: {counter}"
            );
            counter++;
            await _settingsService.WriteAsync(HeliosSettingsKeys.Counter, counter);

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}
