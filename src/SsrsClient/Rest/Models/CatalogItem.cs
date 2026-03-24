using System;
using System.Text.Json.Serialization;

namespace SsrsClient.Rest.Models
{
    /// <summary>Represents an item in the SSRS report catalog.</summary>
    public sealed class CatalogItem
    {
        [JsonPropertyName("Id")]
        public Guid Id { get; private set; }

        [JsonPropertyName("Name")]
        public string Name { get; private set; } = string.Empty;

        [JsonPropertyName("Path")]
        public string Path { get; private set; } = string.Empty;

        [JsonPropertyName("Type")]
        public string Type { get; private set; } = string.Empty;

        [JsonPropertyName("CreatedDate")]
        public DateTime CreatedDate { get; private set; }

        [JsonPropertyName("ModifiedDate")]
        public DateTime ModifiedDate { get; private set; }

        [JsonPropertyName("Description")]
        public string Description { get; private set; }

        [JsonPropertyName("Hidden")]
        public bool Hidden { get; private set; } = false;
    }
}
