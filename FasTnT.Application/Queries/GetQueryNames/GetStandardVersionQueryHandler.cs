﻿using FasTnT.Application.Services;
using FasTnT.Domain.Queries;
using MediatR;

namespace FasTnT.Application.Queries;

public class GetQueryNamesQueryHandler : IRequestHandler<GetQueryNamesQuery, GetQueryNamesResult>
{
    private readonly IEnumerable<IEpcisQuery> _queries;

    public GetQueryNamesQueryHandler(IEnumerable<IEpcisQuery> queries)
    {
        _queries = queries;
    }

    public Task<GetQueryNamesResult> Handle(GetQueryNamesQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult<GetQueryNamesResult>(new(_queries.Select(x => x.Name)));
    }
}
