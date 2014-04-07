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
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

using ArgusTV.Client.Common;
using ArgusTV.ServiceProxy;

namespace ArgusTV.UI.Notifier
{
    [Obfuscation(Exclude=true)]
    public class Config
    {
        private const string _configFileName = "ArgusTVNotifierConfig.xml";

        public Config()
        {
            ServerName = "localhost";
            Port = ServerSettings.DefaultHttpsPort;
            MmcPath = @"..\Scheduler Console\ArgusTV.Scheduler.Console.exe";
            ShowRecordingBalloons = true;
            BalloonTimeoutSeconds = 5;
        }

        #region Properties

        public bool ShowRecordingBalloons { get; set; }

        public int BalloonTimeoutSeconds { get; set; }

        public string MmcPath { get; set; }

        public string ServerName { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        private string _password;

        [XmlIgnore]
        public string Password
        {
            get { return ClientUtility.Decrypt(_password); }
            set { _password = ClientUtility.Encrypt(value); }
        }

        [XmlElement(ElementName = "Password")]
        public string EncryptedPassword
        {
            get { return _password; }
            set { _password = value; }
        }

        public string MacAddresses { get; set; }

        public string IpAddress { get; set; }

        #endregion

        #region Static Methods

        private static Config _current;

        public static Config Current
        {
            get { return _current; }
        }

        public static void Save()
        {
            string configPath = GetConfigPath();

            using (StreamWriter writer = new StreamWriter(configPath, false))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Config));
                serializer.Serialize(writer, Config.Current);
            }
        }

        public static void Load()
        {
            string configPath = GetConfigPath();

            if (File.Exists(configPath))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(configPath))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(Config));
                        _current = (Config)serializer.Deserialize(reader);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Invalid config file." + Environment.NewLine + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _current = new Config();
                }
            }
            else
            {
                _current = new Config();
            }
        }

        private static string GetConfigPath()
        {
            string configDirName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ARGUS TV");
            if (!Directory.Exists(configDirName))
            {
                Directory.CreateDirectory(configDirName);
            }

            string configFileName = Path.Combine(configDirName, _configFileName);

            // Fix the previous wrong location:
            string configFileNameInWrongPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _configFileName);
            if (File.Exists(configFileNameInWrongPath))
            {
                if(File.Exists(configFileName))
                    File.Delete(configFileNameInWrongPath);
                else
                    File.Move(configFileNameInWrongPath, configFileName);
            }
            return configFileName;
        }

        #endregion
    }
}
