using BarberFlow.Application.Common.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace BarberFlow.Infrastructure.Security
{
    public class TokenEncryptionService : ITokenEncryptionService
    {
        // Usa una clave segura y almacénala en configuración segura en producción
        private readonly byte[] _key = Encoding.UTF8.GetBytes("Yx1u32ChrLongEcrtinKey!123567894"); // 32 chars for AES-256
        private readonly byte[] _iv = Encoding.UTF8.GetBytes("16CharF1n160lIr!"); // 16 chars for AES

        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }
            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}
