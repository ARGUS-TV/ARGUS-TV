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

using ArgusTV.DataContracts;

namespace ArgusTV.UI.Console.Wizards.ExportRecordings
{
    internal class ExportRecordingsContext
    {
        private TreeNode[] _recordingNodes;
        private bool _checkNonExisting;
        private List<RecordingSummary> _exportRecordings = new List<RecordingSummary>();

        public ExportRecordingsContext()
        {
        }

        public ExportRecordingsContext(TreeNode[] recordingNodes, bool checkNonExisting)
        {
            _checkNonExisting = checkNonExisting;

            List<TreeNode> nodes = new List<TreeNode>();
            foreach(TreeNode recordingNode in recordingNodes)
            {
                if (ValidateNode(recordingNode))
                {
                    nodes.Add((TreeNode)recordingNode.Clone());
                }
            }
            _recordingNodes = nodes.ToArray();
        }

        public bool ValidateNode(TreeNode recordingNode)
        {
            if (recordingNode.Nodes.Count == 0)
            {
                Recording recording = recordingNode.Tag as Recording;
                if (recording != null)
                {
                    if (!_checkNonExisting
                        || File.Exists(recording.RecordingFileName))
                    {
                        return recording.RecordingStopTime.HasValue;
                    }
                }
            }
            return true;
        }

        public TreeNode[] RecordingNodes
        {
            get { return _recordingNodes; }
        }

        public List<RecordingSummary> ExportRecordings
        {
            get { return _exportRecordings; }
        }
    }
}
