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
using System.Windows.Forms;
using System.Drawing;
using System.Globalization;

namespace ArgusTV.WinForms.Controls
{
    public class EpgTimeGridControl : Control
    {
        private List<IDisposable> _disposables = new List<IDisposable>();
        private Font _timeFont;
        private Brush _timeBrush;
        private Pen _epgBorderPen;
        private Pen _cursorPen;
        private Pen _cursorShadowPen;
        private Font _cursorFont;
        private Brush _cursorBrush;
        private Brush _cursorBgBrush;

        public EpgTimeGridControl()
        {
            base.BackColor = Color.FromArgb(0xf5, 0xf5, 0xf5);
            this.Name = "EpgTimeGridControl";
            this.Size = new Size(5760, 15);

            this.SetStyle(ControlStyles.Selectable, false);
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            _disposables.Add(_timeFont = new Font("Tahoma", 7F, FontStyle.Bold));
            _disposables.Add(_timeBrush = new SolidBrush(Color.FromArgb(0x66, 0x66, 0x66)));
            _disposables.Add(_epgBorderPen = new Pen(Color.FromArgb(0xD5, 0xD5, 0xD5)));
            _disposables.Add(_cursorFont = new Font("Tahoma", 8F, FontStyle.Bold));
            _disposables.Add(_cursorPen = new Pen(Color.FromArgb(0x7F, 0x90, 0x33, 0x33)));
            _disposables.Add(_cursorShadowPen = new Pen(Color.FromArgb(0x30, 0x33, 0x33, 0x33)));
            _disposables.Add(_cursorBrush = new SolidBrush(Color.FromArgb(0x90, 0x33, 0x33)));
            _disposables.Add(_cursorBgBrush = new SolidBrush(Color.FromArgb(0x9F, 0xF5, 0xF5, 0xF5)));
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed
                && disposing)
            {
                foreach (IDisposable disposable in _disposables)
                {
                    disposable.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private TimeSpan? _cursorAtTime;

        public TimeSpan? CursorAtTime
        {
            get { return _cursorAtTime; }
            set
            {
                if (_cursorAtTime != value)
                {
                    if (_cursorAtTime.HasValue)
                    {
                        int position = EpgTimeControl.GetTimeCursorPosition(_cursorAtTime.Value, 0);
                        this.Invalidate(new Rectangle(position - 50, 0, 100, this.Height));
                    }
                    _cursorAtTime = value;
                    if (_cursorAtTime.HasValue)
                    {
                        int position = EpgTimeControl.GetTimeCursorPosition(_cursorAtTime.Value, 0);
                        this.Invalidate(new Rectangle(position - 50, 0, 100, this.Height));
                    }
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            this.Parent.Parent.Focus();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Region clippingRegion = e.Graphics.Clip;

            TimeSpan time = TimeSpan.FromHours(EpgControl.EpgHoursOffset);
            int left = 0;
            const int step = 15;
            for (int count = 0; count < (24 * 60) / step; count++)
            {
                TimeSpan nextTime = time.Add(TimeSpan.FromMinutes(step));
                int nextLeft = EpgTimeControl.GetTimeCursorPosition(nextTime, 0);
                Rectangle visibleRectangle = new Rectangle(left - 1, 0, nextLeft - left + 1, this.Height);
                visibleRectangle.Intersect(e.ClipRectangle);

                Region cellRegion = new Region(visibleRectangle);
                cellRegion.Intersect(clippingRegion);
                if (!cellRegion.IsEmpty(e.Graphics))
                {
                    int lineLeft = Math.Max(0, left - 1);
                    e.Graphics.DrawLine(_epgBorderPen, lineLeft, 0, lineLeft, this.Height - 1);
                    string timeText = EpgTimeControl.GetTimeString(time);
                    e.Graphics.DrawString(timeText, _timeFont, _timeBrush, lineLeft + 1, 2);
                }

                left = nextLeft;
                time = nextTime;
            }

            if (this.CursorAtTime.HasValue)
            {
                int position = EpgTimeControl.GetTimeCursorPosition(_cursorAtTime.Value, -1);
                e.Graphics.DrawLine(_cursorPen, position, 0, position, this.Height - 1);
                e.Graphics.DrawLine(_cursorShadowPen, position + 1, 0, position + 1, this.Height - 1);

                string timeText = EpgTimeControl.GetTimeString(this.CursorAtTime.Value);
                SizeF size = e.Graphics.MeasureString(timeText, _cursorFont);

                e.Graphics.FillRectangle(_cursorBgBrush, position + 2, 1, size.Width, size.Height);
                e.Graphics.DrawString(timeText, _cursorFont, _cursorBrush, position + 2, 0);
            }

            base.OnPaint(e);
        }
    }
}
