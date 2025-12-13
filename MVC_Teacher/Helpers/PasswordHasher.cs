using System;
using System.Security.Cryptography;
using System.Text;

namespace MVC_Teacher.Helpers
{
    // Minimal PBKDF2-based password hasher to be used by MVC_Teacher project
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(salt);

            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                var hash = deriveBytes.GetBytes(32);
                return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
            }
        }

        public static bool VerifyPassword(string providedPassword, string storedHash)
        {
            if (string.IsNullOrEmpty(storedHash) || string.IsNullOrEmpty(providedPassword)) return false;
            var parts = storedHash.Split('.');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var hash = Convert.FromBase64String(parts[1]);

            using (var deriveBytes = new Rfc2898DeriveBytes(providedPassword, salt, 10000, HashAlgorithmName.SHA256))
            {
                var testHash = deriveBytes.GetBytes(32);
                // constant-time comparison
                if (testHash.Length != hash.Length) return false;
                int diff = 0;
                for (int i = 0; i < testHash.Length; i++) diff |= testHash[i] ^ hash[i];
                return diff == 0;
            }
        }
    }
}
