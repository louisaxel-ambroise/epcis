﻿using FasTnT.Application.Services.Users;
using FasTnT.Domain.Queries;

namespace FasTnT.Application.Tests.Context;

public class TestCurrentUser : ICurrentUser
{
    public int UserId => 0;

    public string Username => "test";

    public List<QueryParameter> DefaultQueryParameters => new();
}