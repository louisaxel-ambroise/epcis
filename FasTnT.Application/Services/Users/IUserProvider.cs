using FasTnT.Domain.Model;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Application.Services.Users
{
    public interface IUserProvider
    {
        Task<User> GetByUsernameAndPasswordAsync(string username, string password, CancellationToken cancellationToken);
    }
}
