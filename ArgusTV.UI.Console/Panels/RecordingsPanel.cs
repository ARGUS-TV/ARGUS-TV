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
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using ArgusTV.DataContracts;
using ArgusTV.ServiceProxy;
using ArgusTV.UI.Console.Properties;
using ArgusTV.UI.Console.Wizards.SynchronizeRecordings;
using ArgusTV.UI.Console.Wizards.ExportRecordings;
using ArgusTV.WinForms;

namespace ArgusTV.UI.Console.Panels
{
	public partial class RecordingsPanel : ContentPanel
	{
        private TreeNode[] _recordingNodes;
		private ImageList _treeImageList;
        private Recording _selectedRecording;

		private static class TreeImageIndex
		{
			public const int RecordingFolder = 0;
			public const int RecordingFolderInProgress = 1;
			public const int RecordingFolderOpen = 2;
			public const int RecordingFolderInProgressOpen = 3;
			public const int Recording = 4;
			public const int RecordingInProgress = 5;
            public const int RecordingPartial = 6;
            public const int RecordingUnwatched = 7; // Recording + 3
            public const int RecordingInProgressUnwatched = 8; // RecordingInProgress + 3
            public const int RecordingPartialUnwatched = 9; // RecordingPartialUnwatched + 3
            public const int RecordingFileMissing = 10;
		}

		public RecordingsPanel()
		{
			InitializeComponent();
            _treeImageList = new ImageList();
            _treeImageList.ColorDepth = ColorDepth.Depth32Bit;
            _treeImageList.ImageSize = new Size(16, 16);
            _treeImageList.Images.Add(Resources.RecordingFolder);
            _treeImageList.Images.Add(Resources.RecordingFolderInProgress);
            _treeImageList.Images.Add(Resources.RecordingFolderOpen);
            _treeImageList.Images.Add(Resources.RecordingFolderInProgressOpen);
            _treeImageList.Images.Add(Resources.TvRecording);
            _treeImageList.Images.Add(Resources.TvRecordingInProgress);
            _treeImageList.Images.Add(Resources.TvRecordingPartial);
            _treeImageList.Images.Add(Resources.TvRecordingUnwatched);
            _treeImageList.Images.Add(Resources.TvRecordingInProgressUnwatched);
            _treeImageList.Images.Add(Resources.TvRecordingPartialUnwatched);
            _treeImageList.Images.Add(Resources.TvRecordingMissing);
            _recordingsTreeView.ImageList = _treeImageList;
            _recordingsTreeView.SelectionChanged += new EventHandler<EventArgs>(_recordingsTreeView_SelectionChanged);

            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                _recordingsTreeView.ItemHeight = (int)(_recordingsTreeView.ItemHeight * (graphics.DpiY / 96));
            }
        }

		public override string Title
		{
			get { return "Recorded Programs"; }
		}

		private void RecordingsPanel_Load(object sender, EventArgs e)
		{
            // Setting this index will also load all recordings.
            _channelTypeComboBox.SelectedIndex = (int)ChannelType.Television;
            _showByComboBox.SelectedIndex = (int)RecordingGroupMode.GroupByProgramTitle;
            _includeNonExistingCheckBox.Checked = this.MainForm.IsHttpConnection;

            try
            {
                var processingCommands = MainForm.SchedulerProxy.GetAllProcessingCommands();
                if (processingCommands.Count == 0)
                {
                    _manuallyRunPostProcessingToolStripMenuItem.Visible = false;
                }
                else
                {
                    _manuallyRunPostProcessingToolStripMenuItem.DropDownItems.Clear();
                    foreach (ProcessingCommand processingCommand in processingCommands)
                    {
                        ToolStripItem item = _manuallyRunPostProcessingToolStripMenuItem.DropDownItems.Add(processingCommand.Name + "...");
                        item.Tag = processingCommand;
                        item.Click += postProcessingItem_Click;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
		}

        public override void OnClosed()
        {
            if (_backgroundWorker.IsBusy)
            {
                _backgroundWorker.CancelAsync();
            }
            base.OnClosed();
        }

		private void LoadAllRecordings()
		{
            if (_backgroundWorker.IsBusy)
            {
                _backgroundWorker.CancelAsync();
                while (_backgroundWorker.IsBusy)
                {
                    System.Threading.Thread.Sleep(100);
                    Application.DoEvents();
                }
            }

            _recordingNodes = null;
            _exportButton.Enabled = false;
            _recordingsTreeView.Nodes.Clear();
            ClearDetails();
            _loadingPanel.Visible = true;
            _backgroundWorker.RunWorkerAsync(new BackgroundWorkerArgs(
                (ChannelType)_channelTypeComboBox.SelectedIndex,
                (RecordingGroupMode)_showByComboBox.SelectedIndex, _includeNonExistingCheckBox.Checked));
		}

        private static TreeNode AddRecordingNode(System.Collections.IList nodes, RecordingSummary recording, bool addChannelName)
		{
			TreeNode recordingNode = new TreeNode(GetRecordingDisplayText(recording, addChannelName));
            nodes.Add(recordingNode);
			recordingNode.Tag = recording;
            SetRecordingNodeIcon(recordingNode);
			return recordingNode;
		}

        private static void SetRecordingNodeIcon(TreeNode recordingNode)
        {
            RecordingSummary recording = recordingNode.Tag as RecordingSummary;
            if (!recording.IsFileOnDisk)
            {
                recordingNode.ImageIndex = TreeImageIndex.RecordingFileMissing;
            }
            else
            {
                if (recording.RecordingStopTime.HasValue)
                {
                    recordingNode.ImageIndex = recording.IsPartialRecording ? TreeImageIndex.RecordingPartial : TreeImageIndex.Recording;
                }
                else
                {
                    recordingNode.ImageIndex = TreeImageIndex.RecordingInProgress;
                }
                if (!recording.LastWatchedTime.HasValue)
                {
                    recordingNode.ImageIndex += 3;
                }
            }
            recordingNode.SelectedImageIndex = recordingNode.ImageIndex;
        }

        internal static void AddRecordingNodes(TreeNodeCollection nodes, object groupObject, bool includeNonExisting)
		{
            List<RecordingSummary> recordings = null;
            bool addChannelName = false;

            var controlProxy = new ControlServiceProxy();

            RecordingGroup recordingGroup = groupObject as RecordingGroup;
            switch (recordingGroup.RecordingGroupMode)
            {
                case RecordingGroupMode.GroupBySchedule:
                    recordings = controlProxy.GetRecordingsForSchedule(recordingGroup.ScheduleId, includeNonExisting);
                    addChannelName = true;
                    break;
                case RecordingGroupMode.GroupByChannel:
                    recordings = controlProxy.GetRecordingsOnChannel(recordingGroup.ChannelId, includeNonExisting);
                    break;
                case RecordingGroupMode.GroupByProgramTitle:
                    recordings = controlProxy.GetRecordingsForProgramTitle(recordingGroup.ChannelType, recordingGroup.ProgramTitle, includeNonExisting);
                    addChannelName = true;
                    break;
                case RecordingGroupMode.GroupByCategory:
                    recordings = controlProxy.GetRecordingsForCategory(recordingGroup.ChannelType, recordingGroup.Category, includeNonExisting);
                    addChannelName = true;
                    break;
            }

            if (recordings != null)
			{
				nodes.Clear();
                foreach (RecordingSummary recording in recordings)
				{
                    AddRecordingNode(nodes, recording, addChannelName);
				}
			}
		}

		private static void RefreshGroupNodeImage(TreeNode groupNode)
		{
            RecordingGroup recordingGroup = groupNode.Tag as RecordingGroup;

			if (groupNode.IsExpanded)
			{
                groupNode.ImageIndex = recordingGroup.IsRecording ? TreeImageIndex.RecordingFolderInProgressOpen : TreeImageIndex.RecordingFolderOpen;
			}
			else
			{
                groupNode.ImageIndex = recordingGroup.IsRecording ? TreeImageIndex.RecordingFolderInProgress : TreeImageIndex.RecordingFolder;
			}
			groupNode.SelectedImageIndex = groupNode.ImageIndex;
		}

        private static string FormatDateWithTime(DateTime dateTime, bool includeYear = false)
        {
            includeYear = includeYear && DateTime.Today.Year != dateTime.Year;
            return String.Format("{0} {1}", dateTime.ToString(includeYear ? "dddd dd MMM yyyy" : "dddd dd MMM"), dateTime.ToShortTimeString());
        }

		private string GetGroupDisplayText(object groupObject)
		{
            RecordingGroup recordingGroup = groupObject as RecordingGroup;
            switch (recordingGroup.RecordingGroupMode)
            {
                case RecordingGroupMode.GroupBySchedule:
                    return String.Format("{0} - {2} ({1} recordings)", recordingGroup.ScheduleName, recordingGroup.RecordingsCount, FormatDateWithTime(recordingGroup.LatestProgramStartTime, true));
                case RecordingGroupMode.GroupByChannel:
                    return String.Format("{0} - {2} ({1} recordings)", recordingGroup.ChannelDisplayName, recordingGroup.RecordingsCount, FormatDateWithTime(recordingGroup.LatestProgramStartTime, true));
                case RecordingGroupMode.GroupByProgramTitle:
                    return String.Format("{0} - {2} ({1} recordings)", recordingGroup.ProgramTitle, recordingGroup.RecordingsCount, FormatDateWithTime(recordingGroup.LatestProgramStartTime, true));
                case RecordingGroupMode.GroupByCategory:
                    return String.Format("{0} - {2} ({1} recordings)", String.IsNullOrEmpty(recordingGroup.Category) ? "(none)" : recordingGroup.Category, recordingGroup.RecordingsCount, FormatDateWithTime(recordingGroup.LatestProgramStartTime, true));
            }
			return String.Empty;
		}

        private static string GetRecordingDisplayText(RecordingSummary recording, bool addChannelName)
        {
            StringBuilder programTitle = new StringBuilder(recording.CreateProgramTitle());
            programTitle.Append(" - ");
            if (addChannelName)
            {
                programTitle.Append(recording.ChannelDisplayName).Append(" - ");
            }
            programTitle.Append(FormatDateWithTime(recording.ProgramStartTime, true));
            return programTitle.ToString();
        }

        private RecordingSummary GetSelectedRecording()
		{
			if (_recordingsTreeView.SelectedNodes.Count == 1)
			{
                return _recordingsTreeView.SelectedNode.Tag as RecordingSummary;
			}
			return null;
		}

        private List<RecordingSummary> GetSelectedRecordings()
        {
            List<RecordingSummary> selectedRecordings = new List<RecordingSummary>();
            foreach(TreeNode node in _recordingsTreeView.SelectedNodes)
            {
                RecordingSummary recording = node.Tag as RecordingSummary;
                if (recording != null)
                {
                    selectedRecordings.Add(recording);
                }
            }
            return selectedRecordings;
        }

        private void _showByComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAllRecordings();
        }

        private void _includeNonExistingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            LoadAllRecordings();
        }

        private void _channelTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_showByComboBox.SelectedIndex >= 0)
            {
                LoadAllRecordings();
            }
        }

        private void _recordingsTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			try
			{
				if (e.Node.Nodes.Count == 1
					 && String.IsNullOrEmpty(e.Node.Nodes[0].Text))
				{
                    ChannelType channelType = (ChannelType)_channelTypeComboBox.SelectedIndex;
					AddRecordingNodes(e.Node.Nodes, e.Node.Tag, _includeNonExistingCheckBox.Checked);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void _recordingsTreeView_AfterCollapse(object sender, TreeViewEventArgs e)
		{
			RefreshGroupNodeImage(e.Node);
		}

		private void _recordingsTreeView_AfterExpand(object sender, TreeViewEventArgs e)
		{
			RefreshGroupNodeImage(e.Node);
		}

        private void _recordingsTreeView_SelectionChanged(object sender, EventArgs e)
        {
            List<RecordingSummary> recordings = GetSelectedRecordings();
            try
            {
                _selectedRecording = (recordings.Count == 1) ? MainForm.ControlProxy.GetRecordingById(recordings[0].RecordingId) : null;
            }
            catch (Exception ex)
            {
                _selectedRecording = null;
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (_selectedRecording != null)
            {
                _scheduleNameTextBox.Text = _selectedRecording.ScheduleName;
                _recStartTextBox.Text = FormatDateWithTime(_selectedRecording.RecordingStartTime, true);
                _recStopTextBox.Text = _selectedRecording.RecordingStopTime.HasValue ? FormatDateWithTime(_selectedRecording.RecordingStopTime.Value, true) : "Recording...";
                _isPartialCheckBox.Checked = _selectedRecording.IsPartialRecording;
                _descriptionTextBox.Text = _selectedRecording.CreateCombinedDescription(false);
                _lastWatchedTextBox.Text = _selectedRecording.LastWatchedTime.HasValue ? FormatDateWithTime(_selectedRecording.LastWatchedTime.Value, true) : "Never";

                _keepUntilControl.SetKeepUntil(_selectedRecording.KeepUntilMode, _selectedRecording.KeepUntilValue);
                _keepUntilControl.Enabled = true;
                _applyKeepButton.Enabled = false;

                UpdateKeepUntilLabel(_selectedRecording, recordings[0].IsFileOnDisk);
            }
			else
			{
				ClearDetails();
            }
		}

        private void UpdateKeepUntilLabel(Recording recording, bool isFileOnDisk)
        {
            if (isFileOnDisk)
            {
                if (recording.KeepUntilMode == KeepUntilMode.NumberOfDays
                    && recording.KeepUntilValue.HasValue)
                {
                    DateTime keepUntilTime = recording.RecordingStartTime.AddDays(recording.KeepUntilValue.Value);
                    _keepUntilTextBox.Text = FormatDateWithTime(keepUntilTime, true);
                }
                else
                {
                    _keepUntilTextBox.Text = String.Empty;
                }
            }
            else
            {
                _keepUntilTextBox.Text = "Recorded file not found ...";
            }
        }

		private void ClearDetails()
		{
            _selectedRecording = null;
			_scheduleNameTextBox.Text = String.Empty;
			_recStartTextBox.Text = String.Empty;
			_recStopTextBox.Text = String.Empty;
			_isPartialCheckBox.Checked = false;
			_descriptionTextBox.Text = String.Empty;
			_lastWatchedTextBox.Text = String.Empty;
            _keepUntilTextBox.Text = String.Empty;
            _applyKeepButton.Enabled = false;
            _keepUntilControl.ClearKeepUntil();
            _keepUntilControl.Enabled = (GetSelectedRecordings().Count > 0);
        }

		private void _recordingsTreeView_DoubleClick(object sender, EventArgs e)
		{
			OpenSelectedRecording();
		}

		private void OpenSelectedRecording()
		{
			try
			{
                RecordingSummary recording = GetSelectedRecording();
                if (recording != null)
                {
                    if (Properties.Settings.Default.StreamRecordingsUsingRtsp)
                    {
                        string rtspUrl = null;
                        if (WinFormsUtility.IsVlcInstalled())
                        {
                            rtspUrl = MainForm.ControlProxy.StartRecordingStream(recording.RecordingFileName);
                        }
                        if (String.IsNullOrEmpty(rtspUrl)
                            || !WinFormsUtility.RunStreamPlayer(rtspUrl, false))
                        {
                            MessageBox.Show(this, "Failed to start VLC player.", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            if (!String.IsNullOrEmpty(rtspUrl))
                            {
                                MainForm.ControlProxy.StopRecordingStream(rtspUrl);
                            }
                        }
                    }
                    else if (recording.IsFileOnDisk)
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo(recording.RecordingFileName);
                        startInfo.WindowStyle = ProcessWindowStyle.Normal;
                        startInfo.UseShellExecute = true;
                        System.Diagnostics.Process.Start(startInfo);

                        MainForm.ControlProxy.SetRecordingLastWatched(recording.RecordingFileName);
                        recording.LastWatchedTime = DateTime.Now;
                        SetRecordingNodeIcon(_recordingsTreeView.SelectedNode);
                    }
                    else
                    {
                        MessageBox.Show(this, "Failed to find recording file.", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void OpenContainingFolder()
		{
			try
			{
                RecordingSummary recording = GetSelectedRecording();
				if (recording != null)
				{
					ProcessStartInfo startInfo = new ProcessStartInfo(Path.GetDirectoryName(recording.RecordingFileName));
					startInfo.WindowStyle = ProcessWindowStyle.Normal;
					startInfo.UseShellExecute = true;
                    System.Diagnostics.Process.Start(startInfo);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

        private void ShowRecordingThumbnail()
        {
            try
            {
                if (_recordingsTreeView.SelectedNode != null)
                {
                    RecordingSummary recording = _recordingsTreeView.SelectedNode.Tag as RecordingSummary;
                    if (recording != null)
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        int width = MainForm.IsHttpConnection ? 0 : 480;
                        byte[] thumbnailBytes = MainForm.ControlProxy.GetRecordingThumbnail(recording.RecordingId, width, 0, null, DateTime.MinValue);
                        Cursor.Current = Cursors.Default;

                        if (thumbnailBytes != null
                            && thumbnailBytes.Length > 0)
                        {
                            using (MemoryStream imageStream = new MemoryStream(thumbnailBytes))
                            {
                                using (ThumbnailPopup popup = new ThumbnailPopup())
                                {
                                    popup.Title = _recordingsTreeView.SelectedNode.Text;
                                    popup.ThumbnailImage = Image.FromStream(imageStream);
                                    popup.ShowDialog(this);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show(this, "No thumbnail for this recording.", null, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteSelectedRecordings()
		{
			try
			{
                List<RecordingSummary> recordings = GetSelectedRecordings();
                bool delete = false;
                if (recordings.Count == 1)
                {
                    delete = (DialogResult.Yes == MessageBox.Show(this, "Are you sure you want to delete this recording?",
                        "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2));
                }
                else if (recordings.Count > 1)
                {
                    delete = (DialogResult.Yes == MessageBox.Show(this, "Are you sure you want to delete all selected recordings?",
                        "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2));
                }
				if (delete)
				{
					Cursor.Current = Cursors.WaitCursor;
                    foreach (RecordingSummary recording in recordings)
                    {
                        MainForm.ControlProxy.DeleteRecording(recording.RecordingFileName, true);
                        RemoveRecordingNode(_recordingsTreeView.Nodes, recording);
                    }
                    ClearDetails();

                    RefreshDiskUsage(MainForm.ControlProxy.GetRecordingDisksInfo());
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

        private bool RemoveRecordingNode(TreeNodeCollection nodes, RecordingSummary recording)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Nodes.Count > 0)
                {
                    if (RemoveRecordingNode(node.Nodes, recording))
                    {
                        return true;
                    }
                }
                if (node.Tag == recording)
                {
                    TreeNode parentNode = node.Parent;
                    node.Remove();
                    if (parentNode != null)
                    {
                        if (parentNode.Nodes.Count == 0)
                        {
                            parentNode.Remove();
                        }
                        else
                        {
                            RecordingGroup group = parentNode.Tag as RecordingGroup;
                            if (group != null)
                            {
                                group.RecordingsCount--;
                                parentNode.Text = GetGroupDisplayText(group);
                            }
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        private void ResetLastWatched()
        {
			try
			{
                foreach (TreeNode node in _recordingsTreeView.SelectedNodes)
                {
                    RecordingSummary recording = node.Tag as RecordingSummary;
                    if (recording != null)
                    {
                        MainForm.ControlProxy.SetRecordingLastWatchedPosition(recording.RecordingFileName, null);
                        recording.LastWatchedTime = null;
                        recording.LastWatchedPosition = null;
                        SetRecordingNodeIcon(node);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _keepUntilControl_KeepUntilChanged(object sender, EventArgs e)
        {
            List<RecordingSummary> recordings = GetSelectedRecordings();
            if (recordings.Count == 1)
            {
                _applyKeepButton.Enabled = (recordings[0].KeepUntilMode != _keepUntilControl.KeepUntilMode
                    || recordings[0].KeepUntilValue != _keepUntilControl.KeepUntilValue);
            }
            else
            {
                _applyKeepButton.Enabled = (recordings.Count > 1);
            }
        }

        private void _applyKeepButton_Click(object sender, EventArgs e)
        {
            try
            {
                List<RecordingSummary> recordings = GetSelectedRecordings();
                foreach(RecordingSummary recording in recordings)
                {
                    MainForm.ControlProxy.SetRecordingKeepUntil(
                        recording.RecordingFileName, _keepUntilControl.KeepUntilMode, _keepUntilControl.KeepUntilValue);
                    recording.KeepUntilMode = _keepUntilControl.KeepUntilMode;
                    recording.KeepUntilValue = _keepUntilControl.KeepUntilValue;
                    if (_selectedRecording != null
                        && _selectedRecording.RecordingId == recording.RecordingId)
                    {
                        UpdateKeepUntilLabel(_selectedRecording, recording.IsFileOnDisk);
                    }
                }
                _applyKeepButton.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _syncRecordingButton_Click(object sender, EventArgs e)
        {
            SynchronizeRecordingsWizard wizardForm = new SynchronizeRecordingsWizard();
            if (wizardForm.ShowDialog(this.MainForm) == DialogResult.OK)
            {
                LoadAllRecordings();
            }
        }

        private void _exportButton_Click(object sender, EventArgs e)
        {
            ExportRecordingsContext context = new ExportRecordingsContext(_recordingNodes, _includeNonExistingCheckBox.Checked);
            ExportRecordingsWizard wizardForm = new ExportRecordingsWizard(context);
            if (wizardForm.ShowDialog(this.MainForm) == DialogResult.OK)
            {
                LoadAllRecordings();
            }
        }

        #region Background Worker

        private class BackgroundWorkerArgs
        {
            private ChannelType _channelType;
            private RecordingGroupMode _recordingGroupMode;
            private bool _includeNonExisting;

            public BackgroundWorkerArgs(ChannelType channelType, RecordingGroupMode recordingGroupMode, bool includeNonExisting)
            {
                _channelType = channelType;
                _recordingGroupMode = recordingGroupMode;
                _includeNonExisting = includeNonExisting;
            }

            public ChannelType ChannelType
            {
                get { return _channelType; }
            }

            public RecordingGroupMode RecordingGroupMode
            {
                get { return _recordingGroupMode; }
            }

            public bool IncludeNonExisting
            {
                get { return _includeNonExisting; }
            }
        }

        private class BackgroundWorkerResult
        {
            public List<TreeNode> RecordingNodes { set; get; }

            public RecordingDisksInfo RecordingDisksInfo { set; get; }

            public Exception RecordingDisksInfoException { set; get; }
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DateTime startTime = DateTime.Now;

            BackgroundWorkerArgs args = (BackgroundWorkerArgs)e.Argument;

            BackgroundWorkerResult result = new BackgroundWorkerResult();
            result.RecordingNodes = new List<TreeNode>();

            var controlProxy = new ControlServiceProxy();

            try
            {
                result.RecordingDisksInfo = controlProxy.GetRecordingDisksInfo();
            }
            catch (Exception ex)
            {
                result.RecordingDisksInfo = null;
                result.RecordingDisksInfoException = ex;
            }

            IList<RecordingGroup> groups = controlProxy.GetAllRecordingGroups(args.ChannelType, args.RecordingGroupMode);

            foreach (RecordingGroup group in groups)
            {
                if (_backgroundWorker.CancellationPending)
                {
                    break;
                }

                bool nodeAdded = false;
                List<RecordingSummary> recordings = null;

                if (!args.IncludeNonExisting)
                {
                    recordings = GetRecordingsInGroup(controlProxy, group, args);
                }

                if (args.IncludeNonExisting
                    || recordings != null)
                {
                    int count = recordings == null ? group.RecordingsCount : recordings.Count;
                    if (count == 0)
                    {
                        nodeAdded = true;
                    }
                    else if (group.RecordingGroupMode != RecordingGroupMode.GroupByChannel
                        && group.RecordingGroupMode != RecordingGroupMode.GroupByCategory
                        && count == 1)
                    {
                        if (recordings == null)
                        {
                            recordings = GetRecordingsInGroup(controlProxy, group, args);
                        }
                        if (recordings != null && recordings.Count > 0)
                        {
                            TreeNode recordingNode = AddRecordingNode(result.RecordingNodes, recordings[0], true);
                        }
                        nodeAdded = true;
                    }
                }
                if (!nodeAdded)
                {
                    TreeNode groupNode = new TreeNode(GetGroupDisplayText(group));
                    result.RecordingNodes.Add(groupNode);
                    groupNode.Tag = group;
                    RefreshGroupNodeImage(groupNode);
                    groupNode.Nodes.Add(String.Empty);
                }
            }

            Utility.EnsureMinimumTime(startTime, 250);

            if (!_backgroundWorker.CancellationPending)
            {
                e.Result = result;
            }
        }

        private static List<RecordingSummary> GetRecordingsInGroup(ControlServiceProxy controlProxy, RecordingGroup group, BackgroundWorkerArgs args)
        {
            List<RecordingSummary> recordings = null;
            switch (group.RecordingGroupMode)
            {
                case RecordingGroupMode.GroupBySchedule:
                    recordings = controlProxy.GetRecordingsForSchedule(group.ScheduleId, args.IncludeNonExisting);
                    break;
                case RecordingGroupMode.GroupByChannel:
                    recordings = controlProxy.GetRecordingsOnChannel(group.ChannelId, args.IncludeNonExisting);
                    break;
                case RecordingGroupMode.GroupByProgramTitle:
                    recordings = controlProxy.GetRecordingsForProgramTitle(args.ChannelType, group.ProgramTitle, args.IncludeNonExisting);
                    break;
                case RecordingGroupMode.GroupByCategory:
                    recordings = controlProxy.GetRecordingsForCategory(args.ChannelType, group.Category, args.IncludeNonExisting);
                    break;
            }
            return recordings;
        }

        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled
                && this.Visible)
            {
                _recordingsTreeView.Nodes.Clear();
                _loadingPanel.Visible = false;
                if (e.Error != null)
                {
                    _diskSpaceProgressBar.Value = 0;
                    MessageBox.Show(this, e.Error.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (e.Result != null)
                {
                    BackgroundWorkerResult result = (BackgroundWorkerResult)e.Result;
                    if (result.RecordingDisksInfo == null)
                    {
                        _diskSpaceProgressBar.Value = 0;
                        _toolTip.SetToolTip(_diskSpaceProgressBar,
                            "Error getting recording shares:" + Environment.NewLine + Environment.NewLine + result.RecordingDisksInfoException.Message);
                    }
                    else
                    {
                        RefreshDiskUsage(result.RecordingDisksInfo);
                    }
                    _recordingNodes = result.RecordingNodes.ToArray();
                    _recordingsTreeView.Nodes.AddRange(_recordingNodes);
                    _exportButton.Enabled = true;
                }
                else
                {
                    _diskSpaceProgressBar.Value = 0;
                }
            }
        }

        #endregion

        private void RefreshDiskUsage(RecordingDisksInfo recordingDisksInfo)
        {
            _diskSpaceProgressBar.Value = (int)Math.Round(recordingDisksInfo.PercentageUsed);
            _toolTip.SetToolTip(_diskSpaceProgressBar, String.Format(CultureInfo.CurrentCulture,
                "{0}% free space left ({1} GB free of {2} GB)" + Environment.NewLine + "{3} hours SD recordings left (estimated)" + Environment.NewLine + "{4} hours HD recordings left (estimated)",
                100 - recordingDisksInfo.PercentageUsed,
                Math.Round((decimal)recordingDisksInfo.FreeSpaceBytes / 1073741824M, 2),
                Math.Round((decimal)recordingDisksInfo.TotalSizeBytes / 1073741824M, 2),
                recordingDisksInfo.FreeHoursSD,
                recordingDisksInfo.FreeHoursHD));
        }

        private void _recordingsTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point location = e.Location;
                location.Offset(_recordingsTreeView.Location);
                ShowContextMenu(e.Node, location);
            }
        }

        private void ShowContextMenu(TreeNode node, Point location)
        {
            if (!_recordingsTreeView.SelectedNodes.Contains(node))
            {
                _recordingsTreeView.SelectedNode = node;
            }

            List<RecordingSummary> recordings = GetSelectedRecordings();
            if (recordings.Count > 0)
            {
                ChannelType channelType = (ChannelType)_channelTypeComboBox.SelectedIndex;
                _openToolStripMenuItem.Enabled = (recordings.Count == 1);
                _openContainingFolderToolStripMenuItem.Enabled = (recordings.Count == 1);
                _showRecordingThumbnailToolStripMenuItem.Enabled = (recordings.Count == 1);
                _showRecordingThumbnailToolStripMenuItem.Visible = (channelType == ChannelType.Television);
                _resetWatchedStatusToolStripMenuItem.Visible = AnyNodeRecordingHasLastWatched(recordings);
                _recordingContextMenuStrip.Show(this, location);
            }
        }

        private bool AnyNodeRecordingHasLastWatched(List<RecordingSummary> recordings)
        {
            foreach (RecordingSummary recording in recordings)
            {
                if (recording.LastWatchedTime.HasValue)
                {
                    return true;
                }
            }
            return false;
        }

        private void _recordingsTreeView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Apps
                && _recordingsTreeView.SelectedNode != null)
            {
                TreeNode treeNode = _recordingsTreeView.SelectedNode;
                Point location = treeNode.Bounds.Location;
                location.Offset(_recordingsTreeView.Location);
                location.Offset(8, treeNode.Bounds.Height / 2);
                ShowContextMenu(treeNode, location);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedRecordings();
            }
        }

        private void _openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSelectedRecording();
        }

        private void _openContainingFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenContainingFolder();
        }

        private void _showRecordingThumbnailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowRecordingThumbnail();
        }

        private void _resetWatchedStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetLastWatched();
        }

        private void _deleteRecordingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedRecordings();
        }

        private void postProcessingItem_Click(object sender, EventArgs e)
        {
            List<RecordingSummary> recordings = GetSelectedRecordings();
            if (recordings.Count > 0)
            {
                ToolStripItem item = (ToolStripItem)sender;
                ProcessingCommand processingCommand = (ProcessingCommand)item.Tag;

                SelectProcessingCommandTimeForm selectTimeForm = new SelectProcessingCommandTimeForm();
                selectTimeForm.Recordings = recordings;
                selectTimeForm.ProcessingCommand = processingCommand;
                if (selectTimeForm.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        foreach (RecordingSummary recording in recordings)
                        {
                            MainForm.ControlProxy.RunProcessingCommandOnRecording(recording.RecordingId,
                                processingCommand.ProcessingCommandId, selectTimeForm.RunAtTime);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
