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

using ArgusTV.DataContracts;
using ArgusTV.ServiceAgents;

namespace ArgusTV.UI.Process
{
    public class RecorderTunersCache
    {
        private Dictionary<Guid, PluginService> _pluginServices = new Dictionary<Guid, PluginService>();

        private RecorderTunersCache()
        {
        }

        #region Singleton

        private static RecorderTunersCache Instance
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

            internal static readonly RecorderTunersCache instance = new RecorderTunersCache();
        }

        #endregion

        public static PluginService GetRecorderTunerById(Guid recorderTunerId)
        {
            return Instance.InternalGetRecorderTunerById(recorderTunerId);
        }

        private PluginService InternalGetRecorderTunerById(Guid recorderTunerId)
        {
            lock (_pluginServices)
            {
                if (!_pluginServices.ContainsKey(recorderTunerId))
                {
                    try
                    {
                        using (ControlServiceAgent tvControlAgent = new ControlServiceAgent())
                        {
                            _pluginServices.Clear();
                            PluginService[] pluginServices = tvControlAgent.GetAllPluginServices(false);
                            foreach (PluginService pluginService in pluginServices)
                            {
                                _pluginServices.Add(pluginService.PluginServiceId, pluginService);
                            }
                        }
                    }
                    catch { }
                }
                if (_pluginServices.ContainsKey(recorderTunerId))
                {
                    return _pluginServices[recorderTunerId];
                }
                return null;
            }
        }
    }
}
