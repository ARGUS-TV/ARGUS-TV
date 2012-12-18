using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace ArgusTV.Client.Common
{
    public static class ClientUtility
    {
        private const string _secretKey = "%3f>#8$g2-v;R&7$%1#G5%Dw]F[$656@#S";

        public static string Encrypt(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return String.Empty;
            }
            byte[] resultArray = ProtectedData.Protect(UTF8Encoding.UTF8.GetBytes(text),
                UTF8Encoding.UTF8.GetBytes(_secretKey), DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string cipherString)
        {
            if (String.IsNullOrEmpty(cipherString))
            {
                return String.Empty;
            }
            try
            {
                byte[] resultArray = ProtectedData.Unprotect(Convert.FromBase64String(cipherString),
                    UTF8Encoding.UTF8.GetBytes(_secretKey), DataProtectionScope.CurrentUser);
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (CryptographicException)
            {
                return String.Empty;
            }
        }

        public static string EncodeTo64(string toEncode)
        {
            byte[] bytes = UnicodeEncoding.Unicode.GetBytes(toEncode);
            return Convert.ToBase64String(bytes).Replace("/", "_").Replace("+", "-").TrimEnd('=');
        }

        public static string DecodeFrom64(string toDecode)
        {
            toDecode = toDecode.Replace("_", "/").Replace("-", "+");
            toDecode = toDecode.PadRight(toDecode.Length + (4 - toDecode.Length % 4) % 4, '=');
            byte[] bytes = Convert.FromBase64String(toDecode);
            return UnicodeEncoding.Unicode.GetString(bytes);
        }
    }
}
