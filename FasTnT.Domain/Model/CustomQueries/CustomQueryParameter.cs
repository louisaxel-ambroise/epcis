using FasTnT.Domain.Queries.Poll;

namespace FasTnT.Domain.Model.CustomQueries;

public class CustomQueryParameter : QueryParameter
{
    public CustomQuery Query { get; set; }
}