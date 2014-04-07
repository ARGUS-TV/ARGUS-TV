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
using ArgusTV.UI.Process.Guide;
using ArgusTV.UI.Process;
using ArgusTV.WinForms;
using ArgusTV.ServiceProxy;

namespace ArgusTV.UI.Console.UserControls
{
    public partial class TvChannelProgramsControl : UserControl
    {
        public TvChannelProgramsControl()
        {
            InitializeComponent();
            _programsDataGridView.MouseUp += new MouseEventHandler(_programsDataGridView_MouseUp);
            _iconColumn.ValuesAreIcons = true;
            WinFormsUtility.ResizeDataGridViewColumnsForCurrentDpi(_programsDataGridView);
        }

        public event MouseEventHandler GridMouseUp;

        public bool Sortable
        {
            get
            {
                return (_programsDataGridView.Columns[1].SortMode == DataGridViewColumnSortMode.Automatic);
            }
            set
            {
                foreach (DataGridViewColumn column in _programsDataGridView.Columns)
                {
                    if (column.Index > 0)
                    {
                        column.SortMode = DataGridViewColumnSortMode.Automatic;
                    }
                }
            }
        }

        private UpcomingGuideProgramsDictionary _allUpcomingPrograms;

        public UpcomingGuideProgramsDictionary AllUpcomingPrograms
        {
            get { return _allUpcomingPrograms; }
            set { _allUpcomingPrograms = value; }
        }

        public ChannelProgramsList TvChannelPrograms
        {
            get
            {
                return _programsListBindingSource.DataSource as ChannelProgramsList;
            }
            set
            {
                _programsListBindingSource.DataSource = value;
                _programsListBindingSource.ResetBindings(false);
            }
        }

        public DataGridViewRowCollection Rows
        {
            get { return _programsDataGridView.Rows; }
        }

        public DataGridViewSelectedRowCollection SelectedRows
        {
            get { return _programsDataGridView.SelectedRows; }
        }

        public void RefreshIcons()
        {
            foreach (DataGridViewRow row in _programsDataGridView.Rows)
            {
                Icon icon = Properties.Resources.TransparentIcon;
                string toolTip = null;
                ChannelProgramView programView = row.DataBoundItem as ChannelProgramView;
                if (programView != null)
                {
                    if (_allUpcomingPrograms.ContainsKey(programView.UpcomingProgramId))
                    {
                        GuideUpcomingProgram programInfo = _allUpcomingPrograms[programView.UpcomingProgramId];

                        ArgusTV.WinForms.ProgramIconUtility.GetIconAndToolTip(programInfo.Type, programInfo.CancellationReason, programInfo.IsPartOfSeries,
                                _allUpcomingPrograms.UpcomingRecordings, programInfo.UpcomingRecording, out icon, out toolTip);
                    }
                }
                row.Cells[0].Value = icon;
                row.Cells[0].ToolTipText = toolTip;
            }
        }

        public DataGridView.HitTestInfo HitTest(int x, int y)
        {
            return _programsDataGridView.HitTest(x, y);
        }

        void _programsDataGridView_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.GridMouseUp != null)
            {
                this.GridMouseUp(this, e);
            }
        }

        private void _programsDataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            RefreshIcons();
        }

        private void _programsDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3
                && e.RowIndex >= 0
                && e.RowIndex < _programsDataGridView.Rows.Count)
            {
                ChannelProgramView programView = _programsDataGridView.Rows[e.RowIndex].DataBoundItem as ChannelProgramView;
                GuideProgram guideProgram = Proxies.GuideService.GetProgramById(programView.Program.GuideProgramId).Result;
                using (ProgramDetailsPopup popup = new ProgramDetailsPopup())
                {
                    popup.Channel = programView.Program.Channel;
                    popup.GuideProgram = guideProgram;
                    Point location = Cursor.Position;
                    location.Offset(-250, -40);
                    popup.Location = location;
                    popup.ShowDialog(this);
                }
            }
        }
    }
}
