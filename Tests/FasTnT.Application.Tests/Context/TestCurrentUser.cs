using FasTnT.Application.Services.Users;
using FasTnT.Domain.Queries.Poll;

namespace FasTnT.Application.Tests.Context;

public class TestCurrentUser : ICurrentUser
{
    public int UserId => 0;
    public string Username => "test";
    public bool CanQuery => true;
    public bool CanCapture => true;
    public List<QueryParameter> DefaultQueryParameters => new();
}
