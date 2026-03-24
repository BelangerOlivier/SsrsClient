using RichardSzalay.MockHttp;
using SsrsClient.Auth;
using SsrsClient.Exceptions;
using System.Net;
using System.Text.Json;
using Xunit;
using SsrsClient.Rest;

namespace SsrsClient.Tests
{
    public class RestSsrsClientTests
    {
        private const string BaseUrl = "https://ssrs.example.com/reports/api/v2.0/";

        private static (RestSsrsClient client, MockHttpMessageHandler handler) CreateClient()
        {
            var mockHttp = new MockHttpMessageHandler();
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri(BaseUrl);
            var client = new RestSsrsClient(httpClient, new NtlmAuthProvider());
            return (client, mockHttp);
        }

        private static string ODataJson<T>(IEnumerable<T> items) =>
            JsonSerializer.Serialize(new { value = items });

        [Fact]
        public async Task ListReportsAsync_ReturnsMappedItems()
        {
            var (client, mock) = CreateClient();
            var items = new[]
            {
                new
                {
                    Id = Guid.NewGuid(),
                    Name = "Report1",
                    Path = "/Sales/Report/",
                    Type = "Report",
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                    Hidden = false
                }
            };
            mock.When("*Reports*").Respond("application/json", ODataJson(items));

            var result = await client.ListReportsAsync("/Sales");

            Assert.Single(result);
            Assert.Equal("Report1", result[0].Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("NoLoadingSlash")]
        public async Task ListReportsAsync_InvalidPath_Throws(string path)
        {
            var (client, _) = CreateClient();
            await Assert.ThrowsAsync<ArgumentException>(() =>
                client.ListReportsAsync(path));
        }

        [Fact]
        public async Task GetReportAsync_NotFound_ThrowsSsrsException()
        {
            var (client, mock) = CreateClient();
            mock.When("*Reports*").Respond("application/json", ODataJson(Array.Empty<object>()));

            var ex = await Assert.ThrowsAsync<SsrsException>(() => client.GetReportAsync("/Missing"));
            Assert.Equal(404, ex.StatusCode);
        }

        [Fact]
        public async Task DeleteReportAsync_SendsDeleteRequest()
        {
            var (client, mock) = CreateClient();
            var id = Guid.NewGuid();
            var items = new[]
            {
            new { Id = id, Name = "OldReport", Path = "/Sales/OldReport",
                  Type = "Report", CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, Hidden = false }
        };
            mock.When(HttpMethod.Get, "*Reports*").Respond("application/json", ODataJson(items));
            var deleteReq = mock.When(HttpMethod.Delete, $"*Reports({id})*").Respond(HttpStatusCode.NoContent);

            await client.DeleteReportAsync("/Sales/OldReport");

            Assert.Equal(1, mock.GetMatchCount(deleteReq));
        }

        [Fact]
        public async Task WhenServerReturns500_ThrowsSsrsException()
        {
            var (client, mock) = CreateClient();
            mock.When("*").Respond(HttpStatusCode.InternalServerError, "application/json", "{}");

            var ex = await Assert.ThrowsAsync<SsrsException>(() => client.ListReportsAsync("/Sales"));
            Assert.Equal(500, ex.StatusCode);
        }

        [Fact]
        public void Builder_MissingBaseUrl_Throws() =>
            Assert.Throws<InvalidOperationException>(() =>
                new SsrsClientBuilder().WithNtlm().Build());

        [Fact]
        public void Builder_MissingAuth_Throws() =>
            Assert.Throws<InvalidOperationException>(() =>
                new SsrsClientBuilder().WithBaseUrl("https://example.com/reports/api/v2.0").Build());

        [Fact]
        public void Builder_ValidConfig_ReturnsClient()
        {
            var client = new SsrsClientBuilder()
                .WithBaseUrl("https://example.com/reports/api/v2.0")
                .WithNtlm()
                .Build();
            Assert.NotNull(client);
        }
    }
}
