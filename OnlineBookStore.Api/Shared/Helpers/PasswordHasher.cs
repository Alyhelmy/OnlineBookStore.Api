using System.Security.Cryptography;
using System.Text;

namespace OnlineBookStore.Api.Shared.Helpers
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password) 
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        //Register → hash password → save hash
        //Login → hash entered password → compare with saved hash


        public static bool VerifyPassword(string password, string storedHash) // this method will be used to compare the provided password with the stored hash
        {
            var passwordHash = HashPassword(password);
            return passwordHash == storedHash;
        }

    }
}
