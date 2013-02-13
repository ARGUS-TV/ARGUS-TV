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
using System.Drawing.Drawing2D;
using ArgusTV.WinForms;

using ArgusTV.DataContracts;

namespace ArgusTV.UI.Notifier
{
    public partial class StatusToolTipForm : Form
    {
        private const int _iconTop = 10;
        private const int _programsTop = 34;
        private const int _programsGap = 1;
        private const int _programsExtraGap = 3;

        private const int CS_DROPSHADOW = 0x00020000;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        public StatusToolTipForm()
        {
            InitializeComponent();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cParams = base.CreateParams;
                cParams.ClassStyle |= CS_DROPSHADOW;
                cParams.ExStyle |= WS_EX_NOACTIVATE;
                return cParams;
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            if (_innerPath != null)
            {
                _innerPath.Dispose();
                _innerPath = null;
            }
            if (_outlinePath != null)
            {
                _outlinePath.Dispose();
                _outlinePath = null;
            }
        }

        private ServerStatus _serverStatus;
        private ActiveRecording[] _activeRecordings = new ActiveRecording[0];
        private LiveStream[] _liveStreams = new LiveStream[0];
        private UpcomingRecording _upcomingRecording;

        public void Show(Point location, ServerStatus serverStatus, ActiveRecording[] activeRecordings, LiveStream[] liveStreams, UpcomingRecording upcomingRecording)
        {
            _serverStatus = serverStatus;
            _activeRecordings = activeRecordings;
            _liveStreams = liveStreams;
            _upcomingRecording = upcomingRecording;

            int fontHeight = SystemFonts.MessageBoxFont.Height;
            int height = _programsTop;
            if (_serverStatus != ServerStatus.NotConnected)
            {
                if (activeRecordings.Length > 0)
                {
                    height += fontHeight;
                    height += (2 * fontHeight + _programsGap) * activeRecordings.Length;
                    if (liveStreams.Length > 0
                        || upcomingRecording != null)
                    {
                        height += _programsExtraGap;
                    }
                }
                if (liveStreams.Length > 0)
                {
                    height += fontHeight;
                    height += (fontHeight + _programsGap) * liveStreams.Length;
                    if (upcomingRecording != null)
                    {
                        height += _programsExtraGap;
                    }
                }
                if (upcomingRecording != null)
                {
                    height += 2 * fontHeight + _programsGap;
                }
            }
            height += fontHeight;
            height += 10;
            this.Size = new Size(this.Width, height);

            int left = location.X;
            int top;
            Rectangle workArea = Screen.GetWorkingArea(location);
            if (workArea.Contains(location))
            {
                top = location.Y - 12 - this.Height;
            }
            else
            {
                top = int.MaxValue;
            }

            this.Opacity = 0.0;
            SetBounds(Math.Min(left, workArea.Width - this.Width - 1), Math.Min(top, workArea.Height - this.Height),
                this.Width, this.Height);

            this.Show();
        }

        private bool _opening;

        private GraphicsPath _innerPath;
        private GraphicsPath _outlinePath;

        private void NotificationForm_Load(object sender, EventArgs e)
        {
            _innerPath = RoundedRectangle.Create(1, 1, this.Width - 3, this.Height - 3, 2);
            _outlinePath = RoundedRectangle.Create(0, 0, this.Width, this.Height, 2);
            Region = new Region(_outlinePath);

            _opening = true;
            _animationTimer.Interval = 50;
            _animationTimer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (LinearGradientBrush brush = new LinearGradientBrush(_innerPath.GetBounds(), Color.White, Color.FromArgb(0xe4, 0xe5, 0xf0), LinearGradientMode.Vertical))
            using (SolidBrush textBrush = new SolidBrush(Color.FromArgb(0x57, 0x57, 0x57)))
            using (SolidBrush titleBrush = new SolidBrush(Color.FromArgb(0x00, 0x33, 0x99)))
            {
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                e.Graphics.FillPath(brush, _innerPath);

                e.Graphics.DrawImage(Properties.Resources.Logo, this.Width - Properties.Resources.Logo.Width - 8, 4,
                    Properties.Resources.Logo.Width, Properties.Resources.Logo.Height);

                string text;
                Icon icon;
                if (_serverStatus == ServerStatus.Recording)
                {
                    text = "Recording";
                    icon = Properties.Resources.Recording;
                }
                else if (_serverStatus == ServerStatus.Streaming)
                {
                    text = "Streaming";
                    icon = Properties.Resources.Streaming;
                }
                else if (_serverStatus == ServerStatus.Idle)
                {
                    text = "Idle";
                    icon = Properties.Resources.Idle;
                }
                else
                {
                    text = "Error";
                    icon = Properties.Resources.InError;
                }

                float left = 10;
                e.Graphics.DrawIconUnstretched(icon, new Rectangle((int)left, (int)_iconTop, icon.Width, icon.Height));
                using (Font statusFont = new Font(SystemFonts.MessageBoxFont.FontFamily, SystemFonts.MessageBoxFont.Size + 2))
                {
                    float statusTop = _iconTop + (icon.Height - statusFont.Height) / (float)2;
                    e.Graphics.DrawString(text, statusFont, titleBrush, new PointF(left + icon.Width + 2, statusTop));
                }

                float top = _programsTop;

                if (_serverStatus == ServerStatus.NotConnected)
                {
                    e.Graphics.DrawString("Not connected to the core services.", SystemFonts.MessageBoxFont, textBrush, left - 2, top);
                }
                else
                {
                    if (_activeRecordings.Length > 0)
                    {
                        e.Graphics.DrawString("Currently recording:", SystemFonts.MessageBoxFont, textBrush, left - 2, top);
                        top += SystemFonts.MessageBoxFont.Height;
                        foreach (ActiveRecording activeRecording in _activeRecordings)
                        {
                            top = DrawProgramText(e.Graphics, textBrush, left + 2, top, activeRecording.Program, false);
                        }
                        top += _programsExtraGap;
                    }

                    if (_liveStreams.Length > 0)
                    {
                        e.Graphics.DrawString("Currently streaming:", SystemFonts.MessageBoxFont, textBrush, left - 2, top);
                        top += SystemFonts.MessageBoxFont.Height;
                        foreach (LiveStream liveStream in _liveStreams)
                        {
                            top = DrawliveStreamText(e.Graphics, textBrush, left + 2, top, liveStream);
                        }
                        top += _programsExtraGap;
                    }

                    if (_upcomingRecording == null)
                    {
                        e.Graphics.DrawString("No recordings are scheduled.", SystemFonts.MessageBoxFont, textBrush, left - 2, top);
                    }
                    else
                    {
                        e.Graphics.DrawString("Next scheduled recording:", SystemFonts.MessageBoxFont, textBrush, left - 2, top);
                        top += SystemFonts.MessageBoxFont.Height;
                        top = DrawProgramText(e.Graphics, textBrush, left + 4, top, _upcomingRecording.Program, true);
                    }
                }
            }
        }

        private float GetStringLength(Graphics g, string text, Font font)
        {
            int charsFitted;
            int linesFilled;
            StringFormat sf = (StringFormat)StringFormat.GenericTypographic.Clone();
            sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            return g.MeasureString(text, font, new SizeF(1024, font.Height), sf, out charsFitted, out linesFilled).Width;
        }

        private float DrawliveStreamText(Graphics g, SolidBrush textBrush, float left, float top, LiveStream liveStream)
        {
            left = DrawAndMeasureString(g, SystemFonts.MessageBoxFont, textBrush, left, top, "• ");

            StringFormat trimmedFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
            trimmedFormat.Trimming = StringTrimming.EllipsisCharacter;

            g.DrawString(liveStream.Channel.DisplayName, SystemFonts.MessageBoxFont, Brushes.Black,
                new RectangleF(left, top, this.Width - left - 8, SystemFonts.MessageBoxFont.Height), trimmedFormat);

            return top + SystemFonts.MessageBoxFont.Height + _programsGap;
        }

        private float DrawProgramText(Graphics g, SolidBrush textBrush, float left, float top, UpcomingProgram program, bool showDate)
        {
            left = DrawAndMeasureString(g, SystemFonts.MessageBoxFont, textBrush, left, top, "• ");

            string times = program.StartTime.ToShortTimeString() + "-" + program.StopTime.ToShortTimeString() + " ";
            float titleLeft = DrawAndMeasureString(g, SystemFonts.MessageBoxFont, Brushes.Black, left, top, times);

            StringFormat trimmedFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
            trimmedFormat.Trimming = StringTrimming.EllipsisCharacter;

            using (Font boldFont = new Font(SystemFonts.MessageBoxFont, FontStyle.Bold))
            {
                g.DrawString(program.CreateProgramTitle(), boldFont, Brushes.Black,
                    new RectangleF(titleLeft, top, this.Width - titleLeft - 8, boldFont.Height), trimmedFormat);
                top += boldFont.Height;
            }

            float onLeft = DrawAndMeasureString(g, SystemFonts.MessageBoxFont, textBrush, left, top, "on ");

            string onText = program.Channel.DisplayName;
            if (showDate)
            {
                onText += ", " + program.StartTime.Date.ToLongDateString();
            }
            g.DrawString(onText, SystemFonts.MessageBoxFont, Brushes.Black,
                new RectangleF(onLeft, top, this.Width - onLeft - 8, SystemFonts.MessageBoxFont.Height), trimmedFormat);

            return top + SystemFonts.MessageBoxFont.Height + _programsGap;
        }

        private float DrawAndMeasureString(Graphics g, Font font, Brush brush, float left, float top, string text)
        {
            float width = GetStringLength(g, text, font);
            g.DrawString(text, SystemFonts.MessageBoxFont, brush,
                new RectangleF(left, top, this.Width - left - 8, font.Height), StringFormat.GenericTypographic);
            return left + width;
        }

        private void _animationTimer_Tick(object sender, EventArgs e)
        {
            if (_opening)
            {
                if (this.Opacity < 100)
                {
                    this.Opacity = Math.Min(100.0, this.Opacity + 0.20);
                }
                else
                {
                    _opening = false;
                    _animationTimer.Stop();
                }
            }
        }
    }
}
