using FasTnT.Domain.Enumerations;

namespace FasTnT.Domain.Model.Events;

public class ContactInformation
{
    public StandardBusinessHeader Header { get; set; }
    public ContactInformationType Type { get; set; }
    public string Identifier { get; set; }
    public string Contact { get; set; }
    public string EmailAddress { get; set; }
    public string FaxNumber { get; set; }
    public string TelephoneNumber { get; set; }
    public string ContactTypeIdentifier { get; set; }
}
