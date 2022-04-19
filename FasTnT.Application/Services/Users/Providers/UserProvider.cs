using FasTnT.Domain.Model;
using FasTnT.Infrastructure.Store;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Services.Users;

public class UserProvider : IUserProvider
{
    private readonly EpcisContext _context;

    public UserProvider(EpcisContext context)
    {
        _context = context;
    }

    public async Task<User> GetByUsernameAndPasswordAsync(string username, string password, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(x => x.DefaultQueryParameters)
            .Where(x => x.Username == username)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if(user != default && !PasswordUtils.GetSecuredKey(password, user.Salt).Equals(user.SecuredKey))
        {
            user = null;
        }

        return user;
    }
}
