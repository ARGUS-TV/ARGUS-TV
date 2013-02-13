/*
 *	Copyright (C) 2007-2012 ARGUS TV
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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceModel;

using ArgusTV.Common.Logging;
using ArgusTV.DataContracts;

namespace ArgusTV.Common
{
    public static class Utility
    {
        private const int _maxPath = 255;

        public static string StringToMD5(string text)
        {
            if(String.IsNullOrEmpty(text))
                return null;

            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                byte[] originalBytes = ASCIIEncoding.Default.GetBytes(text);
                return BitConverter.ToString(md5.ComputeHash(originalBytes));
            }
        }

        public static string ByteArrayToHex(byte[] ba)
        {
            char[] c = new char[ba.Length * 2];
            byte b;
            for (int i = 0; i < ba.Length; i++)
            {
                b = ((byte)(ba[i] >> 4));
                c[i * 2] = (char)(b > 9 ? b + 0x37 : b + 0x30);
                b = ((byte)(ba[i] & 0xF));
                c[i * 2 + 1] = (char)(b > 9 ? b + 0x37 : b + 0x30);
            }
            return new string(c);
        }

        public static string CheckStringLength(string input, int maxLength, char separator = ' ', string terminator = "...")
        {
            if (String.IsNullOrEmpty(input) || input.Length <= maxLength)
                return input;

            bool addTerminator = false;
            if (!String.IsNullOrEmpty(terminator) && maxLength > terminator.Length)
            {
                maxLength -= terminator.Length;
                addTerminator = true;
            }

            int index = input.LastIndexOf(separator, maxLength);
            if (index > 0)
                input = input.Substring(0, index).TrimEnd(separator);
            else
                input = input.Substring(0, maxLength);

            return addTerminator ? input + terminator : input;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GetDiskFreeSpaceEx(string drive, out long freeBytesForUser, out long totalBytes, out long freeBytes);

        public static long GetDiskFreeSpace(string fileName)
        {
            string dirName = Path.GetDirectoryName(fileName);

            long freeBytesForUser, totalBytes, freeBytes;
            if (GetDiskFreeSpaceEx(dirName, out freeBytesForUser, out totalBytes, out freeBytes))
            {
                return freeBytes;
            }

            // Let's retry once...
            System.Threading.Thread.Sleep(50);
            if (GetDiskFreeSpaceEx(dirName, out freeBytesForUser, out totalBytes, out freeBytes))
            {
                return freeBytes;
            }

            // In case of a failure, let's return a big number (100GB) to make sure noone starts
            // to assume the disk is full.
            Logger.Warn("Failed to get free disk space on {0}.", dirName);
            return 100L * 1024L * 1024L * 1024L;
        }

        public static int GetPortFromUrl(string url)
        {
            int colonIndex = url.IndexOf(':');
            colonIndex = url.IndexOf(':', colonIndex + 1);
            int slashIndex = url.IndexOf('/', colonIndex + 1);
            string portString = url.Substring(colonIndex + 1, slashIndex - colonIndex - 1);
            return Int32.Parse(portString, CultureInfo.InvariantCulture);
        }

        public static string BuildRecordingBaseFilePath(string format, Recording recording)
        {
            return BuildRecordingBaseFilePath(format, String.Empty, recording);
        }

        private class FormatTag
        {
            private int _index;
            private string _name;

            public FormatTag(int index, string name)
            {
                _index = index;
                _name = name;
            }

            public int Index
            {
                get { return _index; }
            }

            public string Name
            {
                get { return _name; }
            }
        }

        public static string BuildRecordingBaseFilePath(string format, UpcomingRecording upcomingRecording, string scheduleName, int? episodeNumber, int? seriesNumber)
        {
            UpcomingProgram upcoming = upcomingRecording.Program;
            return BuildRecordingBaseFilePath(format, upcoming.Channel.DisplayName, scheduleName, upcoming.Title, upcoming.CreateProgramTitle(),
                upcoming.SubTitle, upcoming.EpisodeNumberDisplay, episodeNumber, seriesNumber, upcoming.StartTime, upcoming.Category);
        }

        public static string BuildRecordingBaseFilePath(string format, string sourceDirectory, Recording recording)
        {
            string result = Path.Combine(sourceDirectory, BuildRecordingBaseFilePath(format, recording.ChannelDisplayName,
                recording.ScheduleName, recording.Title, recording.CreateProgramTitle(), recording.SubTitle, recording.EpisodeNumberDisplay,
                recording.EpisodeNumber, recording.SeriesNumber, recording.ProgramStartTime, recording.Category));
            if (result.Length > _maxPath - 1)
            {
                result = result.Substring(0, _maxPath - 1);
            }
            return result;
        }

        public static string BuildRecordingBaseFilePath(string format, string channelDisplayName, string scheduleName,
            string title, string programTitle, string subTitle, string episodeNumberDisplay, int? episodeNumber, int? seriesNumber, DateTime startTime, string category)
        {
            LimitLength(ref title, 80);
            LimitLength(ref programTitle, 80);
            LimitLength(ref scheduleName, 80);
            LimitLength(ref subTitle, 80);

            format = MakeValidPath(format).Replace(":", String.Empty).Replace(".", String.Empty).TrimStart('\\').TrimStart('/');
            StringBuilder result = new StringBuilder(format);
            ReplaceFormatVariable(result, "%%CHANNEL%%", channelDisplayName);
            ReplaceFormatVariable(result, "%%TVCHANNEL%%", channelDisplayName); // For backwards compatibility.
            ReplaceFormatVariable(result, "%%SCHEDULE%%", scheduleName);
            ReplaceFormatVariable(result, "%%TITLE%%", title);
            ReplaceFormatVariable(result, "%%LONGTITLE%%", programTitle);
            ReplaceFormatVariable(result, "%%EPISODETITLE%%", String.IsNullOrEmpty(subTitle) ? "#" : subTitle);
            ReplaceFormatVariable(result, "%%EPISODENUMBERDISPLAY%%", String.IsNullOrEmpty(episodeNumberDisplay) ? "#" : episodeNumberDisplay);
            ReplaceFormatVariable(result, "%%EPISODENUMBER%%", episodeNumber.HasValue ? episodeNumber.ToString() : "#");
            ReplaceFormatVariable(result, "%%EPISODENUMBER2%%", episodeNumber.HasValue ? episodeNumber.Value.ToString("00") : "00");
            ReplaceFormatVariable(result, "%%EPISODENUMBER3%%", episodeNumber.HasValue ? episodeNumber.Value.ToString("000") : "000");
            ReplaceFormatVariable(result, "%%SERIES%%", seriesNumber.HasValue ? seriesNumber.ToString() : "#");
            ReplaceFormatVariable(result, "%%SERIES2%%", seriesNumber.HasValue ? seriesNumber.Value.ToString("00") : "00");
            ReplaceFormatVariable(result, "%%DATE%%", startTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            ReplaceFormatVariable(result, "%%YEAR%%", startTime.ToString("yyyy", CultureInfo.InvariantCulture));
            ReplaceFormatVariable(result, "%%MONTH%%", startTime.ToString("MM", CultureInfo.InvariantCulture));
            ReplaceFormatVariable(result, "%%DAY%%", startTime.ToString("dd", CultureInfo.InvariantCulture));
            ReplaceFormatVariable(result, "%%DAYOFWEEK%%", startTime.ToString("ddd", CultureInfo.CurrentCulture));
            ReplaceFormatVariable(result, "%%HOURS%%", startTime.ToString("HH", CultureInfo.InvariantCulture));
            ReplaceFormatVariable(result, "%%HOURS12%%", startTime.ToString("hhtt", CultureInfo.InvariantCulture));
            ReplaceFormatVariable(result, "%%MINUTES%%", startTime.ToString("mm", CultureInfo.InvariantCulture));
            ReplaceFormatVariable(result, "%%CATEGORY%%", String.IsNullOrEmpty(category) ? "#" : category);
            return result.ToString();
        }

        private static void LimitLength(ref string text, int maxLength)
        {
            if (text != null
                && text.Length > maxLength)
            {
                text = text.Substring(0, maxLength);
            }
        }

        private static void ReplaceFormatVariable(StringBuilder format, string variable, string value)
        {
            format.Replace(variable, Utility.MakeValidFileName(value));
        }

        public static string GetFreeFileName(string baseFileName, string extension)
        {
            if (!String.IsNullOrEmpty(extension) && !extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            if (baseFileName.Length > _maxPath - extension.Length - 1)
            {
                baseFileName = baseFileName.Substring(0, _maxPath - extension.Length - 1);
            }

            while (baseFileName.Length < 4)
            {
                baseFileName += '_';
            }

            string fileName = baseFileName + extension;

            int count = 1;
            while (File.Exists(fileName)
                && count < 256 * 8)
            {
                fileName = baseFileName.Substring(0, baseFileName.Length - 3) + count.ToString("X", CultureInfo.InvariantCulture).PadLeft(3, '_') + extension;
                count++;
            }

            return fileName;
        }

        public static string MakeValidFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c.ToString(), String.Empty);
            }
            return fileName.TrimEnd('.', ' ');
        }

        public static string MakeValidPath(string path)
        {
            foreach (char c in Path.GetInvalidPathChars())
            {
                path = path.Replace(c.ToString(), String.Empty);
            }
            return path.TrimEnd('.', ' ');
        }

        public static Guid NewSequentialGuid()
        {
            GUIDDATA guiddata;
            if ((UuidCreateSequential(out guiddata) & 0x80000000) != 0) // FAILED(hr)
            {
                throw new InvalidOperationException();
            }

            SwapBytes(guiddata.Data, 0, 3);
            SwapBytes(guiddata.Data, 1, 2);
            SwapBytes(guiddata.Data, 4, 5);
            SwapBytes(guiddata.Data, 6, 7);

            return new Guid(guiddata.Data);
        }

        private static void SwapBytes(byte[] bytes, int index1, int index2)
        {
            byte b = bytes[index1];
            bytes[index1] = bytes[index2];
            bytes[index2] = b;
        }

        public static string[] ParseCommandArguments(string commandLine)
        {
            List<string> arguments = new List<string>();

            StringBuilder argument = new StringBuilder(200);
            bool quotedArgument = false;
            int index = -1;
            while (++index < commandLine.Length)
            {
                char ch = commandLine[index];
                if (argument.Length == 0
                    && !quotedArgument)
                {
                    // Looking for the next argument.
                    if (ch == '"')
                    {
                        quotedArgument = true;
                    }
                    else if (ch != ' ')
                    {
                        argument.Append(ch);
                    }
                }
                else
                {
                    if (quotedArgument)
                    {
                        if (ch == '"')
                        {
                            if (index + 1 < commandLine.Length
                                && commandLine[index + 1] == '"')
                            {
                                argument.Append(ch);
                                index++;
                            }
                            else
                            {
                                arguments.Add(argument.ToString());
                                argument.Length = 0;
                                quotedArgument = false;
                            }
                        }
                        else
                        {
                            argument.Append(ch);
                        }
                    }
                    else
                    {
                        if (ch == ' ')
                        {
                            arguments.Add(argument.ToString());
                            argument.Length = 0;
                            quotedArgument = false;
                        }
                        else
                        {
                            argument.Append(ch);
                        }
                    }
                }
            }
            if (argument.Length > 0
                || quotedArgument)
            {
                arguments.Add(argument.ToString());
            }
            return arguments.ToArray();
        }

        #region API

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct GUIDDATA
        {
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] Data;
        }

        [System.Runtime.InteropServices.DllImport("rpcrt4.dll")]
        private static extern int UuidCreateSequential(out GUIDDATA Uuid);

        #endregion

        #region Image Resizing

        public static Bitmap ResizeImage(Image image, int width, int height, bool useTransparentBackground, int? argbBackground)
        {
            bool useBackground = false;
            if (width > 0
                && height > 0)
            {
                useBackground = useTransparentBackground || argbBackground.HasValue;
            }

            float newWidth;
            float newHeight;
            double aspectHeight = (width / (double)image.Width) * image.Height;
            double aspectWidth = (height / (double)image.Height) * image.Width;
            if (width > 0
                && (height == 0 || aspectHeight <= height))
            {
                newWidth = width;
                newHeight = (float)aspectHeight;
            }
            else if (height > 0
                && (width == 0 || aspectWidth <= width))
            {
                newWidth = (float)aspectWidth;
                newHeight = height;
            }
            else
            {
                newWidth = image.Width;
                newHeight = image.Height;
            }

            Bitmap bitmap = new Bitmap(useBackground ? width : (int)newWidth, useBackground ? height : (int)newHeight,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                if (!useTransparentBackground
                    && argbBackground.HasValue)
                {
                    using (Brush bgBrush = new SolidBrush(Color.FromArgb(argbBackground.Value)))
                    {
                        g.FillRectangle(bgBrush, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
                    }
                }
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, ((float)bitmap.Width - newWidth) / 2, ((float)bitmap.Height - newHeight) / 2, newWidth, newHeight);
            }
            return bitmap;
        }

        public static byte[] ResizeImageBytes(byte[] imageBytes, int width, int height,
            bool useTransparentBackground, int? argbBackground, System.Drawing.Imaging.ImageFormat imageFormat)
        {
            using (MemoryStream memoryStream = new MemoryStream(imageBytes, false))
            {
                return ResizeImageBytes(memoryStream, width, height, useTransparentBackground, argbBackground, imageFormat);
            }
        }

        public static byte[] ResizeImageBytes(Stream imageStream, int width, int height,
            bool useTransparentBackground, int? argbBackground, System.Drawing.Imaging.ImageFormat imageFormat)
        {
            using (Image image = Image.FromStream(imageStream))
            {
                using (MemoryStream outputStream = new MemoryStream(32768))
                {
                    using (Bitmap bitmap = ResizeImage(image, width, height, useTransparentBackground, argbBackground))
                    {
                        bitmap.Save(outputStream, imageFormat);
                    }
                    byte[] result = new byte[outputStream.Length];
                    outputStream.Seek(0, SeekOrigin.Begin);
                    outputStream.Read(result, 0, result.Length);
                    return result;
                }
            }
        }

        #endregion
    }
}
