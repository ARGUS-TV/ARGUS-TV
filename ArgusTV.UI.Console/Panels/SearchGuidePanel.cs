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
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using ArgusTV.DataContracts;
using ArgusTV.UI.Process.Guide;
using ArgusTV.UI.Process.SearchGuide;
using ArgusTV.UI.Process;
using ArgusTV.WinForms;
using ArgusTV.ServiceProxy;

namespace ArgusTV.UI.Console.Panels
{
    public partial class SearchGuidePanel : ContentPanel
    {
        private SearchGuideModel _model;
        private SearchGuideController _controller;

        public SearchGuidePanel()
        {
            InitializeComponent();
            WinFormsUtility.ResizeDataGridViewColumnsForCurrentDpi(_titlesGridView);
        }

        public override string Title
        {
            get { return "Search Guide"; }
        }

        private ChannelProgram GetSelectedProgram()
        {
            ChannelProgramView tvChannelProgramView = null;
            if (_tvChannelProgramsControl.SelectedRows.Count > 0)
            {
                tvChannelProgramView = _tvChannelProgramsControl.SelectedRows[0].DataBoundItem as ChannelProgramView;
            }
            return tvChannelProgramView == null ? null : tvChannelProgramView.Program;
        }

        private void UpcomingProgramsPanel_Load(object sender, EventArgs e)
        {
            _model = new SearchGuideModel();
            _controller = new SearchGuideController(_model);
            _controller.Initialize();
            _channelTypeComboBox.SelectedIndex = (int)ChannelType.Television;
            _titlesPanel.Visible = true;
            _programsPanel.Visible = false;
            _searchTextBox.Select();
        }

        public override void OnClosed()
        {
            _titlesGridView.DataSource = null;
            _tvChannelProgramsControl.TvChannelPrograms = null;
        }

        public override void OnChildClosed(ContentPanel childPanel)
        {
            if (childPanel.DialogResult == DialogResult.OK)
            {
                RefreshAllUpcomingPrograms();
            }
        }

        private void _searchButton_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                _controller.SearchTitles(_searchTextBox.Text);
                ShowFoundTitles();
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

        private void ShowFoundTitles()
        {
            _titlesGridView.DataSource = ProgramTitle.ConvertArray(_model.Titles);
            _titlesPanel.Visible = true;
            _programsPanel.Visible = false;
            EnableButtons();
        }

        private void EnableButtons()
        {
            _showProgramsButton.Enabled = (_titlesGridView.SelectedRows.Count > 0);
        }

        [Obfuscation(Exclude = true)]
        private class ProgramTitle
        {
            public ProgramTitle(string title)
            {
                _title = title;
            }

            private string _title;

            public string Title
            {
                get { return _title; }
            }

            public static List<ProgramTitle> ConvertArray(List<string> titles)
            {
                List<ProgramTitle> programTitles = new List<ProgramTitle>();
                foreach (string title in titles)
                {
                    programTitles.Add(new ProgramTitle(title));
                }
                return programTitles;
            }
        }

        private void RefreshAllUpcomingPrograms()
        {
            _controller.RefreshAllUpcomingPrograms();
            _tvChannelProgramsControl.AllUpcomingPrograms = _model.AllUpcomingGuidePrograms;
            _tvChannelProgramsControl.RefreshIcons();
        }

        private void _searchTextBox_Enter(object sender, EventArgs e)
        {
            MainForm.AcceptButton = _searchButton;
        }

        private void _searchTextBox_Leave(object sender, EventArgs e)
        {
            MainForm.AcceptButton = null;
        }

        private void _tvChannelProgramsControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DataGridView.HitTestInfo htinfo = _tvChannelProgramsControl.HitTest(e.X, e.Y);
                if (htinfo.Type == DataGridViewHitTestType.Cell)
                {
                    ChannelProgramView programView = _tvChannelProgramsControl.Rows[htinfo.RowIndex].DataBoundItem as ChannelProgramView;
                    if (programView != null)
                    {
                        GuideUpcomingProgram upcomingProgramInfo = null;
                        _model.AllUpcomingGuidePrograms.TryGetValue(programView.UpcomingProgramId, out upcomingProgramInfo);

                        _tvChannelProgramsControl.Rows[htinfo.RowIndex].Selected = true;

                        ScheduleType? scheduleType = null;
                        UpcomingProgram upcomingProgram = null;
                        UpcomingGuideProgram upcomingGuideProgram = null;
                        if (upcomingProgramInfo != null
                            && upcomingProgramInfo.ChannelId == programView.Program.Channel.ChannelId)
                        {
                            scheduleType = upcomingProgramInfo.Type;
                            upcomingProgram = upcomingProgramInfo.UpcomingRecording != null ? upcomingProgramInfo.UpcomingRecording.Program : null;
                            upcomingGuideProgram = upcomingProgramInfo.UpcomingGuideProgram;
                        }

                        _programContextMenuStrip.SetTarget(programView.Program.Channel, programView.Program.GuideProgramId,
                            programView.Title, programView.SubTitle, programView.EpisodeNumberDisplay, programView.StartTime,
                            scheduleType, upcomingProgram, upcomingGuideProgram);
                        _programContextMenuStrip.Show(_tvChannelProgramsControl, e.Location);
                    }
                }
            }
        }

        private void _channelTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _titlesGridView.DataSource = new List<ProgramTitle>();
            _titlesPanel.Visible = true;
            _programsPanel.Visible = false;
            EnableButtons();
            _controller.SetChannelType((ChannelType)_channelTypeComboBox.SelectedIndex);
        }

        private void _titlesGridView_DoubleClick(object sender, EventArgs e)
        {
            _showProgramsButton_Click(sender, e);
        }

        private void _showProgramsButton_Click(object sender, EventArgs e)
        {
            if (_titlesGridView.SelectedRows.Count > 0)
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    string title = ((ProgramTitle)_titlesGridView.SelectedRows[0].DataBoundItem).Title;
                    _controller.GetProgramsForTitle(title);
                    _tvChannelProgramsControl.AllUpcomingPrograms = _model.AllUpcomingGuidePrograms;
                    _tvChannelProgramsControl.TvChannelPrograms = _model.CurrentTitlePrograms;
                    _titlesPanel.Visible = false;
                    _programsPanel.Visible = true;
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

        private void _titlesGridView_SelectionChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }

        private void _backButton_Click(object sender, EventArgs e)
        {
            _titlesPanel.Visible = true;
            _programsPanel.Visible = false;
        }

        #region Program Context Menu

        private void _programContextMenuStrip_CreateNewSchedule(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.CreateNewScheduleEventArgs e)
        {
            Utility.ContextCreateNewSchedule(this, e);
        }

        private void _programContextMenuStrip_EditSchedule(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.EditScheduleEventArgs e)
        {
            Utility.ContextEditSchedule(this, e);
        }

        private void _programContextMenuStrip_DeleteSchedule(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.EditScheduleEventArgs e)
        {
            if (Utility.ContextDeleteSchedule(this, e))
            {
                RefreshAllUpcomingPrograms();
            }
        }

        private void _programContextMenuStrip_CancelProgram(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.CancelProgramEventArgs e)
        {
            if (Utility.ContextCancelProgram(this, e))
            {
                RefreshAllUpcomingPrograms();
            }
        }

        private void _programContextMenuStrip_AddRemoveProgramHistory(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.AddRemoveProgramHistoryEventArgs e)
        {
            if (Utility.ContextAddRemoveProgramHistory(this, e))
            {
                RefreshAllUpcomingPrograms();
            }
        }

        private void _programContextMenuStrip_SetProgramPriority(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.SetProgramPriorityEventArgs e)
        {
            if (Utility.ContextSetProgramPriority(this, e))
            {
                RefreshAllUpcomingPrograms();
            }
        }

        private void _programContextMenuStrip_SetProgramPrePostRecord(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.SetProgramPrePostRecordEventArgs e)
        {
            if (Utility.ContextSetProgramPrePostRecord(this, e))
            {
                RefreshAllUpcomingPrograms();
            }
        }

        #endregion
    }
}
