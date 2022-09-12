namespace FasTnT.Domain.Model.CustomQueries;

public class CustomQuery
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<CustomQueryParameter> Parameters { get; set; }
}
