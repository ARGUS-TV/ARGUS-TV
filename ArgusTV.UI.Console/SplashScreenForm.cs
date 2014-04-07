using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ArgusTV.UI.Console
{
    public partial class SplashScreenForm : Form
    {
        public SplashScreenForm()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Do nothing here.
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.DrawImage(ArgusTV.UI.Console.Properties.Resources.SplashScreen, new Rectangle(0, 0, this.Width, this.Height));
        }
    }
}
