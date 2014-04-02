using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Gentle.Framework;

using ArgusTV.DataContracts;
using ArgusTV.Recorder.MediaPortalTvServer.Channels;

namespace ArgusTV.Recorder.MediaPortalTvServer
{
    public partial class CreateChannelLinkForm : Form
    {
        public CreateChannelLinkForm()
        {
            InitializeComponent();
        }

        internal Channel Channel { set; get; }

        private void CreateShareForm_Load(object sender, EventArgs e)
        {
            _channelNameLabel.Text = this.Channel.DisplayName;
            LoadGroups();

            LinkedMediaPortalChannel linkedChannel = ChannelLinks.GetLinkedMediaPortalChannel(this.Channel);
            if (linkedChannel != null)
            {
                SqlBuilder sb = new SqlBuilder(StatementType.Select, typeof(TvDatabase.GroupMap));
                sb.AddConstraint(Operator.Equals, "idChannel", linkedChannel.Id);
                SqlResult result = Broker.Execute(sb.GetStatement());
                List<TvDatabase.GroupMap> groupMaps = (List<TvDatabase.GroupMap>)
                    ObjectFactory.GetCollection(typeof(TvDatabase.GroupMap), result, new List<TvDatabase.GroupMap>());
                if (groupMaps.Count > 0)
                {
                    foreach (ListViewItem item in _groupsListView.Items)
                    {
                        if (item.Tag is int
                            && (int)item.Tag == groupMaps[0].IdGroup)
                        {
                            item.Selected = true;
                            _groupsListView.EnsureVisible(item.Index);
                            break;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                    }

                    foreach (ListViewItem item in _channelsListView.Items)
                    {
                        ChannelItem channelItem = item.Tag as ChannelItem;
                        if (channelItem.Channel.IdChannel == linkedChannel.Id)
                        {
                            item.Selected = true;
                            _channelsListView.EnsureVisible(item.Index);
                            break;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                    }
                }
            }
        }

        private void LoadGroups()
        {
            if (this.Channel.ChannelType == ChannelType.Television)
            {
                List<TvDatabase.ChannelGroup> groups = Utility.GetAllGroups<TvDatabase.ChannelGroup>();
                foreach (TvDatabase.ChannelGroup group in groups)
                {
                    if (group.GroupName.Equals("All Channels", StringComparison.CurrentCultureIgnoreCase))
                    {
                        groups.Remove(group);
                        break;
                    }
                }
                groups.Add(new TvDatabase.ChannelGroup(-1, "All Channels"));
                foreach (TvDatabase.ChannelGroup group in groups)
                {
                    ListViewItem item = new ListViewItem(group.GroupName);
                    item.Tag = group.IdGroup;
                    _groupsListView.Items.Add(item);
                }
            }
            else
            {
                List<TvDatabase.RadioChannelGroup> groups = Utility.GetAllGroups<TvDatabase.RadioChannelGroup>();
                foreach (TvDatabase.RadioChannelGroup group in groups)
                {
                    if (group.GroupName.Equals("All Channels", StringComparison.CurrentCultureIgnoreCase))
                    {
                        groups.Remove(group);
                        break;
                    }
                }
                groups.Add(new TvDatabase.RadioChannelGroup(-1, "All Channels"));
                foreach (TvDatabase.RadioChannelGroup group in groups)
                {
                    ListViewItem item = new ListViewItem(group.GroupName);
                    item.Tag = group.IdGroup;
                    _groupsListView.Items.Add(item);
                }
            }
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            if (_channelsListView.SelectedItems.Count > 0)
            {
                ChannelItem channelItem = _channelsListView.SelectedItems[0].Tag as ChannelItem;
                ChannelLinks.SetLinkedMediaPortalChannel(this.Channel, channelItem.Channel);
            }
            else
            {
                ChannelLinks.ClearLinkedMediaPortalChannel(this.Channel);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void _clearLinkButton_Click(object sender, EventArgs e)
        {
            ChannelLinks.ClearLinkedMediaPortalChannel(this.Channel);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void _channelsListView_DoubleClick(object sender, EventArgs e)
        {
            if (_channelsListView.SelectedItems.Count > 0)
            {
                _okButton_Click(this, EventArgs.Empty);
            }
        }

        private void _groupNameListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_groupsListView.SelectedItems.Count > 0)
            {
                int groupId = (int)_groupsListView.SelectedItems[0].Tag;

                List<ChannelItem> channelItems = new List<ChannelItem>();
                List<TvDatabase.Channel> channels =
                    Utility.GetAllChannelsInGroup(this.Channel.ChannelType, groupId);
                foreach (TvDatabase.Channel channel in channels)
                {
                    channelItems.Add(new ChannelItem(channel));
                }

                _channelsListView.Items.Clear();
                foreach (ChannelItem channelItem in channelItems)
                {
                    ListViewItem item = new ListViewItem(channelItem.ChannelName);
                    item.Tag = channelItem;
                    _channelsListView.Items.Add(item);
                }
            }
            else
            {
                _channelsListView.Items.Clear();
            }
        }

        private class ChannelItem
        {
            private TvDatabase.Channel _channel;

            public ChannelItem(TvDatabase.Channel channel)
            {
                _channel = channel;
            }

            public TvDatabase.Channel Channel
            {
                get { return _channel; }
            }

            public string ChannelName
            {
                get { return _channel.DisplayName; }
            }
        }
    }
}
