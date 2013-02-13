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

namespace ForTheRecord.WinForms
{
    public partial class NotificationForm : Form
    {
        //[DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        //private static extern IntPtr CreateRoundRectRgn(
        //    int nLeftRect, // x-coordinate of upper-left corner
        //    int nTopRect, // y-coordinate of upper-left corner
        //    int nRightRect, // x-coordinate of lower-right corner
        //    int nBottomRect, // y-coordinate of lower-right corner
        //    int nWidthEllipse, // height of ellipse
        //    int nHeightEllipse // width of ellipse
        //    );

#if false
        private const int CS_DROPSHADOW = 131072;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams par = base.CreateParams;
                par.ClassStyle |= CS_DROPSHADOW;
                return par;
            }
        }
#endif

        public NotificationForm()
        {
            InitializeComponent();
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

        public void Show(string title, string text)
        {
            this.Show();
        }

        private Rectangle _targetRectangle;
        private bool _opening;
        private double _openingTop;
        private double _openingStep;

        private GraphicsPath _innerPath;
        private GraphicsPath _outlinePath;

        private void NotificationForm_Load(object sender, EventArgs e)
        {
            _innerPath = RoundedRectangle.Create(1, 1, this.Width - 2, this.Height - 2, 2);
            _outlinePath = RoundedRectangle.Create(0, 0, this.Width, this.Height, 2);
            Region = new Region(_outlinePath);

            Rectangle workArea = Screen.GetWorkingArea(Point.Empty);

            _targetRectangle = new Rectangle(workArea.Width - this.Width, workArea.Height - this.Height, this.Width, this.Height);
            this.Opacity = 0.0;
            SetBounds(_targetRectangle.Left, _targetRectangle.Top, _targetRectangle.Width, _targetRectangle.Height);
            _openingTop = this.Top;
            _openingStep = (this.Top - _targetRectangle.Top) / 10.0;

            _opening = true;
            _animationTimer.Interval = 50;
            _animationTimer.Start();

            _closeTimer.Interval = 5000;
            _closeTimer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (SolidBrush brush = new SolidBrush(SystemColors.InactiveCaption))
            {
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                e.Graphics.FillPath(brush, _innerPath);
            }
        }

        private void _closeTimer_Tick(object sender, EventArgs e)
        {
            _closeTimer.Stop();
            this.Close();
        }

        private void _animationTimer_Tick(object sender, EventArgs e)
        {
            if (_opening)
            {
                if (this.Opacity == 100
                    /* Math.Round(_openingTop) <= _targetRectangle.Top */)
                {
                    _opening = false;
                    _animationTimer.Stop();
                }
                else
                {
                    //_openingTop -= _openingStep;
                    //this.Location = new Point(this.Location.X, (int)Math.Round(_openingTop));
                    this.Opacity = Math.Min(100.0, this.Opacity + 0.08);
                }
            }
        }
    }
}
