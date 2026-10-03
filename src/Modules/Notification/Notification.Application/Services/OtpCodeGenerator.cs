using System.Security.Cryptography;
using System.Text;

namespace Notification.Application.Services
{
    public static class OtpCodeGenerator
    {
        public static string Generate(int length)
        {
            var builder = new StringBuilder(length);

            for (var i = 0; i < length; i++)
                builder.Append(RandomNumberGenerator.GetInt32(0, 10));

            return builder.ToString();
        }

        public static string Hash(string code)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(code));
            return Convert.ToHexString(bytes);
        }
    }
}
