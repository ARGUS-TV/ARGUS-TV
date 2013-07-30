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
using ArgusTV.UI.Process;

namespace ArgusTV.WinForms.Controls
{
    public class EpgProgramsGridControl : Control
    {
        private GuideModel _model;
        private readonly float _widthFactor;
        private readonly float _heightFactor;
        private readonly int _height;

        private List<GuideProgramCell> _guideProgramCells = new List<GuideProgramCell>();
        private GuideProgramCell _highlightedCell;
        private GuideProgramCell _mouseDownCell;

        private List<IDisposable> _disposables = new List<IDisposable>();
        private Font _timeFont;
        private Font _titleFont;
        private Font _titleHighlightFont;
        private Font _episodeFont;
        private Brush _timeBrush;
        private Brush _titleBrush;
        private Brush _episodeBrush;
        private Brush _titleHighlightBrush;
        private Pen _epgBorderPen;
        private Pen _epgDashedBorderPen;
        private Brush _unusedAreaBrush;
        private Brush _highlightCellBrush;
        private Brush _highlightOnNowCellBrush;
        private Pen _cursorPen;
        private Pen _cursorShadowPen;
        private Brush _onNowCellBrush;
        private Brush _recordingCellBrush;
        private Brush _highlightRecordingCellBrush;
        private Brush _channelNotBroadcastedCellBrush;


        private ToolTip _toolTip;

        public event EventHandler<EpgControl.ProgramEventArgs> ProgramClicked;
        public event EventHandler<EpgControl.ProgramEventArgs> ProgramContextMenu;

        public EpgProgramsGridControl()
        {
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                _widthFactor = (graphics.DpiX / 96);
                _heightFactor = (graphics.DpiY / 96);
                _height = (int)(71 * _heightFactor);
                base.BackColor = System.Drawing.Color.White;
                this.Name = "EpgGridControl";
                this.Size = new System.Drawing.Size((int)(5760 * _widthFactor), _height);

                this.SetStyle(ControlStyles.Selectable, false);
                this.SetStyle(
                    ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.ResizeRedraw, true);
                this.UpdateStyles();
            }

            _disposables.Add(_timeFont = new Font("Tahoma", 7F, FontStyle.Bold));
            _disposables.Add(_titleFont = new Font("Tahoma", 9F, FontStyle.Bold));
            _disposables.Add(_timeBrush = new SolidBrush(Color.FromArgb(0x66, 0x66, 0x66)));
            _disposables.Add(_titleBrush = new SolidBrush(Color.FromArgb(0x22, 0x22, 0x22)));
            _disposables.Add(_titleHighlightFont = new Font(_titleFont, FontStyle.Underline | FontStyle.Bold));
            _disposables.Add(_titleHighlightBrush = new SolidBrush(Color.DarkRed));
            _disposables.Add(_epgBorderPen = new Pen(Color.FromArgb(0xD5, 0xD5, 0xD5)));
            _disposables.Add(_epgDashedBorderPen = new Pen(Color.FromArgb(0xD5, 0xD5, 0xD5)));
            _disposables.Add(_unusedAreaBrush = new SolidBrush(Color.FromArgb(0xF0, 0xF0, 0xF0)));
            _disposables.Add(_highlightCellBrush = new SolidBrush(Color.FromArgb(0xf8, 0xf8, 0xfb)));
            _disposables.Add(_cursorPen = new Pen(Color.FromArgb(0x7F, 0x90, 0x33, 0x33)));
            _disposables.Add(_cursorShadowPen = new Pen(Color.FromArgb(0x30, 0x33, 0x33, 0x33)));
            _disposables.Add(_onNowCellBrush = new SolidBrush(Color.FromArgb(0xfd, 0xfd, 0xea)));
            _disposables.Add(_highlightOnNowCellBrush = new SolidBrush(Color.FromArgb(0xfd, 0xfd, 0xd4)));
            _disposables.Add(_recordingCellBrush = new SolidBrush(Color.FromArgb(0xff, 0xe4, 0xe4)));
            _disposables.Add(_highlightRecordingCellBrush = new SolidBrush(Color.FromArgb(0xff, 0xd0, 0xd0)));
            _disposables.Add(_channelNotBroadcastedCellBrush = new SolidBrush(Color.FromArgb(0xF0, 0xF0, 0xF0)));

            _epgDashedBorderPen.DashPattern = new float[] { 4F, 4F };
            _episodeFont = _timeFont;
            _episodeBrush = _timeBrush;
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
                DisposeToolTip();
            }
            base.Dispose(disposing);
        }

        private void DisposeToolTip()
        {
            if (_toolTip != null)
            {
                _toolTip.Dispose();
                _toolTip = null;
            }
        }

        private TimeSpan? _cursorAtTime;

        public TimeSpan? CursorAtTime
        {
            get { return _cursorAtTime; }
            set
            {
                if (_cursorAtTime != value)
                {
                    _cursorAtTime = value;
                    this.Invalidate();
                }
            }
        }

        public void RefreshEpg(GuideModel model)
        {
            _model = model;

            _guideProgramCells.Clear();

            int top = 0;
            foreach (Channel channel in _model.Channels)
            {
                top = CreateEpgChannelCells(channel, top);
            }
            this.Width = (int)(5760 * _widthFactor);
            this.Height = top;

            this.Refresh();
        }

        private int CreateEpgChannelCells(Channel channel, int top)
        {
            DateTime startDate = EpgControl.GetEpgStartTime(_model);
            DateTime endDate = EpgControl.GetEpgEndTime(_model);

            int cellHeight = 0;

            int previousRight = 0;
            if (_model.ProgramsByChannel.ContainsKey(channel.ChannelId))
            {
                bool isTop = (top == 0);
                cellHeight = isTop ? _height : (_height - 1);

                foreach (GuideProgramSummary guideProgram in _model.ProgramsByChannel[channel.ChannelId].Programs)
                {
                    GuideProgramCell cell = new GuideProgramCell();
                    cell.GuideProgram = guideProgram;
                    cell.Channel = channel;
                    cell.IsTop = isTop;
                    cell.ClipLeft = (guideProgram.StartTime < startDate);
                    cell.ClipRight = (guideProgram.StopTime > endDate);
                    cell.StartTime = cell.ClipLeft ? startDate : guideProgram.StartTime;
                    cell.StopTime = cell.ClipRight ? endDate : guideProgram.StopTime;

                    TimeSpan leftSpan = cell.StartTime - startDate;
                    int left = (int)(leftSpan.TotalMinutes * 4 * _widthFactor);

                    if (left > previousRight)
                    {
                        GuideProgramCell emptyCell = new GuideProgramCell();
                        emptyCell.Rectangle = new Rectangle(previousRight, top, left - previousRight, cellHeight);
                        _guideProgramCells.Add(emptyCell);
                    }

                    cell.Rectangle = new Rectangle(left, top, (int)(cell.Duration.TotalMinutes * 4 * _widthFactor), cellHeight);
                    _guideProgramCells.Add(cell);

                    previousRight = cell.Rectangle.Right;
                }
            }
            if (previousRight < this.Width)
            {
                GuideProgramCell emptyCell = new GuideProgramCell();
                emptyCell.Rectangle = new Rectangle(previousRight, top, this.Width, cellHeight);
                _guideProgramCells.Add(emptyCell);
            }

            return top + cellHeight;
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            Invalidate();
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            DateTime startDate = DateTime.Today;
            DateTime endDate = DateTime.Today.AddDays(1);

            Region clippingRegion = e.Graphics.Clip;

            foreach (GuideProgramCell cell in _guideProgramCells)
            {
                Rectangle visibleRectangle = new Rectangle(cell.Rectangle.Location, cell.Rectangle.Size);
                visibleRectangle.Intersect(e.ClipRectangle);

                Region cellRegion = new Region(visibleRectangle);
                cellRegion.Intersect(clippingRegion);
                if (cellRegion.IsEmpty(e.Graphics))
                {
                    continue;
                }

                bool clipLeft = (visibleRectangle.Left != cell.Rectangle.Left) || cell.ClipLeft;
                bool clipRight = (visibleRectangle.Right != cell.Rectangle.Right) || cell.ClipRight;
                Padding innerPadding = new Padding(2, 2, 2, 2);

                bool onNow = false;
                if (_cursorAtTime.HasValue)
                {
                    DateTime cursorTime = DateTime.Today.Add(_cursorAtTime.Value);
                    onNow = (cell.StartTime <= cursorTime) && (cell.StopTime > cursorTime);
                }
                bool isRecording = IsRecording(cell);

                e.Graphics.SetClip(visibleRectangle);
                if (cell.GuideProgram == null)
                {
                    e.Graphics.FillRectangle(_unusedAreaBrush, visibleRectangle);
                }
                else if (!cell.IsBroadcasted)
                {
                    e.Graphics.FillRectangle(_channelNotBroadcastedCellBrush, visibleRectangle);
                }
                else if (cell == _highlightedCell)
                {
                    e.Graphics.FillRectangle(isRecording ? _highlightRecordingCellBrush : (onNow ? _highlightOnNowCellBrush : _highlightCellBrush), visibleRectangle);
                }
                else if (isRecording)
                {
                    e.Graphics.FillRectangle(_recordingCellBrush, visibleRectangle);
                }
                else if (onNow)
                {
                    e.Graphics.FillRectangle(_onNowCellBrush, visibleRectangle);
                }
                if (cell.IsTop)
                {
                    e.Graphics.DrawLine(_epgBorderPen, visibleRectangle.Left, cell.Rectangle.Top, visibleRectangle.Right - 1, cell.Rectangle.Top);
                    innerPadding.Top++;
                }
                if (clipLeft)
                {
                    e.Graphics.DrawLine(_epgDashedBorderPen, visibleRectangle.Left, cell.Rectangle.Top, visibleRectangle.Left, cell.Rectangle.Bottom - 1);
                    innerPadding.Left++;
                }
                else if (cell.Rectangle.Left == 0)
                {
                    e.Graphics.DrawLine(_epgBorderPen, visibleRectangle.Left, cell.Rectangle.Top, visibleRectangle.Left, cell.Rectangle.Bottom - 1);
                    innerPadding.Left++;
                }
                Pen rightBorderPen = _epgBorderPen;
                if (clipRight)
                {
                    rightBorderPen = _epgDashedBorderPen;
                }
                e.Graphics.DrawLine(_epgBorderPen, visibleRectangle.Right - 1, cell.Rectangle.Bottom - 1, visibleRectangle.Left, cell.Rectangle.Bottom - 1);
                e.Graphics.DrawLine(rightBorderPen, visibleRectangle.Right - 1, cell.Rectangle.Top, visibleRectangle.Right - 1, cell.Rectangle.Bottom - 1);

                int innerWidth = visibleRectangle.Width - innerPadding.Horizontal;
                int innerHeight = cell.Rectangle.Height - innerPadding.Vertical;
                if (cell.GuideProgram != null
                    && innerWidth > 0
                    && innerHeight > 0)
                {
                    Rectangle innerRectangle = new Rectangle(visibleRectangle.Left + innerPadding.Left, cell.Rectangle.Top + innerPadding.Top, innerWidth, innerHeight);
                    e.Graphics.SetClip(innerRectangle);

                    TimeSpan time = cell.GuideProgram.StartTime.TimeOfDay;
                    TimeSpan endTime = cell.GuideProgram.StopTime.TimeOfDay;
                    string timeText = EpgTimeControl.GetTimeString(time);
                    string endTimeText = EpgTimeControl.GetTimeString(endTime);
                    string timeLabelText = timeText;
                    if (clipLeft)
                    {
                        timeLabelText = "<" + timeText;
                    }
                    if (clipRight)
                    {
                        timeLabelText = timeText + "-" + endTimeText;
                    }

                    e.Graphics.DrawString(timeLabelText, _timeFont, _timeBrush, innerRectangle.Left, innerRectangle.Top);

                    float titleTop = innerRectangle.Top + _timeFont.GetHeight();
                    int titleLeft = innerRectangle.Left;

                    bool hasIcons = DrawScheduleIcon(cell, e, titleTop, ref titleLeft, _model.UpcomingRecordingsById);
                    hasIcons = DrawScheduleIcon(cell, e, titleTop, ref titleLeft, _model.UpcomingAlertsById) || hasIcons;
                    hasIcons = DrawScheduleIcon(cell, e, titleTop, ref titleLeft, _model.UpcomingSuggestionsById) || hasIcons;

                    int titleWidth = innerRectangle.Width - (titleLeft - innerRectangle.Left);

                    bool isHighlighted = (cell == _highlightedCell);
                    string title = cell.GuideProgram.Title;
                    Font titleFont = isHighlighted ? _titleHighlightFont : _titleFont;
                    Brush titleBrush = isHighlighted ? _titleHighlightBrush : _titleBrush;

                    int charsFitted;
                    int linesFilled;
                    SizeF titleSize = e.Graphics.MeasureString(title, titleFont,
                        new SizeF(titleWidth, titleFont.Height), StringFormat.GenericDefault, out charsFitted, out linesFilled);
                    if (charsFitted < title.Length)
                    {
                        while (charsFitted > 0
                            && title[charsFitted] != ' ')
                        {
                            charsFitted--;
                        }
                        if (charsFitted == 0)
                        {
                            while (charsFitted < title.Length
                                && title[charsFitted] != ' ')
                            {
                                charsFitted++;
                            }
                        }
                    }
                    if (charsFitted > 0)
                    {
                        e.Graphics.DrawString(title.Substring(0, charsFitted), titleFont, titleBrush,
                            new RectangleF(titleLeft, titleTop, titleWidth, titleFont.Height));
                        titleTop += titleFont.GetHeight();
                        if (charsFitted == title.Length)
                        {
                            title = String.Empty;
                        }
                        else
                        {
                            title = title.Substring(charsFitted).TrimStart();
                        }
                    }
                    if (hasIcons)
                    {
                        titleTop++;
                    }
                    if (!String.IsNullOrEmpty(title))
                    {
                        titleSize = e.Graphics.MeasureString(title, titleFont, innerRectangle.Width);
                        e.Graphics.DrawString(title, titleFont, titleBrush, new RectangleF(innerRectangle.Left, titleTop, titleSize.Width, titleSize.Height));
                        titleTop += titleSize.Height;
                    }
                    if (titleTop < innerRectangle.Bottom)
                    {
                        e.Graphics.DrawString(cell.GuideProgram.CreateEpisodeTitle(), _episodeFont, _episodeBrush,
                            new RectangleF(innerRectangle.Left, titleTop, innerRectangle.Width, innerRectangle.Height));
                    }
                }
            }

            if (this.CursorAtTime.HasValue)
            {
                int position = EpgTimeControl.GetTimeCursorPosition(_cursorAtTime.Value, -1);
                Rectangle timeRectangle = new Rectangle(position, 0, 2, this.Height);
                e.Graphics.SetClip(timeRectangle);
                timeRectangle.Intersect(e.ClipRectangle);

                Region cellRegion = new Region(timeRectangle);
                cellRegion.Intersect(clippingRegion);
                if (!cellRegion.IsEmpty(e.Graphics))
                {
                    e.Graphics.DrawLine(_cursorPen, timeRectangle.Left, timeRectangle.Top, timeRectangle.Left, timeRectangle.Bottom - 1);
                    e.Graphics.DrawLine(_cursorShadowPen, timeRectangle.Left + 1, timeRectangle.Top, timeRectangle.Left + 1, timeRectangle.Bottom - 1);
                }
            }

            base.OnPaint(e);
        }

        private bool IsRecording(GuideProgramCell cell)
        {
            GuideUpcomingProgram upcomingProgramInfo;
            if (cell.GuideProgram != null
                && _model.UpcomingRecordingsById.TryGetValue(cell.GetUniqueUpcomingProgramId(), out upcomingProgramInfo))
            {
                return !upcomingProgramInfo.IsCancelled
                    && upcomingProgramInfo.UpcomingRecording != null
                    && upcomingProgramInfo.UpcomingRecording.CardChannelAllocation != null;
            }
            return false;
        }

        private bool DrawScheduleIcon(GuideProgramCell cell, PaintEventArgs e, float titleTop, ref int titleLeft, SerializableDictionary<Guid, GuideUpcomingProgram> upcomingById)
        {
            GuideUpcomingProgram upcomingProgramInfo;
            if (upcomingById.TryGetValue(cell.GetUniqueUpcomingProgramId(), out upcomingProgramInfo))
            {
                Icon icon;
                string toolTip = null;
                if (upcomingProgramInfo.UpcomingRecording != null)
                {
                    toolTip = ProcessUtility.BuildRecordingInfoToolTip(upcomingProgramInfo.UpcomingRecording, "on");
                }
                string toolTip2;
                ProgramIconUtility.GetIconAndToolTip(upcomingProgramInfo.Type, upcomingProgramInfo.CancellationReason,
                    upcomingProgramInfo.IsPartOfSeries, _model.UpcomingRecordings, upcomingProgramInfo.UpcomingRecording,
                    out icon, out toolTip2);
                if (!String.IsNullOrEmpty(toolTip2))
                {
                    if (!String.IsNullOrEmpty(toolTip))
                    {
                        toolTip = toolTip + Environment.NewLine + Environment.NewLine + toolTip2;
                    }
                    else
                    {
                        toolTip = toolTip2;
                    }
                }
                Rectangle iconRectangle = new Rectangle(titleLeft, (int)Math.Round(titleTop), icon.Width, icon.Height);
                if (!String.IsNullOrEmpty(toolTip))
                {
                    cell.ToolTips.Add(new CellToolTip(iconRectangle, toolTip));
                }

                int height = (int)(icon.Height * _heightFactor);
                e.Graphics.DrawIcon(icon, iconRectangle.X, iconRectangle.Y + (int)Math.Floor((height - icon.Height) / 2.0));
                titleLeft += icon.Width;
                return true;
            }
            return false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            GuideProgramCell previousHighlightedCell = _highlightedCell;
            _highlightedCell = null;
            foreach (GuideProgramCell cell in _guideProgramCells)
            {
                if (cell.Rectangle.Contains(e.Location)
                    && cell.GuideProgram != null)
                {
                    _highlightedCell = cell;
                    break;
                }
            }
            if (_highlightedCell != previousHighlightedCell)
            {
                InvalidateCell(previousHighlightedCell);
                InvalidateCell(_highlightedCell);
            }
            this.Cursor = _highlightedCell == null ? Cursors.Default : Cursors.Hand;

            CellToolTip toolTip = null;
            if (_highlightedCell != null)
            {
                toolTip = _highlightedCell.GetToolTipAt(e.Location);
            }

            if (toolTip == null)
            {
                DisposeToolTip();
            }
            else if (_toolTip == null)
            {
                _toolTip = new ToolTip();
                _toolTip.SetToolTip(this, toolTip.Text);
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            this.Parent.Parent.Focus();
        }

        private bool _skipMouseLeave;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _mouseDownCell = GetProgramCellAt(e.Location);
            }
            else if (e.Button == MouseButtons.Right)
            {
                GuideProgramCell cell = GetProgramCellAt(e.Location);
                if (this.ProgramContextMenu != null
                    && cell != null
                    && cell.GuideProgram != null)
                {
                    InvalidateCell(_highlightedCell);
                    _highlightedCell = cell;
                    InvalidateCell(_highlightedCell);
                    _skipMouseLeave = true;
                    this.ProgramContextMenu(this, new EpgControl.ProgramEventArgs(cell.GuideProgram, cell.Channel, e.Location));
                }
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left
                && _mouseDownCell != null
                && _mouseDownCell == GetProgramCellAt(e.Location))
            {
                if (_mouseDownCell.GuideProgram != null
                    && this.ProgramClicked != null)
                {
                    _skipMouseLeave = true;
                    this.ProgramClicked(this, new EpgControl.ProgramEventArgs(_mouseDownCell.GuideProgram, _mouseDownCell.Channel, e.Location));
                }
            }
            base.OnMouseUp(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (!_skipMouseLeave)
            {
                InvalidateCell(_highlightedCell);
                _highlightedCell = null;
            }
            _skipMouseLeave = false;
            this.Cursor = Cursors.Default;
            base.OnMouseLeave(e);
        }

        private void InvalidateCell(GuideProgramCell cell)
        {
            if (cell != null)
            {
                Invalidate(cell.Rectangle);
            }
        }

        private GuideProgramCell GetProgramCellAt(Point location)
        {
            foreach (GuideProgramCell cell in _guideProgramCells)
            {
                if (cell.Rectangle.Contains(location))
                {
                    return cell;
                }
            }
            return null;
        }

        private class GuideProgramCell
        {
            public GuideProgramCell()
            {
                this.ToolTips = new List<CellToolTip>();
            }

            public Rectangle Rectangle { set; get; }

            public bool IsTop { set; get; }

            public GuideProgramSummary GuideProgram { set; get; }

            public Channel Channel { set; get; }

            public bool ClipLeft { set; get; }

            public bool ClipRight { set; get; }

            public DateTime StartTime { set; get; }

            public DateTime StopTime { set; get; }

            public TimeSpan Duration
            {
                get { return this.StopTime - this.StartTime; }
            }

            public bool IsBroadcasted
            {
                get
                {
                    bool isBroadcasted = true;

                    if (Channel.BroadcastStartTime.HasValue && Channel.BroadcastStopTime.HasValue)
                    {                        
                        if (Channel.BroadcastStartTime < Channel.BroadcastStopTime)
                        {
                            isBroadcasted = false;
                            // eg 12:00 - 15:00
                            if ((this.StartTime.TimeOfDay >= Channel.BroadcastStartTime && this.StopTime.TimeOfDay > this.StartTime.TimeOfDay && this.StopTime.TimeOfDay <= Channel.BroadcastStopTime))
                                isBroadcasted = true;
                        }
                        else
                        {
                            // eg 16:30 - 12:00  
                            if (this.StartTime.TimeOfDay >= Channel.BroadcastStopTime && this.StopTime.TimeOfDay <= Channel.BroadcastStartTime && this.StopTime.TimeOfDay >= Channel.BroadcastStopTime)
                                isBroadcasted = false;
                        }   
                    }                    
                    return isBroadcasted;
                }
            }

            public List<CellToolTip> ToolTips { set; get; }

            public Guid GetUniqueUpcomingProgramId()
            {
                return this.GuideProgram.GetUniqueUpcomingProgramId(this.Channel.ChannelId);
            }

            public CellToolTip GetToolTipAt(Point location)
            {
                foreach (CellToolTip toolTip in this.ToolTips)
                {
                    if (toolTip.Rectangle.Contains(location))
                    {
                        return toolTip;
                    }
                }
                return null;
            }
        }

        private class CellToolTip
        {
            public CellToolTip(Rectangle rectangle, string text)
            {
                this.Rectangle = rectangle;
                this.Text = text;
            }

            public Rectangle Rectangle { set; get; }

            public string Text { set; get; }
        }
    }
}
