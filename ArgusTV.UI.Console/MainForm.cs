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
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

using ArgusTV.UI.Console.Panels;
using ArgusTV.DataContracts;
using ArgusTV.ServiceProxy;
using ArgusTV.Client.Common;
using ArgusTV.WinForms;

namespace ArgusTV.UI.Console
{
    public partial class MainForm : Form
    {
        private ConnectionProfile _connectionProfile;
        private string _baseFormTitle;
        private string _formTitle;

        public System.Collections.ObjectModel.ReadOnlyCollection<string> CommandLineArgs { set; get; }

        private List<PluginServiceSetting> _pluginServiceSettings = new List<PluginServiceSetting>();

        internal List<PluginServiceSetting> PluginServiceSettings
        {
            get { return _pluginServiceSettings; }
        }

        private Dictionary<string, object> _session = new Dictionary<string,object>();

        public Dictionary<string, object> Session
        {
            get { return _session; }
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (Properties.Settings.Default.ConsoleWindowPosition != Point.Empty
                    || Properties.Settings.Default.ConsoleWindowSize != Size.Empty)
                {
                    Point consolePosition = Properties.Settings.Default.ConsoleWindowPosition;
                    Size consoleWindowSize = Properties.Settings.Default.ConsoleWindowSize;

                    // Leave defaults, when screenresolution was changed to a lower resolution
                    if (consolePosition.X < Screen.PrimaryScreen.Bounds.Width - 50 &&
                        consolePosition.Y < Screen.PrimaryScreen.Bounds.Height - 50 &&
                        consoleWindowSize.Height < Screen.PrimaryScreen.Bounds.Size.Height &&
                        consoleWindowSize.Width < Screen.PrimaryScreen.Bounds.Size.Width)
                    {
                        this.Location = consolePosition;
                        this.Size = consoleWindowSize;
                    }
                }

                SetMenuMode(MainMenuMode.Normal);

                // Add some default plugin services.
                // Could put these in a config file later on perhaps...?
                _pluginServiceSettings.Add(new PluginServiceSetting("ARGUS", "ARGUS TV Recorder", 49953));
                _pluginServiceSettings.Add(new PluginServiceSetting("TVE", "MediaPortal TV Server", 49842));
#if DEBUG
                _pluginServiceSettings.Add(new PluginServiceSetting("Test", "Test", 49840));
#endif
                _pluginServiceSettings.Add(new PluginServiceSetting("Other", "Other", 0));

                _baseFormTitle = this.Text + " " + Constants.ProductVersion;
                _formTitle = _baseFormTitle;
                this.Text = _baseFormTitle;

                _connectionProfile = null;
                if (!ConnectToArgusTVService(this.CommandLineArgs.Count == 1 ? this.CommandLineArgs[0] : null, false))
                {
                    this.Close();
                }
                else
                {
                    if (DateTime.Now >= Properties.Settings.Default.NextVersionCheck)
                    {
                        NewVersionInfo versionInfo = Proxies.CoreService.IsNewerVersionAvailable();
                        if (versionInfo != null)
                        {
                            Program.App.HideSplash();
                            if (!this.IsDisposed)
                            {
                                if (MessageBox.Show(this, versionInfo.Name + " is available for download." + Environment.NewLine + Environment.NewLine
                                    + "Would you like to see what's new?", "New version", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                {
                                    System.Diagnostics.Process.Start(new ProcessStartInfo(versionInfo.Url));
                                }
                                Properties.Settings.Default.NextVersionCheck = DateTime.Now.AddDays(7);
                                Properties.Settings.Default.Save();
                            }
                        }
                    }
                }

                _tvGuideLinkLabel_LinkClicked(this, null);
            }
            catch (Exception ex)
            {
                Program.App.HideSplash();
                MessageBox.Show(null, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            finally
            {
                Program.App.HideSplash();
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.TopMost = true;
            this.TopMost = false;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.Visible
                && this.WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.ConsoleWindowPosition = this.Location;
                Properties.Settings.Default.ConsoleWindowSize = this.Size;
                Properties.Settings.Default.Save();
            }
        }

        public bool ConnectToArgusTVService(string currentProfileName, bool reconnect)
        {
            bool result = false;
            bool abortConnection = false;
            bool tryConnectionWithoutUI = !reconnect;
            bool skipSelectionForm = false;

            IList<ConnectionProfile> profiles = ConnectionProfiles.GetList();
            if (!reconnect
                && !String.IsNullOrEmpty(currentProfileName))
            {
                _connectionProfile = FindConnectionProfileByName(profiles, currentProfileName);
                skipSelectionForm = (_connectionProfile != null);
            }

            if (_connectionProfile != null)
            {
                currentProfileName = _connectionProfile.Name;
            }
            else
            {
                currentProfileName = Properties.Settings.Default.LastUsedConnectionProfile;
            }

            if (profiles.Count == 1)
            {
                _connectionProfile = profiles[0];
            }
            else if (profiles.Count > 1
                && !skipSelectionForm)
            {
                if (!reconnect)
                {
                    Program.App.HideSplash();
                }
                SelectProfileForm selectProfileForm = new SelectProfileForm();
                selectProfileForm.SetSelectedProfileName(currentProfileName);
                if (selectProfileForm.ShowDialog(this) == DialogResult.OK)
                {
                    _connectionProfile = selectProfileForm.SelectedProfile;
                    if (_connectionProfile != null)
                    {
                        tryConnectionWithoutUI = !selectProfileForm.EditSelectedProfile;
                        if (tryConnectionWithoutUI
                            && !reconnect)
                        {
                            Program.App.ShowSplash();
                        }
                    }
                }
                else
                {
                    result = false;
                    abortConnection = true;
                }
            }

            if (!abortConnection)
            {
                if (tryConnectionWithoutUI
                    && _connectionProfile != null)
                {
                    bool saveProfiles = false;

                    if (_connectionProfile.ServerSettings.Transport == ServiceProxy.ServiceTransport.Https
                        && String.IsNullOrEmpty(_connectionProfile.ServerSettings.Password))
                    {
                        using (LogonForm logonForm = new LogonForm())
                        {
                            logonForm.UserName = _connectionProfile.ServerSettings.UserName;
                            Program.App.HideSplash();
                            if (DialogResult.OK == logonForm.ShowDialog(this))
                            {
                                _connectionProfile.ServerSettings.UserName = logonForm.UserName;
                                _connectionProfile.ServerSettings.Password = logonForm.Password;
                                _connectionProfile.SavePassword = logonForm.SavePassword;
                                saveProfiles = _connectionProfile.SavePassword;
                            }
                        }
                    }

                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;

                        string macAddresses = _connectionProfile.ServerSettings.WakeOnLan.MacAddresses;
                        string ipAddress = _connectionProfile.ServerSettings.WakeOnLan.IPAddress;

                        if (!Proxies.Initialize(_connectionProfile.ServerSettings, false))
                        {
                            Program.App.HideSplash();
                            result = ConnectWithForm();
                        }
                        else
                        {
                            if (saveProfiles
                                || !String.Equals(macAddresses, _connectionProfile.ServerSettings.WakeOnLan.MacAddresses)
                                || !String.Equals(ipAddress, _connectionProfile.ServerSettings.WakeOnLan.IPAddress))
                            {
                                ConnectionProfiles.Save();
                            }
                            RefreshFormTitle();
                            result = true;
                        }
                    }
                    finally
                    {
                        Cursor.Current = Cursors.Default;
                    }
                }
                else
                {
                    if (!reconnect)
                    {
                        Program.App.HideSplash();
                    }
                    result = ConnectWithForm();
                }
            }

            if (result
                && Properties.Settings.Default.LastUsedConnectionProfile != _connectionProfile.Name)
            {
                Properties.Settings.Default.LastUsedConnectionProfile = _connectionProfile.Name;
                Properties.Settings.Default.Save();
            }

            return result;
        }

        private ConnectionProfile FindConnectionProfileByName(IList<ConnectionProfile> profiles, string currentProfileName)
        {
            foreach (ConnectionProfile profile in profiles)
            {
                if (profile.Name.Equals(currentProfileName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return profile;
                }
            }
            return null;
        }

        private bool ConnectWithForm()
        {
            ConnectForm form = new ConnectForm();
            if (_connectionProfile == null)
            {
                form.ServerSettings = new ServerSettings();
            }
            else
            {
                form.ProfileName = _connectionProfile.Name;
                form.ServerSettings = new ServerSettings()
                {
                    ServerName = _connectionProfile.ServerSettings.ServerName,
                    Port = ServerSettings.DefaultHttpPort,
                    Transport = ServiceTransport.Http
                };
            }
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                _connectionProfile = new ConnectionProfile()
                {
                    ServerSettings = form.ServerSettings,
                    SavePassword = form.SavePassword
                };
                if (!String.IsNullOrEmpty(form.ProfileName))
                {
                    _connectionProfile.Name = form.ProfileName;
                    ConnectionProfiles.Add(_connectionProfile);
                }
                RefreshFormTitle();
                return true;
            }
            return false;
        }

        private void RefreshFormTitle()
        {
            string server = _connectionProfile.Name;
            if (String.IsNullOrEmpty(server))
            {
                server = _connectionProfile.ServerSettings.ServerName;
            }
            _formTitle = _baseFormTitle + " [" + server + "]";
            this.Text = _formTitle;
        }

        public bool IsHttpConnection
        {
            get { return _connectionProfile.ServerSettings.Transport == ServiceProxy.ServiceTransport.Http; }
        }

        public void SetMenuMode(MainMenuMode mode)
        {
            _menuFlowLayoutPanel.Visible = (mode == MainMenuMode.Normal);
            _saveCancelFlowLayoutPanel.Visible = (mode == MainMenuMode.SaveCancel);
        }

        #region Navigation

        public ContentPanel ActiveContentPanel
        {
            get
            {
                foreach (ContentPanel contentPanel in _contentContainer.Controls)
                {
                    if (contentPanel.Visible)
                    {
                        return contentPanel;
                    }
                }
                return null;
            }
        }

        public void OpenContentPanel(ContentPanel contentPanel)
        {
            foreach (Control control in _contentContainer.Controls)
            {
                control.Visible = false;
            }
            contentPanel.Dock = DockStyle.Fill;
            _contentContainer.Controls.Add(contentPanel);
            this.Text = _formTitle + " - " + contentPanel.Title;
        }

        public void CloseContentPanel(ContentPanel contentPanel, ContentPanel activatePanel)
        {
            this.Text = _formTitle;
            if (activatePanel != null)
            {
                this.Text = _formTitle + " - " + activatePanel.Title;
                activatePanel.Visible = true;
            }
            CloseAndRemovePanel(contentPanel);
        }

        public void CloseAllContentPanels()
        {
            while (_contentContainer.Controls.Count > 0)
            {
                Control control = _contentContainer.Controls[0];
                ContentPanel contentPanel = control as ContentPanel;
                if (contentPanel != null)
                {
                    CloseAndRemovePanel(contentPanel);
                }
                else
                {
                    _contentContainer.Controls.RemoveAt(0);
                    control.Dispose();
                }
            }
        }

        private void CloseAndRemovePanel(ContentPanel contentPanel)
        {
            contentPanel.Visible = false;
            contentPanel.OnClosed();
            _contentContainer.Controls.Remove(contentPanel);
            contentPanel.Dispose();
        }

        #endregion

        #region Events

        private void _saveMenuItemLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ContentPanel activePanel = this.ActiveContentPanel;
            if (activePanel != null)
            {
                activePanel.OnSave();
            }
        }

        private void _cancelMenuItemLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ContentPanel activePanel = this.ActiveContentPanel;
            if (activePanel != null)
            {
                activePanel.OnCancel();
            }
        }

        private void _tvGuideLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CloseAllContentPanels();
            OpenContentPanel(new GuidePanel());
        }

        private void _searchGuideMenuItemLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CloseAllContentPanels();
            OpenContentPanel(new SearchGuidePanel());
        }

        private void _liveTVLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CloseAllContentPanels();
            OpenContentPanel(new LiveTvPanel());
        }

        private void _upcomingRecordingsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CloseAllContentPanels();
            OpenContentPanel(new UpcomingProgramsPanel(ScheduleType.Recording));
        }

        private void _upcomingSuggestionsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CloseAllContentPanels();
            OpenContentPanel(new UpcomingProgramsPanel(ScheduleType.Suggestion));
        }

        private void _upcomingAlertsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CloseAllContentPanels();
            OpenContentPanel(new UpcomingProgramsPanel(ScheduleType.Alert));
        }

        private void _activeRecordingsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CloseAllContentPanels();
            OpenContentPanel(new ActiveRecordingsPanel());
        }

        private void _recordingsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CloseAllContentPanels();
            OpenContentPanel(new SchedulesPanel(ScheduleType.Recording));
        }

        private void _suggestionsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CloseAllContentPanels();
            OpenContentPanel(new SchedulesPanel(ScheduleType.Suggestion));
        }

        private void _alertsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CloseAllContentPanels();
            OpenContentPanel(new SchedulesPanel(ScheduleType.Alert));
        }

        private void _settingsMenuItemLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CloseAllContentPanels();
            OpenContentPanel(new SettingsPanel());
        }

        private void _showRecordingsMenuItemLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CloseAllContentPanels();
            OpenContentPanel(new RecordingsPanel());
        }

        private void _editChannelsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CloseAllContentPanels();
            OpenContentPanel(new ChannelsPanel());
        }

        private void _guideChannelsMenuItemLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CloseAllContentPanels();
            OpenContentPanel(new GuideChannelsPanel());
        }

        private void _channelGroupsMenuItemLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CloseAllContentPanels();
            OpenContentPanel(new ChannelGroupsPanel());
        }

        private void _tunerRecordersLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CloseAllContentPanels();
            OpenContentPanel(new PluginServicesPanel());
        }

        private void _processingCommandsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CloseAllContentPanels();
            OpenContentPanel(new ProcessingCommandsPanel());
        }

        private void _recordingFormatsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CloseAllContentPanels();
            OpenContentPanel(new RecordingFormatsPanel());
        }

        private void _logLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CloseAllContentPanels();
            OpenContentPanel(new LogPanel());
        }
        
        private void _aboutLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CloseAllContentPanels();
            OpenContentPanel(new AboutPanel());
        }
        #endregion
    }
}
