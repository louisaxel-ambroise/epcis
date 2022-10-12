namespace FasTnT.Domain;

public sealed class Constants
{
    public static Constants Instance { get; set; } = new();

    public int MaxEventsCapturePerCall { get; set; } = 500;
    public int MaxEventsReturnedInQuery { get; set; } = 20_000;
    public string VendorVersion { get; set; } = "2.0.1";
}
