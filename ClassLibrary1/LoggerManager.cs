using Contracts;
using Serilog;

namespace LoggerService
{
    public class LoggerManager : ILoggerManager
    {
        public void LogDebug(string message) => Log.Debug(message);
        public void LogError(string message) => Log.Error(message);  
        public void LogInfo(string message) => Log.Information(message);
        public void LogWarning(string message) => Log.Warning(message);
    }
}
