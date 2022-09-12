using FasTnT.Domain.CutomQueries;
using FasTnT.Domain.Queries.Poll;

namespace FasTnT.Domain.Queries.CustomQueries;

public record CustomQueryDefinitionResult(CustomQuery Query) : IEpcisResponse;
