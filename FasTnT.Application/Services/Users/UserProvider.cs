using FasTnT.Domain.Model;
using FasTnT.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace FasTnT.Application.Services.Users;
 
public class UserProvider : IUserProvider
{
    private readonly EpcisContext _context;
    private readonly IHostEnvironment _environment;

    public UserProvider(EpcisContext context, IHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<User> GetByUsernameAndPasswordAsync(string username, string password, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(x => x.DefaultQueryParameters)
            .Where(x => x.Username == username)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if(user == default && _environment.IsDevelopment())
        {
            user = await CreateUser(username, password, cancellationToken).ConfigureAwait(false);
        }
        else if(user != default && !PasswordUtils.GetSecuredKey(password, user.Salt).Equals(user.SecuredKey))
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
