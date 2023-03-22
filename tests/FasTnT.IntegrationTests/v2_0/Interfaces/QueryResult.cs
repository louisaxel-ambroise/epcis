using System.Text.Json.Serialization;

namespace FasTnT.IntegrationTests.v2_0.Interfaces
{
    public class QueryResult
    {
        [JsonPropertyName("@context")]
        public object[] Context { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("schemaVersion")]
        public string SchemaVersion { get; set; }
        [JsonPropertyName("creationDate")]
        public DateTime CreationDate { get; set; }
        [JsonPropertyName("epcisBody")]
        public ResultBody EpcisBody { get; set; }

        public class ResultBody
        {
            [JsonPropertyName("queryResults")]
            public QueryResults QueryResults { get; set; }
        }

        public class QueryResults
        {
            [JsonPropertyName("queryName")]
            public string QueryName { get; set; }
            [JsonPropertyName("subscriptionID")]
            public string SubscriptionId { get; set; }
            [JsonPropertyName("resultsBody")]
            public QueryResultBody ResultBody { get; set; }
        }

        public class QueryResultBody
        {
            [JsonPropertyName("eventList")]
            public List<object> EventList { get; set; }
        }
    }
}
