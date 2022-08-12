using System;
using System.Security.Cryptography;
using System.Text;
using TaskScheduler.Domain;

namespace TaskScheduler.Database
{
    public class ScheduleTaskLinkGenerator : IScheduleTaskLinkGenerator
    {
        readonly static Random charGenerator = new Random();
        readonly static int randomPartMinLen = 4;
        readonly static int randomPartMaxLen = 8;
        readonly static string randomPartAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
            + "abcdefghijklmnopqrstuvwxyz" + "0123456789" + "-_";
        public string GenerateLink(string userInfo, string scheduleTaskInfo, int iteration)
        {
            string toHash = userInfo + scheduleTaskInfo + iteration.ToString();

            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(toHash));

            string hashPart = Convert.ToBase64String(hash);

            var link = new StringBuilder(hashPart.Length + randomPartMaxLen);

            link.Replace('=', 'A');
            link.Replace('+', '_');
            link.Replace('/', '-');

            lock (charGenerator)
            {
                int randomPartLen = charGenerator.Next(randomPartMinLen, randomPartMaxLen + 1);

                for (int i = 0; i < randomPartLen; i++)
                    link.Append(randomPartAlphabet[charGenerator.Next(randomPartAlphabet.Length)]);
            }

            return link.ToString();
        }
    }
}
