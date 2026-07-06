using System.Security.Cryptography;
using System.Text;

namespace BarberFlow.Application.Common.Security
{
    public static class Sha256Hasher
    {
        public static string Hash(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            var bytes = Encoding.UTF8.GetBytes(value);
            var hash = SHA256.HashData(bytes);

            return Convert.ToHexString(hash);
        }
    }
}
