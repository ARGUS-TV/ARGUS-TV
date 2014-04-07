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
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.IO;

using ArgusTV.Common.Logging;
using ArgusTV.ServiceProxy;
using ArgusTV.DataContracts;
using ArgusTV.GuideImporter.Interfaces;


namespace ArgusTV.GuideImporter
{
    static class Program
    {
        // Arguments
        private const string _argMode = "mode";
        private const string _argPlugin = "plugin";
        // Predefined argument values
        private const string _argModeValueQuiet = "quiet";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Logger.SetLogFilePath("GuideImporter.log", Properties.Settings.Default.LogLevel);
            Logger.Write("Guide importer started.");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Dictionary<string, string> parsedArguments = ParseArguments(args);
            bool runsQuiet = NeedsToRunQuiet(parsedArguments);

            string initErrorMessage;
            InitializeArgusTVServiceChannelFactories(out initErrorMessage);
            if (ProxyFactory.IsInitialized)
            {
                try
                {
                    Hoster hoster = new Hoster();
                    if (runsQuiet)
                    {
                        string pluginToUse = GetSelectedPlugin(parsedArguments);
                        if (String.IsNullOrEmpty(pluginToUse))
                        {
                            Logger.Write(TraceEventType.Error, "Invalid number of arguments supplied!");
                            Hoster.LogMessageInArgusTV("Invalid number of arguments supplied!", LogSeverity.Information);
                        }
                        else
                        {
                            Logger.Write("Importing, using plugin:" + pluginToUse );
                            hoster.Import(pluginToUse);
                        }
                    }
                    else
                    {
                        Application.Run(new MainForm(hoster));
                    }
                }
                catch (Exception e)
                {
                    if (!String.IsNullOrEmpty(e.Message))
                    {
                        Logger.Write("Exception during import: " + e.Message);
                        if (e.InnerException != null && !String.IsNullOrEmpty(e.InnerException.Message))
                            Logger.Write("InnerException: " + e.InnerException.Message);
                    }
                    else
                    {
                        EventLogger.WriteEntry(e);
                    }
                }
            }
            else
            {
                if (!runsQuiet)
                {
                    MessageBox.Show("Could not connect to ARGUS TV:" + Environment.NewLine + Environment.NewLine
                                    + initErrorMessage + Environment.NewLine + Environment.NewLine
                                    + "Check your configuration!", "Guide Importer", MessageBoxButtons.OK);
                }
                else
                {
                    Logger.Write(TraceEventType.Error, "Could not connect to ARGUS TV: {0} - check your configuration!", initErrorMessage);
                    //EventLogger.WriteEntry("Could not initialize ARGUS TV serviceChannelFactories, check your configuration !", EventLogEntryType.Error);
                }
            }
        }

        #region Helpers

        private static void InitializeArgusTVServiceChannelFactories(out string errorMessage)
        {
            try
            {
                errorMessage = null;
                ServerSettings serverSettings = new ServerSettings();
                serverSettings.ServerName = Properties.Settings.Default.ArgusTVServerName;
                serverSettings.Transport = ServiceTransport.Http;
                serverSettings.Port = Properties.Settings.Default.ArgusTVPort;
                ProxyFactory.Initialize(serverSettings, true);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        private static Dictionary<string, string> ParseArguments(string[] arguments)
        {
            Dictionary<string, string> parsedArguments = new Dictionary<string, string>();
            foreach (string argument in arguments)
            {
                string[] splitedArguments = SplitArgument(argument);

                if (splitedArguments != null )
                {
                    parsedArguments.Add(splitedArguments[0], splitedArguments[1]);
                }
            }
            return parsedArguments;        
        }

        private static string[] SplitArgument(string argument)
        { 
            char[] separators = {':','='};
            string[] splitedArguments = null;

            foreach(char separator in separators)
            {
                string[] parts = argument.Split(separator);
                if (parts.Length == 2 && parts[0].Length > 0 && parts[1].Length > 1)
                {
                    splitedArguments = parts;
                    break;
                }
            }
            return splitedArguments;
        }

        private static bool NeedsToRunQuiet(Dictionary<string, string> arguments)
        {
            bool needsToRunQuiet = false;

            if (arguments.ContainsKey(_argMode) && arguments[_argMode].Equals(_argModeValueQuiet, StringComparison.InvariantCultureIgnoreCase))
            {
                needsToRunQuiet = true;
            }
            return needsToRunQuiet;
        }

        private static string GetSelectedPlugin(Dictionary<string, string> arguments)
        {
            if (arguments.ContainsKey(_argPlugin))
            {
                return arguments[_argPlugin];
            }
            return String.Empty;
        }
        #endregion
    }
}
