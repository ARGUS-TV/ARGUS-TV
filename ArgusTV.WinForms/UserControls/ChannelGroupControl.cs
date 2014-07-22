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
using ArgusTV.ServiceProxy;

namespace ArgusTV.WinForms.UserControls
{
    public partial class ChannelGroupControl : UserControl
    {
        private Dictionary<ChannelType, List<ChannelGroup>> _channelGroups = new Dictionary<ChannelType, List<ChannelGroup>>();

        public event EventHandler SelectedGroupChanged;

        public ChannelGroupControl()
        {
            InitializeComponent();
        }

        public bool ShowAllChannelsOnTop { get; set; }

        public ChannelType ChannelType
        {
            get { return (ChannelType)_channelTypeComboBox.SelectedIndex; }
        }

        public ChannelGroup SelectedGroup
        {
            get { return _channelGroupsComboBox.SelectedItem as ChannelGroup; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!this.DesignMode)
            {
                _channelTypeComboBox.SelectedIndex = (int)ChannelType.Television;
            }
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            _channelGroupsComboBox.Width = this.Width - _channelGroupsComboBox.Left;
        }

        private void LoadGroups()
        {
            ChannelType channelType = this.ChannelType;
            if (!_channelGroups.ContainsKey(channelType))
            {
                try
                {
                    List<ChannelGroup> channelGroups = new List<ChannelGroup>(Proxies.SchedulerService.GetAllChannelGroups(channelType, true).Result);
                    ChannelGroup allChannelsGroup = new ChannelGroup()
                    {
                        ChannelGroupId = channelType == ChannelType.Television ? ChannelGroup.AllTvChannelsGroupId : ChannelGroup.AllRadioChannelsGroupId,
                        ChannelType = channelType,
                        GroupName = "All Channels",
                        VisibleInGuide = true
                    };
                    if (this.ShowAllChannelsOnTop)
                    {
                        channelGroups.Insert(0, allChannelsGroup);
                    }
                    else
                    {
                        channelGroups.Add(allChannelsGroup);
                    }
                    _channelGroups[channelType] = channelGroups;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            _channelGroupsComboBox.DataSource = _channelGroups[channelType];
            _channelGroupsComboBox.DisplayMember = "GroupName";
            _channelGroupsComboBox.ValueMember = "ChannelGroupId";
        }

        private void _channelTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadGroups();
        }

        private void _channelGroupsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.SelectedGroupChanged != null)
            {
                this.SelectedGroupChanged(this, EventArgs.Empty);
            }
        }
    }
}
