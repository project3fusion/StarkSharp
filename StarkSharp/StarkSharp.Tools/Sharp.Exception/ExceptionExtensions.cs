using System;
using StarkSharp.Tools.Notification;

namespace StarkSharp.Tools.Exception
{
    /// <summary>
    /// Extension methods for exception handling
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Converts any exception to StarkSharpException
        /// </summary>
        public static StarkSharpException ToStarkSharpException(this System.Exception exception)
        {
            return ErrorHandler.HandleException(exception);
        }

        /// <summary>
        /// Logs the exception using Notify
        /// </summary>
        public static void LogException(this System.Exception exception, NotificationPlatform platform = NotificationPlatform.Console)
        {
            var starkSharpException = exception.ToStarkSharpException();
            var message = $"[{starkSharpException.ErrorCode}] {starkSharpException.Message}";
            Notify.ShowNotification(message, NotificationType.Error, platform);
        }

        /// <summary>
        /// Gets a user-friendly error message
        /// </summary>
        public static string GetUserFriendlyMessage(this System.Exception exception)
        {
            var starkSharpException = exception.ToStarkSharpException();
            return ErrorCodeHelper.GetErrorMessage(starkSharpException.ErrorCode);
        }

        /// <summary>
        /// Gets error code from exception
        /// </summary>
        public static StarkSharpErrorCode GetErrorCode(this System.Exception exception)
        {
            if (exception is StarkSharpException starkSharpException)
            {
                return starkSharpException.ErrorCode;
            }
            return ErrorHandler.HandleException(exception).ErrorCode;
        }

        /// <summary>
        /// Gets error category from exception
        /// </summary>
        public static string GetErrorCategory(this System.Exception exception)
        {
            if (exception is StarkSharpException starkSharpException)
            {
                return starkSharpException.ErrorCategory;
            }
            return ErrorHandler.HandleException(exception).ErrorCategory;
        }

        /// <summary>
        /// Checks if exception is of specific error code
        /// </summary>
        public static bool IsErrorCode(this System.Exception exception, StarkSharpErrorCode errorCode)
        {
            return exception.GetErrorCode() == errorCode;
        }

        /// <summary>
        /// Checks if exception is in specific category
        /// </summary>
        public static bool IsErrorCategory(this System.Exception exception, string category)
        {
            return exception.GetErrorCategory() == category;
        }
    }
}

