using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Services.Users;

public interface ICurrentUser
{
    public string UserName { get; }
    public string UserId { get; }
    public IEnumerable<QueryParameter> DefaultQueryParameters { get; }
}
