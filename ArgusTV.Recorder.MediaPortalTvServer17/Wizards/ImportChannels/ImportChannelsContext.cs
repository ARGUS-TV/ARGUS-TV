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
using System.Text;
using System.Net;
using System.Windows.Forms;
using System.IO;

using Gentle.Framework;

using ArgusTV.DataContracts;
using ArgusTV.Recorder.MediaPortalTvServer.Channels;

namespace ArgusTV.Recorder.MediaPortalTvServer.Wizards.ImportChannels
{
    internal class ImportChannelsContext
    {
        private TreeNode[] _channelNodes;
        private List<ImportChannel> _importChannels = new List<ImportChannel>();
        private ChannelType _channelType = ChannelType.Television;

        public ImportChannelsContext()
        {
        }

        public ImportChannelsContext(ChannelType channelType)
        {
            _channelType = channelType;

            List<TreeNode> nodes = new List<TreeNode>();

            if (channelType == ChannelType.Television)
            {
                List<TvDatabase.ChannelGroup> groups = Utility.GetAllGroups<TvDatabase.ChannelGroup>();
                groups.Add(new TvDatabase.ChannelGroup(-1, "All Channels"));
                foreach (TvDatabase.ChannelGroup group in groups)
                {
                    TreeNode groupNode = new TreeNode(group.GroupName);
                    groupNode.Tag = group;
                    nodes.Add(groupNode);
                    groupNode.Nodes.Add(String.Empty);
                }
            }
            else
            {
                List<TvDatabase.RadioChannelGroup> groups = Utility.GetAllGroups<TvDatabase.RadioChannelGroup>();
                groups.Add(new TvDatabase.RadioChannelGroup(-1, "All Channels"));
                foreach (TvDatabase.RadioChannelGroup group in groups)
                {
                    TreeNode groupNode = new TreeNode(group.GroupName);
                    groupNode.Tag = group;
                    nodes.Add(groupNode);
                    groupNode.Nodes.Add(String.Empty);
                }
            }

            _channelNodes = nodes.ToArray();
        }

        public ChannelType ChannelType
        {
            get { return _channelType; }
        }

        public TreeNode[] ChannelNodes
        {
            get { return _channelNodes; }
        }

        public List<ImportChannel> ImportChannels
        {
            get { return _importChannels; }
        }

        public class ImportChannel
        {
            public ImportChannel(TvDatabase.Channel mpChannel, string groupName, int groupSequence)
            {
                this.Channel = mpChannel;
                this.GroupName = groupName;
                this.GroupSequence = groupSequence;
                if (mpChannel.ChannelNumber > 0)
                {
                    this.LogicalChannelNumber = mpChannel.ChannelNumber;
                }
            }

            public TvDatabase.Channel Channel { set; get; }

            public string GroupName { set; get; }

            public int GroupSequence { set; get; }

            public int? LogicalChannelNumber { get; set; }
        }
    }
}
