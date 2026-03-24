using System;

namespace SsrsClient.Exceptions
{
    /// <summary>Thrown when an SSRS API call fails.</summary>
    public sealed class SsrsException : Exception
    {
        /// <summary>The HTTP status code returned by the server, if applicable.</summary>
        public int? StatusCode { get; }

        /// <summary>The raw error body returned by the server, if available.</summary>
        public string ResponseBody { get; }

        /// <summary>
        /// Initializes a new instance of the SsrsException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SsrsException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the SsrsException class with a specified error message and a reference to the
        /// inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null reference if no inner exception is
        /// specified.</param>
        public SsrsException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new instance of the SsrsException class with a specified error message, HTTP status code, and
        /// optional response body.
        /// </summary>
        /// <remarks>Use this constructor to capture additional context from an SSRS service error,
        /// including the HTTP status code and any response content, for improved error handling and
        /// diagnostics.</remarks>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="statusCode">The HTTP status code returned by the SSRS service that caused the exception.</param>
        /// <param name="responseBody">The response body returned by the SSRS service, or null if not available.</param>
        public SsrsException(string message, int statusCode, string responseBody = null) : base(message)
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
        }
    }
}
