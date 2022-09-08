using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Queries;
using System.Text.Json;

namespace FasTnT.Formatter.v2_0.Json.Formatters;

public static class JsonResponseFormatter
{
    public static string Format(IEpcisResponse response)
    {
        return response switch
        {
            PollResponse poll => FormatPoll(poll),
            _ => FormatError(EpcisException.Default)
        };
    }

    public static string FormatError(EpcisException error)
    {
        return JsonSerializer.Serialize(new
        {
            Type = $"epcisException:{error.ExceptionType}",
            Title = error.Message,
            Status = 400
        });
    }

    private static string FormatPoll(PollResponse response)
    {
        return JsonSerializer.Serialize(response.EventList.Select(JsonEventFormatter.FormatEvent));
    }
}
