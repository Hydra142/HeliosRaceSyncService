using Microsoft.Extensions.Logging;
using Lambda.Core.Contracts.Services;

namespace Lambda.Core.Helpers;

public static class LogHelper
{
    public static void ConfigureFile(ILoggingBuilder loggingBuilder, ILambdaCoreConfiguration lambdaCoreConfiguration)
    {
        var logsDirectory = lambdaCoreConfiguration.GetLogsDirectory();
        var logPath = Path.Combine(logsDirectory, "{0:yyyy}-{0:MM}-{0:dd}.log");

        loggingBuilder.AddFile(logPath, fileLoggerOpts =>
        {
            fileLoggerOpts.FormatLogFileName = fileName =>
            {
                if (Directory.Exists(logsDirectory))
                {
                    var expiredLogs =
                        new DirectoryInfo(logsDirectory)
                        .GetFiles("*.log")
                        .Where(p => p.CreationTime < DateTime.Now.AddMonths(-1).AddDays(-1))
                        .ToArray();

                    foreach (var file in expiredLogs)
                    {
                        File.Delete(file.FullName);
                    }
                }

                return string.Format(fileName, DateTime.UtcNow);
            };
        });
    }
}
