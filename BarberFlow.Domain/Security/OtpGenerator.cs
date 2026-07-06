using System.Security.Cryptography;

namespace BarberFlow.Domain.Security
{
    public static class OtpGenerator
    {
        public static string Generate(int digits = 6)
        {
            var min = (int)Math.Pow(10, digits - 1);
            var max = (int)Math.Pow(10, digits);

            return RandomNumberGenerator
                .GetInt32(min, max)
                .ToString();
        }
    }
}
