using StarkSharp.Tools.Notification.NotifyPlatform;
using System;
using System.IO;

namespace StarkSharp.Tools.Notification
{
    public enum NotificationType
    {
        Info,
        Warning,
        Error
    }

    public enum NotificationPlatform
    {
        DotNet,
        Unity,
        Console
    }

    public class Notify
    {
        /*
         
        
         Notify.ShowNotification("Message Info", NotificationType.Info, Platform.DotNet);
         Notify.ShowNotification("Message Error", NotificationType.Error, Platform.Unity);
             
       */

        private static string logFilePath = "Logs";

        public static void ShowNotification(string message, NotificationType type, NotificationPlatform platform)
        {
            switch (platform)
            {
                case NotificationPlatform.DotNet:
                    DotnetNotify.HandleDotNetNotification(message, type);
                    break;
                case NotificationPlatform.Unity:
                    UnityNotify.HandleUnityNotification(message, type);
                    break;
                default:
                    throw new ArgumentException("Undefine platform");
            }
        }

      

        public static void LogToFile(string message, string type)
        {
            string fileName = $"{logFilePath}/{DateTime.Now:yyyy-MM-dd}.log";

            try
            {
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
