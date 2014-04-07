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
using ArgusTV.UI.Console.Properties;
using ArgusTV.WinForms;

namespace ArgusTV.UI.Console.UserControls
{
    public partial class LogListControl : UserControl
    {
        public LogListControl()
        {
            InitializeComponent();
            _iconColumn.ValuesAreIcons = true;
            WinFormsUtility.ResizeDataGridViewColumnsForCurrentDpi(_logDataGridView);
        }

        public SortableBindingList<LogEntry> LogEntries
        {
            get { return _logDataGridView.DataSource as SortableBindingList<LogEntry>; }
            set
            {
                _logDataGridView.AutoGenerateColumns = false;
                _logDataGridView.DataSource = value;
            }
        }

        private void _logDataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in _logDataGridView.Rows)
            {
                Icon icon = Properties.Resources.TransparentIcon;
                string toolTip = null;
                LogEntry logEntry = row.DataBoundItem as LogEntry;
                if (logEntry != null)
                {
                    toolTip = logEntry.LogSeverity.ToString();
                    switch (logEntry.LogSeverity)
                    {
                        case LogSeverity.Error:
                        case LogSeverity.Fatal:
                            icon = Resources.LogError;
                            break;

                        case LogSeverity.Warning:
                            icon = Resources.LogWarning;
                            break;

                        case LogSeverity.Information:
                            icon = Resources.LogInformation;
                            break;
                    }
                }
                row.Cells[0].Value = icon;
                row.Cells[0].ToolTipText = toolTip;
            }

        }
    }
}
