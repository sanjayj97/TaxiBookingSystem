
using System;
using System.Security.Cryptography;

namespace TaxiBookingSystem.Services
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            byte[] salt = new byte[16];

            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }

            const int iterations = 100000;

            using (var pbkdf2 = new Rfc2898DeriveBytes(
                password, salt, iterations,
                HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32);

                return iterations + "." +
                    Convert.ToBase64String(salt) + "." +
                    Convert.ToBase64String(hash);
            }
        }


        public static bool VerifyPassword(
            string password, string storedValue)
        {
            try
            {
                string[] parts = storedValue.Split('.');

                if (parts.Length != 3)
                    return false;

                int iterations = int.Parse(parts[0]);
                byte[] salt = Convert.FromBase64String(parts[1]);
                byte[] storedHash = Convert.FromBase64String(parts[2]);

                // Reject malformed or unexpectedly expensive hash settings.
                if (iterations < 100000 || iterations > 1000000 ||
                    salt.Length != 16 || storedHash.Length != 32)
                    return false;

                using (var pbkdf2 = new Rfc2898DeriveBytes(
                    password, salt, iterations,
                    HashAlgorithmName.SHA256))
                {
                    byte[] calculatedHash = pbkdf2.GetBytes(32);

                    int difference = 0;

                    for (int i = 0; i < storedHash.Length; i++)
                        difference |= storedHash[i] ^ calculatedHash[i];

                    return difference == 0;
                }
            }
            catch (FormatException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }
        }

    }


}
