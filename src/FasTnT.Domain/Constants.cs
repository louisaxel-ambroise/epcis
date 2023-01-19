namespace FasTnT.Domain;

public sealed class Constants
{
    public static Constants Instance { get; set; } = new();

    public int MaxEventsCapturePerCall { get; init; } = 500;
    public int MaxEventsReturnedInQuery { get; init; } = 20_000;
    public string VendorVersion { get; init; } = "2.3.1";
}
