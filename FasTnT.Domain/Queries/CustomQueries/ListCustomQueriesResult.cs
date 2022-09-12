﻿using FasTnT.Domain.Model.CustomQueries;
using FasTnT.Domain.Queries.Poll;

namespace FasTnT.Domain.Queries.CustomQueries;

public record ListCustomQueriesResult(IEnumerable<CustomQuery> Queries) : IEpcisResponse;
