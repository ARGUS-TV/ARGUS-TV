/*
 *	Copyright (C) 2007-2013 ARGUS TV
 *	http://www.argus-tv.com
 *
 *  This Program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *
 *  This Program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with GNU Make; see the file COPYING.  If not, write to
 *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 *  http://www.gnu.org/copyleft/gpl.html
 *
 */
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
