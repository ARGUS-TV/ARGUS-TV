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
using System.Text;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

using NLog;
using NLog.Config;
using NLog.Targets;

namespace ArgusTV.Common.Logging
{
    public static class Logger
    {
        private static NLog.Logger _log;

        private static SourceLevels _sourceLevels = SourceLevels.Off;

        #region Logging Enabled?

        public static bool IsVerboseEnabled
        {
            get { return _sourceLevels >= SourceLevels.Verbose; }
        }

        public static bool IsInformationEnabled
        {
            get { return _sourceLevels >= SourceLevels.Information; }
        }

        public static bool IsWarningEnabled
        {
            get { return _sourceLevels >= SourceLevels.Warning; }
        }

        public static bool IsErrorEnabled
        {
            get { return _sourceLevels >= SourceLevels.Error; }
        }

        public static bool IsCriticalEnabled
        {
            get { return _sourceLevels >= SourceLevels.Critical; }
        }

        public static string LogLevelAsString
        {
            get
            {
                if (IsVerboseEnabled)
                    return SourceLevels.Verbose.ToString();
                if (IsInformationEnabled)
                    return SourceLevels.Information.ToString();
                if (IsWarningEnabled)
                    return SourceLevels.Warning.ToString();
                if (IsErrorEnabled)
                    return SourceLevels.Error.ToString();
                if (IsCriticalEnabled)
                    return SourceLevels.Critical.ToString();
                return SourceLevels.Off.ToString();
            }
        }

        public static string ArgusTVLogFolder
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ARGUS TV", "Logs"); }
        }

        public static string LogFilePath { get; private set; }

        #endregion

        /// <summary>
        /// Warning: this is not tread-safe, so only call this at startup or at a time that you are sure your
        /// process is not performing any logging!
        /// </summary>
        /// <param name="filePath">The path to the log file.</param>
        /// <param name="sourceLevels">The lowest log level to log.</param>
        public static void SetLogFilePath(string filePath, SourceLevels sourceLevels)
        {
            if (!Path.IsPathRooted(filePath))
            {
                filePath = Path.Combine(ArgusTVLogFolder, filePath);
            }
            LogFilePath = filePath;

            var config = LogManager.Configuration;
            if (config == null)
            {
                config = new LoggingConfiguration();
            }

            var fileTarget = new FileTarget
            {
                ConcurrentWrites = true,
                FileName = filePath,
                Layout = "${longdate} [${pad:padding=-5:fixedLength=true:inner=${level}}][${threadname:whenEmpty=${threadid}}]: ${message}",
                BufferSize = 256 * 1024,
                ArchiveFileName = filePath.Replace(".log", "_{#}.log"),
                ArchiveAboveSize = 1000 * 1000, // 1MB
                ArchiveNumbering = ArchiveNumberingMode.Rolling,
                MaxArchiveFiles = 9
            };
            config.AddTarget("file", fileTarget);

            LogLevel level = SetLogLevel(sourceLevels);
            var rule = new LoggingRule("DiskLogger", level, fileTarget);
            config.LoggingRules.Add(rule);

            LogManager.Configuration = config;
            _log = LogManager.GetLogger("DiskLogger");
        }

        public static void Shutdown()
        {
            LogManager.Shutdown();
        }

        private static LogLevel SetLogLevel(SourceLevels sourceLevels)
        {
            LogLevel level = LogLevel.Debug;
            if (sourceLevels == SourceLevels.Information)
            {
                level = LogLevel.Info;
            }
            else if (sourceLevels == SourceLevels.Warning)
            {
                level = LogLevel.Warn;
            }
            else if (sourceLevels == SourceLevels.Error)
            {
                level = LogLevel.Error;
            }
            else if (sourceLevels == SourceLevels.Critical)
            {
                level = LogLevel.Fatal;
            }
            _sourceLevels = sourceLevels;
            return level;
        }

        public static void Write(string message, params object[] args)
        {
            if (_log.IsInfoEnabled)
            {
                _log.Info(message, args);
            }
        }

        public static void Info(string message, params object[] args)
        {
            if (_log.IsInfoEnabled)
            {
                _log.Info(message, args);
            }
        }

        public static void Warn(string message, params object[] args)
        {
            if (_log.IsWarnEnabled)
            {
                _log.Warn(message, args);
            }
        }

        public static void Error(string message, params object[] args)
        {
            if (_log.IsErrorEnabled)
            {
                _log.Error(message, args);
            }
        }

        public static void Critical(string message, params object[] args)
        {
            if (_log.IsFatalEnabled)
            {
                _log.Fatal(message, args);
            }
        }

        public static void Verbose(string message, params object[] args)
        {
            if (_log.IsDebugEnabled)
            {
                _log.Debug(message, args);
            }
        }

        public static void Write(TraceEventType severity, string message, params object[] args)
        {
            Write(null, severity, message, args);
        }

        public static void Write(string category, TraceEventType severity, string message, params object[] args)
        {
            switch (severity)
            {
                case TraceEventType.Verbose: Verbose(message, args); break;
                case TraceEventType.Information: Info(message, args); break;
                case TraceEventType.Warning: Warn(message, args); break;
                case TraceEventType.Error: Error(message, args); break;
                case TraceEventType.Critical: Critical(message, args); break;
            }
        }
    }
}
