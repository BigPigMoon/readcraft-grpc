using System.Text;
using System.Security.Cryptography;

namespace Server.Utils
{
    internal class HashUtil
    {
        public static string HashPassowd(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());

        public static bool VerifyPassword(string password, string hashedPassword) =>
            BCrypt.Net.BCrypt.Verify(password, hashedPassword);

        public static string HashToken(string token) =>
            ComputeSHA256Hash(token);

        public static bool VerifyToken(string token, string hashedToken) =>
            ComputeSHA256Hash(token) == hashedToken;

        private static string ComputeSHA256Hash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}
