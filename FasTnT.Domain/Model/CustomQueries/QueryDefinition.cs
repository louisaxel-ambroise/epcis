namespace FasTnT.Domain.CutomQueries;

public class CustomQuery
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<CustomQueryParameter> Parameters { get; set; }
}
