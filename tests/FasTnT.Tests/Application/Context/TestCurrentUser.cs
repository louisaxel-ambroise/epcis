namespace FasTnT.Tests.Application.Context;

public class TestCurrentUser : ICurrentUser
{
    public string UserName => "test_user";
    public string UserId => "test";
    public IEnumerable<QueryParameter> DefaultQueryParameters => Enumerable.Empty<QueryParameter>();
}
