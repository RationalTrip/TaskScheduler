using System;
using System.Security.Cryptography;
using System.Text;
using TaskScheduler.Domain;

namespace TaskScheduler.Database
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string salt, string password)
        {
            string toHash = salt + password;

            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(toHash));

            return Convert.ToBase64String(hash);
        }
    }
}
