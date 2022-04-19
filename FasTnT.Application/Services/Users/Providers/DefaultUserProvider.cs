using FasTnT.Domain.Model;
using FasTnT.Infrastructure.Store;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Services.Users;

public class DefaultUserProvider : IUserProvider
{
    private readonly EpcisContext _context;

    public DefaultUserProvider(EpcisContext context)
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

        if(user == default)
        {
            user = await CreateUser(username, password, cancellationToken).ConfigureAwait(false);
        }
        else if(!PasswordUtils.GetSecuredKey(password, user.Salt).Equals(user.SecuredKey))
        {
            user = null;
        }

        return user;
    }

    private async Task<User> CreateUser(string username, string password, CancellationToken cancellationToken)
    {
        var salt = PasswordUtils.GetSalt();
        var user = new User
        {
            Username = username,
            Salt = salt,
            SecuredKey = PasswordUtils.GetSecuredKey(password, salt),
            CanCapture = true,
            CanQuery = true,
            DefaultQueryParameters = new[] { new UserDefaultQueryParameter { Name = "EQ_username", Values = new[] { username } } }.ToList()
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return user;
    }
}
