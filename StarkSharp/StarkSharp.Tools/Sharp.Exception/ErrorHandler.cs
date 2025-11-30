using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StarkSharp.Base.Net.Exception;
using StarkSharp.Rpc;
using StarkSharp.Tools.Notification;

namespace StarkSharp.Tools.Exception
{
    /// <summary>
    /// Centralized error handler for StarkSharp
    /// </summary>
    public static class ErrorHandler
    {
        /// <summary>
        /// Handles any exception and converts it to StarkSharpException
        /// </summary>
        public static StarkSharpException HandleException(System.Exception exception)
        {
            if (exception is StarkSharpException starkSharpException)
            {
                return starkSharpException;
            }

            return exception switch
            {
                // Network exceptions
                HttpRequestException httpEx => new StarkSharpException(
                    StarkSharpErrorCode.NetworkError,
                    $"HTTP request failed: {httpEx.Message}",
                    httpEx
                ),
                TaskCanceledException taskEx when taskEx.InnerException is TimeoutException => new StarkSharpException(
                    StarkSharpErrorCode.ConnectionTimeout,
                    "Connection timed out",
                    taskEx
                ),
                TaskCanceledException taskEx2 => new StarkSharpException(
                    StarkSharpErrorCode.Cancelled,
                    "Operation was cancelled",
                    taskEx2
                ),
                TimeoutException => new StarkSharpException(
                    StarkSharpErrorCode.Timeout,
                    "Operation timed out",
                    exception
                ),

                // RPC exceptions
                JsonRpcException rpcEx => HandleJsonRpcException(rpcEx),
                
                // Base.Net exceptions
                NetException netEx => new StarkSharpException(
                    StarkSharpErrorCode.NetworkError,
                    $"Network error: {netEx.Message}",
                    netEx
                ),
                ClientError clientEx => new StarkSharpException(
                    StarkSharpErrorCode.ClientError,
                    $"Client error: {clientEx.Message}",
                    clientEx
                ),

                // Standard .NET exceptions
                ArgumentNullException argNullEx => new StarkSharpException(
                    StarkSharpErrorCode.NullReference,
                    $"Null argument: {argNullEx.ParamName}",
                    argNullEx
                ),
                ArgumentException argEx => new StarkSharpException(
                    StarkSharpErrorCode.InvalidParameter,
                    $"Invalid argument: {argEx.Message}",
                    argEx
                ),
                NullReferenceException nullEx => new StarkSharpException(
                    StarkSharpErrorCode.NullReference,
                    "Null reference encountered",
                    nullEx
                ),
                InvalidOperationException invalidOpEx => new StarkSharpException(
                    StarkSharpErrorCode.InvalidOperation,
                    invalidOpEx.Message,
                    invalidOpEx
                ),
                InvalidCastException castEx => new StarkSharpException(
                    StarkSharpErrorCode.SerializationError,
                    $"Type conversion failed: {castEx.Message}",
                    castEx
                ),

                // JSON exceptions
                JsonException jsonEx => new StarkSharpException(
                    StarkSharpErrorCode.JsonParseError,
                    $"JSON error: {jsonEx.Message}",
                    jsonEx
                ),
                JsonSerializationException jsonSerEx => new StarkSharpException(
                    StarkSharpErrorCode.SerializationError,
                    $"JSON serialization error: {jsonSerEx.Message}",
                    jsonSerEx
                ),

                // Cryptographic exceptions
                CryptographicException cryptoEx => new StarkSharpException(
                    StarkSharpErrorCode.CryptographicError,
                    $"Cryptographic error: {cryptoEx.Message}",
                    cryptoEx
                ),

                // Default
                _ => new StarkSharpException(
                    StarkSharpErrorCode.UnknownError,
                    $"Unexpected error: {exception.Message}",
                    exception
                )
            };
        }

        /// <summary>
        /// Handles JsonRpcResponse errors
        /// </summary>
        public static StarkSharpException HandleJsonRpcError(JsonRpcResponse response)
        {
            if (response?.error == null)
            {
                return new StarkSharpException(
                    StarkSharpErrorCode.RpcError,
                    "RPC response contains error but error details are missing"
                );
            }

            var errorCode = response.error.code;
            var errorMessage = response.error.message ?? "Unknown RPC error";

            var starkSharpErrorCode = errorCode switch
            {
                -32700 => StarkSharpErrorCode.RpcParseError,
                -32600 => StarkSharpErrorCode.RpcInvalidRequest,
                -32601 => StarkSharpErrorCode.RpcMethodNotFound,
                -32602 => StarkSharpErrorCode.RpcInvalidParams,
                -32603 => StarkSharpErrorCode.RpcInternalError,
                >= -32099 and <= -32000 => StarkSharpErrorCode.RpcServerError,
                _ => StarkSharpErrorCode.RpcError
            };

            return new StarkSharpException(
                starkSharpErrorCode,
                $"RPC Error ({errorCode}): {errorMessage}",
                response.error
            );
        }

        /// <summary>
        /// Handles JsonRpcException
        /// </summary>
        private static StarkSharpException HandleJsonRpcException(JsonRpcException exception)
        {
            return new StarkSharpException(
                StarkSharpErrorCode.RpcError,
                $"RPC exception: {exception.Message}",
                exception
            );
        }

        /// <summary>
        /// Handles HTTP status codes
        /// </summary>
        public static StarkSharpException HandleHttpStatusCode(HttpStatusCode statusCode, string message = null)
        {
            var errorCode = statusCode switch
            {
                HttpStatusCode.BadRequest => StarkSharpErrorCode.ClientError,
                HttpStatusCode.Unauthorized => StarkSharpErrorCode.ClientError,
                HttpStatusCode.Forbidden => StarkSharpErrorCode.ClientError,
                HttpStatusCode.NotFound => StarkSharpErrorCode.ClientError,
                HttpStatusCode.RequestTimeout => StarkSharpErrorCode.ConnectionTimeout,
                HttpStatusCode.InternalServerError => StarkSharpErrorCode.ServerError,
                HttpStatusCode.BadGateway => StarkSharpErrorCode.ServerError,
                HttpStatusCode.ServiceUnavailable => StarkSharpErrorCode.ServerError,
                HttpStatusCode.GatewayTimeout => StarkSharpErrorCode.ConnectionTimeout,
                _ => StarkSharpErrorCode.HttpError
            };

            return new StarkSharpException(
                errorCode,
                message ?? $"HTTP {statusCode} error",
                new { StatusCode = statusCode }
            );
        }

        /// <summary>
        /// Safely executes an action and handles exceptions
        /// </summary>
        public static T SafeExecute<T>(Func<T> action, StarkSharpErrorCode defaultErrorCode = StarkSharpErrorCode.UnknownError)
        {
            try
            {
                return action();
            }
            catch (System.Exception ex)
            {
                var handledException = HandleException(ex);
                if (handledException.ErrorCode == StarkSharpErrorCode.UnknownError && defaultErrorCode != StarkSharpErrorCode.UnknownError)
                {
                    handledException = new StarkSharpException(defaultErrorCode, handledException.Message, handledException);
                }
                throw handledException;
            }
        }

        /// <summary>
        /// Safely executes an async action and handles exceptions
        /// </summary>
        public static async Task<T> SafeExecuteAsync<T>(Func<Task<T>> action, StarkSharpErrorCode defaultErrorCode = StarkSharpErrorCode.UnknownError)
        {
            try
            {
                return await action();
            }
            catch (System.Exception ex)
            {
                var handledException = HandleException(ex);
                if (handledException.ErrorCode == StarkSharpErrorCode.UnknownError && defaultErrorCode != StarkSharpErrorCode.UnknownError)
                {
                    handledException = new StarkSharpException(defaultErrorCode, handledException.Message, handledException);
                }
                throw handledException;
            }
        }

        /// <summary>
        /// Safely executes an action and handles exceptions, returns result or error
        /// </summary>
        public static (bool Success, T Result, StarkSharpException Error) TryExecute<T>(Func<T> action)
        {
            try
            {
                var result = action();
                return (true, result, null);
            }
            catch (System.Exception ex)
            {
                return (false, default(T), HandleException(ex));
            }
        }

        /// <summary>
        /// Safely executes an async action and handles exceptions, returns result or error
        /// </summary>
        public static async Task<(bool Success, T Result, StarkSharpException Error)> TryExecuteAsync<T>(Func<Task<T>> action)
        {
            try
            {
                var result = await action();
                return (true, result, null);
            }
            catch (System.Exception ex)
            {
                return (false, default(T), HandleException(ex));
            }
        }
    }

    /// <summary>
    /// Custom exception for JSON RPC errors
    /// </summary>
    public class JsonRpcException : System.Exception
    {
        public JsonRpcException(string message) : base(message) { }
        public JsonRpcException(string message, System.Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Custom exception for cryptographic operations
    /// </summary>
    public class CryptographicException : System.Exception
    {
        public CryptographicException(string message) : base(message) { }
        public CryptographicException(string message, System.Exception innerException) : base(message, innerException) { }
    }
}

