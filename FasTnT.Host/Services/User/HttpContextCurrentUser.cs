using FasTnT.Application.Services.Users;
using FasTnT.Domain.Queries.Poll;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace FasTnT.Host.Services.User
{
    public class HttpContextCurrentUser : ICurrentUser
    {
        public int UserId { get; init; }
        public string Username { get; init; }
        public List<QueryParameter> DefaultQueryParameters { get; init; } = new();

        public HttpContextCurrentUser(IHttpContextAccessor contextAccessor)
        {
            var user = contextAccessor?.HttpContext?.User;

            if(user == default)
            {
                return;
            }

            UserId = int.Parse(user.Claims.SingleOrDefault(x => x.Type == nameof(UserId)).Value);
            Username = user.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Name).Value;
            DefaultQueryParameters = JsonConvert.DeserializeObject<List<QueryParameter>>(user.Claims.SingleOrDefault(x => x.Type == nameof(DefaultQueryParameters)).Value);
        }
    }
}
