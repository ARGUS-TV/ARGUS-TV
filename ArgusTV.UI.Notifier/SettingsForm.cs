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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Security;

using ArgusTV.DataContracts;
using ArgusTV.ServiceProxy;

namespace ArgusTV.UI.Notifier
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            Config.Load();
            _serverHttpsTextBox.Text = Config.Current.ServerName;
            _portHttpsNumericUpDown.Value = Config.Current.Port;
            _userNameTextBox.Text = Config.Current.UserName;
            _passwordTextBox.Text = Config.Current.Password;
            _showRecordingBalloonsCheckBox.Checked = Config.Current.ShowRecordingBalloons;
            _balloonTimeoutNumericUpDown.Value = Config.Current.BalloonTimeoutSeconds;
            _mmcPathTextBox.Text = Config.Current.MmcPath;
#if DEBUG
            if (Config.Current.ServerName == "localhost")
            {
                _portHttpsNumericUpDown.Value = Config.Current.Port - 2;
            }
#endif
            EnableButtons();
        }

        private void EnableButtons()
        {
            _balloonSecsLabel.Enabled = _showRecordingBalloonsCheckBox.Checked;
            _balloonTimeoutLabel.Enabled = _showRecordingBalloonsCheckBox.Checked;
            _balloonTimeoutNumericUpDown.Enabled = _showRecordingBalloonsCheckBox.Checked;
        }

        private void _showRecordingBalloonsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }

        private void _browseMmcButton_Click(object sender, EventArgs e)
        {
            if (_openMmcFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                _mmcPathTextBox.Text = _openMmcFileDialog.FileName;
            }
        }

        private void _testConnectionsButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(_serverHttpsTextBox.Text.Trim()))
            {
                MessageBox.Show(this, "No local or remote connection settings entered!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (SetAndConnect(true))
            {
                MessageBox.Show(this, "Connection succeeded!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            if (SetAndConnect(false))
            {
                Config.Current.ShowRecordingBalloons = _showRecordingBalloonsCheckBox.Checked;
                Config.Current.BalloonTimeoutSeconds = (int)_balloonTimeoutNumericUpDown.Value;
                Config.Current.MmcPath = _mmcPathTextBox.Text;
                Config.Save();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private bool SetAndConnect(bool test)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                ServerSettings serverSettings = new ServerSettings();
                serverSettings.ServerName = _serverHttpsTextBox.Text.Trim();
                if (!String.IsNullOrEmpty(serverSettings.ServerName))
                {
                    serverSettings.Port = (int)_portHttpsNumericUpDown.Value;
                    serverSettings.Transport = ServiceTransport.Https;
                    serverSettings.UserName = _userNameTextBox.Text;
                    serverSettings.Password = _passwordTextBox.Text;

#if DEBUG
                    if (serverSettings.ServerName == "localhost")
                    {
                        serverSettings.Port += 2;
                        serverSettings.Transport = ServiceTransport.Http;
                    }
#endif
                    ProxyFactory.Initialize(serverSettings, true);
                    
                    Config.Current.ServerName = serverSettings.ServerName;
                    Config.Current.Port = serverSettings.Port;
                    Config.Current.MacAddresses = serverSettings.WakeOnLan.MacAddresses;
                    Config.Current.IpAddress = serverSettings.WakeOnLan.IPAddress;
                    Config.Current.UserName = serverSettings.UserName;
                    Config.Current.Password = serverSettings.Password;
                }
                else
                {
                    Config.Current.ServerName = String.Empty;
                    Config.Current.Port = ServerSettings.DefaultHttpsPort;
                }
                return true;
            }
            catch (ArgusTVNotFoundException ex)
            {
                if (MessageBox.Show(this, ex.Message, null, test ? MessageBoxButtons.OK : MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    return !test;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Failed to connect to ARGUS TV." + Environment.NewLine + Environment.NewLine + ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
            return false;
        }
    }
}
