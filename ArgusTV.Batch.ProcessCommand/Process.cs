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
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Diagnostics;

using ArgusTV.ServiceAgents;
using ArgusTV.DataContracts;

namespace ArgusTV.Batch.ProcessCommand
{
    public class Process
    {
        private const string _moduleName = "ProcessCommand";

        #region Sample Commandargs
        // Help
        // MyVideo \\tcf\recordings\Man bijt hond\Man bijt hond_één_2008-09-24_23-47.ts
        // Log arg1 arg2 arg3
        #endregion
        static void Main(string[] args)
        {
            try
            {
                if (args.Length > 0 && String.Equals(args[0], "Help", StringComparison.InvariantCultureIgnoreCase))
                {
                    ShowHelp();
                }                
                else if (args.Length >= 2 && String.Equals(args[0], "MyVideo", StringComparison.InvariantCultureIgnoreCase))
                {
                    InitializeServiceChannelFactories();
                    if (ServiceChannelFactories.IsInitialized)
                    {
                        using (ControlServiceAgent controlServiceAgent = new ControlServiceAgent())
                        {
                            string recordedFileName = args[1];

                            Recording recording = controlServiceAgent.GetRecordingByFileName(recordedFileName);
                            if (recording != null)
                            {
                                Tags tags = new Tags();
                                tags.SimpleTags.Add(new SimpleTag("TITLE", recording.Title));
                                tags.SimpleTags.Add(new SimpleTag("COMMENT", recording.Description));
                                tags.SimpleTags.Add(new SimpleTag("GENRE", recording.Category));
                                tags.SimpleTags.Add(new SimpleTag("CHANNEL_NAME", recording.ChannelDisplayName));

                                string myVideoFileName = Path.ChangeExtension(recordedFileName, "xml");
                                FileStream outFile = new FileStream(myVideoFileName, FileMode.Create, FileAccess.Write, FileShare.None);
                                try
                                {
                                    XmlSerializerNamespaces nspaces = new XmlSerializerNamespaces();
                                    nspaces.Add(String.Empty, String.Empty);

                                    XmlSerializer serializer = new XmlSerializer(typeof(Tags));
                                    serializer.Serialize(outFile, tags, nspaces);
                                }
                                catch (SerializationException ex)
                                {
                                    Debug.WriteLine("Failed to serialize. Reason: " + ex.Message, "Error");
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(ex.Message);
                                }
                                finally
                                {
                                    outFile.Close();
                                }
                            }
                        }
                    }
                }
                else if (args.Length > 0 && String.Equals(args[0], "Log", StringComparison.InvariantCultureIgnoreCase))
                {
                    InitializeServiceChannelFactories();
                    if (ServiceChannelFactories.IsInitialized)
                    {
                        using (LogServiceAgent logServiceAgent = new LogServiceAgent())
                        {
                            StringBuilder message = new StringBuilder(String.Format("ProcessCommandTester called with {0} args :", args.Length));
                            foreach (string arg in args)
                            {
                                message.AppendFormat(" {0},", arg);
                            }
                            logServiceAgent.LogMessage(_moduleName, LogSeverity.Information, message.ToString(0, message.Length - 1));
                        }
                    }
                }
            }
			catch {}
        }

        private static void InitializeServiceChannelFactories()
		{
			ServerSettings serverSettings = new ServerSettings();
			serverSettings.ServerName = Properties.Settings.Default.ArgusTVServerName;
			serverSettings.Port = Properties.Settings.Default.ArgusTVPort;
            ServiceChannelFactories.Initialize(serverSettings, false);
		}

        private static void ShowHelp()
        {
            Console.WriteLine("ArgusTV.Batch.ProcessCommand help.");
            Console.WriteLine("Possible arguments :" + Environment.NewLine);
            Console.WriteLine("Help                 - Shows this info");
            Console.WriteLine("MyVideo %%FILE%%     - Generates MyVideo xmlFile");
            Console.WriteLine("Log                  - Logs in ARGUS TV, with all extra args supplied" + Environment.NewLine);
            Console.ReadLine();
        }
    }
}
