using System.Net.Http;

namespace SsrsClient.Auth
{
    /// <summary>
    /// Abstraction for supplying authentication to SSRS HTTP requests.
    /// Implement this to support custom schemes (e.g. OAuth, Kerberos).
    /// </summary>
    public interface IAuthProvider
    {
        /// <summary>Configures the HttpClientHandler with credentials.</summary>
        void Configure(HttpClientHandler handler);

        /// <summary>Optionally applies per-request headers (e.g. Authorization: Bearer).</summary>
        void ApplyHeaders(HttpRequestMessage request);
    }
}
