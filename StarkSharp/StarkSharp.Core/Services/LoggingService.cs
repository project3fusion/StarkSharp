using StarkSharp.Core.Interfaces;
using StarkSharp.Tools.Notification;

namespace StarkSharp.Core.Services
{
    /// <summary>
    /// Logging service implementation
    /// </summary>
    public class LoggingService : ILoggingService
    {
        private readonly IPlatform _platform;

        public LoggingService(IPlatform platform)
        {
            _platform = platform;
        }

        public void Log(string message, NotificationType type)
        {
            _platform?.PlatformLog(message, type);
        }

        public void LogInfo(string message)
        {
            Log(message, NotificationType.Info);
        }

        public void LogWarning(string message)
        {
            Log(message, NotificationType.Warning);
        }

        public void LogError(string message)
        {
            Log(message, NotificationType.Error);
        }

        public void LogSuccess(string message)
        {
            Log(message, NotificationType.Success);
        }
    }
}

