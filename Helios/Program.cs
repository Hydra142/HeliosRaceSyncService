using Helios;
using Helios.Services;
using Lambda.Core.Contracts.Services;
using Lambda.Core.Helpers;
using Lambda.Core.Services;
using Lambda.ServiceController.Contracts.Services;
using Lambda.ServiceController.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddLogging(loggingBuilder => LogHelper.ConfigureFile(loggingBuilder, new HeliosConfiguration()));

        services.AddSingleton<ILambdaCoreConfiguration, HeliosConfiguration>();
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<ISettingsService<HeliosSettingsKeys>, SettingsService<HeliosSettingsKeys>>();

        services.AddHostedService<Worker>();

        services.AddSingleton<ILambdaServiceControllerConfiguration, HeliosConfiguration>();
        services.AddSingleton<IServiceController, ServiceController>();
    })
    .UseWindowsService(options => options.ServiceName = "Helios")
    .Build();

await host.RunAsync();
