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
using System.Windows.Forms;
using System.Drawing;
using System.Globalization;

using ArgusTV.DataContracts;
using ArgusTV.UI.Process.Guide;
using ArgusTV.Client.Common;
using ArgusTV.ServiceAgents;

namespace ArgusTV.WinForms.Controls
{
    public class EpgChannelsGridControl : Control
    {
        private GuideModel _model;
        private readonly float _widthFactor;
        private readonly float _heightFactor;
        private readonly int _height;

        private List<ChannelCell> _channelCells = new List<ChannelCell>();

        private List<IDisposable> _disposables = new List<IDisposable>();
        private Font _channelFont;
        private Brush _channelBrush;

        public EpgChannelsGridControl()
        {
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                _widthFactor = graphics.DpiX / 96;
                _heightFactor = graphics.DpiY / 96;
                _height = (int)(71 * _heightFactor);
                base.BackColor = Color.FromArgb(0xf5, 0xf5, 0xf5);
                this.Name = "EpgChannelsGridControl";
                this.Size = new Size((int)(100 * _widthFactor), _height);
                this.SetStyle(ControlStyles.Selectable, false);
                this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
                this.UpdateStyles();
            }

            _disposables.Add(_channelFont = new Font("Tahoma", 11F, FontStyle.Bold));
            _disposables.Add(_channelBrush = new SolidBrush(Color.FromArgb(0x22, 0x22, 0x22)));
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

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            this.Parent.Parent.Focus();
        }

        public void RefreshEpg(GuideModel model)
        {
            _model = model;

            _channelCells.Clear();

            int top = 0;
            foreach (Channel channel in _model.Channels)
            {
                top = CreateChannelCell(channel, top);
            }
            this.Height = top;

            this.Refresh();
        }

        private int CreateChannelCell(Channel channel, int top)
        {
            int cellHeight = 0;

            if (_model.ProgramsByChannel.ContainsKey(channel.ChannelId))
            {
                bool isTop = (top == 0);
                cellHeight = isTop ? _height : (_height - 1);

                ChannelCell cell = new ChannelCell()
                {
                    Channel = channel,
                    IsTop = isTop,
                    Rectangle = new Rectangle(0, top, (int) (100 * _widthFactor), cellHeight)
                };
                _channelCells.Add(cell);
            }

            return top + cellHeight;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Region clippingRegion = e.Graphics.Clip;

            using (SchedulerServiceAgent tvSchedulerAgent = new SchedulerServiceAgent())
            {
                foreach (var cell in _channelCells)
                {
                    Rectangle visibleRectangle = new Rectangle(cell.Rectangle.Location, cell.Rectangle.Size);
                    visibleRectangle.Intersect(e.ClipRectangle);

                    Region cellRegion = new Region(visibleRectangle);
                    cellRegion.Intersect(clippingRegion);
                    if (cellRegion.IsEmpty(e.Graphics))
                    {
                        continue;
                    }

                    e.Graphics.SetClip(visibleRectangle);

                    Padding innerPadding = new Padding(2, 2, 2, 2);

                    if (cell.Channel != null)
                    {
                        Rectangle innerRectangle = new Rectangle(cell.Rectangle.Left + innerPadding.Left, cell.Rectangle.Top + innerPadding.Top,
                            cell.Rectangle.Width - innerPadding.Horizontal, cell.Rectangle.Height - innerPadding.Vertical);

                        Image logoImage = null;
                        try
                        {
                            logoImage = ChannelLogosCache.GetLogoImage(tvSchedulerAgent, cell.Channel, (int)(64 * _widthFactor), (int)(64 * _heightFactor));
                        }
                        catch
                        {
                            logoImage = null;
                        }

                        if (logoImage == null)
                        {
                            e.Graphics.DrawString(cell.Channel.DisplayName, _channelFont, _channelBrush,
                                new RectangleF(innerRectangle.Left, innerRectangle.Top + 6, innerRectangle.Width, innerRectangle.Height));
                        }
                        else
                        {
                            e.Graphics.DrawImage(logoImage, innerRectangle.Left + (int)Math.Round((innerRectangle.Width - logoImage.Width) / 2F),
                                innerRectangle.Top + (int)Math.Round((innerRectangle.Height - logoImage.Height) / 2F),
                                logoImage.Width, logoImage.Height);
                        }
                    }
                }
            }
            base.OnPaint(e);
        }

        private class ChannelCell
        {
            public Rectangle Rectangle { set; get; }

            public bool IsTop { set; get; }

            public Channel Channel { set; get; }
        }
    }
}
