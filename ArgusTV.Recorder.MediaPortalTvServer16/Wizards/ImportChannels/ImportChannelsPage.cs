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
using System.Windows.Forms;
using ArgusTV.Recorder.MediaPortalTvServer.Channels;

namespace ArgusTV.Recorder.MediaPortalTvServer.Wizards.ImportChannels
{
    internal partial class ImportChannelsPage : ImportChannelsPageBase
    {
        public ImportChannelsPage()
        {
            InitializeComponent();
        }

        public override string PageTitle
        {
            get { return "Import MediaPortal Channels"; }
        }

        public override string PageInformation
        {
            get { return "Select all channels you would like to import from MediaPortal TV Server into ARGUS TV. Channels that are already linked are not shown."; }
        }

        public override void OnEnterPage(bool isBack)
        {
            if (!isBack)
            {
                _recordingsTreeView.Nodes.Clear();
                _recordingsTreeView.Nodes.AddRange(this.Context.ChannelNodes);
                _recordingsTreeView.InitializeNodesState(_recordingsTreeView.Nodes);
            }
        }

        public override void OnLeavePage(bool isBack)
        {
            this.Context.ImportChannels.Clear();
            AddToImportList(this.Context.ImportChannels, _recordingsTreeView.Nodes);
        }

        private void AddToImportList(List<ImportChannelsContext.ImportChannel> importChannels, TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.StateImageIndex == ThreeStateTreeView.ItemState.Checked)
                {
                    ImportChannelsContext.ImportChannel importChannel = node.Tag as ImportChannelsContext.ImportChannel;
                    if (importChannel != null)
                    {
                        importChannels.Add(importChannel);
                    }
                }
                AddToImportList(importChannels, node.Nodes);
            }
        }

        private void _recordingsTreeView_BeforeNodeStateChangeByUser(object sender, TreeViewEventArgs e)
        {
            EnsureChildren(e.Node);
        }

        private void _recordingsTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            EnsureChildren(e.Node);
        }

        private void EnsureChildren(TreeNode node)
        {
            try
            {
                if (node.Nodes.Count == 1
                     && String.IsNullOrEmpty(node.Nodes[0].Text))
                {
                    int groupId;
                    string groupName;
                    int groupSequence;
                    TvDatabase.ChannelGroup group = node.Tag as TvDatabase.ChannelGroup;
                    if (group == null)
                    {
                        TvDatabase.RadioChannelGroup radioGroup = node.Tag as TvDatabase.RadioChannelGroup;
                        groupId = radioGroup.IdGroup;
                        groupName = radioGroup.GroupName;
                        groupSequence = 0;
                    }
                    else
                    {
                        groupId = group.IdGroup;
                        groupName = group.GroupName;
                        groupSequence = group.SortOrder;
                    }

                    List<TvDatabase.Channel> channels = Utility.GetAllChannelsInGroup(this.Context.ChannelType, groupId);

                    node.Nodes.Clear();
                    foreach (TvDatabase.Channel channel in channels)
                    {
                        if (ChannelLinks.GetChannelLinkForMediaPortalChannel(channel) == null
                            && !ChannelLinks.IsAutoLinked(channel))
                        {
                            TreeNode channelNode = new TreeNode(channel.DisplayName);
                            channelNode.Tag = new ImportChannelsContext.ImportChannel(channel, groupId < 0 ? null : groupName, groupSequence);
                            node.Nodes.Add(channelNode);
                        }
                    }
                    _recordingsTreeView.InitializeNodesState(node.Nodes);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
