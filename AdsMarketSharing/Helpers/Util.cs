using Microsoft.AspNetCore.Http;
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http.Features;
using System.Linq;

namespace AdsMarketSharing.Helpers
{
    public class Util
    {
        private static Random random = new Random();
        public static String HmacSHA512(string key, String inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }

        public static string GetIpAddress(HttpContext httpContext)
        {
            if (!string.IsNullOrEmpty(httpContext.Request.Headers["X-Forwarded-For"]))
            {
                return httpContext.Request.Headers["X-Forwarded-For"];
            }
            return httpContext.Connection?.RemoteIpAddress?.ToString();
        }

        public static string RandomString(int length)
        { 
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
