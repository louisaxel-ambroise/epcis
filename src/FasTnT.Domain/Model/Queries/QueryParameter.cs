namespace FasTnT.Domain.Model.Queries;

public class QueryParameter
{
    public string Name { get; set; }
    public string[] Values { get; set; }

    public static QueryParameter Create(string name, params string[] values) => new() { Name = name, Values = values };
}
