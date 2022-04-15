using FasTnT.Application.Services.Users;
using FasTnT.Domain.Queries;
using Newtonsoft.Json;
using System.Security.Claims;

namespace FasTnT.Host.Services.User;

public class HttpContextCurrentUser : ICurrentUser
{
    public int UserId { get; init; }
    public string Username { get; init; }
    public bool CanQuery { get; init; }
    public bool CanCapture { get; init; }
    public List<QueryParameter> DefaultQueryParameters { get; init; } = new();

    public HttpContextCurrentUser(IHttpContextAccessor contextAccessor)
    {
        var user = contextAccessor?.HttpContext?.User;

        if(user == default)
        {
            return;
        }

        UserId = int.Parse(user.Claims.SingleOrDefault(x => x.Type == nameof(UserId)).Value);
        Username = user.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Name).Value;
        CanQuery = bool.TryParse(user.Claims.SingleOrDefault(x => x.Type == nameof(CanQuery)).Value, out bool canQuery) && canQuery;
        CanCapture = bool.TryParse(user.Claims.SingleOrDefault(x => x.Type == nameof(CanCapture)).Value, out bool canCapture) && canCapture;
        DefaultQueryParameters = JsonConvert.DeserializeObject<List<QueryParameter>>(user.Claims.SingleOrDefault(x => x.Type == nameof(DefaultQueryParameters)).Value);
    }
}
