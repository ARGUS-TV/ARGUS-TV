/*
 *	Copyright (C) 2007-2012 ARGUS TV
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

namespace ArgusTV.WinForms.Controls
{
    public class EpgProgramsControl : ScrollableControl
    {
        private EpgProgramsGridControl _epgProgramsGridControl = new EpgProgramsGridControl();

        public event EventHandler<EventArgs> ScrollPositionChanged;
        public event EventHandler<EpgControl.ProgramEventArgs> ProgramClicked;
        public event EventHandler<EpgControl.ProgramEventArgs> ProgramContextMenu;

        public EpgProgramsControl()
        {
            this.AutoScroll = true;
            _epgProgramsGridControl.Size = new Size(_epgProgramsGridControl.Width, 0);
            _epgProgramsGridControl.LocationChanged += _epgProgramsGridControl_LocationChanged;
            _epgProgramsGridControl.ProgramClicked += _epgProgramsGridControl_ProgramClicked;
            _epgProgramsGridControl.ProgramContextMenu += _epgProgramsGridControl_ProgramContextMenu;
            this.Controls.Add(_epgProgramsGridControl);
        }

        public TimeSpan? CursorAtTime
        {
            get { return _epgProgramsGridControl.CursorAtTime; }
            set { _epgProgramsGridControl.CursorAtTime = value; }
        }

        private void _epgProgramsGridControl_ProgramContextMenu(object sender, EpgControl.ProgramEventArgs e)
        {
            if (this.ProgramContextMenu != null)
            {
                this.ProgramContextMenu(this, e.Offset(_epgProgramsGridControl.Location));
            }
        }

        private void _epgProgramsGridControl_ProgramClicked(object sender, EpgControl.ProgramEventArgs e)
        {
            if (this.ProgramClicked != null)
            {
                this.ProgramClicked(this, e.Offset(_epgProgramsGridControl.Location));
            }
        }

        public int ScrollTop
        {
            get { return this.AutoScrollPosition.Y; }
            set { this.AutoScrollPosition = new Point(-this.AutoScrollPosition.X, -value); }
        }

        public int ScrollLeft
        {
            get { return this.AutoScrollPosition.X; }
            set { this.AutoScrollPosition = new Point(-value, -this.AutoScrollPosition.Y); }
        }

        public Point ScrollPosition
        {
            get { return this.AutoScrollPosition; }
            set { this.AutoScrollPosition = new Point(-value.X, -value.Y); }
        }

        private void _epgProgramsGridControl_LocationChanged(object sender, EventArgs e)
        {
            if (this.ScrollPositionChanged != null)
            {
                this.ScrollPositionChanged(this, EventArgs.Empty);
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            _epgProgramsGridControl.Invalidate();
        }

        public void RefreshEpg(GuideModel model)
        {
            _epgProgramsGridControl.RefreshEpg(model);
        }
    }
}
