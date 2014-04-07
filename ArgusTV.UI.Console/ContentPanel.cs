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
using System.Text;
using System.Windows.Forms;

namespace ArgusTV.UI.Console
{
    public class ContentPanel : UserControl
    {
        private ContentPanel _ownerPanel;

        public ContentPanel OwnerPanel
        {
            get { return _ownerPanel; }
        }

        private DialogResult _dialogResult;

        public DialogResult DialogResult
        {
            get { return _dialogResult; }
            set { _dialogResult = value; }
        }

        public virtual string Title
        {
            get { return String.Empty; }
        }

        private MainForm _mainForm;

        public MainForm MainForm
        {
            get
            {
                if (_mainForm == null)
                {
                    Control control = this.Parent;
                    while (control != null)
                    {
                        MainForm mainForm = control as MainForm;
                        if (mainForm != null)
                        {
                            _mainForm = mainForm;
                            break;
                        }
                        control = control.Parent;
                    }
                }
                return _mainForm;
            }
        }

        public void OpenPanel(ContentPanel ownerPanel)
        {
            _ownerPanel = ownerPanel;
            _ownerPanel.MainForm.OpenContentPanel(this);
        }

        public void ClosePanel()
        {
            ClosePanel(DialogResult.None);
        }

        public void ClosePanel(DialogResult result)
        {
            this.DialogResult = result;
            this.MainForm.CloseContentPanel(this, this.OwnerPanel);
            if (this.OwnerPanel != null)
            {
                this.OwnerPanel.OnChildClosed(this);
            }
        }

        public virtual void OnClosed()
        {
        }

        public virtual void OnChildClosed(ContentPanel childPanel)
        {
        }

        public virtual void OnSave()
        {
        }

        public virtual void OnCancel()
        {
            ClosePanel(DialogResult.Cancel);
        }

        protected override void WndProc(ref Message m)
        {
            try
            {
                base.WndProc(ref m);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this.MainForm, ex.Message, "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.ClosePanel(DialogResult.Abort);
            }
        }
    }
}
