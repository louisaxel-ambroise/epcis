using System.Security.Cryptography;
using System.Text;

namespace FasTnT.Application.Services.Users
{
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

            return Encoding.UTF8.GetString(deriveBytes.GetBytes(256));
        }
    }
}
