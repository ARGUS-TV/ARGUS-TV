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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ArgusTV.DataContracts;
using ArgusTV.UI.Process;
using ArgusTV.WinForms;
using ArgusTV.ServiceProxy;

namespace ArgusTV.UI.Console.Panels
{
    public partial class ChannelsPanel : ContentPanel
    {
        private const int _saveBatchSize = 50;

        private SortableBindingList<GuideChannel> _guideChannels;
        private List<Channel> _channels;
        private HashSet<Guid> _changedChannelIds = new HashSet<Guid>();
        private List<Channel> _deletedChannels;
        private bool _isAllChannels;

        public ChannelsPanel()
        {
            InitializeComponent();
            _channelGroupControl.ShowAllChannelsOnTop = true;
            WinFormsUtility.ResizeDataGridViewColumnsForCurrentDpi(_channelsDataGridView);
            _changedChannelIds.Add(Guid.Empty);
        }

        public override string Title
        {
            get { return "Channels"; }
        }

        private void ChannelsPanel_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                MainForm.SetMenuMode(MainMenuMode.SaveCancel);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        public override void OnClosed()
        {
            MainForm.SetMenuMode(MainMenuMode.Normal);
            _channelsBindingSource.DataSource = null;
            _channelsBindingSource.ResetBindings(false);
            _guideChannelColumn.DataSource = null;
        }

        private void LoadChannels()
        {
            try
            {
                _deletedChannels = new List<Channel>();

                RefreshGuideChannels();

                ChannelGroup group = _channelGroupControl.SelectedGroup;
                if (group != null)
                {
                    _channels = new List<Channel>(
                        Proxies.SchedulerService.GetChannelsInGroup(group.ChannelGroupId, false).Result);
                    _isAllChannels = (group.ChannelGroupId == ChannelGroup.AllTvChannelsGroupId
                        || group.ChannelGroupId == ChannelGroup.AllRadioChannelsGroupId);
                }
                else
                {
                    _channels = new List<Channel>();
                    _isAllChannels = false;
                }
                _channelsBindingSource.DataSource = _channels;
                _channelsBindingSource.ResetBindings(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshGuideChannels()
        {
            _guideChannels = new SortableBindingList<GuideChannel>(Proxies.GuideService.GetAllChannels(_channelGroupControl.ChannelType).Result);
            _guideChannels.Insert(0, new GuideChannel()
            {
                Name = String.Empty,
                XmlTvId = String.Empty,
                ChannelType = _channelGroupControl.ChannelType
            });
            _guideChannelColumn.DataPropertyName = "GuideChannelId";
            _guideChannelColumn.DisplayMember = "Name";
            _guideChannelColumn.ValueMember = "GuideChannelId";
            _guideChannelColumn.DataSource = _guideChannels;
        }

        private Channel GetSelectedChannel()
        {
            Channel channel = null;
            if (_channelsDataGridView.SelectedRows.Count > 0)
            {
                channel = _channelsDataGridView.SelectedRows[0].DataBoundItem as Channel;
            }
            return channel;
        }

        private void EnableButtons()
        {
            if (!_inChannelMoving)
            {
                _moveTopButton.Enabled = _isAllChannels && (_channelsDataGridView.SelectedRows.Count == 1);
                _moveUpButton.Enabled = _isAllChannels && (_channelsDataGridView.SelectedRows.Count == 1);
                _moveDownButton.Enabled = _isAllChannels && (_channelsDataGridView.SelectedRows.Count == 1);
                _moveBottomButton.Enabled = _isAllChannels && (_channelsDataGridView.SelectedRows.Count == 1);
                _visibleOnButton.Enabled = (_channelsDataGridView.SelectedRows.Count > 0);
                _visibleOffButton.Enabled = (_channelsDataGridView.SelectedRows.Count > 0);
                _deleteButton.Enabled = (_channelsDataGridView.SelectedRows.Count > 0);
                _sortByLcnButton.Enabled = _isAllChannels && (_channels.Count > 0);
                _createNewButton.Enabled = _isAllChannels;
            }
        }

        private void _channelsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }

        public override void OnSave()
        {
            SaveChanges();
            ClosePanel();
        }

        private void SaveChanges()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                CalculateChannelSequences();

                List<Channel> changedChannels = new List<Channel>();
                foreach (Channel channel in _channels)
                {
                    if (_changedChannelIds.Contains(channel.ChannelId))
                    {
                        changedChannels.Add(channel);
                        if (changedChannels.Count >= _saveBatchSize)
                        {
                            Proxies.SchedulerService.SaveChannels(changedChannels.ToArray());
                            changedChannels.Clear();
                        }
                    }
                }
                if (changedChannels.Count > 0)
                {
                    Proxies.SchedulerService.SaveChannels(changedChannels.ToArray());
                }

                foreach (Channel channel in _deletedChannels)
                {
                    if (channel.ChannelId != Guid.Empty)
                    {
                        Proxies.SchedulerService.DeleteChannel(channel.ChannelId, true);
                    }
                }
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

        private bool HasChangedChannels()
        {
            CalculateChannelSequences();
            return _channels.Any(c => _changedChannelIds.Contains(c.ChannelId));
        }

        private bool HasChanges()
        {
            return (_deletedChannels.Count > 0
                || HasChangedChannels());
        }

        private void _visibleOnButton_Click(object sender, EventArgs e)
        {
            ChangeVisibility(true);
        }

        private void _visibleOffButton_Click(object sender, EventArgs e)
        {
            ChangeVisibility(false);
        }

        private void ChangeVisibility(bool visible)
        {
            List<int> selectedIndexes = new List<int>();
            foreach (DataGridViewRow row in _channelsDataGridView.SelectedRows)
            {
                selectedIndexes.Add(row.Index);
                Channel channel = row.DataBoundItem as Channel;
                if (channel != null
                    && channel.VisibleInGuide != visible)
                {
                    channel.VisibleInGuide = visible;
                    _changedChannelIds.Add(channel.ChannelId);
                }
            }
            _channelsBindingSource.ResetBindings(false);
            foreach (int index in selectedIndexes)
            {
                _channelsDataGridView.Rows[index].Selected = true;
            }
        }

        private void _deleteButton_Click(object sender, EventArgs e)
        {
            List<Channel> channelsToRemove = new List<Channel>();
            foreach (DataGridViewRow row in _channelsDataGridView.SelectedRows)
            {
                Channel channel = row.DataBoundItem as Channel;
                if (channel != null)
                {
                    channelsToRemove.Add(channel);
                }
            }
            foreach (Channel channel in channelsToRemove)
            {
                _channels.Remove(channel);
                _deletedChannels.Add(channel);
            }
            _channelsBindingSource.ResetBindings(false);
            _channelsDataGridView.ClearSelection();
        }

        private void _createNewButton_Click(object sender, EventArgs e)
        {
            Channel channel = new Channel();
            channel.ChannelType = _channelGroupControl.ChannelType;
            channel.DisplayName = "-Unnamed-";
            channel.VisibleInGuide = true;
            _channelsBindingSource.Add(channel);
            _channelsDataGridView.CurrentCell = _channelsDataGridView.Rows[_channelsDataGridView.Rows.Count - 1].Cells[2];
            _channelsDataGridView.BeginEdit(true);
        }

        private void _moveTopButton_Click(object sender, EventArgs e)
        {
            MoveSelectedChannelTo(MoveTo.Top);
        }

        private void _moveUpButton_Click(object sender, EventArgs e)
        {
            MoveSelectedChannelTo(MoveTo.Up);
        }

        private void _moveDownButton_Click(object sender, EventArgs e)
        {
            MoveSelectedChannelTo(MoveTo.Down);
        }

        private void _moveBottomButton_Click(object sender, EventArgs e)
        {
            MoveSelectedChannelTo(MoveTo.Bottom);
        }

        private enum MoveTo
        {
            Top,
            Up,
            Down,
            Bottom
        }

        private bool _inChannelMoving;

        private void MoveSelectedChannelTo(MoveTo moveTo)
        {
            Channel channel = GetSelectedChannel();
            if (channel != null)
            {
                int index = _channels.IndexOf(channel);
                int newIndex = index;
                switch (moveTo)
                {
                    case MoveTo.Top:
                        newIndex = 0;
                        break;

                    case MoveTo.Bottom:
                        newIndex = _channels.Count - 1;
                        break;

                    case MoveTo.Up:
                        if (newIndex > 0)
                        {
                            newIndex--;
                        }
                        break;

                    case MoveTo.Down:
                        if (newIndex < _channels.Count - 1)
                        {
                            newIndex++;
                        }
                        break;
                }
                if (index != newIndex)
                {
                    _inChannelMoving = true;
                    try
                    {
                        _channels.RemoveAt(index);
                        _channels.Insert(newIndex, channel);
                        _channelsBindingSource.ResetBindings(false);
                        _channelsDataGridView.Rows[newIndex].Selected = true;
                        _channelsDataGridView.CurrentCell = _channelsDataGridView.Rows[newIndex].Cells[0];
                    }
                    finally
                    {
                        _inChannelMoving = false;
                    }
                }
            }
        }

        private void CalculateChannelSequences()
        {
            if (_isAllChannels)
            {
                int sequence = 0;
                string previousName = null;
                foreach (Channel channel in _channels)
                {
                    if (previousName != null)
                    {
                        if (String.Compare(channel.DisplayName, previousName, StringComparison.InvariantCultureIgnoreCase) < 0)
                        {
                            sequence++;
                        }
                    }
                    if (channel.Sequence != sequence)
                    {
                        channel.Sequence = sequence;
                        _changedChannelIds.Add(channel.ChannelId);
                    }
                    previousName = channel.DisplayName;
                }
            }
        }

        private void _sortByLcnButton_Click(object sender, EventArgs e)
        {
            if (_channels.Count > 0)
            {
                List<Channel> unsortedChannels = new List<Channel>(_channels);
                _channels.Sort(
                    delegate(Channel c1, Channel c2)
                    {
                        if (c1.LogicalChannelNumber.HasValue
                            && c2.LogicalChannelNumber.HasValue)
                        {
                            return c1.LogicalChannelNumber.Value.CompareTo(c2.LogicalChannelNumber.Value);
                        }
                        else if (c1.LogicalChannelNumber.HasValue)
                        {
                            return -1;
                        }
                        else if (c2.LogicalChannelNumber.HasValue)
                        {
                            return 1;
                        }
                        return unsortedChannels.IndexOf(c1).CompareTo(unsortedChannels.IndexOf(c2));
                    });
                _channelsBindingSource.ResetBindings(false);
            }
        }

        private void _channelGroupControl_SelectedGroupChanged(object sender, EventArgs e)
        {
            if (_channels != null
                && HasChanges())
            {
                if (DialogResult.Yes == MessageBox.Show(this, "Save changes?", this.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    SaveChanges();
                }
            }
            LoadChannels();
        }

        private void _channelsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var channel = _channelsDataGridView.Rows[e.RowIndex].DataBoundItem as Channel;
                _changedChannelIds.Add(channel.ChannelId);
            }
        }
    }
}
