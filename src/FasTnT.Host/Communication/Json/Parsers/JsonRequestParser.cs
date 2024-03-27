using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Host.Subscriptions.Formatters;
using System.Text.Json;

namespace FasTnT.Host.Communication.Json.Parsers
{
    public static class JsonRequestParser
    {
        public static async Task<StoredQuery> ParseCustomQueryRequestAsync(Stream body, CancellationToken cancellationToken)
        {
            var document = await JsonDocument.ParseAsync(body, default, cancellationToken);

            var name = document.RootElement.GetProperty("name").GetString();
            var parameters = ParseQueryParameters(document.RootElement.GetProperty("query"));

            return new()
            {
                Name = name,
                Parameters = parameters
            };
        }

        public static async Task<Subscription> ParseSubscriptionRequestAsync(Stream body, CancellationToken cancellationToken)
        {
            var document = await JsonDocument.ParseAsync(body, default, cancellationToken);

            return new()
            {
                Destination = document.RootElement.GetProperty("dest").GetString(),
                FormatterName = nameof(JsonSubscriptionFormatter),
                ReportIfEmpty = document.RootElement.GetProperty("reportIfEmpty").GetBoolean(),
                SignatureToken = document.RootElement.TryGetProperty("signatureToken", out var token) ? token.GetString() : default,
                Schedule = ParseSubscriptionSchedule(document.RootElement.TryGetProperty("schedule", out var schedule) ? schedule : default),
                Trigger = document.RootElement.TryGetProperty("stream", out var stream) && stream.GetBoolean() ? "stream" : default
            };
        }

        private static SubscriptionSchedule ParseSubscriptionSchedule(JsonElement jsonElement)
        {
            return jsonElement.ValueKind == JsonValueKind.Undefined
                ? null
                : new()
                {
                    Second = jsonElement.TryGetProperty("second", out var second) ? second.GetString() : string.Empty,
                    Minute = jsonElement.TryGetProperty("minute", out var minute) ? minute.GetString() : string.Empty,
                    Hour = jsonElement.TryGetProperty("hour", out var hour) ? hour.GetString() : string.Empty,
                    Month = jsonElement.TryGetProperty("month", out var month) ? month.GetString() : string.Empty,
                    DayOfMonth = jsonElement.TryGetProperty("dayOfMonth", out var dom) ? dom.GetString() : string.Empty,
                    DayOfWeek = jsonElement.TryGetProperty("dayOfWeek", out var dow) ? dow.GetString() : string.Empty,
                };
        }

        private static List<QueryParameter> ParseQueryParameters(JsonElement jsonElement)
        {
            var parameters = new List<QueryParameter>();

            if (jsonElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in jsonElement.EnumerateObject())
                {
                    var values = property.Value.EnumerateArray().Select(x => x.GetString()).ToArray();

                    parameters.Add(new() { Name = property.Name, Values = values });
                }
            }

            return parameters;
        }
    }
}
