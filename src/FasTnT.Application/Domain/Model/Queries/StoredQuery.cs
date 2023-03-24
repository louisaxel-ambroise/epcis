namespace FasTnT.Application.Domain.Model.Queries;

public class StoredQuery
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string UserId { get; set; }
    public List<QueryParameter> Parameters { get; set; } = new();
}
