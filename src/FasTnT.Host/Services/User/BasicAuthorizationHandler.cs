using FasTnT.Application.Services.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace FasTnT.Host.Services.User;

public class BasicAuthentication(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    private const string Authorization = nameof(Authorization);
    private const string Basic = nameof(Basic);

    public static string SchemeName => Basic + Authorization;

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        return Context.GetEndpoint()?.Metadata?.GetMetadata<AuthorizeAttribute>() is null
            ? Task.Run(AuthenticateResult.NoResult)
            : Task.Run(ProcessAuthenticationAsync);
    }

    private AuthenticateResult ProcessAuthenticationAsync()
    {
        try
        {
            var authValue = RetrieveAuthorizationValue(Request);

            if (string.IsNullOrEmpty(authValue))
            {
                return AuthenticateResult.Fail("Missing Authorization Header");
            }
            else
            {
                var (username, password) = ParseAuthenticationHeader(authValue);

                return Authenticated(username, password, ["fastnt.query", "fastnt.capture"]);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Invalid authorization information");

            return AuthenticateResult.Fail(ex.Message);
        }
    }

    private static string RetrieveAuthorizationValue(HttpRequest request)
    {
        if (request.Headers.TryGetValue(Authorization, out var headerValue))
        {
            var authHeader = AuthenticationHeaderValue.Parse(headerValue);

            if (authHeader.Scheme != Basic)
            {
                throw new Exception($"Invalid Authorization scheme: {authHeader.Scheme}");
            }

            return authHeader.Parameter;
        }
        else if (request.Query.TryGetValue("auth", out var queryValue))
        {
            return queryValue;
        }

        return null;
    }

    private static (string username, string password) ParseAuthenticationHeader(string authHeader)
    {
        var credentialBytes = Convert.FromBase64String(authHeader);
        var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');

        return credentials.Length == 2
            ? (credentials[0], credentials[1])
            : throw new FormatException("Authorization header must contain 2 values separated by ':'");
    }

    private AuthenticateResult Authenticated(string username, string password, IEnumerable<string> requiredClaims)
    {
        var userHash = Hash(username, password);
        var tenantParameter = new { Name = "EQ_userID", Values = new[] { userHash } };
        var claims = new List<Claim>
        {
            new(nameof(ICurrentUser.UserName), username),
            new(nameof(ICurrentUser.UserId), userHash),
            new(nameof(ICurrentUser.DefaultQueryParameters), JsonSerializer.Serialize(new[]{ tenantParameter }))
        };
        claims.AddRange(requiredClaims.Select(x => new Claim(x, x)));

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Basic));

        Request.HttpContext.User = principal;

        return AuthenticateResult.Success(new AuthenticationTicket(principal, Basic));
    }

    private static string Hash(string username, string password)
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(string.Join('#', username, password)));

        return string.Concat(hash.Select(x => x.ToString("X2")));
    }
}
