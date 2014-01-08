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
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using ArgusTV.DataContracts;

namespace ArgusTV.WinForms
{
    public partial class ProgramDetailsPopup : Form
    {
        public ProgramDetailsPopup()
        {
            InitializeComponent();
        }

        private GuideProgram _guideProgram;

        public GuideProgram GuideProgram
        {
            get { return _guideProgram; }
            set { _guideProgram = value; }
        }

        private Channel _channel;

        public Channel Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }

        private void ProgramDetailsPopup_Load(object sender, EventArgs e)
        {
            _channelLabel.Text = _channel.DisplayName;
            _dateLabel.Text = _guideProgram.StartTime.ToLongDateString();
            _widescreenLabel.Visible = (0 != (_guideProgram.Flags & GuideProgramFlags.WidescreenAspectRatio));
            _timeLabel.Text = _guideProgram.StartTime.ToShortTimeString() + "-" + _guideProgram.StopTime.ToShortTimeString();
            _titleLabel.Text = _guideProgram.Title;
            if (String.IsNullOrEmpty(_guideProgram.SubTitle))
            {
                _detailsTableLayoutPanel.RowStyles[2].SizeType = SizeType.Absolute;
                _detailsTableLayoutPanel.RowStyles[2].Height = 0;
            }
            else
            {
                _subTitleLabel.Text = _guideProgram.SubTitle;
            }

            string description = _guideProgram.CreateCombinedDescription(false);
            _descriptionLabel.Text = description;

            using (Graphics g = this.CreateGraphics())
            {
                SizeF size = g.MeasureString(_descriptionLabel.Text, _descriptionLabel.Font, _descriptionLabel.Width);
                double heightFactor = g.DpiY / 96;
                this.Height = (int)Math.Max(100.0 * heightFactor, 75 + Math.Round(size.Height * heightFactor) + 10);
            }
        }

        private void _closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void WindowDragMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        #region P/Invoke

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        #endregion
    }
}
