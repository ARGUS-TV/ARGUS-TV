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

namespace ArgusTV.UI.Console.UserControls
{
    public partial class ScheduleDaysOfWeekControl : UserControl
    {
        public event EventHandler ScheduleDaysOfWeekChanged;

        public ScheduleDaysOfWeekControl()
        {
            InitializeComponent();
        }

        [DefaultValue(ScheduleDaysOfWeek.None)]
        public ScheduleDaysOfWeek ScheduleDaysOfWeek
        {
            get
            {
                ScheduleDaysOfWeek daysOfWeek = ScheduleDaysOfWeek.None;
                if (_mondayCheckBox.Checked)
                {
                    daysOfWeek |= ScheduleDaysOfWeek.Mondays;
                }
                if (_tuesdayCheckBox.Checked)
                {
                    daysOfWeek |= ScheduleDaysOfWeek.Tuesdays;
                }
                if (_wednesdayCheckBox.Checked)
                {
                    daysOfWeek |= ScheduleDaysOfWeek.Wednesdays;
                }
                if (_thursdayCheckBox.Checked)
                {
                    daysOfWeek |= ScheduleDaysOfWeek.Thursdays;
                }
                if (_fridayCheckBox.Checked)
                {
                    daysOfWeek |= ScheduleDaysOfWeek.Fridays;
                }
                if (_saturdayCheckBox.Checked)
                {
                    daysOfWeek |= ScheduleDaysOfWeek.Saturdays;
                }
                if (_sundayCheckBox.Checked)
                {
                    daysOfWeek |= ScheduleDaysOfWeek.Sundays;
                }
                return daysOfWeek;
            }
            set
            {
                _mondayCheckBox.Checked = (value & ScheduleDaysOfWeek.Mondays) != 0;
                _tuesdayCheckBox.Checked = (value & ScheduleDaysOfWeek.Tuesdays) != 0;
                _wednesdayCheckBox.Checked = (value & ScheduleDaysOfWeek.Wednesdays) != 0;
                _thursdayCheckBox.Checked = (value & ScheduleDaysOfWeek.Thursdays) != 0;
                _fridayCheckBox.Checked = (value & ScheduleDaysOfWeek.Fridays) != 0;
                _saturdayCheckBox.Checked = (value & ScheduleDaysOfWeek.Saturdays) != 0;
                _sundayCheckBox.Checked = (value & ScheduleDaysOfWeek.Sundays) != 0;
            }
        }

        private void _checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ScheduleDaysOfWeekChanged != null)
            {
                this.ScheduleDaysOfWeekChanged(this, EventArgs.Empty);
            }
        }
    }
}
