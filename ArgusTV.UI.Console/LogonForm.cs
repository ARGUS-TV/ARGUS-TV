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
using System.Text;
using System.Windows.Forms;

namespace ArgusTV.UI.Console
{
    public partial class LogonForm : Form
    {
        public LogonForm()
        {
            InitializeComponent();
        }

        public string UserName
        {
            get { return _userNameTextBox.Text.Trim(); }
            set { _userNameTextBox.Text = value; }
        }

        public string Password
        {
            get { return _passwordTextBox.Text; }
            set { _passwordTextBox.Text = value; }
        }

        public bool SavePassword
        {
            get { return _savePasswordCheckBox.Checked; }
        }

        private void LogonForm_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(this.UserName))
            {
                _passwordTextBox.Select();
            }
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}