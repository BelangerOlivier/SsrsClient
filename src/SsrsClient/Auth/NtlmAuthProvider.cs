using System.Net;
using System.Net.Http;

namespace SsrsClient.Auth
{
    /// <summary>Authenticates using Windows/NTLM credentials</summary>
    public sealed class NtlmAuthProvider : IAuthProvider
    {
        private readonly ICredentials _credentials;

        /// <summary>Uses the current Windows identity (the process user account).</summary>
        public NtlmAuthProvider()
        {
            _credentials = CredentialCache.DefaultNetworkCredentials;
        }

        /// <summary>Uses explicit Windows credentials.</summary>
        public NtlmAuthProvider(string username, string password, string domain = null)
        {
            _credentials = new NetworkCredential(username, password, domain);
        }

        /// <inheritdoc />
        public void Configure(HttpClientHandler handler)
        {
            handler.UseDefaultCredentials =
                _credentials == CredentialCache.DefaultNetworkCredentials;

            if (_credentials is NetworkCredential nc)
                handler.Credentials = nc;
        }

        /// <inheritdoc />
        public void ApplyHeaders(HttpRequestMessage request)
        {

        }
    }
}
