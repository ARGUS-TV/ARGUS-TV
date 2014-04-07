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
using System.Text;
using System.Windows.Forms;

using ArgusTV.DataContracts;
using ArgusTV.UI.Process;
using ArgusTV.WinForms;

namespace ArgusTV.UI.Console.Panels
{
    public partial class GuideChannelsPanel : ContentPanel
    {
        private SortableBindingList<GuideChannel> _guideChannels;
        private List<GuideChannel> _deletedChannels;

        public GuideChannelsPanel()
        {
            InitializeComponent();
            WinFormsUtility.ResizeDataGridViewColumnsForCurrentDpi(_channelsDataGridView);
        }

        public override string Title
        {
            get { return "Guide Channels"; }
        }

        private ChannelType ChannelType
        {
            get { return (ChannelType)_channelTypeComboBox.SelectedIndex; }
        }

        private void ChannelsPanel_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                MainForm.SetMenuMode(MainMenuMode.SaveCancel);

                _channelTypeComboBox.SelectedIndex = (int)ChannelType.Television;
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
        }

        private void LoadAllChannels()
        {
            try
            {
                _deletedChannels = new List<GuideChannel>();
                _guideChannels = new SortableBindingList<GuideChannel>(MainForm.GuideProxy.GetAllChannels(this.ChannelType));
                _channelsBindingSource.DataSource = _guideChannels;
                _channelsBindingSource.ResetBindings(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EnableButtons()
        {
            _deleteButton.Enabled = (_channelsDataGridView.SelectedRows.Count > 0);
        }

        private void _channelsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }

        public override void OnSave()
        {
            if (HasChanges())
            {
                SaveChanges();
            }
            ClosePanel();
        }

        private bool HasChanges()
        {
            return _guideChannels != null
                && _deletedChannels.Count > 0;
        }

        private void SaveChanges()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                foreach (GuideChannel guideChannel in _deletedChannels)
                {
                    if (guideChannel.GuideChannelId != Guid.Empty)
                    {
                        MainForm.GuideProxy.DeleteChannel(guideChannel.GuideChannelId);
                    }
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void _deleteButton_Click(object sender, EventArgs e)
        {
            if (_channelsDataGridView.SelectedRows.Count > 0)
            {
                List<GuideChannel> channelsToRemove = new List<GuideChannel>();
                foreach (DataGridViewRow row in _channelsDataGridView.SelectedRows)
                {
                    GuideChannel guideChannel = row.DataBoundItem as GuideChannel;
                    if (guideChannel != null)
                    {
                        channelsToRemove.Add(guideChannel);
                    }
                }
                foreach (GuideChannel guideChannel in channelsToRemove)
                {
                    _guideChannels.Remove(guideChannel);
                    _deletedChannels.Add(guideChannel);
                }
                _channelsDataGridView.ClearSelection();
            }
        }

        private void _channelTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (HasChanges())
            {
                if (DialogResult.Yes == MessageBox.Show(this, "Save changes?", this.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    SaveChanges();
                }
            }
            LoadAllChannels();
        }
    }
}
