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
using System.Xml;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;

using ArgusTV.DataContracts;

namespace ArgusTV.ServiceProxy
{
    /// <summary>
    /// The service proxies to the ARGUS TV Scheduler service.
    /// </summary>
    public sealed class Proxies
    {
        private object _syncLock = new object();

        private Proxies()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
        }

        #region Singleton

        /// <summary>
        /// The factories singleton.
        /// </summary>
        internal static Proxies Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly Proxies instance = new Proxies();
        }

        #endregion

        #region Proxies

        private static ConfigurationServiceProxy _configurationServiceProxy;

        /// <summary>
        /// The (thread-safe) proxy to the Configuration service.
        /// </summary>
        public static ConfigurationServiceProxy ConfigurationService
        {
            get
            {
                if (_configurationServiceProxy == null)
                {
                    lock (Instance._syncLock)
                    {
                        if (_configurationServiceProxy == null)
                        {
                            _configurationServiceProxy = new ConfigurationServiceProxy();
                        }
                    }
                }
                return _configurationServiceProxy;
            }
        }

        private static ControlServiceProxy _controlServiceProxy;

        /// <summary>
        /// The (thread-safe) proxy to the Control service.
        /// </summary>
        public static ControlServiceProxy ControlService
        {
            get
            {
                if (_controlServiceProxy == null)
                {
                    lock (Instance._syncLock)
                    {
                        if (_controlServiceProxy == null)
                        {
                            _controlServiceProxy = new ControlServiceProxy();
                        }
                    }
                }
                return _controlServiceProxy;
            }
        }

        private static CoreServiceProxy _coreServiceProxy;

        /// <summary>
        /// The (thread-safe) proxy to the Core service.
        /// </summary>
        public static CoreServiceProxy CoreService
        {
            get
            {
                if (_coreServiceProxy == null)
                {
                    lock (Instance._syncLock)
                    {
                        if (_coreServiceProxy == null)
                        {
                            _coreServiceProxy = new CoreServiceProxy();
                        }
                    }
                }
                return _coreServiceProxy;
            }
        }

        private static GuideServiceProxy _guideServiceProxy;

        /// <summary>
        /// The (thread-safe) proxy to the Guide service.
        /// </summary>
        public static GuideServiceProxy GuideService
        {
            get
            {
                if (_guideServiceProxy == null)
                {
                    lock (Instance._syncLock)
                    {
                        if (_guideServiceProxy == null)
                        {
                            _guideServiceProxy = new GuideServiceProxy();
                        }
                    }
                }
                return _guideServiceProxy;
            }
        }

        private static LogServiceProxy _logServiceProxy;

        /// <summary>
        /// The (thread-safe) proxy to the Log service.
        /// </summary>
        public static LogServiceProxy LogService
        {
            get
            {
                if (_logServiceProxy == null)
                {
                    lock (Instance._syncLock)
                    {
                        if (_logServiceProxy == null)
                        {
                            _logServiceProxy = new LogServiceProxy();
                        }
                    }
                }
                return _logServiceProxy;
            }
        }

        private static SchedulerServiceProxy _schedulerServiceProxy;

        /// <summary>
        /// The (thread-safe) proxy to the Scheduler service.
        /// </summary>
        public static SchedulerServiceProxy SchedulerService
        {
            get
            {
                if (_schedulerServiceProxy == null)
                {
                    lock (Instance._syncLock)
                    {
                        if (_schedulerServiceProxy == null)
                        {
                            _schedulerServiceProxy = new SchedulerServiceProxy();
                        }
                    }
                }
                return _schedulerServiceProxy;
            }
        }

        private static void ResetProxies()
        {
            _configurationServiceProxy = null;
            _controlServiceProxy = null;
            _coreServiceProxy = null;
            _guideServiceProxy = null;
            _logServiceProxy = null;
            _schedulerServiceProxy = null;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize the factories for the given server settings.
        /// </summary>
        /// <param name="serverSettings">Where to locate the ARGUS TV Scheduler service.</param>
        /// <param name="throwError">If set to true an exception may be thrown, if false any errors will be swallowed.</param>
        /// <returns>If throwError was false, a boolean indicating success or failure.</returns>
        public static bool Initialize(ServerSettings serverSettings, bool throwError)
        {
            return Proxies.Instance.InternalInitialize(serverSettings, throwError);
        }

        private bool _isInitialized;

        /// <summary>
        /// Will be true if the channel factories have been succesfully initialized.
        /// </summary>
        public static bool IsInitialized
        {
            get { return Proxies.Instance._isInitialized; }
        }

        private ServerSettings _serverSettings;

        /// <summary>
        /// The current settings used to locate the ARGUS TV Scheduler service.
        /// </summary>
        public static ServerSettings ServerSettings
        {
            get { return Proxies.Instance._serverSettings; }
        }

        private bool InternalInitialize(ServerSettings serverSettings, bool throwError)
        {
            lock (_syncLock)
            {
                ServerSettings previousServerSettings = _serverSettings;
                bool previousIsInitialized = _isInitialized;

                try
                {
                    _isInitialized = false;
                    _serverSettings = serverSettings;
                    DateTime firstTryTime = DateTime.Now;
                    for (; ; )
                    {
                        try
                        {
                            PingAndCheckServer(serverSettings);
                            break;
                        }
                        catch
                        {
                            TimeSpan span = DateTime.Now - firstTryTime;
                            if (!serverSettings.WakeOnLan.Enabled
                                || span.TotalSeconds >= serverSettings.WakeOnLan.TimeoutSeconds)
                            {
                                throw;
                            }
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                }
                catch
                {
                    _serverSettings = previousServerSettings;
                    _isInitialized = previousIsInitialized;
                    if (!throwError)
                    {
                        return false;
                    }
                    throw;
                }

                ResetProxies();
                _isInitialized = true;
                return true;
            }
        }

        private void PingAndCheckServer(ServerSettings serverSettings)
        {
            var proxy = new CoreServiceProxy();

            int apiComparison;
            try
            {
                apiComparison = proxy.Ping(Constants.RestApiVersion);
            }
            catch (ArgusTVException ex)
            {
                throw new ArgusTVNotFoundException(ex, "'{0}:{1}' not found, is the service running?", serverSettings.ServerName, serverSettings.Port);
            }
            if (apiComparison < 0)
            {
                throw new ArgusTVException("ARGUS TV Recorder on server is more recent, upgrade client");
            }
            else if (apiComparison > 0)
            {
                throw new ArgusTVException("ARGUS TV Recorder on server is too old, upgrade server");
            }
            serverSettings.WakeOnLan.MacAddresses = String.Join(";", proxy.GetMacAddresses());
            serverSettings.WakeOnLan.IPAddress = WakeOnLan.GetIPAddress(serverSettings.ServerName);
        }

        #endregion
    }
}
