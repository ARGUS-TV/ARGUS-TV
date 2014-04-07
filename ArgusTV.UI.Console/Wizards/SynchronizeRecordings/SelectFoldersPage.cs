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
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using ArgusTV.DataContracts;
using ArgusTV.ServiceProxy;
using ArgusTV.WinForms;

namespace ArgusTV.UI.Console.Wizards.SynchronizeRecordings
{
    internal partial class SelectFoldersPage : SynchronizeRecordingsPageBase
    {
        private List<RecFolder> _folders = new List<RecFolder>();

        public SelectFoldersPage()
        {
            InitializeComponent();
            WinFormsUtility.ResizeDataGridViewColumnsForCurrentDpi(_foldersDataGridView);
        }

        public override string PageTitle
        {
            get { return "Recording Folders"; }
        }

        public override string PageInformation
        {
            get { return "Please select all UNC recording paths and video extensions to scan."; }
        }

        public override void OnEnterPage(bool isBack)
        {
            _extensionsTextBox.Text = Properties.Settings.Default.SynchronizeExtensions;

            if (!isBack)
            {
                EnableButtons();
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    this.Context.RecordingFolders.Clear();
                    foreach (string share in Proxies.ControlService.GetRecordingShares().Result)
                    {
                        if (!String.IsNullOrWhiteSpace(share)
                            && !ContainsFolder(this.Context.RecordingFolders, share))
                        {
                            this.Context.RecordingFolders.Add(share);
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

            _folders.Clear();
            foreach (string folder in this.Context.RecordingFolders)
            {
                _folders.Add(new RecFolder(folder));
            }
            _foldersBindingSource.DataSource = _folders;
            _foldersBindingSource.ResetBindings(true);
            EnableButtons();
        }

        private static bool ContainsFolder(List<string> folders, string path)
        {
            foreach (string folder in folders)
            {
                if (folder.Equals(path, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public override void OnLeavePage(bool isBack)
        {
            if (!isBack)
            {
                Properties.Settings.Default.SynchronizeExtensions = _extensionsTextBox.Text;
                Properties.Settings.Default.Save();

                this.Context.RecordingFolders.Clear();
                foreach (RecFolder folder in _folders)
                {
                    this.Context.RecordingFolders.Add(folder.Folder);
                }
            }
        }

        public override bool IsNextAllowed()
        {
            foreach (RecFolder folder in _folders)
            {
                if (!folder.Folder.StartsWith(@"\\"))
                {
                    MessageBox.Show(this, folder.Folder + " is not a UNC path!", null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            return true;
        }

        [Obfuscation(Exclude = true)]
        private class RecFolder
        {
            public RecFolder(string folder)
            {
                this.Folder = folder;
            }

            public string Folder { get; set; }
        }

        private void EnableButtons()
        {
            bool rowSelected = (_foldersDataGridView.SelectedRows.Count > 0);
            _deleteButton.Enabled = rowSelected;
            _openButton.Enabled = rowSelected;
        }

        private void _addFolderButton_Click(object sender, EventArgs e)
        {
            _foldersBindingSource.Add(new RecFolder(@"\\MACHINE\Share"));
            _foldersDataGridView.CurrentCell = _foldersDataGridView.Rows[_foldersDataGridView.Rows.Count - 1].Cells[0];
            _foldersDataGridView.BeginEdit(true);
        }

        private void _deleteButton_Click(object sender, EventArgs e)
        {
            if (_foldersDataGridView.SelectedRows.Count > 0)
            {
                RecFolder folder = _foldersDataGridView.SelectedRows[0].DataBoundItem as RecFolder;
                _folders.Remove(folder);
                _foldersBindingSource.ResetBindings(false);
            }
        }

        private void _openButton_Click(object sender, EventArgs e)
        {
            if (_foldersDataGridView.SelectedRows.Count > 0)
            {
                try
                {
                    RecFolder folder = _foldersDataGridView.SelectedRows[0].DataBoundItem as RecFolder;
                    ProcessStartInfo startInfo = new ProcessStartInfo(folder.Folder);
                    startInfo.WindowStyle = ProcessWindowStyle.Normal;
                    startInfo.UseShellExecute = true;
                    System.Diagnostics.Process.Start(startInfo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void _foldersDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }
    }
}
