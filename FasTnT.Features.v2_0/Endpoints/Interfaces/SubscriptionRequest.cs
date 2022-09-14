namespace FasTnT.Features.v2_0.Endpoints.Interfaces;

public record SubscriptionRequest(string Destination)
{
    public static ValueTask<SubscriptionRequest> BindAsync(HttpContext context)
    {
        throw new NotImplementedException();
    }
}
