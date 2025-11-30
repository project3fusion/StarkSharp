using StarkSharp.Tools.Notification;

namespace StarkSharp.Core.Interfaces
{
    /// <summary>
    /// Service interface for logging operations
    /// </summary>
    public interface ILoggingService
    {
        void Log(string message, NotificationType type);
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogSuccess(string message);
    }
}

