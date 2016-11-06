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
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgusTV.Common.Logging
{
    public abstract class CustomLogger
    {
        private static NLog.Logger _log;
        private static LogLevel _logLevel = LogLevel.Off;

        public CustomLogger()
        {
            this.IsDebugEnabled = _logLevel <= LogLevel.Debug;
            this.IsInfoEnabled = _logLevel <= LogLevel.Info;
            this.IsWarnEnabled = _logLevel <= LogLevel.Warn;
            this.IsErrorEnabled = _logLevel <= LogLevel.Error;
            this.IsFatalEnabled = _logLevel <= LogLevel.Fatal;
        }

        public bool IsDebugEnabled { get; private set; }

        public bool IsErrorEnabled { get; private set; }

        public bool IsFatalEnabled { get; private set; }

        public bool IsInfoEnabled { get; private set; }

        public bool IsWarnEnabled { get; private set; }

        public void Debug(string message, Exception exception)
        {
            if (IsDebugEnabled)
            {
                _log.Debug(exception, message);
            }
        }

        public void Debug(string message)
        {
            if (IsDebugEnabled)
            {
                _log.Debug(message);
            }
        }

        public void DebugFormat(string format, params object[] args)
        {
            if (IsDebugEnabled)
            {
                _log.Debug(format, args);
            }
        }

        public void Error(string message, Exception exception)
        {
            if (IsErrorEnabled)
            {
                _log.Error(exception, message);
            }
        }

        public void Error(string message)
        {
            if (IsErrorEnabled)
            {
                _log.Error(message);
            }
        }

        public void ErrorFormat(string format, params object[] args)
        {
            if (IsErrorEnabled)
            {
                _log.Error(format, args);
            }
        }

        public void Fatal(string message, Exception exception)
        {
            if (IsFatalEnabled)
            {
                _log.Fatal(exception, message);
            }
        }

        public void Fatal(string message)
        {
            if (IsFatalEnabled)
            {
                _log.Fatal(message);
            }
        }

        public void Info(string message, Exception exception)
        {
            if (IsInfoEnabled)
            {
                _log.Info(exception, message);
            }
        }

        public void Info(string message)
        {
            if (IsInfoEnabled)
            {
                _log.Info(message);
            }
        }

        public void InfoFormat(string format, params object[] args)
        {
            if (IsInfoEnabled)
            {
                _log.Info(format, args);
            }
        }

        public void Warn(string message, Exception exception)
        {
            if (IsWarnEnabled)
            {
                _log.Warn(exception, message);
            }
        }

        public void Warn(string message)
        {
            if (IsWarnEnabled)
            {
                _log.Warn(message);
            }
        }

        public void WarnFormat(string format, params object[] args)
        {
            if (IsWarnEnabled)
            {
                _log.Warn(format, args);
            }
        }

        protected static void InitializeLogger(string logLevelSettingName, string fileName)
        {
            _logLevel = LogLevel.Warn;
			var levelAsString = ConfigurationManager.AppSettings.Get(logLevelSettingName);
			if (!String.IsNullOrEmpty(levelAsString))
			{
				_logLevel = LogLevel.FromString(levelAsString);
			}

            string filePath = Path.Combine(ArgusTV.Common.Logging.Logger.ArgusTVLogFolder, fileName);

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

			var rule = new LoggingRule(logLevelSettingName, _logLevel, fileTarget);
            config.LoggingRules.Add(rule);

            LogManager.Configuration = config;
			_log = LogManager.GetLogger(logLevelSettingName);
        }
    }
}
