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

using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4Hierarchy = log4net.Repository.Hierarchy;

namespace ArgusTV.Common.Logging
{
    public static class Logger
    {
        private static string _argusTV = "ArgusTV";
        private static ILog _log;

        private static SourceLevels _sourceLevels = SourceLevels.Off;

        public static void ConfigureNHibernateLoggers()
        {
#if DEBUG
            LogManager.GetRepository().Threshold = Level.Warn;
#else
            LogManager.GetRepository().Threshold = Level.Error;
#endif
        }

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
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"ARGUS TV\Logs"); }
        }

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

            Level level = Level.Debug;
            if (_sourceLevels == SourceLevels.Information)
            {
                level = Level.Info;
            }
            else if (_sourceLevels == SourceLevels.Warning)
            {
                level = Level.Warn;
            }
            else if (_sourceLevels == SourceLevels.Error)
            {
                level = Level.Error;
            }
            else if (_sourceLevels == SourceLevels.Critical)
            {
                level = Level.Fatal;
            }
            _sourceLevels = sourceLevels;

            log4Hierarchy.Hierarchy hierarchy =
                (log4Hierarchy.Hierarchy)LogManager.GetAllRepositories().FirstOrDefault(r => r.Name == _argusTV);
            if (hierarchy == null)
            {
                hierarchy = (log4Hierarchy.Hierarchy)LogManager.CreateRepository(_argusTV);
            }
            hierarchy.Root.RemoveAllAppenders();

            RollingFileAppender roller = new RollingFileAppender();
            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "%date [%-5level][%thread]: %message%newline";
            patternLayout.ActivateOptions();
            roller.Layout = patternLayout;
            roller.AppendToFile = true;
            roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            roller.MaxSizeRollBackups = 4;
            roller.MaximumFileSize = "1000KB";
            roller.StaticLogFileName = true;
            roller.File = filePath;
            roller.ActivateOptions();
            roller.AddFilter(new log4net.Filter.LevelRangeFilter()
            {
                LevelMin = level,
                LevelMax = Level.Fatal
            });
            log4net.Config.BasicConfigurator.Configure(hierarchy, roller);

            log4Hierarchy.Logger coreLogger = hierarchy.GetLogger(_argusTV) as log4Hierarchy.Logger;
            coreLogger.Level = level;

            _log = LogManager.GetLogger(hierarchy.Name, _argusTV);
        }

        public static void Write(string message, params object[] args)
        {
            if (_log.IsInfoEnabled)
            {
                _log.InfoFormat(message, args);
            }
        }

        public static void Info(string message, params object[] args)
        {
            if (_log.IsInfoEnabled)
            {
                _log.InfoFormat(message, args);
            }
        }

        public static void Warn(string message, params object[] args)
        {
            if (_log.IsWarnEnabled)
            {
                _log.WarnFormat(message, args);
            }
        }

        public static void Error(string message, params object[] args)
        {
            if (_log.IsErrorEnabled)
            {
                _log.ErrorFormat(message, args);
            }
        }

        public static void Critical(string message, params object[] args)
        {
            if (_log.IsFatalEnabled)
            {
                _log.FatalFormat(message, args);
            }
        }

        public static void Verbose(string message, params object[] args)
        {
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat(message, args);
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
