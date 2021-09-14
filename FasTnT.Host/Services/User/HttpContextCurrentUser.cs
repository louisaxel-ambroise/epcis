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
        public int Id { get; init; }
        public string Username { get; init; }
        public List<QueryParameter> DefaultQueryParameters { get; init; }

        public HttpContextCurrentUser(IHttpContextAccessor contextAccessor)
        {
            var user = contextAccessor.HttpContext.User;

            Id = int.Parse(user.Claims.SingleOrDefault(x => x.Type == "UserId").Value);
            Username = user.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Name).Value;
            DefaultQueryParameters = JsonConvert.DeserializeObject<List<QueryParameter>>(user.Claims.SingleOrDefault(x => x.Type == "DefaultQueryParameters").Value);
        }
    }
}
