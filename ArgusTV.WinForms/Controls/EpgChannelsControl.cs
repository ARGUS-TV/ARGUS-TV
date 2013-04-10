/*
 *	Copyright (C) 2007-2013 ARGUS TV
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
    public class EpgChannelsControl : ScrollableControl
    {
        private EpgChannelsGridControl _epgChannelsGridControl = new EpgChannelsGridControl();

        public EpgChannelsControl()
        {
            this.BackColor = Color.FromArgb(0xf5, 0xf5, 0xf5);
            _epgChannelsGridControl.Size = new Size(_epgChannelsGridControl.Width, 0);
            this.Controls.Add(_epgChannelsGridControl);
        }

        public int ScrollTop
        {
            get { return _epgChannelsGridControl.Top; }
            set { _epgChannelsGridControl.Top = value; }
        }

        public void RefreshEpg(GuideModel model)
        {
            _epgChannelsGridControl.RefreshEpg(model);
        }
    }
}
