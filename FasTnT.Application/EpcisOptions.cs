using FasTnT.Application.Services.Users;
using FasTnT.Application.Services.Users.Providers;
using FasTnT.Application.Store;
using Microsoft.Extensions.DependencyInjection;

namespace FasTnT.Application;

public class EpcisOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public int CommandTimeout { get; set; } = 60;
    public Func<IServiceProvider, ICurrentUser> CurrentUser { get; set; } = _ => null;
    public Func<IServiceProvider, IUserProvider> UserProvider { get; set; } = svc => new UserProvider(svc.GetService<EpcisContext>());
}