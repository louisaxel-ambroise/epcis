using FasTnT.Application.Domain.Model.Events;

namespace FasTnT.Application.Services.Events;

public static class EventHash
{
    public static string Compute(Event _)
    {
        return $"urn:uuid:{Guid.NewGuid()}";
    }
}
