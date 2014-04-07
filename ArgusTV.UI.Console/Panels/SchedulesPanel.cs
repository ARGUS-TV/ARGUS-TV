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
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using ArgusTV.DataContracts;
using ArgusTV.UI.Process;
using ArgusTV.WinForms;

namespace ArgusTV.UI.Console.Panels
{
    public partial class SchedulesPanel : ContentPanel
    {
        private static readonly float _widthFactor;
        private static readonly float _heightFactor;

        static SchedulesPanel()
        {
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                _widthFactor = graphics.DpiX / 96;
                _heightFactor = graphics.DpiY / 96;
            }
        }

        public SchedulesPanel()
        {
            InitializeComponent();
            _iconColumn.ValuesAreIcons = true;
        }

        public override string Title
        {
            get { return _scheduleType.ToString() + " Schedules"; ; }
        }

        public SchedulesPanel(ScheduleType scheduleType)
        {
            InitializeComponent();
            _iconColumn.ValuesAreIcons = true;
            _scheduleType = scheduleType;
            _schedulesDataGridView.ColumnHeadersHeight = (int)(23 * _heightFactor);
            WinFormsUtility.ResizeDataGridViewColumns(_schedulesDataGridView, _widthFactor);
        }

        private ScheduleType _scheduleType = ScheduleType.Recording;

        public ScheduleType ScheduleType
        {
            get { return _scheduleType; }
            set { _scheduleType = value; }
        }

        public ChannelType ChannelType
        {
            get { return (ChannelType)_channelTypeComboBox.SelectedIndex; }
        }

        private void SchedulesPanel_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                _createManualScheduleButton.Visible = (_scheduleType == ScheduleType.Recording);

                _channelTypeComboBox.SelectedIndex = (int)ChannelType.Television;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        public override void OnClosed()
        {
            _schedulesBindingSource.DataSource = null;
            _schedulesBindingSource.ResetBindings(false);
        }

        public override void OnChildClosed(ContentPanel childPanel)
        {
            if (childPanel.DialogResult == DialogResult.OK)
            {
                LoadAllSchedules(false);
            }
        }

        private void _createNewButton_Click(object sender, EventArgs e)
        {
            try
            {
                EditSchedulePanel editPanel = new EditSchedulePanel();
                editPanel.Schedule = MainForm.SchedulerProxy.CreateNewSchedule(this.ChannelType, _scheduleType);
                editPanel.OpenPanel(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _createManualScheduleButton_Click(object sender, EventArgs e)
        {
            try
            {
                EditSchedulePanel editPanel = new EditSchedulePanel();
                editPanel.ForceManualSchedule = true;
                editPanel.Schedule = MainForm.SchedulerProxy.CreateNewSchedule(this.ChannelType, _scheduleType);
                editPanel.OpenPanel(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAllSchedules(bool deleteObsolete)
        {
            try
            {
                var schedules = MainForm.SchedulerProxy.GetAllSchedules(this.ChannelType, _scheduleType, deleteObsolete);
                _schedulesBindingSource.DataSource = new SortableBindingList<ScheduleSummary>(schedules);
                _schedulesBindingSource.ResetBindings(false);
                EnableButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EnableButtons()
        {
            bool multiSelect = (_schedulesDataGridView.SelectedRows.Count > 1);
            bool singleSelect = (_schedulesDataGridView.SelectedRows.Count == 1);
            _editButton.Enabled = singleSelect;
            _deleteButton.Enabled = singleSelect || multiSelect;
            _exportButton.Enabled = singleSelect || multiSelect;
        }

        private void _schedulesDataGridView_DoubleClick(object sender, EventArgs e)
        {
            EditSelectedSchedule();
        }

        private void _editButton_Click(object sender, EventArgs e)
        {
            EditSelectedSchedule();
        }

        private void _exportButton_Click(object sender, EventArgs e)
        {
            try
            {
                List<Schedule> schedules = new List<Schedule>();

                DialogResult result = MessageBox.Show(this,
                    "Hit 'Yes' to export all schedules or 'No' to only" + Environment.NewLine + "export the selected schedule(s).",
                    "Export", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    foreach (DataGridViewRow row in _schedulesDataGridView.Rows)
                    {
                        ScheduleSummary scheduleSummary = row.DataBoundItem as ScheduleSummary;
                        schedules.Add(MainForm.SchedulerProxy.GetScheduleById(scheduleSummary.ScheduleId));
                    }
                }
                else if (result == DialogResult.No)
                {
                    foreach (DataGridViewRow selectedRow in _schedulesDataGridView.SelectedRows)
                    {
                        ScheduleSummary scheduleSummary = selectedRow.DataBoundItem as ScheduleSummary;
                        schedules.Add(MainForm.SchedulerProxy.GetScheduleById(scheduleSummary.ScheduleId));
                    }
                }
                if (schedules.Count > 0)
                {
                    ExportScheduleList exportList = new ExportScheduleList(MainForm.SchedulerProxy, MainForm.ControlProxy, schedules);

                    _saveFileDialog.FileName = _scheduleType.ToString() + "Schedules_" + DateTime.Today.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xml";
                    if (_saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        exportList.Serialize(_saveFileDialog.FileName);

                        MessageBox.Show(this, String.Format("{0} schedule(s) exported.", schedules.Count), "Export",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _importButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (_openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    List<ImportSchedule> schedules;

                    bool retry = false;
                    do
                    {
                        ExportScheduleList scheduleList = ExportScheduleList.Deserialize(_openFileDialog.FileName);

                        List<string> errors;
                        schedules = scheduleList.Convert(MainForm.SchedulerProxy, out errors);

                        if (errors.Count > 0)
                        {
                            StringBuilder errorMessage = new StringBuilder("The following errors occurred:");
                            errorMessage.AppendLine();
                            errorMessage.AppendLine();
                            foreach (string error in errors)
                            {
                                errorMessage.Append(" - ").Append(error).AppendLine();
                            }

                            DialogResult result = MessageBox.Show(this, errorMessage.ToString(), "Import",
                                MessageBoxButtons.RetryCancel, MessageBoxIcon.Question);
                            retry = (result == DialogResult.Retry);
                            if (result != DialogResult.Ignore)
                            {
                                schedules.Clear();
                            }
                        }
                    }
                    while (retry);

                    if (schedules.Count > 0)
                    {
                        bool importHistory = !ImportSchedulesHaveHistory(schedules)
                            || (DialogResult.Yes == MessageBox.Show(this, "Do you also want to import the previously recorded"
                                    + Environment.NewLine + "history of the schedules.",
                                    "Import", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2));

                        foreach (ImportSchedule schedule in schedules)
                        {
                            Schedule importedSchedule = MainForm.SchedulerProxy.SaveSchedule(schedule.Schedule);
                            if (importHistory
                                && schedule.History.Length > 0)
                            {
                                MainForm.ControlProxy.ImportPreviouslyRecordedHistory(importedSchedule.ScheduleId, schedule.History);
                            }
                        }
                        LoadAllSchedules(false);
                        MessageBox.Show(this, String.Format("{0} schedule(s) imported.", schedules.Count), "Import",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ImportSchedulesHaveHistory(List<ImportSchedule> schedules)
        {
            foreach (ImportSchedule schedule in schedules)
            {
                if (schedule.History.Length > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void _deleteButton_Click(object sender, EventArgs e)
        {
            if (_schedulesDataGridView.SelectedRows.Count > 0
                && DialogResult.Yes == MessageBox.Show(this, "Are you sure you want to delete all the highlighted schedules?", "Delete",
                                                       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
            {
                try
                {
                    foreach (DataGridViewRow selectedRow in _schedulesDataGridView.SelectedRows)
                    {
                        ScheduleSummary scheduleSummary = selectedRow.DataBoundItem as ScheduleSummary;
                        MainForm.SchedulerProxy.DeleteSchedule(scheduleSummary.ScheduleId);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                LoadAllSchedules(false);
                _schedulesDataGridView.ClearSelection();
            }
        }

        private void _deleteObsoleteButton_Click(object sender, EventArgs e)
        {
            LoadAllSchedules(true);
        }

        private void EditSelectedSchedule()
        {
            ScheduleSummary schedule = GetSelectedSchedule();
            if (schedule != null)
            {
                try
                {
                    EditSchedulePanel editPanel = new EditSchedulePanel();
                    editPanel.Schedule = MainForm.SchedulerProxy.GetScheduleById(schedule.ScheduleId);
                    editPanel.OpenPanel(this);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private ScheduleSummary GetSelectedSchedule()
        {
            ScheduleSummary schedule = null;
            if (_schedulesDataGridView.SelectedRows.Count == 1)
            {
                schedule = _schedulesDataGridView.SelectedRows[0].DataBoundItem as ScheduleSummary;
            }
            return schedule;
        }

        private void _schedulesDataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            try
            {
                if (_schedulesDataGridView.SelectedRows.Count > 0)
                {
                    DataGridViewRow row = _schedulesDataGridView.SelectedRows[0];
                    if (row.Cells[0].Selected
                        && row.Cells[0].Value != null
                        && (bool)row.Cells[0].EditedFormattedValue != (bool)row.Cells[0].Value)
                    {
                        ScheduleSummary schedule = row.DataBoundItem as ScheduleSummary;
                        schedule.IsActive = (bool)row.Cells[0].EditedFormattedValue;
                        ScheduleSummary modifiedSchedule = MainForm.SchedulerProxy.SaveScheduleSummary(schedule);
                        SortableBindingList<ScheduleSummary> schedulesList = (SortableBindingList<ScheduleSummary>)_schedulesBindingSource.DataSource;
                        int index = schedulesList.IndexOf(schedule);
                        schedulesList[index] = modifiedSchedule;
                        _schedulesBindingSource.ResetBindings(false);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _schedulesDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex != 0
                && e.RowIndex >= 0
                && e.RowIndex < _schedulesBindingSource.Count)
            {
                ScheduleSummary schedule = _schedulesDataGridView.Rows[e.RowIndex].DataBoundItem as ScheduleSummary;
                if (schedule != null
                    && schedule.IsActive)
                {
                    e.CellStyle.ForeColor = Color.Black;
                    e.CellStyle.SelectionForeColor = Color.Black;
                }
                else
                {
                    e.CellStyle.ForeColor = Color.DarkGray;
                    e.CellStyle.SelectionForeColor = Color.Gray;
                }
            }
        }

        private void _schedulesDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }

        private void _schedulesDataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in _schedulesDataGridView.Rows)
            {
                Icon icon = Properties.Resources.TransparentIcon;
                ScheduleSummary schedule = row.DataBoundItem as ScheduleSummary;
                if (schedule != null)
                {
                    icon = ProgramIconUtility.GetIcon(_scheduleType, !schedule.IsOneTime);
                }
                DataGridViewImageCell cell = (DataGridViewImageCell)row.Cells[1];
                cell.Value = icon;
                cell.ValueIsIcon = true;
            }
        }

        private void _channelTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAllSchedules(false);
        }
    }
}
