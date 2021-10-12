using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Subscriptions
{
    public interface ISubscriptionResultSender
    {
        Task<bool> Send<T>(string destination, T epcisResponse, CancellationToken cancellationToken);
    }
}
