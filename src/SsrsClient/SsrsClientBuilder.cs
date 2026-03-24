using SsrsClient.Auth;
using SsrsClient.Rest;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SsrsClient
{
    public sealed class SsrsClientBuilder
    {
        private string _baseUrl;
        private IAuthProvider _authProvider;
        private bool _ignoreSslErrors;
        private TimeSpan _timeout = TimeSpan.FromSeconds(30);

        /// <summary>The REST base URL is typically: https://yourserver/reports/api/v2.0</summary>
        public SsrsClientBuilder WithBaseUrl(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("Base URL is required.", nameof(baseUrl));

            _baseUrl = baseUrl.TrimEnd('/') + "/";
            return this;
        }

        /// <summary>Authenticates using the current Windows identity.</summary>
        public SsrsClientBuilder WithNtlm()
        {
            _authProvider = new NtlmAuthProvider();
            return this;
        }

        /// <summary>Authenticates using explicit Windows credentials.</summary>
        public SsrsClientBuilder WithNtlm(string username, string password, string domain = "")
        {
            _authProvider = new NtlmAuthProvider(username, password, domain);
            return this;
        }

        /// <summary>
        /// Authenticates using Basic auth.
        /// <br/>
        /// ⚠️ WARNING: Only use over HTTPS. Load credentials from env vars or a secrets manager.
        /// </summary>
        public SsrsClientBuilder WithBasic(string username, string password)
        {
            _authProvider = new BasicAuthProvider(username, password);
            return this;
        }

        /// <summary>Plugs in a custom auth provider (e.g. for OAuth or Kerberos).</summary>
        public SsrsClientBuilder WithCustomAuth(IAuthProvider authProvider)
        {
            if (authProvider == null)
                throw new ArgumentNullException(nameof(authProvider));

            _authProvider = authProvider;
            return this;
        }

        /// <summary>Sets the HTTP request timeout. Defaults to 30 seconds.</summary>
        public SsrsClientBuilder WithTimeout(TimeSpan timeout)
        {
            _timeout = timeout;
            return this;
        }

        /// <summary>
        /// Disables SSL certificate validation.
        /// <br/>
        /// ⚠️ WARNING: Development/testing only - never use in production.
        /// </summary>
        public SsrsClientBuilder IgnoreSslErrors()
        {
            _ignoreSslErrors = true;
            return this;
        }

        /// <summary>Builds and returns an ISsrsClient targeting the SSRS REST API.</summary>
        public ISsrsClient Build()
        {
            if (string.IsNullOrWhiteSpace(_baseUrl))
                throw new InvalidOperationException("A base URL is required. Call WithBaseUrl() before Build().");

            if (_authProvider == null)
                throw new InvalidOperationException("An auth provider is requried. Call WithNtlm(), WithBasic() or WithCustomAuth() before Build().");

            var handler = new HttpClientHandler();
            _authProvider.Configure(handler);

            if (_ignoreSslErrors)
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(_baseUrl),
                Timeout = _timeout
            };

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return new RestSsrsClient(httpClient, _authProvider);
        }
    }
}
