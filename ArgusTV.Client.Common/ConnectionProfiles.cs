using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ArgusTV.Client.Common
{
    [Serializable]
    [Obfuscation(Exclude = true)]
    public sealed class ConnectionProfiles
    {
        private static XmlSerializer _xmlSerializer;

        private SortedList<string, ConnectionProfile> _profiles = new SortedList<string, ConnectionProfile>();

        static ConnectionProfiles()
        {
            _xmlSerializer = new XmlSerializer(typeof(ConnectionProfileList));
        }

        [XmlRoot("profiles")]
        public class ConnectionProfileList : List<ConnectionProfile>
        {
            public ConnectionProfileList()
            {
            }

            public ConnectionProfileList(IList<ConnectionProfile> profiles)
                : base(profiles)
            {
            }
        }

        private ConnectionProfiles()
        {
            LoadProfiles();
        }

        #region Singleton

        private static ConnectionProfiles Instance
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

            internal static readonly ConnectionProfiles instance = new ConnectionProfiles();
        }

        #endregion

        public static void Add(ConnectionProfile profile)
        {
            Instance.InternalAdd(profile);
        }

        public static void Remove(string name)
        {
            Instance.InternalRemove(name);
        }

        public static IList<ConnectionProfile> GetList()
        {
            return Instance.InternalGetList();
        }

        public static void Save()
        {
            Instance.SaveProfiles();
        }

        #region Serialization

        private string SettingsFileName
        {
            get
            {
                return Path.Combine(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ARGUS TV"),
                    "ConnectionProfiles.config");
            }
        }

        private void LoadProfiles()
        {
            string settingsFileName = this.SettingsFileName;

            try
            {
                if (File.Exists(settingsFileName))
                {
                    using (StreamReader reader = new StreamReader(settingsFileName, Encoding.UTF8))
                    {
                        _profiles.Clear();
                        ConnectionProfileList profilesList = (ConnectionProfileList)_xmlSerializer.Deserialize(reader);
                        DecryptPasswords(profilesList);
                        foreach (ConnectionProfile profile in profilesList)
                        {
                            _profiles[profile.Name] = profile;
                        }
                    }
                }
            }
            catch
            {
                _profiles.Clear();
            }
        }

        private void SaveProfiles()
        {
            string settingsFileName = this.SettingsFileName;
            string settingsDirName = Path.GetDirectoryName(settingsFileName);
            if (!Directory.Exists(settingsDirName))
            {
                Directory.CreateDirectory(settingsDirName);
            }
            using (StreamWriter writer = new StreamWriter(this.SettingsFileName, false, Encoding.UTF8))
            {
                IList<ConnectionProfile> profiles = InternalGetList();
                Dictionary<string, string> storedPasswords = EncryptPasswords(profiles);
                _xmlSerializer.Serialize(writer, profiles);
                RestorePasswords(profiles, storedPasswords);
            }
        }

        #endregion

        #region Private Methods

        private void InternalAdd(ConnectionProfile profile)
        {
            _profiles[profile.Name] = profile;
            SaveProfiles();
        }

        private void InternalRemove(string name)
        {
            if (_profiles.ContainsKey(name))
            {
                _profiles.Remove(name);
                SaveProfiles();
            }
        }

        private IList<ConnectionProfile> InternalGetList()
        {
            return new ConnectionProfileList(_profiles.Values);
        }

        private static Dictionary<string, string> EncryptPasswords(IList<ConnectionProfile> profiles)
        {
            Dictionary<string, string> storedPasswords = new Dictionary<string, string>();
            foreach (ConnectionProfile profile in profiles)
            {
                storedPasswords[profile.Name] = profile.ServerSettings.Password;
                if (!String.IsNullOrEmpty(profile.ServerSettings.Password))
                {
                    if (profile.SavePassword)
                    {
                        profile.ServerSettings.Password = ClientUtility.Encrypt(profile.ServerSettings.Password);
                    }
                    else
                    {
                        profile.ServerSettings.Password = null;
                    }
                }
            }
            return storedPasswords;
        }

        private static void RestorePasswords(IList<ConnectionProfile> profiles, Dictionary<string, string> storedPasswords)
        {
            foreach (ConnectionProfile profile in profiles)
            {
                profile.ServerSettings.Password = storedPasswords[profile.Name];
            }
        }

        private static void DecryptPasswords(IList<ConnectionProfile> profiles)
        {
            foreach (ConnectionProfile profile in profiles)
            {
                if (profile.SavePassword
                    && !String.IsNullOrEmpty(profile.ServerSettings.Password))
                {
                    profile.ServerSettings.Password = ClientUtility.Decrypt(profile.ServerSettings.Password);
                }
            }
        }

        #endregion
    }
}
