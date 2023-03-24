using FasTnT.Application.Domain.Model.Queries;

namespace FasTnT.Host.Features.v1_2.Endpoints.Interfaces;

public record PollEvents(IEnumerable<QueryParameter> Parameters);
public record PollMasterData(IEnumerable<QueryParameter> Parameters);
