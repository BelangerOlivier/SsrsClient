using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SsrsClient.Rest.Models
{
    /// <summary>OData-style wrapper returned by SSRS REST list endpoints.</summary>
    internal sealed class ODataResponse<T>
    {
        [JsonInclude]
        [JsonPropertyName("@odata.context")]
        public string ODataContext { get; private set; } = string.Empty;

        [JsonInclude]
        [JsonPropertyName("value")]
        public List<T> Value { get; private set; } = new List<T>();
    }
}
