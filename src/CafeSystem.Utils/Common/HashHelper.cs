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
        
        /// <summary>
        /// Encode input to Base64
        /// </summary>
        /// <param name="input">The string which you want to convert</param>
        /// <returns>The converted base64 string</returns>
        public static string Base64Encode(this string input) {
            var plainTextBytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Decode Base64 data to it's original representation.
        /// </summary>
        /// <param name="base64EncodedData">The encoded data</param>
        /// <returns>The base64 decoded string</returns>
        public static string Base64Decode(this string base64EncodedData) {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

    }
}