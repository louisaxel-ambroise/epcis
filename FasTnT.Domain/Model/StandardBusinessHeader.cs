namespace FasTnT.Domain.Model
{
    public class StandardBusinessHeader
    {
        public Request Request { get; set; }
        public string Version { get; set; }
        public List<ContactInformation> ContactInformations { get; set; } = new();
        public string Standard { get; set; }
        public string TypeVersion { get; set; }
        public string InstanceIdentifier { get; set; }
        public string Type { get; set; }
        public DateTime? CreationDateTime { get; set; }
    }
}
