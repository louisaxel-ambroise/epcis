using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Domain.Model.CustomQueries;

public class StoredQuery
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string DataSource { get; set; }
    public List<StoredQueryParameter> Parameters { get; set; } = new();
    public List<Subscription> Subscriptions { get; set; } = new();
}
