using FasTnT.Application.Domain.Model;

namespace FasTnT.Application.Validators;

public static class HeaderValidator
{
    public static bool IsValid(StandardBusinessHeader header)
    {
        return header is null || !string.IsNullOrEmpty(header.InstanceIdentifier);
    }
}
