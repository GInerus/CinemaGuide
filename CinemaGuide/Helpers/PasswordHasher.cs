using System.Security.Cryptography;
using System.Text;

namespace CinemaGuide.Helpers
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha.ComputeHash(bytes);

                return Convert.ToHexString(hashBytes);
            }
        }
    }
}
