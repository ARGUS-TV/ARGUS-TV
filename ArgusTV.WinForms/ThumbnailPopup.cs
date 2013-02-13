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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ArgusTV.WinForms
{
    public partial class ThumbnailPopup : Form
    {
        private Size _maxSize;
        private double _heightRatio;
        private double _widthRatio;

        public ThumbnailPopup()
        {
            InitializeComponent();
        }

        public Image ThumbnailImage
        {
            get
            {
                return _thumbnailPictureBox.Image;
            }
            set
            {
                _thumbnailPictureBox.Image = value;
                int width = (this.Width - this.ClientSize.Width) + value.Width;
                int height = (this.Height - this.ClientSize.Height) + value.Height;
                _maxSize = new Size(width, height);
                this.Size = _maxSize;
                this.MinimumSize = new Size(200, (int)Math.Round((200.0 * this.Size.Height) / this.Size.Width));
                this.MaximumSize = _maxSize;
                _widthRatio = (double)_maxSize.Width;
                _heightRatio = (double)_maxSize.Height;
            }
        }

        public string Title
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        private void _thumbnailPictureBox_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region Aspect Ratio Resizing

        const int WM_SIZING = 0x214;
        const int WMSZ_LEFT = 1;
        const int WMSZ_RIGHT = 2;
        const int WMSZ_TOP = 3;
        const int WMSZ_BOTTOM = 6;

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SIZING)
            {
                RECT rc = (RECT)Marshal.PtrToStructure(m.LParam, typeof(RECT));
                int res = m.WParam.ToInt32();
                if (res == WMSZ_LEFT || res == WMSZ_RIGHT)
                {
                    //Left or right resize -> adjust height (bottom)
                    rc.Bottom = rc.Top + (int)(_heightRatio * this.Width / _widthRatio);
                }
                else if (res == WMSZ_TOP || res == WMSZ_BOTTOM)
                {
                    //Up or down resize -> adjust width (right)
                    rc.Right = rc.Left + (int)(_widthRatio * this.Height / _heightRatio);
                }
                else if (res == WMSZ_RIGHT + WMSZ_BOTTOM)
                {
                    //Lower-right corner resize -> adjust height (could have been width)
                    rc.Bottom = rc.Top + (int)(_heightRatio * this.Width / _widthRatio);
                }
                else if (res == WMSZ_LEFT + WMSZ_TOP)
                {
                    //Upper-left corner -> adjust width (could have been height)
                    rc.Left = rc.Right - (int)(_widthRatio * this.Height / _heightRatio);
                }
                Marshal.StructureToPtr(rc, m.LParam, true);
            }

            base.WndProc(ref m);
        }

        #endregion
    }
}
