namespace FasTnT.Domain.CutomQueries;

public class CustomQueryParameter
{
    public string Name { get; set; }
    public string[] Value { get; set; }
    public CustomQuery Query { get; set; }
}