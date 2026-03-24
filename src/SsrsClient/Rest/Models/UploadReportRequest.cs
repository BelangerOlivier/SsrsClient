using System.Text.Json.Serialization;

namespace SsrsClient.Rest.Models
{
    internal sealed class UploadReportRequest
    {
        public UploadReportRequest(string name, string path, string content)
        {
            Name = name;
            Path = path;
            Content = content;
        }

        [JsonPropertyName("@odata.type")]
        public string ODataType { get; private set; } = "#Model.Report";

        [JsonPropertyName("Name")]
        public string Name { get; private set; } = string.Empty;

        [JsonPropertyName("Path")]
        public string Path { get; private set; } = string.Empty;

        /// <summary>Base64-encoded RDL content.</summary>
        public string Content { get; private set; } = string.Empty;

        [JsonPropertyName("Hidden")]
        public bool Hidden { get; private set; } = false;
    }
}
