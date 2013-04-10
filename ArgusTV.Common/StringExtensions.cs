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
using System.Globalization;
using System.Linq;
using System.Text;

namespace ArgusTV.Common
{
    public static class StringExtensions
    {
        public static T ParseEnum<T>(this string source)
        {
            if (!typeof(T).IsEnum)
            {
                throw new NotSupportedException("Type must be an enum");
            }
            return (T)Enum.Parse(typeof(T), source);
        }

        public static bool ParseBool(this string source)
        {
            return ParseBool(source, false);
        }

        public static bool ParseBool(this string source, bool defaultValue)
        {
            if (String.IsNullOrEmpty(source))
            {
                return defaultValue;
            }
            return bool.Parse(source);
        }

        public static DateTime ParseIsoDate(this string source)
        {
            string[] formats = new string[] { "yyyy-MM-ddTHH:mm:sszzz", "yyyy-MM-ddTHH:mm:ss", "yyyyMMddTHHmmss", "yyyyMMdd", "yyyy-MM-dd" };
            return DateTime.ParseExact(source, formats, CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite|DateTimeStyles.AllowTrailingWhite|DateTimeStyles.AssumeLocal);
        }
    }
}
