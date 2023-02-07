using System;
using System.Text;
using System.Security.Cryptography;

namespace CafeSystem.Utils
{
    public static class HashHelper
    {
        /// <summary>
        /// Hash input string with an optional salt.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="salt"></param>
        public static string HashString(this string input, string salt = null)
        {
            if (salt.IsNullOrEmpty()) input = $"{input}{salt}";

            using (var md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(i.ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }


        /// <summary>
        /// Returns MD5 hash for a given string.
        /// </summary>
        /// <param name="input">The string which you want to MD5</param>
        public static string Md5Hash(this string input)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            var encodedBytes = md5.ComputeHash(Encoding.Default.GetBytes(input));
            return BitConverter.ToString(encodedBytes);
        }
        
    }
}