using System.Diagnostics;

namespace Lambda.ServiceController.Helpers;

public static class ProcessHelper
{
    public static Process? RunAsAdministrator(string fileName, IEnumerable<string>? argumentList = null)
    {
        var process = new Process
        {
            StartInfo = new()
            {
                FileName = fileName,
                UseShellExecute = true,
                //WindowStyle = ProcessWindowStyle.Hidden,
                Verb = "runas"
            }
        };

        if (argumentList != null)
        {
            foreach (var argument in argumentList)
            {
                process.StartInfo.ArgumentList.Add(argument);
            }
        }

        try
        {
            process.Start();
            return process;
        }
        catch
        {
            return null;
        }
    }
}
