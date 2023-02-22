namespace Lambda.ServiceController.Contracts.Services;

public enum ServiceControllerAction
{
    Start,
    Toggle,
    Stop
}

public interface IServiceController
{
    Task DispatchAction(ServiceControllerAction action);
}
