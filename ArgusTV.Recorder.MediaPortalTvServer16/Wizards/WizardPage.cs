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

namespace ArgusTV.Recorder.MediaPortalTvServer.Wizards
{
    internal partial class WizardPage<TContext> : UserControl
        where TContext : new()
    {
        private WizardForm<TContext> _parentWizard;

        public WizardPage()
        {
            InitializeComponent();
        }

        public virtual TContext Context
        {
            get { return _parentWizard.Context; }
        }

        public virtual WizardForm<TContext> ParentWizard
        {
            get { return _parentWizard; }
            set { _parentWizard = value; }
        }

        public virtual string PageTitle
        {
            get { return String.Empty; }
        }

        public virtual string PageInformation
        {
            get { return String.Empty; }
        }

        public virtual void OnEnterPage(bool isBack)
        {
        }

        public virtual void OnLeavePage(bool isBack)
        {
        }

        public virtual bool IsPageActive()
        {
            return true;
        }

        public virtual bool IsBackAllowed()
        {
            return true;
        }

        public virtual bool IsNextAllowed()
        {
            return true;
        }

        public virtual bool OnFinish()
        {
            return true;
        }

        public virtual void OnCancel()
        {
        }
    }
}
