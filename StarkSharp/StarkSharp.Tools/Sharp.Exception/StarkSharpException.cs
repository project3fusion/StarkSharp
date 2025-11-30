using System;

namespace StarkSharp.Tools.Exception
{
    /// <summary>
    /// Base exception class for all StarkSharp exceptions
    /// </summary>
    public class StarkSharpException : System.Exception
    {
        public StarkSharpErrorCode ErrorCode { get; }
        public string ErrorCategory { get; }
        public object AdditionalData { get; set; }
        public DateTime Timestamp { get; }

        public StarkSharpException(StarkSharpErrorCode errorCode, string message) 
            : base(message)
        {
            ErrorCode = errorCode;
            ErrorCategory = ErrorCodeHelper.GetCategory(errorCode);
            Timestamp = DateTime.UtcNow;
        }

        public StarkSharpException(StarkSharpErrorCode errorCode, string message, System.Exception innerException) 
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            ErrorCategory = ErrorCodeHelper.GetCategory(errorCode);
            Timestamp = DateTime.UtcNow;
        }

        public StarkSharpException(StarkSharpErrorCode errorCode, string message, object additionalData) 
            : base(message)
        {
            ErrorCode = errorCode;
            ErrorCategory = ErrorCodeHelper.GetCategory(errorCode);
            AdditionalData = additionalData;
            Timestamp = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"[{ErrorCode}] [{ErrorCategory}] {Message} (Timestamp: {Timestamp:yyyy-MM-dd HH:mm:ss})";
        }
    }
}

