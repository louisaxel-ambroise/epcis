using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Host.Subscriptions.Formatters;

public interface ISubscriptionFormatter
{
    public string ContentType { get; }

    string FormatResult(string name, QueryResponse response);
    string FormatError(string name, string query, EpcisException error);
}
