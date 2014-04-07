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

using ArgusTV.DataContracts;
using ArgusTV.Common;
using ArgusTV.UI.Console.Panels;
using ArgusTV.UI.Console.Controls;

namespace ArgusTV.UI.Console.Wizards.ExportRecordings
{
    internal partial class ExportRecordingsPage : ExportRecordingsPageBase
    {
        public ExportRecordingsPage()
        {
            InitializeComponent();
        }

        public override string PageTitle
        {
            get { return "Export Recordings"; }
        }

        public override string PageInformation
        {
            get { return "Select all recordings you would like to export from ARGUS TV to an external device or network location."; }
        }

        public override void OnEnterPage(bool isBack)
        {
            if (!isBack)
            {
                _recordingsTreeView.Nodes.Clear();
                _recordingsTreeView.Nodes.AddRange(this.Context.RecordingNodes);
                _recordingsTreeView.InitializeNodesState(_recordingsTreeView.Nodes);
            }
        }

        public override void OnLeavePage(bool isBack)
        {
            this.Context.ExportRecordings.Clear();
            AddToExportList(this.Context.ExportRecordings, _recordingsTreeView.Nodes);
        }

        private void AddToExportList(List<RecordingSummary> exportRecordings, TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.StateImageIndex == ThreeStateTreeView.ItemState.Checked)
                {
                    RecordingSummary recording = node.Tag as RecordingSummary;
                    if (recording != null)
                    {
                        exportRecordings.Add(recording);
                    }
                }
                AddToExportList(exportRecordings, node.Nodes);
            }
        }

        private void _allButton_Click(object sender, EventArgs e)
        {
            SelectAllRecordings(true);
        }

        private void _noneButton_Click(object sender, EventArgs e)
        {
            SelectAllRecordings(false);
        }

        private void SelectAllRecordings(bool selected)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                foreach (TreeNode node in _recordingsTreeView.Nodes)
                {
                    _recordingsTreeView.SetNodeState(node, selected ? ThreeStateTreeView.ItemState.Checked : ThreeStateTreeView.ItemState.Unchecked);
                }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
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
                    RecordingsPanel.AddRecordingNodes(node.Nodes, node.Tag, false);
                    _recordingsTreeView.InitializeNodesState(node.Nodes);
                    for (int index = node.Nodes.Count - 1; index >= 0; index--)
                    {
                        if (!this.Context.ValidateNode(node.Nodes[index]))
                        {
                            node.Nodes.RemoveAt(index);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
