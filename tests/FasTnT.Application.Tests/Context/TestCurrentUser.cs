﻿using FasTnT.Application.Services.Users;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.Context;

public class TestCurrentUser : ICurrentUser
{
    public string UserName => "test_user";
    public string UserId => "test";
    public IEnumerable<QueryParameter> DefaultQueryParameters => Enumerable.Empty<QueryParameter>();
}
