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
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Linq;

using ArgusTV.GuideImporter.Interfaces;

namespace ArgusTV.GuideImporter
{
    internal abstract class PluginLoader<T> where T:class, INamedPlugin
    {
        #region Private Members

        private Dictionary<string, T> _plugins = new Dictionary<string, T>();
        private string _pluginRootPath;
        #endregion

        public PluginLoader(string pluginRootPath)
        {
            _pluginRootPath = pluginRootPath;
            LoadPlugins();
        }

        #region Public Methods

        public T GetPluginByName(string pluginName)
        {
            string foundPluginName = _plugins.Keys.SingleOrDefault(p => p.Equals(pluginName, StringComparison.InvariantCultureIgnoreCase));
            if (!String.IsNullOrEmpty(foundPluginName))
            {
                return _plugins[foundPluginName];
            }
            return null;
        }

        public string[] PluginNames
        {
            get
            {
                List<string> pluginNames = new List<string>(_plugins.Keys);
                return pluginNames.ToArray();
            }
        }

        public static List<T> GetPluginsInFolder(string folder)
        {
            List<T> plugins = new List<T>();

            foreach (string file in Directory.GetFiles(folder, "*.dll"))
            {
                try
                {
                    Assembly assembly = Assembly.LoadFile(file);
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (!type.IsClass || type.IsNotPublic)
                        {
                            continue;
                        }

                        Type[] interfaces = type.GetInterfaces();
                        if (((IList)interfaces).Contains(typeof(T)))
                        {
                            object pluginObject = Activator.CreateInstance(type);
                            try
                            {
                                T plugin = (T)pluginObject;
                                plugin.InstallationPath = folder;
                                plugins.Add(plugin);
                            }
                            catch { }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
            return plugins;
        }
        #endregion

        #region Private Methods

        private void LoadPlugins()
        {
            _plugins.Clear();
            DirectoryInfo dirInfo = new DirectoryInfo(_pluginRootPath);
            if (dirInfo.Exists)
            {
                foreach (DirectoryInfo subDirInfo in dirInfo.GetDirectories())
                {
                    List<T> plugins = GetPluginsInFolder(subDirInfo.FullName);
                    if (plugins.Count > 0)
                    {
                        foreach (T plugin in plugins)
                        {
                            if (!_plugins.ContainsKey(plugin.Name))
                            {
                                _plugins.Add(plugin.Name, plugin);
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
