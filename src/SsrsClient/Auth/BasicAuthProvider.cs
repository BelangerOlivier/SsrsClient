using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SsrsClient.Auth
{
    /// <summary>
    /// Authenticates using HTTP Basic Authentication.
    /// <br/>⚠️ WARNING: Only use over HTTPS. Never hardcode credentials.
    /// </summary>
    public sealed class BasicAuthProvider : IAuthProvider
    {
        private readonly string _encodedCredentials;

        /// <summary>
        /// Initializes a new instance of the BasicAuthProvider class using the specified username and password for HTTP
        /// Basic Authentication.
        /// </summary>
        /// <param name="username">The username to use for authentication. Cannot be null, empty, or consist only of white-space characters.</param>
        /// <param name="password">The password to use for authentication. Cannot be null, empty, or consist only of white-space characters.</param>
        /// <exception cref="ArgumentException">Thrown if username or password is null, empty, or consists only of white-space characters.</exception>
        public BasicAuthProvider(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required", nameof(username));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required", nameof(password));

            _encodedCredentials = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{username}:{password}")
            );
        }

        /// <inheritdoc />
        public void Configure(HttpClientHandler handler)
        {
            // Applied per-request via headers.
        }

        /// <inheritdoc />
        public void ApplyHeaders(HttpRequestMessage request)
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Basic", _encodedCredentials);
        }
    }
}
