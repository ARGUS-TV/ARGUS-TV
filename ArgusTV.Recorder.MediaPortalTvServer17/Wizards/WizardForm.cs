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

using ArgusTV.Recorder.MediaPortalTvServer.Properties;

namespace ArgusTV.Recorder.MediaPortalTvServer.Wizards
{
    internal partial class WizardForm<TContext> : Form
        where TContext : new()
    {
        private TContext _context;
        private int _currentIndex;
        private WizardPage<TContext> _currentPage;

        public WizardForm()
        {
            InitializeComponent();
            _headerPictureBox.BackgroundImage = Resources.WizardHeader;
            _context = new TContext();
        }

        public WizardForm(TContext context)
        {
            InitializeComponent();
            _headerPictureBox.BackgroundImage = Resources.WizardHeader;
            _context = context;
        }

        public virtual TContext Context
        {
            get { return _context; }
        }

        public void AddPage(WizardPage<TContext> page)
        {
            page.Dock = DockStyle.Fill;
            _pagesPanel.Controls.Add(page);
        }

        private void WizardForm_Load(object sender, EventArgs e)
        {
            _currentIndex = -1;
            _currentPage = null;
            if (_pagesPanel.Controls.Count > 0)
            {
                SwitchToNextPanel();
            }
        }

        public bool SwitchToNextPanel()
        {
            int newIndex = _currentIndex + 1;
            if (newIndex >= _pagesPanel.Controls.Count)
            {
                return false;
            }
            while (!SwitchToPanel(newIndex, false))
            {
                newIndex++;
                if (newIndex >= _pagesPanel.Controls.Count)
                {
                    return false;
                }
            }
            return true;
        }

        public bool SwitchToPreviousPanel()
        {
            int newIndex = _currentIndex - 1;
            if (newIndex < 0)
            {
                return false;
            }
            while (!SwitchToPanel(newIndex, true))
            {
                newIndex--;
                if (newIndex < 0)
                {
                    return false;
                }
            }
            return true;
        }

        private bool SwitchToPanel(int index, bool isBack)
        {
            WizardPage<TContext> pageToActivate = (WizardPage<TContext>)_pagesPanel.Controls[index];
            pageToActivate.ParentWizard = this;
            if (!pageToActivate.IsPageActive())
            {
                return false;
            }

            _headerTitleLabel.Text = pageToActivate.PageTitle;
            _headerInfoLabel.Text = pageToActivate.PageInformation;
            pageToActivate.Visible = true;
            for (int pageIndex = 0; pageIndex < _pagesPanel.Controls.Count; pageIndex++)
            {
                if (pageIndex != index)
                {
                    if (pageIndex == _currentIndex)
                    {
                        ((WizardPage<TContext>)_pagesPanel.Controls[pageIndex]).OnLeavePage(isBack);
                    }
                    _pagesPanel.Controls[pageIndex].Visible = false;
                }
            }

            if (index == _pagesPanel.Controls.Count - 1)
            {
                _finishButton.Visible = true;
                _nextButton.Visible = false;
            }
            else
            {
                _finishButton.Visible = false;
                _nextButton.Visible = (index < _pagesPanel.Controls.Count - 1);
            }
            _backButton.Visible = (index > 0);

            _currentIndex = index;
            _currentPage = pageToActivate;
            pageToActivate.OnEnterPage(isBack);
            return true;
        }

        private void _nextButton_Click(object sender, EventArgs e)
        {
            if (_currentIndex >= 0
                && _currentIndex < _pagesPanel.Controls.Count - 1
                && _currentPage.IsNextAllowed())
            {
                SwitchToNextPanel();
            }
        }

        private void _backButton_Click(object sender, EventArgs e)
        {
            if (_currentIndex > 0
                && _currentPage.IsBackAllowed())
            {
                SwitchToPreviousPanel();
            }
        }

        private void _finishButton_Click(object sender, EventArgs e)
        {
            if (_currentPage != null
                && _currentPage.IsNextAllowed())
            {
                if (_currentPage.OnFinish())
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            if (_currentPage != null)
            {
                _currentPage.OnCancel();
            }
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
