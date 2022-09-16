using FasTnT.Application.Services.Queries;
using FasTnT.Domain.Model.CustomQueries;
using System.Text.Json;

namespace FasTnT.Features.v2_0.Communication.Json.Parsers
{
    public static class JsonRequestParser
    {
        public static async Task<StoredQuery> ParseCustomQueryRequestAsync(Stream body, CancellationToken cancellationToken)
        {
            var document = await JsonDocument.ParseAsync(body, default, cancellationToken);

            var name = document.RootElement.GetProperty("name").GetString();
            var parameters = ParseQueryParameters(document.RootElement.GetProperty("query"));

            return new ()
            {
                Name = name,
                DataSource = nameof(SimpleEventQuery),
                Parameters = parameters
            };
        }

        private static IEnumerable<StoredQueryParameter> ParseQueryParameters(JsonElement jsonElement)
        {
            var parameters = new List<StoredQueryParameter>();

            if(jsonElement.ValueKind == JsonValueKind.Object)
            {
                foreach(var property in jsonElement.EnumerateObject())
                {
                    var values = property.Value.EnumerateArray().Select(x => x.GetString()).ToArray();

                    parameters.Add(new() { Name = property.Name, Values = values });
                }
            }

            return parameters;
        }
    }
}
