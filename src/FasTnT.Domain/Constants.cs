namespace FasTnT.Domain;

public static class Constants
{
    public static int MaxEventsCapturePerCall { get; set; } = 500;
    public static int MaxEventsReturnedInQuery { get; set; } = 20_000;
    public static string VendorVersion { get; set; } = "2.0.1";
}
