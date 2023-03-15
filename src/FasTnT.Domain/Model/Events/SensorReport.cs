namespace FasTnT.Domain.Model.Events;

public class SensorReport
{
    public int Index { get; set; }
    public int SensorIndex { get; set; }
    public string Type { get; set; }
    public string DeviceId { get; set; }
    public string RawData { get; set; }
    public string DataProcessingMethod { get; set; }
    public string CoordinateReferenceSystem { get; set; }
    public DateTime? Time { get; set; }
    public string Microorganism { get; set; }
    public string ChemicalSubstance { get; set; }
    public float? Value { get; set; }
    public string Component { get; set; }
    public string StringValue { get; set; }
    public bool BooleanValue { get; set; }
    public string HexBinaryValue { get; set; }
    public string UriValue { get; set; }
    public float? MinValue { get; set; }
    public float? MaxValue { get; set; }
    public float? MeanValue { get; set; }
    public float? PercRank { get; set; }
    public float? PercValue { get; set; }
    public string UnitOfMeasure { get; set; }
    public float? SDev { get; set; }
    public string DeviceMetadata { get; set; }
}