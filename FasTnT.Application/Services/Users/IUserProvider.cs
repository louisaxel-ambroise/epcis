using FasTnT.Domain.Model;

namespace FasTnT.Application.Services.Users
{
    public interface IUserProvider
    {
        Task<User> GetByUsernameAndPasswordAsync(string username, string password, CancellationToken cancellationToken);
    }
}
