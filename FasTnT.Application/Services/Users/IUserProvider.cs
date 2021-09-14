using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Application.Services.Users
{
    public interface IUserProvider
    {
        Task<Domain.Model.User> GetByUsernameAndPasswordAsync(string username, string password, CancellationToken cancellationToken);
    }
}
