using SsrsClient.Auth;
using SsrsClient.Exceptions;
using SsrsClient.Rest.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Linq;

namespace SsrsClient.Rest
{
    /// <summary>
    /// SSRS REST API implementation. Requires SSRS 2017+ or Power BI Report Server.
    /// </summary>
    internal sealed class RestSsrsClient : ISsrsClient, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthProvider _authProvider;

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        internal RestSsrsClient(HttpClient httpClient, IAuthProvider authProvider)
        {
            _httpClient = httpClient;
            _authProvider = authProvider;
        }

        // --- Reports ---

        public async Task<IReadOnlyList<CatalogItem>> ListReportsAsync(
            string folderPath,
            CancellationToken cancellationToken = default
        )
        {
            ValidatePath(folderPath);
            var encoded = Uri.EscapeDataString(folderPath);
            var request = BuildRequest(HttpMethod.Get,
                $"Reports?$filter=Path eq '{encoded}'&$select=Id,Name,Path,Type,CreatedDate,ModifiedDate,Hidden");

            var response = await SendAsync(request, cancellationToken);
            var result = await DeserializeAsync<ODataResponse<CatalogItem>>(response, cancellationToken);

            return result.Value.AsReadOnly();
        }

        public async Task<CatalogItem> GetReportAsync(
            string reportPath,
            CancellationToken cancellationToken = default
        )
        {
            ValidatePath(reportPath);
            var encoded = Uri.EscapeDataString(reportPath);
            var request = BuildRequest(HttpMethod.Get,
                $"Reports?$filter=Path eq '{encoded}'");

            var response = await SendAsync(request, cancellationToken);
            var result = await DeserializeAsync<ODataResponse<CatalogItem>>(response, cancellationToken);

            var item = result.Value.FirstOrDefault();
            return item == null
                ? throw new SsrsException($"Report not found: {reportPath}", 404)
                : item;
        }

        public async Task<byte[]> DownloadReportAsync(
            string reportPath,
            CancellationToken cancellationToken = default
        )
        {
            var item = await GetReportAsync(reportPath, cancellationToken);
            var request = BuildRequest(HttpMethod.Get,
                $"Reports({item.Id})/Content/$value");
            var response = await SendAsync(request, cancellationToken);
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<CatalogItem> UploadReportAsync(
            string folderPath,
            string reportName,
            byte[] rdlContent,
            bool overwrite = false,
            CancellationToken cancellationToken = default
        )
        {
            ValidatePath(folderPath);
            if (string.IsNullOrWhiteSpace(reportName))
                throw new ArgumentException("Report name is required.", nameof(reportName));
            if (rdlContent == null || rdlContent.Length == 0)
                throw new ArgumentException("RDL content cannot be empty.", nameof(rdlContent));

            if (overwrite)
            {
                var fullPath = $"{folderPath.TrimEnd('/')}/{reportName}";
                try
                {
                    await DeleteReportAsync(fullPath, cancellationToken);
                }
                catch (SsrsException ex) when (ex.StatusCode == 404)
                {
                    /* Report doesn't exist */
                }
            }

            var body = new UploadReportRequest(
                name: reportName,
                path: folderPath,
                content: Convert.ToBase64String(rdlContent)
            );

            var request = BuildRequest(HttpMethod.Post, "Reports");
            request.Content = JsonContent.Create(body);

            var response = await SendAsync(request, cancellationToken);
            return await DeserializeAsync<CatalogItem>(response, cancellationToken);
        }

        public async Task DeleteReportAsync(
            string reportPath,
            CancellationToken cancellationToken = default
        )
        {
            var item = await GetReportAsync(reportPath, cancellationToken);
            var request = BuildRequest(HttpMethod.Delete, $"Reports({item.Id})");
            await SendAsync(request, cancellationToken);
        }

        // --- Folders ---

        public async Task<IReadOnlyList<CatalogItem>> ListFoldersAsync(
            string folderPath,
            CancellationToken cancellationToken = default
        )
        {
            ValidatePath(folderPath);
            var encoded = Uri.EscapeDataString(folderPath);
            var request = BuildRequest(HttpMethod.Get,
                $"Folders?$filter=startswith(Path,'{encoded}')&$select=Id,Name,Path,Type,CreatedDate,ModifiedDate");

            var response = await SendAsync(request, cancellationToken);
            var result = await DeserializeAsync<ODataResponse<CatalogItem>>(response, cancellationToken);

            return result.Value.AsReadOnly();
        }

        public async Task<CatalogItem> CreateFolderAsync(
            string folderPath,
            CancellationToken cancellationToken = default
        )
        {
            ValidatePath(folderPath);
            var name = folderPath.Split('/').Last();
            var parent = folderPath.Contains('/')
                ? folderPath.Substring(0, folderPath.LastIndexOf('/'))
                : "/";

            var body = new { Name = name, Path = parent };
            var request = BuildRequest(HttpMethod.Post, "Folders");
            request.Content = JsonContent.Create(body);

            var response = await SendAsync(request, cancellationToken);
            return await DeserializeAsync<CatalogItem>(response, cancellationToken);
        }

        public async Task DeleteFolderAsync(string folderPath, CancellationToken cancellationToken = default)
        {
            ValidatePath(folderPath);
            var encoded = Uri.EscapeDataString(folderPath);
            var findRequest = BuildRequest(HttpMethod.Get,
                $"Folders?$filter=Path eq '{encoded}'");
            var findResponse = await SendAsync(findRequest, cancellationToken);
            var result = await DeserializeAsync<ODataResponse<CatalogItem>>(findResponse, cancellationToken);

            var folder = result.Value.FirstOrDefault();
            if (folder == null)
                throw new SsrsException($"Folder not found: {folderPath}", 404);

            var deleteRequest = BuildRequest(HttpMethod.Delete, $"Folders({folder.Id}");
            await SendAsync(deleteRequest, cancellationToken);
        }

        // --- Helpers ---
        private HttpRequestMessage BuildRequest(
            HttpMethod method,
            string relativeUrl
        )
        {
            var request = new HttpRequestMessage(method, relativeUrl);
            _authProvider.ApplyHeaders(request);
            return request;
        }

        private async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new SsrsException(
                    $"SSRS request failed: {response.StatusCode} {response.ReasonPhrase}",
                    (int)response.StatusCode,
                    body
                );
            }

            return response;
        }

        private static async Task<T> DeserializeAsync<T>(
            HttpResponseMessage response,
            CancellationToken cancellationToken
        )
        {
            var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
            return object.Equals(result, default(T))
                ? throw new SsrsException("Received null response from SSRS.")
                : result;
        }

        private static void ValidatePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            if (!path.StartsWith("/"))
                throw new ArgumentException("Path must start with '/'.", nameof(path));
        }

        public void Dispose() => _httpClient.Dispose();
    }
}
