using FasTnT.Domain.Model.Queries;

namespace FasTnT.Domain.Model.CustomQueries;

public class StoredQueryParameter : QueryParameter
{
    public StoredQuery Query { get; set; }
}