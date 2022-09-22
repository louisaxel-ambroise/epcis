using FasTnT.Application.EfCore.Services.Users;
using FasTnT.Application.EfCore.Store;
using FasTnT.Application.Services.Users;
using Microsoft.Extensions.DependencyInjection;

namespace FasTnT.Application.EfCore;

public class EpcisOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public int CommandTimeout { get; set; } = 60;
    public Func<IServiceProvider, ICurrentUser> CurrentUser { get; set; } = _ => null;
    public Func<IServiceProvider, IUserProvider> UserProvider { get; set; } = svc => new UserProvider(svc.GetService<EpcisContext>());
}