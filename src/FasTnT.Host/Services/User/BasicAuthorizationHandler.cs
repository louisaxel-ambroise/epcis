using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using FasTnT.Application.Services.Users;
using System.Text.Json;
using System.Security.Cryptography;

namespace FasTnT.Host.Authorization;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string Authorization = nameof(Authorization);
    private const string Basic = nameof(Basic);

    public static string SchemeName => Basic + Authorization;

    public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        return Task.Run(() =>
        {
            if (!Request.Headers.ContainsKey(Authorization))
            {
                return AuthenticateResult.Fail("Missing Authorization Header");
            }

            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers[Authorization]);

            if (authHeader.Scheme != Basic)
            {
                return AuthenticateResult.Fail($"Invalid Authorization scheme: {authHeader.Scheme}");
            }

            try
            {
                var (username, password) = ParseAuthenticationHeader(authHeader);

                return Authenticated(username, password, new[] { "fastnt.query", "fastnt.capture" });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Invalid header format");

                return AuthenticateResult.Fail(ex.Message);
            }
        });
    }

    private static (string username, string password) ParseAuthenticationHeader(AuthenticationHeaderValue authHeader)
    {
        var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
        var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');

        return credentials.Length == 2
            ? (credentials[0], credentials[1])
            : throw new FormatException("Authorization header must contain 2 values separated by ':'");
    }

    private AuthenticateResult Authenticated(string username, string password, IEnumerable<string> requiredClaims)
    {
        var userHash = Hash(username, password);
        var claims = new List<Claim>()
        {
            new Claim(nameof(ICurrentUser.UserName), username),
            new Claim(nameof(ICurrentUser.UserId), userHash.ToString(), typeof(Guid).Name),
            new Claim(nameof(ICurrentUser.DefaultQueryParameters), JsonSerializer.Serialize(new[]{ new { Name = "EQ_userID", Values = new[] { userHash.ToString() } } }))
        };
        claims.AddRange(requiredClaims.Select(x => new Claim(x, x)));

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        Request.HttpContext.User = principal;

        return AuthenticateResult.Success(ticket);
    }

    private static string Hash(string username, string password)
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(string.Join('#', username, password)));
        
        return string.Concat(hash.Select(x => x.ToString("X2")));
    }
}
