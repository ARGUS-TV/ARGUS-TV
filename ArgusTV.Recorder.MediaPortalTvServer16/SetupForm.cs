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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using TvLibrary.Log;
using TvEngine;
using TvControl;

using ArgusTV.DataContracts;
using ArgusTV.Recorder.MediaPortalTvServer.Channels;
using ArgusTV.Recorder.MediaPortalTvServer.Wizards.ImportChannels;
using ArgusTV.ServiceProxy;

namespace ArgusTV.Recorder.MediaPortalTvServer
{
    public partial class SetupForm : SetupTv.SectionSettings
    {
        private SchedulerServiceProxy _schedulerProxy;

        public SetupForm()
        {
            InitializeComponent();
            _uncPathsBindingSource.DataSource = typeof(List<UncPathItem>);
            _channelItemsBindingSource.DataSource = typeof(List<LinkedChannelItem>);
        }

        #region Properties

        private TvServerPlugin _plugin;

        public TvServerPlugin Plugin
        {
            get { return _plugin; }
            set { _plugin = value; }
        }

        #endregion

        #region SetupTv.SectionSettings

        public override void OnSectionActivated()
        {
            Log.Info("ArgusTV.Recorder.MediaPortalTvServer: Configuration activated");

            _plugin.LoadSettings();
            _serverTextBox.Text = _plugin.ServerSettings.ServerName;
            _portNumericUpDown.Value = _plugin.ServerSettings.Port;

            _pluginTcpNumericUpDown.Value = _plugin.RecorderTunerTcpPort;

            _restartOnResumeCheckBox.Checked = _plugin.RestartTvServerOnResume;

            _syncTve3EpgcheckBox.Checked = _plugin.EpgSyncOn;
            _syncTve3EpgcheckBox_CheckedChanged(this, EventArgs.Empty);

            if (_plugin.EpgSyncAutoCreateChannels)
            {
                _epgAutoCreateChannelsDvbRadioButton.Checked = true;
            }
            else if (_plugin.EpgSyncAutoCreateChannelsWithGroup)
            {
                _epgAutoCreateChannelsWithGroupRadioButton.Checked = true;
            }
            else
            {
                _epgOnlyLinkedChannelsRadioButton.Checked = true;
            }

            ConnectToArgusTV();

            base.OnSectionActivated();
        }

        public override void OnSectionDeActivated()
        {
            Log.Info("ArgusTV.Recorder.MediaPortalTvServer: Configuration deactivated");

            DisconnectFromArgusTV();

            _plugin.RestartTvServerOnResume = _restartOnResumeCheckBox.Checked;

            _plugin.EpgSyncOn = _syncTve3EpgcheckBox.Checked;
            _plugin.EpgSyncAutoCreateChannels = _epgAutoCreateChannelsDvbRadioButton.Checked;
            _plugin.EpgSyncAutoCreateChannelsWithGroup = _epgAutoCreateChannelsWithGroupRadioButton.Checked;

            _plugin.RecorderTunerTcpPort = (int)_pluginTcpNumericUpDown.Value;

            _plugin.ServerSettings.ServerName = _serverTextBox.Text.Trim();
            _plugin.ServerSettings.Port = (int)_portNumericUpDown.Value;
            _plugin.SaveSettings();

            base.OnSectionDeActivated();
        }

        #endregion

        private class UncPathItem
        {
            private string _cardName;
            private string _recordingPath;
            private string _message;
            private bool _hasError;

            public UncPathItem(string cardName, string recordingPath, string message, bool hasError)
            {
                _cardName = cardName;
                _recordingPath = recordingPath;
                _message = message;
                _hasError = hasError;
            }

            public string CardName
            {
                get { return _cardName; }
            }

            public string RecordingPath
            {
                get { return _recordingPath; }
            }

            public string Message
            {
                get { return _message; }
            }

            public bool HasError
            {
                get { return _hasError; }
            }
        }

        private class LinkedChannelItem
        {
            private Channel _channel;
            private string _message;
            private Color _rowColor;

            public LinkedChannelItem(Channel channel, string message, Color rowColor)
            {
                _channel = channel;
                _message = message;
                _rowColor = rowColor;
            }

            public Channel Channel
            {
                get { return _channel; }
            }

            public string ChannelName
            {
                get { return _channel.DisplayName; }
            }

            public string Message
            {
                get { return _message; }
                set { _message = value; }
            }

            public Color RowColor
            {
                get { return _rowColor; }
                set { _rowColor = value; }
            }
        }

        private void ConnectToArgusTV()
        {
            DisconnectFromArgusTV();

            _plugin.InitializeArgusTVConnection(this);
            _notConnectedPanel.Visible = !_plugin.IsArgusTVConnectionInitialized;
            _channelsPanel.Visible = _plugin.IsArgusTVConnectionInitialized;

            LoadUncPaths();

            if (_plugin.IsArgusTVConnectionInitialized)
            {
                _schedulerProxy = new SchedulerServiceProxy();
            }
        }

        private void DisconnectFromArgusTV()
        {
            _schedulerProxy = null;
        }

        private void _connectButton_Click(object sender, EventArgs e)
        {
            _plugin.ServerSettings.ServerName = _serverTextBox.Text.Trim();
            _plugin.ServerSettings.Port = (int)_portNumericUpDown.Value;
            ConnectToArgusTV();
        }

        private void _channelTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadChannels();
        }

        private void _refreshButton_Click(object sender, EventArgs e)
        {
            LoadChannels();
        }

        private void _tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_tabControl.SelectedTab == _channelsTabPage
                && _plugin.IsArgusTVConnectionInitialized)
            {
                if (_channelTypeComboBox.SelectedIndex < 0)
                {
                    _channelTypeComboBox.SelectedIndex = (int)ChannelType.Television;
                }
            }
        }

        private void LoadUncPaths()
        {
            try
            {
                bool anyError = false;

                List<UncPathItem> pathItems = new List<UncPathItem>();
                List<UncPathItem> tsPathItems = new List<UncPathItem>();

                List<TvDatabase.Card> mediaPortalCards = Utility.GetAllCards();
                foreach (TvDatabase.Card card in mediaPortalCards)
                {
                    anyError = anyError | AddUncPathItem(pathItems, card.Name, card.RecordingFolder);
                    anyError = anyError | AddUncPathItem(tsPathItems, card.Name, card.TimeShiftFolder);
                }

                _uncPathsBindingSource.DataSource = pathItems;
                _uncTimeshiftingBindingSource.DataSource = tsPathItems;
                EnableUncButtons();

                if (anyError)
                {
                    MessageBox.Show(this, "You must set up a share with full permissions for the ARGUS TV Scheduler service account for all recording folders!", null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static bool AddUncPathItem(List<UncPathItem> pathItems, string name, string path)
        {
            bool hasError = true;
            string message;

            string uncPath = Common.ShareExplorer.GetUncPathForLocalPath(path);
            if (!String.IsNullOrEmpty(uncPath))
            {
                message = uncPath;
                path = uncPath;
                hasError = false;
            }
            else
            {
                message = path;
                hasError = true;
            }

            pathItems.Add(new UncPathItem(name, path, message, hasError));

            return hasError;
        }

        private void LoadChannels()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                ChannelType channelType = (ChannelType)_channelTypeComboBox.SelectedIndex;
                List<Channel> channels = new List<Channel>(_schedulerProxy.GetAllChannels(channelType, false));

                ChannelLinks.RemoveObsoleteLinks(channelType, channels);

                List<LinkedChannelItem> channelItems = new List<LinkedChannelItem>();

                foreach (Channel channel in channels)
                {
                    string message;
                    Color rowColor;
                    GetLinkedMessageAndColor(channel, out message, out rowColor);
                    channelItems.Add(new LinkedChannelItem(channel, message, rowColor));
                }

                _channelItemsBindingSource.DataSource = channelItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void _uncPathsDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dataGridView = sender as DataGridView;
            if (e.ColumnIndex == 1
                && e.RowIndex >= 0
                && e.RowIndex < _uncPathsBindingSource.Count)
            {
                UncPathItem linkItem = dataGridView.Rows[e.RowIndex].DataBoundItem as UncPathItem;
                if (linkItem != null
                    && linkItem.HasError)
                {
                    e.CellStyle.ForeColor = Color.Red;
                    e.CellStyle.SelectionForeColor = Color.Red;
                }
                else
                {
                    e.CellStyle.ForeColor = Color.DarkGreen;
                    e.CellStyle.SelectionForeColor = Color.DarkGreen;
                }
            }
        }

        private void _channelsDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 1
                && e.RowIndex >= 0
                && e.RowIndex < _channelItemsBindingSource.Count)
            {
                LinkedChannelItem linkItem = _channelsDataGridView.Rows[e.RowIndex].DataBoundItem as LinkedChannelItem;
                if (linkItem != null)
                {
                    e.CellStyle.ForeColor = linkItem.RowColor;
                    e.CellStyle.SelectionForeColor = linkItem.RowColor;
                }
            }
        }

        private void EnableUncButtons()
        {
            if (_uncPathsDataGridView.SelectedRows.Count > 0)
            {
                UncPathItem linkItem = _uncPathsDataGridView.SelectedRows[0].DataBoundItem as UncPathItem;
                _createUncShareButton.Enabled = linkItem.HasError;
            }
            else
            {
                _createUncShareButton.Enabled = false;
            }
            if (_uncTimeshiftingDataGridView.SelectedRows.Count > 0)
            {
                UncPathItem linkItem = _uncTimeshiftingDataGridView.SelectedRows[0].DataBoundItem as UncPathItem;
                _createTimeshiftingShareButton.Enabled = linkItem.HasError;
            }
            else
            {
                _createTimeshiftingShareButton.Enabled = false;
            }
        }

        private void EnableChannelButtons()
        {
            _linkChannelButton.Enabled = (_channelsDataGridView.SelectedRows.Count == 1);
        }

        private void _syncTve3EpgcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _epgOnlyLinkedChannelsRadioButton.Enabled = _syncTve3EpgcheckBox.Checked;
            _epgAutoCreateChannelsDvbRadioButton.Enabled = _syncTve3EpgcheckBox.Checked;
            _epgAutoCreateChannelsWithGroupRadioButton.Enabled = _syncTve3EpgcheckBox.Checked;
        }

        private void _refreshUncButton_Click(object sender, EventArgs e)
        {
            LoadUncPaths();
        }

        private void _uncPathsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            EnableUncButtons();
        }

        private void _channelsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            EnableChannelButtons();
        }

        private void _linkChannelButton_Click(object sender, EventArgs e)
        {
            if (_channelsDataGridView.SelectedRows.Count > 0)
            {
                LinkedChannelItem linkItem = _channelsDataGridView.SelectedRows[0].DataBoundItem as LinkedChannelItem;
                CreateChannelLinkForm form = new CreateChannelLinkForm();
                form.Channel = linkItem.Channel;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    ChannelLinks.Save();
                    string message;
                    Color rowColor;
                    GetLinkedMessageAndColor(linkItem.Channel, out message, out rowColor);
                    linkItem.Message = message;
                    linkItem.RowColor = rowColor;
                    _channelItemsBindingSource.ResetBindings(false);
                }
            }
        }

        private void _channelsDataGridView_DoubleClick(object sender, EventArgs e)
        {
            _linkChannelButton_Click(this, EventArgs.Empty);
        }

        private void GetLinkedMessageAndColor(Channel channel, out string message, out Color rowColor)
        {
            bool isAutoLinked;
            bool duplicateChannelsFound;
            LinkedMediaPortalChannel linkedChannel =
               ChannelLinks.GetLinkedMediaPortalChannel(channel, out isAutoLinked, out duplicateChannelsFound);
            if (duplicateChannelsFound)
            {
                message = "More than one channel found, change name or link manually";
                rowColor = Color.Red;
            }
            else if (linkedChannel == null)
            {
                message = "Channel not linked, change name or link manually";
                rowColor = Color.Red;
            }
            else if (isAutoLinked)
            {
                message = "Linked (auto)";
                rowColor = Color.Black;
            }
            else
            {
                message = "Linked to " + linkedChannel.DisplayName;
                rowColor = Color.DarkGreen;
            }
        }

        private void _uncTimeshiftingDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            EnableUncButtons();
        }

        private void _createUncShareButton_Click(object sender, EventArgs e)
        {
            if (_uncPathsDataGridView.SelectedRows.Count > 0)
            {
                ShowCreateShareForm(_uncPathsDataGridView.SelectedRows[0].DataBoundItem as UncPathItem);
            }
        }

        private void _createTimeshiftingShareButton_Click(object sender, EventArgs e)
        {
            if (_uncTimeshiftingDataGridView.SelectedRows.Count > 0)
            {
                ShowCreateShareForm(_uncTimeshiftingDataGridView.SelectedRows[0].DataBoundItem as UncPathItem);
            }
        }

        private void ShowCreateShareForm(UncPathItem linkItem)
        {
            CreateShareForm form = new CreateShareForm();
            form.LocalPath = linkItem.RecordingPath;
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                LoadUncPaths();
            }
        }

        private void _importChannelsButton_Click(object sender, EventArgs e)
        {
            ImportChannelsWizard wizardForm = new ImportChannelsWizard((ChannelType)_channelTypeComboBox.SelectedIndex);
            if (wizardForm.ShowDialog(this) == DialogResult.OK)
            {
                LoadChannels();
            }
        }
    }
}
