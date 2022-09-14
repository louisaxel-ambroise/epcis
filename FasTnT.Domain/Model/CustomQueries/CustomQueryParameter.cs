using FasTnT.Domain.Model.Queries;

namespace FasTnT.Domain.Model.CustomQueries;

public class CustomQueryParameter : QueryParameter
{
    public CustomQuery Query { get; set; }
}