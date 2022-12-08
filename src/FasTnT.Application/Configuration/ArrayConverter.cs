using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace FasTnT.Application.Configuration;

public class ArrayConverter : ValueConverter<string[], string>
{
    public ArrayConverter()
        : base(
            v => JsonSerializer.Serialize(v, default(JsonSerializerOptions)),
            v => JsonSerializer.Deserialize<string[]>(v, default(JsonSerializerOptions))
        )
    {
    }
}
