using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Newtonsoft.Json;
using FasTnT.Application.Services.Users;
using FasTnT.Domain.Queries;

namespace FasTnT.Host.Authorization;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string HeaderKey = "Authorization";
    private const string AuthorizationScheme = "Basic";

    public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey(HeaderKey))
        {
            Logger.LogError("Missing {headerKey} Header", HeaderKey);

            return AuthenticateResult.Fail($"Missing {HeaderKey} Header");
        }
            
        var authHeader = AuthenticationHeaderValue.Parse(Request.Headers[HeaderKey]);

        if(authHeader.Scheme != AuthorizationScheme)
        {
            Logger.LogError("Invalid Authorization scheme: {scheme}", authHeader.Scheme);

            return AuthenticateResult.Fail($"Invalid Authorization scheme {authHeader.Scheme}");
        }

        try
        {
            var (username, password) = ParseAuthenticationHeader(authHeader);

            Logger.LogInformation("Retrieve user information for user {username}", username);

            return await AuthenticateUser(username, password, Request.HttpContext.RequestAborted).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Invalid header format");

            return AuthenticateResult.Fail(ex.Message);
        }
    }

    private static (string username, string password) ParseAuthenticationHeader(AuthenticationHeaderValue authHeader)
    {
        var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
        var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');

        return credentials.Length == 2
            ? (credentials[0], credentials[1])
            : throw new FormatException($"{HeaderKey} header must contain 2 values separated by ':'");
    }

    private async Task<AuthenticateResult> AuthenticateUser(string username, string password, CancellationToken cancellationToken)
    {
        var userProvider = Request.HttpContext.RequestServices.GetService<IUserProvider>();
        var user = await userProvider
            .GetByUsernameAndPasswordAsync(username, password, cancellationToken)
            .ConfigureAwait(false);

        if (user != null)
        {
            Logger.LogInformation("Authentication succeeded for user {username}", username);

            return Authenticated(username, user);
        }
        else
        {
            Logger.LogWarning("Invalid credentials");

            return AuthenticateResult.Fail($"Invalid credentials.");
        }
    }

    private AuthenticateResult Authenticated(string username, Domain.Model.User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, username),
            new Claim(ClaimTypes.Name, username),
            new Claim("UserId", user.Id.ToString()),
            new Claim("CanQuery", user.CanQuery.ToString()),
            new Claim("CanCapture", user.CanCapture.ToString()),
            new Claim("DefaultQueryParameters", JsonConvert.SerializeObject(user.DefaultQueryParameters.Select(p => new QueryParameter(p.Name, p.Values))))
        };

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        Request.HttpContext.User = principal;

        return AuthenticateResult.Success(ticket);
    }
}
