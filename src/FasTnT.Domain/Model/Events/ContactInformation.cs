using FasTnT.Domain.Enumerations;

namespace FasTnT.Domain.Model.Events;

public class ContactInformation
{
    public ContactInformationType Type { get; init; }
    public string Identifier { get; init; }
    public string Contact { get; init; }
    public string EmailAddress { get; init; }
    public string FaxNumber { get; init; }
    public string TelephoneNumber { get; init; }
    public string ContactTypeIdentifier { get; init; }
}
