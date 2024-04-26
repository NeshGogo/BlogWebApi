using Contracts;
using Serilog;

namespace LoggerService
{
    public class LoggerManager : ILoggerManager
    {
        private static ILogger logger = LoggerManager.logger;

        public void LogDebug(string message) => logger.Debug(message);
        public void LogError(string message) => logger.Error(message);  
        public void LogInfo(string message) => logger.Information(message);
        public void LogWarning(string message) => logger.Warning(message);
    }
}
