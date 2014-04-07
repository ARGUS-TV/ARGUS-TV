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

using ArgusTV.DataContracts;
using ArgusTV.UI.Process;
using ArgusTV.WinForms;

namespace ArgusTV.UI.Console.Panels
{
    public partial class ScheduleHistoryPanel : ContentPanel
    {
        private static readonly float _widthFactor;
        private static readonly float _heightFactor;

        static ScheduleHistoryPanel()
        {
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                _widthFactor = graphics.DpiX / 96;
                _heightFactor = graphics.DpiY / 96;
            }
        }

        public ScheduleHistoryPanel()
        {
            InitializeComponent();
            _schedulesDataGridView.ColumnHeadersHeight = (int)(23 * _heightFactor);
            WinFormsUtility.ResizeDataGridViewColumns(_schedulesDataGridView, _widthFactor);
        }

        public override string Title
        {
            get { return "Previously Recorded Programs History"; }
        }

        private Schedule _schedule;

        public Schedule Schedule
        {
            get { return _schedule; }
            set { _schedule = value; }
        }

        private SortableBindingList<ScheduleRecordedProgram> _recordedPrograms;
        private List<ScheduleRecordedProgram> _deletedPrograms;
        private bool _clearHistory;

        private void ScheduleHistoryPanel_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                MainForm.SetMenuMode(MainMenuMode.SaveCancel);
                _nameTextBox.Text = _schedule.Name;
                LoadHistory();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        public override void OnClosed()
        {
            _historyBindingSource.DataSource = null;
            _historyBindingSource.ResetBindings(false);
        }

        public override void OnSave()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                if (_clearHistory)
                {
                    MainForm.ControlProxy.ClearPreviouslyRecordedHistory(_schedule.ScheduleId);
                }
                else
                {
                    foreach (ScheduleRecordedProgram recordedProgram in _deletedPrograms)
                    {
                        MainForm.ControlProxy.DeleteFromPreviouslyRecordedHistory(recordedProgram.ScheduleRecordedProgramId);
                    }
                }

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

        private void LoadHistory()
        {
            try
            {
                _deletedPrograms = new List<ScheduleRecordedProgram>();
                _clearHistory = false;
                _recordedPrograms = new SortableBindingList<ScheduleRecordedProgram>(
                    MainForm.ControlProxy.GetPreviouslyRecordedHistory(_schedule.ScheduleId));
                _historyBindingSource.DataSource = _recordedPrograms;
                _historyBindingSource.ResetBindings(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _deleteButton_Click(object sender, EventArgs e)
        {
            ScheduleRecordedProgram recordedProgram = GetSelectedRecordedProgram();
            if (recordedProgram != null)
            {
                _recordedPrograms.Remove(recordedProgram);
                _deletedPrograms.Add(recordedProgram);
            }
        }

        private void _clearHistoryButton_Click(object sender, EventArgs e)
        {
            _recordedPrograms.Clear();
            _deletedPrograms.Clear();
            _clearHistory = true;
        }

        private ScheduleRecordedProgram GetSelectedRecordedProgram()
        {
            ScheduleRecordedProgram recordedProgram = null;
            if (_schedulesDataGridView.SelectedRows.Count > 0)
            {
                recordedProgram = _schedulesDataGridView.SelectedRows[0].DataBoundItem as ScheduleRecordedProgram;
            }
            return recordedProgram;
        }
    }
}
