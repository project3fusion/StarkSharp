using System;
using Newtonsoft.Json;

namespace StarkSharp.Tools.Exception
{
    /// <summary>
    /// Standardized error response structure
    /// </summary>
    public class ErrorResponse
    {
        [JsonProperty("errorCode")]
        public int ErrorCode { get; set; }

        [JsonProperty("errorName")]
        public string ErrorName { get; set; }

        [JsonProperty("errorCategory")]
        public string ErrorCategory { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("additionalData")]
        public object AdditionalData { get; set; }

        [JsonProperty("innerException")]
        public string InnerException { get; set; }

        public ErrorResponse()
        {
            Timestamp = DateTime.UtcNow;
        }

        public ErrorResponse(StarkSharpException exception) : this()
        {
            ErrorCode = (int)exception.ErrorCode;
            ErrorName = exception.ErrorCode.ToString();
            ErrorCategory = exception.ErrorCategory;
            Message = exception.Message;
            AdditionalData = exception.AdditionalData;
            Timestamp = exception.Timestamp;

            if (exception.InnerException != null)
            {
                InnerException = exception.InnerException.ToString();
            }
        }

        public static ErrorResponse FromException(System.Exception exception)
        {
            var starkSharpException = exception.ToStarkSharpException();
            return new ErrorResponse(starkSharpException);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public override string ToString()
        {
            return $"[{ErrorCode}] [{ErrorCategory}] {Message}";
        }
    }
}

