using FasTnT.Domain.Queries.GetSubscriptionIds;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Application.Queries.GetVendorVersion
{
    public class GetSubscriptionIdsQueryHandler : IRequestHandler<GetSubscriptionIdsQuery, GetSubscriptionIdsResult>
    {
        public Task<GetSubscriptionIdsResult> Handle(GetSubscriptionIdsQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult<GetSubscriptionIdsResult>(new(new[] { "sub1" }));
        }
    }
}