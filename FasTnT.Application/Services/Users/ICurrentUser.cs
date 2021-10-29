using FasTnT.Domain.Queries.Poll;

namespace FasTnT.Application.Services.Users
{
    public interface ICurrentUser
    {
        public int UserId { get; }
        public string Username { get; }
        public List<QueryParameter> DefaultQueryParameters { get; }
    }
}
