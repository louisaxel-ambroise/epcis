using System.Text.Json.Serialization;

namespace FasTnT.IntegrationTests.Interfaces;

public class CollectionResult
{
    [JsonPropertyName("@context")]
    public string Context { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("member")]
    public string[] Members { get; set; }
}