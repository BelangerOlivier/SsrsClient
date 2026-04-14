using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SsrsClient.Rest.Models
{
    /// <summary>Represents an item in the SSRS report catalog.</summary>
    public sealed class CatalogItem
    {
        [JsonInclude]
        [JsonPropertyName("Id")]
        public Guid Id { get; private set; }

        [JsonInclude]
        [JsonPropertyName("Name")]
        public string Name { get; private set; } = string.Empty;

        [JsonInclude]
        [JsonPropertyName("Description")]
        public string Description { get; private set; } = null;

        [JsonInclude]
        [JsonPropertyName("Path")]
        public string Path { get; private set; } = string.Empty;

        [JsonInclude]
        [JsonPropertyName("Type")]
        public string Type { get; private set; } = string.Empty;

        [JsonInclude]
        [JsonPropertyName("Hidden")]
        public bool Hidden { get; private set; } = false;

        [JsonInclude]
        [JsonPropertyName("Size")]
        public int Size { get; private set; } = 0;

        [JsonInclude]
        [JsonPropertyName("ModifiedBy")]
        public string ModifiedBy { get; private set; } = string.Empty;

        [JsonInclude]
        [JsonPropertyName("ModifiedDate")]
        public DateTime ModifiedDate { get; private set; }

        [JsonInclude]
        [JsonPropertyName("CreatedBy")]
        public string CreatedBy { get; private set; } = string.Empty;

        [JsonInclude]
        [JsonPropertyName("CreatedDate")]
        public DateTime CreatedDate { get; private set; }

        [JsonInclude]
        [JsonPropertyName("ParentFolderId")]
        public Guid? ParentFolderId { get; private set; } = null;

        [JsonInclude]
        [JsonPropertyName("ContentType")]
        public string ContentType { get; private set; } = null;

        [JsonInclude]
        [JsonPropertyName("Content")]
        public string Content { get; private set; } = string.Empty;

        [JsonInclude]
        [JsonPropertyName("IsFavorite")]
        public bool IsFavorite { get; private set; } = false;

        [JsonInclude]
        [JsonPropertyName("Roles")]
        public List<object> Roles { get; private set; } = new List<object>();

        [JsonInclude]
        [JsonPropertyName("HasDataSources")]
        public bool HasDataSources { get; private set; } = false;

        [JsonInclude]
        [JsonPropertyName("HasSharedDataSets")]
        public bool HasSharedDataSets { get; private set; } = false;

        [JsonInclude]
        [JsonPropertyName("HasParameters")]
        public bool HasParameters { get; private set; } = false;
    }
}
