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
using System.Linq;
using System.Text;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using ArgusTV.UI.Process.Guide;

namespace ArgusTV.WinForms.Controls
{
    public class EpgTimeControl : ScrollableControl
    {
        private readonly EpgTimeGridControl _epgTimeGridControl = new EpgTimeGridControl();
        private static readonly float _widthFactor;

        static EpgTimeControl()
        {
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                _widthFactor = (graphics.DpiX / 96);
            }
        }

        public EpgTimeControl()
        {
            base.BackColor = Color.FromArgb(0xf5, 0xf5, 0xf5);
            this.Controls.Add(_epgTimeGridControl);
        }

        public int ScrollLeft
        {
            get { return _epgTimeGridControl.Left; }
            set { _epgTimeGridControl.Left = value; }
        }

        public TimeSpan? CursorAtTime
        {
            get { return _epgTimeGridControl.CursorAtTime; }
            set { _epgTimeGridControl.CursorAtTime = value; }
        }

        internal static int GetTimeCursorPosition(TimeSpan cursorAtTime, int offset)
        {
            if (cursorAtTime.TotalHours < EpgControl.EpgHoursOffset)
            {
                cursorAtTime = cursorAtTime.Add(TimeSpan.FromHours(24 - EpgControl.EpgHoursOffset));
            }
            else
            {
                cursorAtTime = cursorAtTime.Subtract(TimeSpan.FromHours(EpgControl.EpgHoursOffset));
            }
            return Math.Max(0, (int)(cursorAtTime.TotalMinutes * 4 * _widthFactor) + offset);
        }

        internal static string GetTimeString(TimeSpan timeSpan)
        {
            return DateTime.Today.Add(timeSpan).ToString("t", CultureInfo.CurrentCulture);
        }
    }
}
