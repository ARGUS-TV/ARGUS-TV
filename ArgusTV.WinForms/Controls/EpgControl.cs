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
using System.Windows.Forms;

using ArgusTV.UI.Process.Guide;
using ArgusTV.DataContracts;

namespace ArgusTV.WinForms.Controls
{
    public class EpgControl : ScrollableControl
    {
        private GuideModel _model;
        private EpgChannelsControl _epgChannelsControl;
        private EpgProgramsControl _epgProgramsControl;
        private EpgTimeControl _epgTimeControl;
        private Timer _cursorTimer;

        public const int EpgHoursOffset = 3;

        #region Events

        public event EventHandler<ProgramEventArgs> ProgramClicked;
        public event EventHandler<ProgramEventArgs> ProgramContextMenu;

        public class ProgramEventArgs : EventArgs
        {
            public ProgramEventArgs(GuideProgramSummary guideProgram, Channel channel, Point location)
            {
                this.GuideProgram = guideProgram;
                this.Channel = channel;
                this.Location = location;
                this.UpcomingProgramId = guideProgram.GetUniqueUpcomingProgramId(channel.ChannelId);
            }

            public Guid UpcomingProgramId { set; get; }

            public GuideProgramSummary GuideProgram { set; get; }

            public Channel Channel { set; get; }

            public Point Location { set; get; }

            internal ProgramEventArgs Offset(Point p)
            {
                Point location = this.Location;
                location.Offset(p);
                return new ProgramEventArgs(this.GuideProgram, this.Channel, location);
            }
        }

        #endregion

        public EpgControl()
        {
            this.BackColor = Color.FromArgb(0xf5, 0xf5, 0xf5);

            _epgTimeControl = new EpgTimeControl();
            _epgTimeControl.Location = new Point(107, 3);
            _epgTimeControl.Size = new Size(this.Width - _epgTimeControl.Left, 15);
            this.Controls.Add(_epgTimeControl);

            _epgChannelsControl = new EpgChannelsControl();
            _epgChannelsControl.Location = new Point(3, _epgTimeControl.Height + 3);
            _epgChannelsControl.Size = new Size(100, this.Height - _epgChannelsControl.Top - 18);
            this.Controls.Add(_epgChannelsControl);

            _epgProgramsControl = new EpgProgramsControl();
            _epgProgramsControl.Location = new Point(107, _epgTimeControl.Height + 3);
            _epgProgramsControl.Size = new Size(this.Width - _epgProgramsControl.Left, this.Height - _epgProgramsControl.Top);
            _epgProgramsControl.ScrollPositionChanged += _epgProgramsControl_ScrollPositionChanged;
            _epgProgramsControl.ProgramClicked += _epgProgramsControl_ProgramClicked;
            _epgProgramsControl.ProgramContextMenu += _epgProgramsControl_ProgramContextMenu;
            this.Controls.Add(_epgProgramsControl);

            _cursorTimer = new Timer();
            _cursorTimer.Tick += _cursorTimer_Tick;
            _cursorTimer.Interval = 5000;
            _cursorTimer.Start();
        }

        public Point ScrollPosition
        {
            get { return _epgProgramsControl.ScrollPosition; }
            set
            {
                _epgProgramsControl.ScrollPosition = value;
                _epgChannelsControl.ScrollTop = value.Y;
            }
        }

        public static DateTime GetEpgStartTime(GuideModel model)
        {
            return model.GuideDateTime.Date.AddHours(EpgHoursOffset);
        }

        public static DateTime GetEpgEndTime(GuideModel model)
        {
            return model.GuideDateTime.Date.AddDays(1).AddHours(EpgHoursOffset);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed
                && disposing)
            {
                _cursorTimer.Stop();
                _cursorTimer.Dispose();
            }
            base.Dispose(disposing);
        }

        public void RefreshEpg(GuideModel model)
        {
            _model = model;
            _epgChannelsControl.RefreshEpg(model);
            _epgProgramsControl.RefreshEpg(model);
            SetTimeIndicator();
        }

        public void GotoNowTime()
        {
            int pos = EpgTimeControl.GetTimeCursorPosition(DateTime.Now.TimeOfDay, -180);
            pos = Math.Min(pos, 5760 - _epgProgramsControl.Width);
            _epgProgramsControl.ScrollLeft = -pos;
            SetTimeIndicator();
        }

        #region Overrides

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _epgProgramsControl.Focus();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _epgProgramsControl.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            _epgChannelsControl.Height = this.Height - _epgChannelsControl.Top - 18;
            _epgTimeControl.Width = this.Width - _epgTimeControl.Left - 18;
            _epgProgramsControl.Size = new Size(this.Width - _epgProgramsControl.Left, this.Height - _epgProgramsControl.Top);
        }

        #endregion

        #region Event Handling

        private void _epgProgramsControl_ProgramContextMenu(object sender, EpgControl.ProgramEventArgs e)
        {
            if (this.ProgramContextMenu != null)
            {
                this.ProgramContextMenu(this, e.Offset(_epgProgramsControl.Location));
            }
        }

        private void _epgProgramsControl_ProgramClicked(object sender, EpgControl.ProgramEventArgs e)
        {
            if (this.ProgramClicked != null)
            {
                this.ProgramClicked(this, e.Offset(_epgProgramsControl.Location));
            }
        }

        private void _epgProgramsControl_ScrollPositionChanged(object sender, EventArgs e)
        {
            _epgChannelsControl.ScrollTop = _epgProgramsControl.ScrollTop;
            _epgTimeControl.ScrollLeft = _epgProgramsControl.ScrollLeft;
        }

        private void _cursorTimer_Tick(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                SetTimeIndicator();
            }
        }

        private void SetTimeIndicator()
        {
            DateTime now = DateTime.Now;
            if (_model != null
                && EpgControl.GetEpgStartTime(_model) <= now
                && now <= EpgControl.GetEpgEndTime(_model))
            {
                TimeSpan time = new TimeSpan(now.TimeOfDay.Hours, now.TimeOfDay.Minutes, 0);
                _epgTimeControl.CursorAtTime = time;
                _epgProgramsControl.CursorAtTime = time;
            }
            else
            {
                _epgTimeControl.CursorAtTime = null;
                _epgProgramsControl.CursorAtTime = null;
            }
        }

        #endregion
    }
}
