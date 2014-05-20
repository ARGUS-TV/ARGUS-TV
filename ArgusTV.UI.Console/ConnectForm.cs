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

using ArgusTV.ServiceProxy;

namespace ArgusTV.UI.Console
{
    public partial class ConnectForm : Form
    {
        public ConnectForm()
        {
            InitializeComponent();
        }

        private ServerSettings _serverSettings;

        public ServerSettings ServerSettings
        {
            get { return _serverSettings; }
            set { _serverSettings = (ServerSettings)value.Clone(); }
        }

        private bool _savePassword;

        public bool SavePassword
        {
            get { return _savePassword; }
        }

        private string _profileName;

        public string ProfileName
        {
            get { return _profileName; }
            set { _profileName = value; }
        }

        private void ConnectForm_Load(object sender, EventArgs e)
        {
            if (_serverSettings.Transport == ServiceTransport.Https)
            {
                _transportComboBox.SelectedIndex = 1;
            }
            else
            {
                _transportComboBox.SelectedIndex = 0;
            }
            _serverTextBox.Text = _serverSettings.ServerName;
            _portNumericUpDown.Value = _serverSettings.Port;
            _useWolCheckBox.Checked = _serverSettings.WakeOnLan.Enabled;
            _wolSecondsNumericUpDown.Value = _serverSettings.WakeOnLan.TimeoutSeconds;
            EnableWakeOnLan();
            _saveAsProfileCheckBox.Checked = !String.IsNullOrEmpty(_profileName);
            _profileNameTextBox.Enabled = !String.IsNullOrEmpty(_profileName);
            _profileNameTextBox.Text = _profileName;
        }

        private void _transportComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ContainsFocus)
            {
                int port = ServerSettings.DefaultHttpPort;
                if (_transportComboBox.SelectedIndex == 1)
                {
                    port = ServerSettings.DefaultHttpsPort;
                }
                _portNumericUpDown.Value = port;
            }
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            try
            {
                _serverSettings.ServerName = _serverTextBox.Text.Trim();
                ServiceTransport transport = ServiceTransport.Http;
                if (_transportComboBox.SelectedIndex == 1)
                {
                    transport = ServiceTransport.Https;
                }
                _serverSettings.Transport = transport;
                _serverSettings.Port = (int)_portNumericUpDown.Value;
                _serverSettings.WakeOnLan.Enabled = _useWolCheckBox.Checked;
                _serverSettings.WakeOnLan.TimeoutSeconds = (int)_wolSecondsNumericUpDown.Value;

                bool doConnect = true;

                if (_serverSettings.Transport == ServiceTransport.Https)
                {
                    using (LogonForm logonForm = new LogonForm())
                    {
                        logonForm.UserName = _serverSettings.UserName;
                        logonForm.Password = _serverSettings.Password;
                        doConnect = (DialogResult.OK == logonForm.ShowDialog(this));
                        _savePassword = logonForm.SavePassword;
                        _serverSettings.UserName = logonForm.UserName;
                        _serverSettings.Password = logonForm.Password;
                    }
                }

                if (doConnect)
                {
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        Proxies.Initialize(_serverSettings, true);

                        if (_saveAsProfileCheckBox.Checked)
                        {
                            _profileName = _profileNameTextBox.Text.Trim();
                        }
                        else
                        {
                            _profileName = null;
                        }
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    finally
                    {
                        Cursor.Current = Cursors.Default;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _useWolCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _wolSecondsNumericUpDown.Enabled = _useWolCheckBox.Checked;
        }

        private void _serverTextBox_TextChanged(object sender, EventArgs e)
        {
            EnableWakeOnLan();
            if (_saveAsProfileCheckBox.Checked)
            {
                _profileNameTextBox.Text = _serverTextBox.Text.Trim();
            }
        }

        private void EnableWakeOnLan()
        {
            bool enabled = !String.IsNullOrEmpty(_serverSettings.WakeOnLan.MacAddresses)
                && String.Equals(_serverSettings.ServerName, _serverTextBox.Text, StringComparison.InvariantCultureIgnoreCase);
            if (!enabled)
            {
                _useWolCheckBox.Checked = false;
            }
            _useWolCheckBox.Enabled = enabled;
            _allowLabel.Enabled = enabled;
            _secondsLabel.Enabled = enabled;
            _wolSecondsNumericUpDown.Enabled = enabled;
        }

        private void _saveAsProfileCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _profileNameTextBox.Enabled = _saveAsProfileCheckBox.Checked;
            if (_saveAsProfileCheckBox.Checked
                && String.IsNullOrEmpty(_profileNameTextBox.Text))
            {
                _profileNameTextBox.Text = _serverTextBox.Text.Trim();
            }
        }
    }
}
