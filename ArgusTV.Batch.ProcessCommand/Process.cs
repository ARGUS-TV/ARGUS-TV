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
using System.Management;

using ArgusTV.ServiceAgents;
using ArgusTV.ServiceContracts;
using ArgusTV.DataContracts;

namespace ArgusTV.Batch.ProcessCommand
{
    public class Process
    {
        private const string _moduleName = "ProcessCommand";

        static int Main(string[] args)
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
                    Log(LogSeverity.Information, "Creating MP xml");
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
                            else
                            {
                                Log(LogSeverity.Error, string.Format("Failed to find recording \"{0}\"", recordedFileName));
                            }
                        }
                    }
                }
                else if (args.Length == 2 && String.Equals(args[0], "Log", StringComparison.InvariantCultureIgnoreCase))
                {
                    Log(LogSeverity.Information, args[1]);
                }
                else if (args.Length >= 3 && String.Equals(args[0], "Delete", StringComparison.InvariantCultureIgnoreCase))
                {
                    InitializeServiceChannelFactories();
                    Log(LogSeverity.Information, string.Format("Deleting {0} from ARGUS TV {1} the actual recording", args[1], (args[2] == "1" ? "and" : "but not")));
                    if (ServiceChannelFactories.IsInitialized)
                    {
                        using (ControlServiceAgent controlServiceAgent = new ControlServiceAgent())
                        {
                            string recordedFileName = args[1];
                            Boolean keepGile = (args[2] == "1" ? true : false);
                            controlServiceAgent.DeleteRecording(recordedFileName, keepGile);

                        }
                    }
                }
                else if (args.Length == 4 && String.Equals(args[0], "description", StringComparison.InvariantCultureIgnoreCase))
                {
                    InitializeServiceChannelFactories();
                    string recordedFileName = args[1];
                    string outputFileName = Path.ChangeExtension(args[1], args[2]);
                    string fileString = args[3].ToLower();
                    Log(LogSeverity.Information, string.Format("Creating description file for {0} called {1}", args[1], outputFileName));
                    if (ServiceChannelFactories.IsInitialized)
                    {
                        using (ControlServiceAgent controlServiceAgent = new ControlServiceAgent())
                        {
                            Recording tvRecording = controlServiceAgent.GetRecordingByFileName(recordedFileName);
                            fileString = fileString.Replace("{description}", tvRecording.Description);
                            fileString = fileString.Replace("{starttime}", tvRecording.StartTime.ToString());
                            fileString = fileString.Replace("{stoptime}", tvRecording.StopTime.ToString());
                            fileString = fileString.Replace("{title}", tvRecording.Title.ToString());
                            fileString = fileString.Replace("{episodenumber}", tvRecording.EpisodeNumber.ToString());
                            fileString = fileString.Replace("{seriesnumber}", tvRecording.SeriesNumber.ToString());
                            using (StreamWriter descriptionfile = new StreamWriter(outputFileName, false, Encoding.UTF8))
                            {
                                descriptionfile.WriteLine(fileString);
                            }
                        }
                    }
                }
                else if (args.Length >= 3 && String.Equals(args[0], "Rename", StringComparison.InvariantCultureIgnoreCase))
                {
                    InitializeServiceChannelFactories();
                    Log(LogSeverity.Information, string.Format("Renaming {0} to {1}", args[1], args[2]));
                    if (ServiceChannelFactories.IsInitialized)
                    {
                        using (ControlServiceAgent controlServiceAgent = new ControlServiceAgent())
                        {
                            string recordedFileName = args[1];
                            string newFileName = args[2];
                            Recording recording;

                            using (ControlServiceAgent controlAgent = new ControlServiceAgent())
                            {
                                recording = controlAgent.GetRecordingByFileName(newFileName);
                            }

                            if (recording == null)
                            {
                                string lUncPath;
                                if (newFileName.StartsWith("\\"))
                                    lUncPath = newFileName;
                                else
                                {
                                    try
                                    {
                                        lUncPath = GetUncPath(newFileName);
                                    }
                                    catch (IOException)
                                    {
                                        Console.Error.WriteLine("Could not get a UNC path for recording");
                                        Log(LogSeverity.Warning, string.Format("Could not convert {0} to an unc path", newFileName));
                                        LogArgs(LogSeverity.Information, args);
                                        return -2;
                                    }
                                }
                                controlServiceAgent.ChangeRecordingFile(recordedFileName, lUncPath, null, null);
                            }
                            else
                            {
                                Console.Error.WriteLine("Could not move recording as another one with same name allready exsist");
                                Log(LogSeverity.Warning, string.Format("Could not rename as a recording with the name {0} allready exist in database", newFileName));
                                LogArgs(LogSeverity.Information, args);
                                return -2;
                            }
                        }
                    }
                }
                else if (args.Length >= 2 && String.Equals(args[0], "IsPartial", StringComparison.InvariantCultureIgnoreCase))
                {
                    InitializeServiceChannelFactories();
                    if (ServiceChannelFactories.IsInitialized)
                    {
                        using (ControlServiceAgent controlServiceAgent = new ControlServiceAgent())
                        {
                            string recordedFileName = args[1];
                            Recording recording;

                            using (ControlServiceAgent controlAgent = new ControlServiceAgent())
                            {
                                recording = controlAgent.GetRecordingByFileName(recordedFileName);
                            }

                            if (recording != null)
                                return (recording.IsPartialRecording ? -1 : 0);
                            else
                            {
                                Console.Error.WriteLine("Could not check recording as it could not be found in the database");
                                Log(LogSeverity.Warning, string.Format("{0} not found in the ARGUS TV database", args[1]));
                                LogArgs(LogSeverity.Information, args);
                                return -2;
                            }
                        }
                    }
                    else
                    {
                        return -2;
                    }
                }
                else if (args.Length >= 2 && String.Equals(args[0], "Exist", StringComparison.InvariantCultureIgnoreCase))
                {
                    InitializeServiceChannelFactories();
                    if (ServiceChannelFactories.IsInitialized)
                    {
                        using (ControlServiceAgent controlServiceAgent = new ControlServiceAgent())
                        {
                            string recordedFileName = args[1];
                            Recording recording;

                            using (ControlServiceAgent controlAgent = new ControlServiceAgent())
                            {
                                recording = controlAgent.GetRecordingByFileName(recordedFileName);
                            }

                            if (recording != null)
                                return 0;
                            else
                                return -1;
                        }
                    }
                    else
                    {
                        return -2;
                    }
                }
                else if (args.Length >= 1 && String.Equals(args[0], "Wake", StringComparison.InvariantCultureIgnoreCase))
                {

                }
                else
                {
                    Log(LogSeverity.Warning, "Batch processor did not get valid command(s)");
                    LogArgs(LogSeverity.Information, args);
                }
            }
            catch
            {
                return -2;
            }
            if (ServiceChannelFactories.IsInitialized)
                return 0;
            else
                return -3;
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
            Console.WriteLine("Help                       - Shows this info");
            Console.WriteLine("MyVideo %%FILE%%           - Generates MyVideo xmlFile");
            Console.WriteLine("Log                        - Logs in ARGUS TV, with all extra args supplied" + Environment.NewLine);
            Console.WriteLine("Delete %%FILE%% DelFile    - Deletes the ARGUS TV entry of the recording, Del recording, 1 for delete the recorded file too, 0 for just the DB reference.");
            Console.WriteLine("Rename %%FILE%% NewFile    - Renames the file in the database. Usefull for transcoding, source filename has to be passed UNC format");
            Console.WriteLine("IsPartial %%FILE%%         - Will set %ERRORLEVEL% to 0 if recording is not partial, and -1 if it is partial");
            Console.WriteLine("Description %%FILE%% Extention whattooutput     - Creates a description file");
            Console.WriteLine("Exist %%FILE%%             - returns 0 if file is under ARGUS TV control, else -1");
            //Console.WriteLine("Wake %%FILE%%              - wake the mediacenter");
            Console.ReadLine();
        }

        private static string GetUncPath(string path)
        {
            string uncPath = string.Empty;
            using (ManagementClass managementClass = new ManagementClass("Win32_Share"))
            {
                foreach (ManagementObject item in managementClass.GetInstances())
                {
                    if (item.Properties["Path"].Value.ToString().Trim() == "" | item.Properties["Name"].Value.ToString().EndsWith("$") == true)
                        continue;
                    if (path.StartsWith(item.Properties["Path"].Value.ToString(), StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        uncPath = @"\\" + Environment.MachineName + @"\";
                        uncPath += item.Properties["Name"].Value;
                        uncPath += path.Substring(item.Properties["Path"].Value.ToString().Length);
                        break;
                    }

                }
            }
            if (string.IsNullOrEmpty(uncPath))
            {
                throw new IOException("No unc path found for path");
            }
            return uncPath;
        }

        private static void Log(LogSeverity severity, string message)
        {
            if (!ServiceChannelFactories.IsInitialized)
                InitializeServiceChannelFactories();

            if (ServiceChannelFactories.IsInitialized)
            {
                using (LogServiceAgent logServiceAgent = new LogServiceAgent())
                {
                    logServiceAgent.LogMessage(_moduleName, severity, message);
                }
            }
        }

        private static void LogArgs(LogSeverity severity, string[] args)
        {
            if (!ServiceChannelFactories.IsInitialized)
                InitializeServiceChannelFactories();

            if (ServiceChannelFactories.IsInitialized)
            {
                using (LogServiceAgent logServiceAgent = new LogServiceAgent())
                {
                    StringBuilder message = new StringBuilder(String.Format("BatchProcessor called with {0} args :", args.Length));
                    foreach (string arg in args)
                    {
                        message.AppendFormat(" {0},", arg);
                    }
                    logServiceAgent.LogMessage(_moduleName, severity, message.ToString(0, message.Length - 1));
                }
            }
        }
    }
}