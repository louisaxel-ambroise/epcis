using System.Security.Cryptography;

namespace FasTnT.Application.EfCore.Services.Users;

public static class PasswordUtils
{
    static readonly RandomNumberGenerator RandomGenerator = RandomNumberGenerator.Create();

    public static byte[] GetSalt()
    {
        var data = new byte[20];
        RandomGenerator.GetBytes(data);

        return data;
    }

    public static string GetSecuredKey(string password, byte[] salt)
    {
        var deriveBytes = new Rfc2898DeriveBytes(password, salt, 1_000);

        return Convert.ToBase64String(deriveBytes.GetBytes(256));
    }
}
