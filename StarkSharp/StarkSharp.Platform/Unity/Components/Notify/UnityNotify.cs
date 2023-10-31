using System;

namespace StarkSharp.Tools.Notification.NotifyPlatform
{
    public class UnityNotify : Notify
    {
        public static void HandleUnityNotification(string message, NotificationType type)
        {
            LogToFile(message, type.ToString());

            switch (type)
            {
                case NotificationType.Info:
                    UnityEngine.Debug.Log($"[INFO] {message}");
                    break;
                case NotificationType.Warning:
                    UnityEngine.Debug.LogWarning($"[WARNING] {message}");
                    break;
                case NotificationType.Error:
                    UnityEngine.Debug.LogError($"[ERROR] {message}");
                    break;
                default:
                    throw new ArgumentException("Invalid declaration type");
            }
        }
    }
}
