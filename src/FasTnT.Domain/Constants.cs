namespace FasTnT.Domain;

public sealed class Constants
{
    public static Constants Instance { get; set; } = new();

    public int MaxEventsCapturePerCall { get; init; } = 500;
    public int CaptureSizeLimit { get; init; } = 1_024;
    public int MaxEventsReturnedInQuery { get; init; } = 20_000;
    public Version VendorVersion { get; init; } = Version.Parse("2.5.1");
}
