using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SsrsClient.Rest.Models
{
    /// <summary>OData-style wrapper returned by SSRS REST list endpoints.</summary>
    internal sealed class ODataResponse<T>
    {
        [JsonPropertyName("value")]
        public List<T> Value { get; private set; } = new List<T>();
    }
}
