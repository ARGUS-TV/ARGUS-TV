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
using System.Runtime.InteropServices;

using ArgusTV.DataContracts;
using ArgusTV.UI.Console.Properties;
using ArgusTV.UI.Process;
using ArgusTV.WinForms;

namespace ArgusTV.UI.Console.Panels
{
    public partial class ChannelGroupsPanel : ContentPanel
    {
        private const string _dummyChannelItemText = "@NOT_LOADED_YET@";

        private List<ChannelGroup> _deletedGroups;
        private HashSet<Guid> _changedGroupIds = new HashSet<Guid>();
        private List<Channel> _allChannels;
        private SortableBindingList<Channel> _availableChannels;
        private Dictionary<Guid, bool> _groupsWithDirtyChannels;
        private ChannelGroup _activeGroup = null;
        private ImageList _treeImageList;

        private static class TreeImageIndex
        {
            public const int ChannelGroupClosed = 0;
            public const int ChannelGroupOpen = 1;
            public const int Channel = 2;
        }

        public ChannelGroupsPanel()
        {
            InitializeComponent();
            WinFormsUtility.ResizeDataGridViewColumnsForCurrentDpi(_channelsDataGridView);
        }

        public override string Title
        {
            get { return "Channel Groups"; }
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

                _treeImageList = new ImageList();
                _treeImageList.ColorDepth = ColorDepth.Depth32Bit;
                _treeImageList.ImageSize = new Size(16, 16);
                _treeImageList.Images.Add(Resources.ChannelGroupClosed);
                _treeImageList.Images.Add(Resources.ChannelGroupOpen);
                _treeImageList.Images.Add(Resources.Channel);
                _groupsTreeView.ImageList = _treeImageList;

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

        private void LoadAllGroups()
        {
            try
            {
                _deletedGroups = new List<ChannelGroup>();
                _groupsWithDirtyChannels = new Dictionary<Guid, bool>();
                _availableChannels = new SortableBindingList<Channel>();
                List<ChannelGroup> channelGroups = new List<ChannelGroup>(
                    MainForm.SchedulerProxy.GetAllChannelGroups(this.ChannelType, false));
                _allChannels = new List<Channel>(
                    MainForm.SchedulerProxy.GetAllChannels(this.ChannelType, false));
                _channelsBindingSource.DataSource = _availableChannels;
                _channelsBindingSource.ResetBindings(false);

                _groupsTreeView.Nodes.Clear();
                foreach (ChannelGroup channelGroup in channelGroups)
                {
                    TreeNode groupNode = _groupsTreeView.Nodes.Add(channelGroup.GroupName);
                    groupNode.ImageIndex = TreeImageIndex.ChannelGroupClosed;
                    groupNode.SelectedImageIndex = TreeImageIndex.ChannelGroupClosed;
                    groupNode.Checked = channelGroup.VisibleInGuide;
                    groupNode.Tag = channelGroup;
                    groupNode.Nodes.Add(_dummyChannelItemText);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<Channel> GetSelectedAvailableChannels()
        {
            List<Channel> channels = new List<Channel>();
            if (_channelsDataGridView.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in _channelsDataGridView.Rows)
                {
                    if (row.Selected)
                    {
                        channels.Add(row.DataBoundItem as Channel);
                    }
                }
            }
            return channels;
        }

        private void EnableButtons()
        {
            if (!_inMoving)
            {
                ChannelGroup selectedGroup = GetSelectedGroup();
                ChannelGroup currentGroup = GetCurrentGroup();
                Channel selectedGroupChannel = GetSelectedGroupChannel();
                List<Channel> selectedAvailableChannels = GetSelectedAvailableChannels();
                _addChannelButton.Enabled = (selectedAvailableChannels.Count > 0 && currentGroup != null);
                _removeChannelButton.Enabled = (selectedGroupChannel != null);
                _moveTopButton.Enabled = (selectedGroup != null || selectedGroupChannel != null);
                _moveUpButton.Enabled = (selectedGroup != null || selectedGroupChannel != null);
                _moveDownButton.Enabled = (selectedGroup != null || selectedGroupChannel != null);
                _moveBottomButton.Enabled = (selectedGroup != null || selectedGroupChannel != null);
                _deleteButton.Enabled = (selectedGroup != null);
                _sortByLcnButton.Enabled = (currentGroup != null);
            }
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

                List<ChannelGroup> changedGroups = GetChangedGroups();
                foreach (ChannelGroup channelGroup in changedGroups)
                {
                    ChannelGroup savedGroup = MainForm.SchedulerProxy.SaveChannelGroup(channelGroup);
                    channelGroup.ChannelGroupId = savedGroup.ChannelGroupId;
                    _groupsWithDirtyChannels[channelGroup.ChannelGroupId] = true;
                }

                foreach (TreeNode groupNode in _groupsTreeView.Nodes)
                {
                    ChannelGroup channelGroup = groupNode.Tag as ChannelGroup;
                    if (_groupsWithDirtyChannels.ContainsKey(channelGroup.ChannelGroupId))
                    {
                        List<Guid> channelIds = GetChildChannelIds(groupNode);
                        MainForm.SchedulerProxy.SetChannelGroupMembers(channelGroup.ChannelGroupId, channelIds.ToArray());
                    }
                }

                foreach (ChannelGroup channelGroup in _deletedGroups)
                {
                    MainForm.SchedulerProxy.DeleteChannelGroup(channelGroup.ChannelGroupId, false, false);
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

        private bool HasChanges()
        {
            return (_groupsWithDirtyChannels.Count > 0
                || _deletedGroups.Count > 0
                || GetChangedGroups().Count > 0);
        }

        private List<ChannelGroup> GetChangedGroups()
        {
            List<ChannelGroup> changedGroups = new List<ChannelGroup>();

            string previousName = null;
            int sequence = 0;
            foreach (TreeNode groupNode in _groupsTreeView.Nodes)
            {
                ChannelGroup channelGroup = groupNode.Tag as ChannelGroup;
                if (previousName != null)
                {
                    if (String.Compare(channelGroup.GroupName, previousName, StringComparison.InvariantCultureIgnoreCase) < 0)
                    {
                        sequence++;
                    }
                }
                channelGroup.VisibleInGuide = groupNode.Checked;
                channelGroup.Sequence = sequence;
                if (channelGroup.ChannelGroupId == Guid.Empty
                    || _changedGroupIds.Contains(channelGroup.ChannelGroupId))
                {
                    changedGroups.Add(channelGroup);
                }
                previousName = channelGroup.GroupName;
            }
            return changedGroups;
        }

        private List<Guid> GetChildChannelIds(TreeNode groupNode)
        {
            List<Guid> channelIds = new List<Guid>();
            EnsureGroupChannels(groupNode);
            foreach (TreeNode childNode in groupNode.Nodes)
            {
                Channel channel = childNode.Tag as Channel;
                channelIds.Add(channel.ChannelId);
            }
            return channelIds;
        }

        private void _deleteButton_Click(object sender, EventArgs e)
        {
            ChannelGroup channelGroup = GetSelectedGroup();
            if (channelGroup != null)
            {
                if (channelGroup.ChannelGroupId != Guid.Empty)
                {
                    _deletedGroups.Add(channelGroup);
                }
                TreeNode node = _groupsTreeView.SelectedNode;
                _groupsTreeView.SelectedNode = null;
                node.Remove();
                EnableButtons();
            }
        }

        private void _createNewButton_Click(object sender, EventArgs e)
        {
            ChannelGroup channelGroup = new ChannelGroup();
            channelGroup.ChannelType = this.ChannelType;
            channelGroup.GroupName = "-Unnamed-";
            channelGroup.VisibleInGuide = true;

            TreeNode newNode = _groupsTreeView.Nodes.Add(channelGroup.GroupName);
            newNode.Checked = channelGroup.VisibleInGuide;
            newNode.Tag = channelGroup;
            _groupsTreeView.SelectedNode = newNode;
            newNode.BeginEdit();
        }

        private void _sortByLcnButton_Click(object sender, EventArgs e)
        {
            TreeNode groupNode = GetCurrentGroupNode();
            if (groupNode != null
                && EnsureGroupChannels(groupNode))
            {
                List<Channel> channels = new List<Channel>();
                foreach (TreeNode node in groupNode.Nodes)
                {
                    channels.Add(node.Tag as Channel);
                }

                if (channels.Count > 0)
                {
                    List<Channel> unsortedChannels = new List<Channel>(channels);
                    channels.Sort(
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

                    try
                    {
                        _groupsTreeView.SuspendLayout();
                        groupNode.Nodes.Clear();
                        foreach (Channel channel in channels)
                        {
                            AddChannelNode(groupNode, channel);
                        }
                    }
                    finally
                    {
                        _groupsTreeView.ResumeLayout();
                    }
                }
            }
        }

        private void _moveTopButton_Click(object sender, EventArgs e)
        {
            MoveSelectedItemTo(MoveTo.Top);
        }

        private void _moveUpButton_Click(object sender, EventArgs e)
        {
            MoveSelectedItemTo(MoveTo.Up);
        }

        private void _moveDownButton_Click(object sender, EventArgs e)
        {
            MoveSelectedItemTo(MoveTo.Down);
        }

        private void _moveBottomButton_Click(object sender, EventArgs e)
        {
            MoveSelectedItemTo(MoveTo.Bottom);
        }

        private enum MoveTo
        {
            Top,
            Up,
            Down,
            Bottom
        }

        private bool _inMoving;

        private void MoveSelectedItemTo(MoveTo moveTo)
        {
            TreeNode node = _groupsTreeView.SelectedNode;
            if (node != null)
            {
                TreeNodeCollection nodes = node.Parent == null ? _groupsTreeView.Nodes : node.Parent.Nodes;
                int index = node.Index;
                int newIndex = index;
                switch (moveTo)
                {
                    case MoveTo.Top:
                        newIndex = 0;
                        break;

                    case MoveTo.Bottom:
                        newIndex = nodes.Count - 1;
                        break;

                    case MoveTo.Up:
                        if (newIndex > 0)
                        {
                            newIndex--;
                        }
                        break;

                    case MoveTo.Down:
                        if (newIndex < nodes.Count)
                        {
                            newIndex++;
                        }
                        break;
                }
                if (index != newIndex)
                {
                    _inMoving = true;
                    try
                    {
                        _groupsTreeView.SelectedNode = null;
                        nodes.RemoveAt(index);
                        nodes.Insert(newIndex, node);
                        if (node.Parent != null)
                        {
                            ChannelGroup currentGroup = node.Parent.Tag as ChannelGroup;
                            _groupsWithDirtyChannels[currentGroup.ChannelGroupId] = true;
                            RemoveCheckboxOnNode(node);
                        }
                        else
                        {
                            RemoveGroupChannelsCheckBoxes(node);
                        }
                        _groupsTreeView.SelectedNode = node;
                    }
                    finally
                    {
                        _inMoving = false;
                    }
                }
            }
        }

        private TreeNode GetCurrentGroupNode()
        {
            if (_groupsTreeView.SelectedNode != null)
            {
                TreeNode node = _groupsTreeView.SelectedNode;
                if (node.Tag is Channel)
                {
                    node = node.Parent;
                }
                return node;
            }
            return null;
        }

        private ChannelGroup GetCurrentGroup()
        {
            TreeNode node = GetCurrentGroupNode();
            if (node != null)
            {
                return node.Tag as ChannelGroup;
            }
            return null;
        }

        private ChannelGroup GetSelectedGroup()
        {
            if (_groupsTreeView.SelectedNode != null)
            {
                return _groupsTreeView.SelectedNode.Tag as ChannelGroup;
            }
            return null;
        }

        private Channel GetSelectedGroupChannel()
        {
            if (_groupsTreeView.SelectedNode != null)
            {
                return _groupsTreeView.SelectedNode.Tag as Channel;
            }
            return null;
        }

        private void _channelTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_allChannels != null
                && HasChanges())
            {
                if (DialogResult.Yes == MessageBox.Show(this, "Save changes?", this.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    SaveChanges();
                }
            }
            LoadAllGroups();
        }

        private void _channelsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }

        private void _groupsTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            ChannelGroup group = e.Node.Tag as ChannelGroup;
            if (group != null
                && !String.IsNullOrEmpty(e.Label))
            {
                if (group.GroupName != e.Label)
                {
                    group.GroupName = e.Label;
                    _changedGroupIds.Add(group.ChannelGroupId);
                }
            }
        }

        private void _groupsTreeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            e.CancelEdit = (e.Node.Tag is Channel);
        }

        private void _groupsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ChannelGroup group = GetCurrentGroup();
            if (group != _activeGroup)
            {
                RefreshAvailableChannels(group);
                _activeGroup = group;
            }
            if (group != null
                && !e.Node.IsExpanded)
            {
                e.Node.Expand();
            }
            EnableButtons();
        }

        private void _groupsTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = !EnsureGroupChannels(e.Node);
        }

        private void _groupsTreeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            e.Node.ImageIndex = TreeImageIndex.ChannelGroupOpen;
            e.Node.SelectedImageIndex = TreeImageIndex.ChannelGroupOpen;
        }

        private void _groupsTreeView_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            e.Node.ImageIndex = TreeImageIndex.ChannelGroupClosed;
            e.Node.SelectedImageIndex = TreeImageIndex.ChannelGroupClosed;
        }

        private bool EnsureGroupChannels(TreeNode groupNode)
        {
            ChannelGroup group = groupNode.Tag as ChannelGroup;
            if (group != null)
            {
                if (groupNode.Nodes.Count == 1
                    && groupNode.Nodes[0].Text == _dummyChannelItemText)
                {
                    try
                    {
                        var channels = MainForm.SchedulerProxy.GetChannelsInGroup(group.ChannelGroupId, false);

                        groupNode.Nodes.Clear();
                        foreach (Channel channel in channels)
                        {
                            AddChannelNode(groupNode, channel);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
            return true;
        }

        private void RefreshAvailableChannels(ChannelGroup group)
        {
            _availableChannels.Clear();
            if (group != null)
            {
                foreach (Channel channel in _allChannels)
                {
                    if (!GroupContainsChannel(group, channel.ChannelId))
                    {
                        _availableChannels.Add(channel);
                    }
                }
            }
            _channelsBindingSource.ResetBindings(false);
        }

        private void _channelsDataGridView_DoubleClick(object sender, EventArgs e)
        {
            _addChannelButton_Click(sender, e);
        }

        private void _addChannelButton_Click(object sender, EventArgs e)
        {
            ChannelGroup currentGroup = GetCurrentGroup();
            if (currentGroup != null)
            {
                List<Channel> availableChannels = GetSelectedAvailableChannels();
                bool dirty = false;
                foreach (Channel availableChannel in availableChannels)
                {
                    if (!GroupContainsChannel(currentGroup, availableChannel.ChannelId))
                    {
                        TreeNode groupNode = GetNodeForGroup(currentGroup);
                        AddChannelNode(groupNode, availableChannel);
                        dirty = true;
                    }
                }
                if (dirty)
                {
                    _groupsWithDirtyChannels[currentGroup.ChannelGroupId] = true;
                    RefreshAvailableChannels(currentGroup);
                }
            }
        }

        private void _removeChannelButton_Click(object sender, EventArgs e)
        {
            ChannelGroup currentGroup = GetCurrentGroup();
            Channel channel = GetSelectedGroupChannel();
            if (channel != null)
            {
                TreeNode groupNode = GetNodeForGroup(currentGroup);
                RemoveChannelNode(groupNode, channel);
                _groupsWithDirtyChannels[currentGroup.ChannelGroupId] = true;
                RefreshAvailableChannels(currentGroup);
            }
        }

        private bool GroupContainsChannel(ChannelGroup group, Guid channelId)
        {
            TreeNode groupNode = GetNodeForGroup(group);
            if (groupNode != null)
            {
                EnsureGroupChannels(groupNode);
                foreach (TreeNode childNode in groupNode.Nodes)
                {
                    Channel channel = childNode.Tag as Channel;
                    if (channel.ChannelId == channelId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private TreeNode GetNodeForGroup(ChannelGroup group)
        {
            foreach (TreeNode node in _groupsTreeView.Nodes)
            {
                if (node.Tag == group)
                {
                    return node;
                }
            }
            return null;
        }

        private TreeNode AddChannelNode(TreeNode groupNode, Channel channel)
        {
            EnsureGroupChannels(groupNode);
            TreeNode childNode = groupNode.Nodes.Add(channel.CombinedDisplayName);
            RemoveCheckboxOnNode(childNode);
            childNode.ImageIndex = TreeImageIndex.Channel;
            childNode.SelectedImageIndex = TreeImageIndex.Channel;
            if (!groupNode.IsExpanded)
            {
                groupNode.Expand();
            }
            childNode.Tag = channel;
            return childNode;
        }

        private void RemoveGroupChannelsCheckBoxes(TreeNode groupNode)
        {
            foreach (TreeNode childNode in groupNode.Nodes)
            {
                RemoveCheckboxOnNode(childNode);
            }
        }

        private void RemoveChannelNode(TreeNode groupNode, Channel channel)
        {
            EnsureGroupChannels(groupNode);
            foreach (TreeNode childNode in groupNode.Nodes)
            {
                if (childNode.Tag == channel)
                {
                    groupNode.Nodes.Remove(childNode);
                    break;
                }
            }
        }

        #region P/Invoke

        private void RemoveCheckboxOnNode(TreeNode node)
        {
            TVITEM tvItem = new TVITEM();
            tvItem.hItem = node.Handle;
            tvItem.mask = TVIF_STATE;
            tvItem.stateMask = TVIS_STATEIMAGEMASK;
            tvItem.state = 0;
            IntPtr lparam = Marshal.AllocHGlobal(Marshal.SizeOf(tvItem));
            try
            {
                Marshal.StructureToPtr(tvItem, lparam, false);
                SendMessage(_groupsTreeView.Handle, TVM_SETITEM, IntPtr.Zero, lparam);
            }
            finally
            {
                Marshal.FreeHGlobal(lparam);
            }
        }

        public const int TVIF_STATE = 0x8;
        public const int TVIS_STATEIMAGEMASK = 0xF000;
        public const int TV_FIRST = 0x1100;
        public const int TVM_SETITEM = TV_FIRST + 63;

        public struct TVITEM
        {
            public int mask;
            public IntPtr hItem;
            public int state;
            public int stateMask;
            [MarshalAs(UnmanagedType.LPTStr)]
            public String lpszText;
            public int cchTextMax;
            public int iImage;
            public int iSelectedImage;
            public int cChildren;
            public IntPtr lParam;
        }

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        #endregion
    }
}
