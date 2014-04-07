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

using ArgusTV.DataContracts;
using RestSharp;
using RestSharp.Extensions;

namespace ArgusTV.ServiceProxy
{
    /// <summary>
    /// High-level logging service.
    /// </summary>
    public partial class LogServiceProxy : RestProxyBase
    {
        /// <summary>
        /// Constructs a channel to the service.
        /// </summary>
        public LogServiceProxy()
            : base("Log")
        {
        }

        /// <summary>
        /// Log a message.
        /// </summary>
        /// <param name="module">The name of the module that is logging the message.</param>
        /// <param name="logSeverity">The severity of the message.</param>
        /// <param name="message">The message text.</param>
        public void LogMessage(string module, LogSeverity logSeverity, string message)
        {
            var request = NewRequest("/Message", Method.POST);
            request.AddBody(new
            {
                Module = module,
                Severity = logSeverity,
                Message = message
            });
            Execute(request);
        }

        /// <summary>
        /// Get all log entries for a certain module.
        /// </summary>
        /// <param name="lowerDate">Return messages logged after this date.</param>
        /// <param name="upperDate">Return messages logged before this date.</param>
        /// <param name="maxEntries">The maximum number of messages to return.</param>
        /// <param name="module">The name of the module, or null.</param>
        /// <param name="severity">The severity of the messages, or null.</param>
        /// <param name="maxEntriesReached">Will be set to true if more than 'maxEntries' messages where available.</param>
        /// <returns>An array containing zero or more log message.</returns>
        public List<LogEntry> GetLogEntries(DateTime lowerDate, DateTime upperDate, int maxEntries, string module, LogSeverity? severity, out bool maxEntriesReached)
        {
            var request = NewRequest("/Entries/{lowerDate}/{upperDate}/{maxEntries}", Method.POST);
            request.AddParameter("lowerDate", ToIso8601(lowerDate), ParameterType.UrlSegment);
            request.AddParameter("upperDate", ToIso8601(upperDate), ParameterType.UrlSegment);
            request.AddParameter("maxEntries", maxEntries, ParameterType.UrlSegment);
            if (module != null)
            {
                request.AddParameter("module", module, ParameterType.QueryString);
            }
            if (severity.HasValue)
            {
                request.AddParameter("severity", severity.Value, ParameterType.QueryString);
            }
            var result = Execute<LogEntriesResult>(request);
            maxEntriesReached = result.MaxEntriesReached;
            return result.LogEntries;
        }

        private class LogEntriesResult
        {
            public List<LogEntry> LogEntries { get; set; }
            public bool MaxEntriesReached { get; set; }
        }

        /// <summary>
        /// Get all available modules currently in the log.
        /// </summary>
        /// <returns>An array containing zero or more module names.</returns>
        public List<string> GetAllModules()
        {
            var request = NewRequest("/Modules", Method.GET);
            return Execute<List<string>>(request);
        }

        /// <summary>
        /// Send a test-mail using the current SMTP settings.
        /// </summary>
        public void SendTestMail()
        {
            var request = NewRequest("/TestMail", Method.POST);
            Execute(request);
        }
    }
}
