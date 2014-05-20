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
using System.Globalization;

using ArgusTV.DataContracts;
using ArgusTV.UI.Process;
using ArgusTV.WinForms;
using ArgusTV.ServiceProxy;

namespace ArgusTV.UI.Console.Panels
{
    public partial class ProcessingCommandsPanel : ContentPanel
    {
        private SortableBindingList<ProcessingCommand> _processingCommands;
        private HashSet<Guid> _changedCommandIds = new HashSet<Guid>();
        private List<ProcessingCommand> _deletedCommands;
        private ProcessingCommand _displayedCommand;

        public ProcessingCommandsPanel()
        {
            InitializeComponent();
            WinFormsUtility.ResizeDataGridViewColumnsForCurrentDpi(_commandsDataGridView);
            _changedCommandIds.Add(Guid.Empty);
        }

        public override string Title
        {
            get { return "Processing Commands"; }
        }

        private void ProcessingCommandsPanel_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                MainForm.SetMenuMode(MainMenuMode.SaveCancel);

                LoadAllCommands();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        public override void OnClosed()
        {
            MainForm.SetMenuMode(MainMenuMode.Normal);
            _commandsBindingSource.DataSource = null;
            _commandsBindingSource.ResetBindings(false);
        }

        private void LoadAllCommands()
        {
            try
            {
                _deletedCommands = new List<ProcessingCommand>();
                _processingCommands = new SortableBindingList<ProcessingCommand>(Proxies.SchedulerService.GetAllProcessingCommands());
                _commandsBindingSource.DataSource = _processingCommands;
                _commandsBindingSource.ResetBindings(false);
                UpdateSelectedCommand();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private ProcessingCommand GetSelectedCommand()
        {
            ProcessingCommand processingCommand = null;
            if (_commandsDataGridView.SelectedRows.Count > 0)
            {
                processingCommand = _commandsDataGridView.SelectedRows[0].DataBoundItem as ProcessingCommand;
            }
            return processingCommand;
        }

        private void UpdateSelectedCommand()
        {
            ProcessingCommand selectedCommand = GetSelectedCommand();
            _deleteButton.Enabled = (selectedCommand != null);
            _nameTextBox.Enabled = (selectedCommand != null);
            _commandTextBox.Enabled = (selectedCommand != null);
            _browseButton.Enabled = (selectedCommand != null);
            _argumentsTextBox.Enabled = (selectedCommand != null);
            _isDefaultTelevisionCheckBox.Enabled = (selectedCommand != null);
            _isDefaultRadioCheckBox.Enabled = (selectedCommand != null);
            _runModeComboBox.Enabled = (selectedCommand != null);
            _runAtTimePicker.Enabled = (selectedCommand != null);
            if (selectedCommand == null)
            {
                _nameTextBox.Text = String.Empty;
                _commandTextBox.Text = String.Empty;
                _argumentsTextBox.Text = String.Empty;
                _isDefaultTelevisionCheckBox.Checked = false;
                _isDefaultRadioCheckBox.Checked = false;
                _runModeComboBox.SelectedIndex = -1;
                _runAtTimePicker.Value = DateTime.Today;
            }
            else
            {
                _nameTextBox.Text = selectedCommand.Name;
                _commandTextBox.Text = selectedCommand.CommandPath;
                _argumentsTextBox.Text = selectedCommand.Arguments;
                _isDefaultTelevisionCheckBox.Checked = selectedCommand.IsDefaultTelevision;
                _isDefaultRadioCheckBox.Checked = selectedCommand.IsDefaultRadio;
                _runModeComboBox.SelectedIndex = (int)selectedCommand.RunMode;
                if (selectedCommand.RunMode == ProcessingRunMode.AtTime)
                {
                    DateTime atTime = DateTime.Today;
                    if (selectedCommand.RunAtHours.HasValue
                        && selectedCommand.RunAtMinutes.HasValue)
                    {
                        atTime = atTime.Add(new TimeSpan(selectedCommand.RunAtHours.Value, selectedCommand.RunAtMinutes.Value, 0));
                    }
                    _runAtTimePicker.Value = atTime;
                }
                else
                {
                    _runAtTimePicker.Value = DateTime.Today;
                    _runAtTimePicker.Enabled = false;
                }
            }
            _displayedCommand = selectedCommand;
        }

        private void _commandsDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(this, _commandsDataGridView.Columns[e.ColumnIndex].HeaderText + ": " + e.Exception.Message, null,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            e.ThrowException = false;
        }

        private void _commandsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            Apply();
            UpdateSelectedCommand();
        }

        public bool Apply()
        {
            if (_displayedCommand != null)
            {
                if (String.IsNullOrEmpty(_nameTextBox.Text)
                    || String.IsNullOrEmpty(_commandTextBox.Text))
                {
                    MessageBox.Show(this, "Command name and path must be set.", null,
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                string text = _nameTextBox.Text.Trim();
                if (_displayedCommand.Name != text
                    || _displayedCommand.CommandPath != _commandTextBox.Text
                    || _displayedCommand.Arguments != _argumentsTextBox.Text
                    || _displayedCommand.IsDefaultTelevision != _isDefaultTelevisionCheckBox.Checked
                    || _displayedCommand.IsDefaultRadio != _isDefaultRadioCheckBox.Checked
                    || _displayedCommand.RunMode != (ProcessingRunMode)_runModeComboBox.SelectedIndex)
                {
                    _changedCommandIds.Add(_displayedCommand.ProcessingCommandId);
                }
                _displayedCommand.Name = text;
                _displayedCommand.CommandPath = _commandTextBox.Text;
                _displayedCommand.Arguments = _argumentsTextBox.Text;
                _displayedCommand.IsDefaultTelevision = _isDefaultTelevisionCheckBox.Checked;
                _displayedCommand.IsDefaultRadio = _isDefaultRadioCheckBox.Checked;
                _displayedCommand.RunMode = (ProcessingRunMode)_runModeComboBox.SelectedIndex;
                if (_displayedCommand.RunMode == ProcessingRunMode.AtTime)
                {
                    if (_displayedCommand.RunAtHours != _runAtTimePicker.Value.Hour
                        || _displayedCommand.RunAtMinutes != _runAtTimePicker.Value.Minute)
                    {
                        _changedCommandIds.Add(_displayedCommand.ProcessingCommandId);
                    }
                    _displayedCommand.RunAtHours = _runAtTimePicker.Value.Hour;
                    _displayedCommand.RunAtMinutes = _runAtTimePicker.Value.Minute;
                }
                else
                {
                    if (_displayedCommand.RunAtHours != null
                        || _displayedCommand.RunAtMinutes != null)
                    {
                        _changedCommandIds.Add(_displayedCommand.ProcessingCommandId);
                    }
                    _displayedCommand.RunAtHours = null;
                    _displayedCommand.RunAtMinutes = null;
                }
            }
            return true;
        }

        public override void OnSave()
        {
            if (Apply())
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    foreach (ProcessingCommand command in _processingCommands)
                    {
                        if (_changedCommandIds.Contains(command.ProcessingCommandId)
                            && !String.IsNullOrEmpty(command.Name)
                            && !String.IsNullOrEmpty(command.CommandPath))
                        {
                            Proxies.SchedulerService.SaveProcessingCommand(command);
                        }
                    }

                    foreach (ProcessingCommand command in _deletedCommands)
                    {
                        if (command.ProcessingCommandId != Guid.Empty)
                        {
                            Proxies.SchedulerService.DeleteProcessingCommand(command.ProcessingCommandId);
                        }
                    }

                    _commandsBindingSource.RemoveSort();
                    ClosePanel();
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
        }

        public override void OnCancel()
        {
            _displayedCommand = null;
            _commandsBindingSource.RemoveSort();
            base.OnCancel();
        }

        private void _deleteButton_Click(object sender, EventArgs e)
        {
            ProcessingCommand command = GetSelectedCommand();
            if (command != null)
            {
                _processingCommands.Remove(command);
                _deletedCommands.Add(command);
            }
        }

        private void _createNewButton_Click(object sender, EventArgs e)
        {
            ProcessingCommand command = new ProcessingCommand();
            command.Name = "-Unnamed-";
            command.Arguments = "%%FILE%%";
            command.RunMode = ProcessingRunMode.Post;
            _commandsBindingSource.Add(command);
            _commandsDataGridView.CurrentCell = _commandsDataGridView.Rows[_commandsDataGridView.Rows.Count - 1].Cells[0];
        }

        private void _browseButton_Click(object sender, EventArgs e)
        {
            if (_openCommandDialog.ShowDialog(this) == DialogResult.OK)
            {
                _commandTextBox.Text = _openCommandDialog.FileName;
            }
        }

        private void _runModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _runAtTimePicker.Enabled = (_runModeComboBox.SelectedIndex == (int)ProcessingRunMode.AtTime);
        }
    }
}
