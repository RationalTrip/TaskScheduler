using System;
using System.Text;
using TaskScheduler.Domain;

namespace TaskScheduler.Database
{
    public class SaltGenerator : ISaltGenerator
    {
        readonly static Random generator = new Random();
        readonly static string alphabet = "./0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        readonly static int saltLength = 16;
        public string GenerateSalt()
        {
            var salt = new StringBuilder(saltLength);

            lock (generator)
            {
                for(int i = 0; i < saltLength; i++)
                {
                    int charPosition = generator.Next(alphabet.Length);
                    salt.Append(alphabet[charPosition]);
                }
            }

            return salt.ToString();
        }
    }
}
