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
using System.Net;

using ArgusTV.ServiceProxy;

namespace ArgusTV.UI.MediaPortal
{
    public partial class SetupForm : Form
    {
        public SetupForm()
        {
            InitializeComponent();
        }

        private ServerSettings _serverSettings;
        public ServerSettings ServerSettings
        {
            get { return _serverSettings; }
            set { _serverSettings = value; }
        }

        private bool _noClientStandbyWhenNotHome;
        public bool NoClientStandbyWhenNotHome
        {
            get { return _noClientStandbyWhenNotHome; }
            set { _noClientStandbyWhenNotHome = value; }
        }

        private bool _preferRtspForLiveTv;
        public bool PreferRtspForLiveTv
        {
            get { return _preferRtspForLiveTv; }
            set { _preferRtspForLiveTv = value; }
        }

        private bool _playRecordingsOverRtsp;
        public bool PlayRecordingsOverRtsp
        {
            get { return _playRecordingsOverRtsp; }
            set { _playRecordingsOverRtsp = value; }
        }

        private bool _autoStreamingMode;
        public bool AutoStreamingMode
        {
            get { return _autoStreamingMode; }
            set { _autoStreamingMode = value; }
        }

        private bool _isSingleSeat = false;
        public bool IsSingleSeat
        {
            get { return _isSingleSeat; }
        }

        private bool _disableRadio = false;
        public bool DisableRadio
        {
            get { return _disableRadio; }
            set { _disableRadio = value; }
        }

        private void ConnectForm_Load(object sender, EventArgs e)
        {
            _serverTextBox.Text = _serverSettings.ServerName;
            _portNumericUpDown.Value = _serverSettings.Port;
            _useWolCheckBox.Checked = _serverSettings.WakeOnLan.Enabled;
            _wolSecondsNumericUpDown.Value = _serverSettings.WakeOnLan.TimeoutSeconds;
            _wolSecondsNumericUpDown.Enabled = _serverSettings.WakeOnLan.Enabled;
            _noStandbyWhenNotHomeCheckBox.Checked = _noClientStandbyWhenNotHome;
            _preferRtspForLiveTVCheckBox.Checked = _preferRtspForLiveTv;
            _playRecordingsOverRtspCheckBox.Checked = _playRecordingsOverRtsp;
            _streamingModeCheckBox.Checked = _autoStreamingMode;
            _disableRadioCheckBox.Checked = _disableRadio;
        }

        private void _radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ContainsFocus)
            {
                _portNumericUpDown.Value = ServerSettings.DefaultHttpPort;
            }
        }

        private void _testButton_Click(object sender, EventArgs e)
        {
            if (TestConnection())
            {
                string setupType = _isSingleSeat == true ? "Single-seat" : "Multi-seat";
                MessageBox.Show(this, "Connection succeeded!" + Environment.NewLine + setupType + " setup detected!", Utility.GetLocalizedText(TextId.Information), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            if (TestConnection())
            {
                _noClientStandbyWhenNotHome = _noStandbyWhenNotHomeCheckBox.Checked;
                _preferRtspForLiveTv = _preferRtspForLiveTVCheckBox.Checked;
                _playRecordingsOverRtsp = _playRecordingsOverRtspCheckBox.Checked;
                _autoStreamingMode = _streamingModeCheckBox.Checked;
                _disableRadio = _disableRadioCheckBox.Checked;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private bool TestConnection()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                _serverSettings.ServerName = _serverTextBox.Text.Trim();
                _serverSettings.Transport = ServiceTransport.Http;
                _serverSettings.Port = (int)_portNumericUpDown.Value;
                _serverSettings.WakeOnLan.Enabled = _useWolCheckBox.Checked;
                _serverSettings.WakeOnLan.TimeoutSeconds = (int)_wolSecondsNumericUpDown.Value;
                ProxyFactory.Initialize(_serverSettings, true);
                _isSingleSeat = Utility.IsThisASingleSeatSetup(_serverSettings.ServerName);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Utility.GetLocalizedText(TextId.Error), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
            return false;
        }

        private void _useWolCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _wolSecondsNumericUpDown.Enabled = _useWolCheckBox.Checked;
        }

        private void _streamingModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _preferRtspForLiveTVCheckBox.Enabled = !_streamingModeCheckBox.Checked;
            _playRecordingsOverRtspCheckBox.Enabled = !_streamingModeCheckBox.Checked;
        }
    }
}
