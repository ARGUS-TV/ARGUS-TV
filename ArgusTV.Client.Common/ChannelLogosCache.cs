/*
 *	Copyright (C) 2007-2014 ARGUS TV
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
using System.IO;
using System.Drawing;

using ArgusTV.DataContracts;
using ArgusTV.ServiceProxy;

namespace ArgusTV.Client.Common
{
    public static class ChannelLogosCache
    {
        private static string _cacheBasePath;

        static ChannelLogosCache()
        {
            _cacheBasePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"ARGUS TV\LogosCache");
        }

        public static Image GetLogoImage(Channel channel, int width, int height)
        {
            return GetLogoImage(channel.ChannelId, channel.DisplayName, width, height);
        }

        public static Image GetLogoImage(Guid channelId, string channelDisplayName, int width, int height)
        {
            string fileName = GetLogoPath(channelId, channelDisplayName, width, height);
            return fileName == null ? null : Image.FromFile(fileName);
        }

        public static string GetLogoPath(Channel channel, int width, int height)
        {
            return GetLogoPath(channel.ChannelId, channel.DisplayName, width, height);
        }

        public static string GetLogoPath(Guid channelId, string channelDisplayName, int width, int height)
        {
            string cachePath = Path.Combine(_cacheBasePath, width.ToString(CultureInfo.InvariantCulture) + "x" + height.ToString(CultureInfo.InvariantCulture));
            Directory.CreateDirectory(cachePath);

            string logoImagePath = Path.Combine(cachePath, MakeValidFileName(channelDisplayName) + ".png");

            DateTime modifiedDateTime = DateTime.MinValue;
            if (File.Exists(logoImagePath))
            {
                modifiedDateTime = File.GetLastWriteTime(logoImagePath);
            }

            byte[] imageBytes = Proxies.SchedulerService.GetChannelLogo(channelId, width, height, modifiedDateTime).Result;
            if (imageBytes == null)
            {
                if (File.Exists(logoImagePath))
                {
                    File.Delete(logoImagePath);
                }
            }
            else if (imageBytes.Length > 0)
            {
                using (FileStream imageStream = new FileStream(logoImagePath, FileMode.Create))
                {
                    imageStream.Write(imageBytes, 0, imageBytes.Length);
                    imageStream.Close();
                }
            }

            return File.Exists(logoImagePath) ? logoImagePath : null;
        }

        private static string MakeValidFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c.ToString(), String.Empty);
            }
            return fileName.TrimEnd('.', ' ');
        }
    }
}
