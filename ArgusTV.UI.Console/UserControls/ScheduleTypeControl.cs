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
    public partial class ScheduleTypeControl : UserControl
    {
        private static class ScheduleTypeIndex
        {
            public const int Recording = 0;
            public const int Suggestion = 1;
            public const int Alert = 2;
        }

        public event EventHandler ScheduleTypeChanged;

        public ScheduleTypeControl()
        {
            InitializeComponent();
            _scheduleTypeComboBox.SelectedIndexChanged += new EventHandler(_scheduleTypeComboBox_SelectedIndexChanged);
            this.ScheduleType = ScheduleType.Recording;
        }

        void _scheduleTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ScheduleTypeChanged != null)
            {
                this.ScheduleTypeChanged(this, EventArgs.Empty);
            }
        }

        public ScheduleType ScheduleType
        {
            get
            {
                if (_scheduleTypeComboBox.SelectedIndex == ScheduleTypeIndex.Suggestion)
                {
                    return ScheduleType.Suggestion;
                }
                else if (_scheduleTypeComboBox.SelectedIndex == ScheduleTypeIndex.Alert)
                {
                    return ScheduleType.Alert;
                }
                return ScheduleType.Recording;
            }
            set
            {
                if (value == ScheduleType.Suggestion)
                {
                    _scheduleTypeComboBox.SelectedIndex = ScheduleTypeIndex.Suggestion;
                }
                else if (value == ScheduleType.Alert)
                {
                    _scheduleTypeComboBox.SelectedIndex = ScheduleTypeIndex.Alert;
                }
                else
                {
                    _scheduleTypeComboBox.SelectedIndex = ScheduleTypeIndex.Recording;
                }
            }
        }
    }
}
