using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace FasTnT.Host.Authorization
{
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
                return AuthenticateResult.Fail($"Missing {HeaderKey} Header");
            }
            
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers[HeaderKey]);

            if(authHeader.Scheme != AuthorizationScheme)
            {
                return AuthenticateResult.Fail("Invalid Authorization scheme");
            }

            try
            {
                var (username, password) = ParseAuthenticationHeader(authHeader);

                return await AuthenticateUser(username, password);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex.Message);
            }
        }

        private (string username, string password) ParseAuthenticationHeader(AuthenticationHeaderValue authHeader)
        {
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');

            return credentials.Length == 2
                ? (credentials[0], credentials[1])
                : throw new FormatException($"{HeaderKey} header must contain 2 values separated by ':'");
        }

        private async Task<AuthenticateResult> AuthenticateUser(string username, string password)
        {
            //var mediator = Request.HttpContext.RequestServices.GetService<IMediator>();
            //var response = await mediator.Send(new UserLogInRequest { Username = username, Password = password });
            var response = new { Authorized = true, User = new ClaimsPrincipal()};

            if (response.Authorized)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, username),
                    new Claim(ClaimTypes.Name, username),
                };

                var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                Request.HttpContext.User = response.User;
                
                return AuthenticateResult.Success(ticket);
            }
            else
            {
                return AuthenticateResult.Fail($"Invalid {HeaderKey} Header");
            }
        }
    }
}
