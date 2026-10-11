using System;
using System.IO;

using StarkSharp.Tools.Notification.NotifyPlatform;

namespace StarkSharp.Tools.Notification
{
    public enum NotificationType
    {
        Info,
        Warning,
        Error,
        Success
    }
    public enum NotificationPlatform
    {
        DotNet,
        Console
    }
    public class Notify
    {
        private static string logFilePath = "Logs/";
        public static void ShowNotification(string message, NotificationType type, NotificationPlatform platform = NotificationPlatform.DotNet)
        {
            DotnetNotify.HandleDotNetNotification(message, type);
        }
        public static void LogToFile(string message, string type)
        {
            string logDirectory = Path.GetDirectoryName(logFilePath);
            string fileName = $"{logFilePath}/{DateTime.Now:yyyy-MM-dd}.log";

            try
            {
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                using (StreamWriter writer = new StreamWriter(fileName, true))
                {
                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{type}] {message}");
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"File write error: {ex.Message}");
            }
        }

    }
}
