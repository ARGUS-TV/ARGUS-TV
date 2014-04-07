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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ArgusTV.DataContracts;
using ArgusTV.UI.Process;
using System.Diagnostics;

namespace ArgusTV.UI.Console.Panels
{
    public partial class AboutPanel : ContentPanel
    {
        public AboutPanel()
        {
            InitializeComponent();
        }

        public override string Title
        {
            get { return "About"; }
        }

        private void LogPanel_Load(object sender, EventArgs e)
        {
        }

        public override void OnClosed()
        {
        }

        private void AboutPanel_Load(object sender, EventArgs e)
        {
        }

        private void _pictureBoxDot_i_Click(object sender, EventArgs e)
        {
            OpenUrlInDefaultBrowser(_linkLabelDot_i.Tag.ToString());
        }

        private void _pictureBoxDonations_Click(object sender, EventArgs e)
        {
            OpenUrlInDefaultBrowser(_linkLabelDonations.Tag.ToString());
        }

        private void _linkLabelDot_i_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenUrlInDefaultBrowser(_linkLabelDot_i.Tag.ToString());
        }

        private void _linkLabelDonations_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenUrlInDefaultBrowser(_linkLabelDonations.Tag.ToString());
        }

        private void OpenUrlInDefaultBrowser(string url)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(url);
            System.Diagnostics.Process.Start(processStartInfo);
        }
    }
}
