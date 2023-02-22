using System.Diagnostics;
using System.ServiceProcess;
using Microsoft.Win32;

internal class Program
{
    public static void Main(string[] args)
    {
        var serviceName = args[0];
        var binPath = args[1];
        var action = args.Length == 3 ? args[2] : "toggle";

        ServiceController service = new ServiceController(serviceName);

        if (!IsServiceInstalled(service))
        {
            InstallService(serviceName, binPath);
            service = new ServiceController(serviceName);
        }

        if (!IsBinPathMatches(serviceName, binPath))
        {
            DeleteService(serviceName);
            InstallService(serviceName, binPath);
            service = new ServiceController(serviceName);
        }

        if (action == "start")
        {
            StartService(service);
        }
        else if (action == "stop")
        {
            StopService(service);
        }
        else if (action == "toggle")
        {
            ToggleService(service);
        }
        else
        {
            return;
        }
    }

    private static void InstallService(string serviceName, string binPath)
    {
        Process.Start("sc.exe", $"create {serviceName} binPath= {binPath}").WaitForExit();
    }

    private static void DeleteService(string serviceName)
    {
        Process.Start("sc.exe", $"delete {serviceName}").WaitForExit();
    }

    private static void StartService(ServiceController service)
    {
        if (service.Status.Equals(ServiceControllerStatus.Running))
        {
            return;
        }

        service.Start();
        service.WaitForStatus(ServiceControllerStatus.Running);
        service.Refresh();
    }

    private static void StopService(ServiceController service)
    {
        if (service.Status.Equals(ServiceControllerStatus.Stopped)) return;

        service.Stop();
        service.WaitForStatus(ServiceControllerStatus.Stopped);
        service.Refresh();
    }

    private static void ToggleService(ServiceController service)
    {
        if ((service.Status.Equals(ServiceControllerStatus.Stopped)) ||
        (service.Status.Equals(ServiceControllerStatus.StopPending)))
        {
            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running);
        }
        else
        {
            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped);
        }
        service.Refresh();
    }

    private static bool IsServiceInstalled(ServiceController service)
    {
        try
        {
            var temp = service.DisplayName;
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    private static bool IsBinPathMatches(string serviceName, string binPath)
    {
        var services = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services");

        return string.Equals((string?)services?.OpenSubKey(serviceName)?.GetValue("ImagePath"), binPath, StringComparison.InvariantCultureIgnoreCase);
    }
}
