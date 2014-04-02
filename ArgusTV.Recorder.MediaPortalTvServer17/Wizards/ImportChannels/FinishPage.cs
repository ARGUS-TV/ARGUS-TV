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
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

using ArgusTV.ServiceAgents;
using ArgusTV.DataContracts;

namespace ArgusTV.Recorder.MediaPortalTvServer.Wizards.ImportChannels
{
    internal partial class FinishPage : ImportChannelsPageBase
    {
        public FinishPage()
        {
            InitializeComponent();
        }

        public override string PageTitle
        {
            get { return "Finish"; }
        }

        public override string PageInformation
        {
            get { return "Import channels from MediaPortal into ARGUS TV."; }
        }

        public override void OnEnterPage(bool isBack)
        {
            if (this.Context.ImportChannels.Count == 0)
            {
                _noWorkPanel.Visible = true;
                _finishPanel.Visible = false;
            }
            else
            {
                _noWorkPanel.Visible = false;
                _finishPanel.Visible = true;
                _exportLabel.Text = FormatRecordingsLabel("1 channel", "{0} channels", this.Context.ImportChannels.Count);
            }
        }

        private string FormatRecordingsLabel(string format1, string format, int count)
        {
            return String.Format(CultureInfo.CurrentCulture, count == 1 ? format1 : format, count);
        }

        private void _importInNamedGroupRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _groupNameTextBox.Enabled = _importInNamedGroupRadioButton.Checked;
        }

        public override bool OnFinish()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                _destinationPanel.Enabled = false;
                _exportProgressBar.Minimum = 0;
                _exportProgressBar.Value = 0;
                _exportProgressBar.Maximum = this.Context.ImportChannels.Count;
                _exportProgressBar.Visible = true;
                Application.DoEvents();

                using (SchedulerServiceAgent tvSchedulerAgent = new SchedulerServiceAgent())
                using (GuideServiceAgent tvGuideAgent = new GuideServiceAgent())
                {
                    _channelMembersByName.Clear();
                    _channelGroupsByName.Clear();
                    ChannelGroup[] allGroups = tvSchedulerAgent.GetAllChannelGroups(this.Context.ChannelType, false);
                    foreach (ChannelGroup channelGroup in allGroups)
                    {
                        _channelGroupsByName[channelGroup.GroupName] = channelGroup;
                    }

                    int count = 0;
                    foreach (ImportChannelsContext.ImportChannel importChannel in this.Context.ImportChannels)
                    {
                        _exportingFileLabel.Text = importChannel.Channel.DisplayName;
                        Application.DoEvents();
                        ImportChannel(tvGuideAgent, tvSchedulerAgent, importChannel);
                        _exportProgressBar.Value = ++count;
                    }

                    foreach (string groupName in _channelGroupsByName.Keys)
                    {
                        if (_channelMembersByName.ContainsKey(groupName))
                        {
                            tvSchedulerAgent.SetChannelGroupMembers(
                                _channelGroupsByName[groupName].ChannelGroupId, _channelMembersByName[groupName].ToArray());
                        }
                    }

                    Channels.ChannelLinks.Save();
                }

                return true;
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                _destinationPanel.Enabled = true;
                _exportProgressBar.Visible = false;
            }
            finally
            {
                _exportingFileLabel.Text = String.Empty;
                Cursor.Current = Cursors.Default;
            }
            return false;
        }

        private Dictionary<string, ChannelGroup> _channelGroupsByName = new Dictionary<string, ChannelGroup>();
        private Dictionary<string, List<Guid>> _channelMembersByName = new Dictionary<string, List<Guid>>();

        private void ImportChannel(GuideServiceAgent tvGuideAgent, SchedulerServiceAgent tvSchedulerAgent, ImportChannelsContext.ImportChannel importChannel)
        {
            Guid channelId;

            Channels.ChannelLink channelLink = Channels.ChannelLinks.GetChannelLinkForMediaPortalChannel(importChannel.Channel);
            if (channelLink == null)
            {
                Channel channel = new Channel();
                channel.ChannelType = this.Context.ChannelType;
                channel.VisibleInGuide = true;
                channel.DisplayName = importChannel.Channel.DisplayName;
                channel.LogicalChannelNumber = importChannel.LogicalChannelNumber;
                channel.Sequence = importChannel.Channel.SortOrder;
                channel = tvSchedulerAgent.SaveChannel(channel);
                Channels.ChannelLinks.SetLinkedMediaPortalChannel(channel, importChannel.Channel);

                channelId = channel.ChannelId;
            }
            else
            {
                channelId = channelLink.ChannelId;
            }

            if (!_importChannelsOnlyRadioButton.Checked)
            {
                string groupName;
                int groupSequence;
                if (_importChannelsAndGroupsRadioButton.Checked)
                {
                    groupName = importChannel.GroupName;
                    groupSequence = importChannel.GroupSequence;
                }
                else
                {
                    groupName = _groupNameTextBox.Text.Trim();
                    groupSequence = 0;
                }
                if (!String.IsNullOrEmpty(importChannel.GroupName))
                {
                    if (!_channelGroupsByName.ContainsKey(groupName))
                    {
                        ChannelGroup channelGroup = new ChannelGroup();
                        channelGroup.GroupName = groupName;
                        channelGroup.VisibleInGuide = true;
                        channelGroup.ChannelType = this.Context.ChannelType;
                        channelGroup.Sequence = groupSequence;
                        channelGroup = tvSchedulerAgent.SaveChannelGroup(channelGroup);
                        _channelGroupsByName[groupName] = channelGroup;
                        _channelMembersByName[groupName] = new List<Guid>();
                    }

                    if (!_channelMembersByName.ContainsKey(groupName))
                    {
                        _channelMembersByName[groupName] = new List<Guid>(
                            tvSchedulerAgent.GetChannelGroupMembers(_channelGroupsByName[groupName].ChannelGroupId));
                    }

                    _channelMembersByName[groupName].Add(channelId);
                }
            }
        }
    }
}
