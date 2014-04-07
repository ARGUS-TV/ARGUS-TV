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
    public partial class SchedulePriorityControl : UserControl
    {
        private static class SchedulePriorityIndex
        {
            public const int VeryLow = 0;
            public const int Low = 1;
            public const int Normal = 2;
            public const int High = 3;
            public const int VeryHigh = 4;
        }

        public SchedulePriorityControl()
        {
            InitializeComponent();
            this.SchedulePriority = SchedulePriority.Normal;
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            _schedulePriorityComboBox.Width = this.Width - _schedulePriorityComboBox.Left;
        }

        public SchedulePriority SchedulePriority
        {
            get
            {
                return (SchedulePriority)(_schedulePriorityComboBox.SelectedIndex - 2);
            }
            set
            {
                _schedulePriorityComboBox.SelectedIndex = 2 + (int)value;
            }
        }
    }
}
