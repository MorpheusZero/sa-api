using System.Security.Cryptography;

namespace SoulArenasAPI.Util
{
    public static class CryptoHelper
    {
        private const int SaltSize = 32;
        private const int HashSize = 32;
        private const int Iterations = 100000;

        public static Task<string> HashPasswordAsync(string password)
        {
            return Task.Run(() =>
            {
                byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

                byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    Iterations,
                    HashAlgorithmName.SHA512,
                    HashSize);

                string saltBase64 = Convert.ToBase64String(salt);
                string hashBase64 = Convert.ToBase64String(hash);

                return $"{saltBase64}:{hashBase64}";
            });
        }

        public static Task<bool> VerifyPasswordAsync(string password, string hashedPassword)
        {
            return Task.Run(() =>
            {
                var parts = hashedPassword.Split(':');
                if (parts.Length != 2)
                {
                    return false;
                }

                byte[] salt = Convert.FromBase64String(parts[0]);
                byte[] storedHash = Convert.FromBase64String(parts[1]);

                byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    Iterations,
                    HashAlgorithmName.SHA512,
                    HashSize);

                return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
            });
        }
    }
}
