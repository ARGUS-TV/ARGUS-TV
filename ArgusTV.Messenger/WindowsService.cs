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
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;
using System.Text;

using ArgusTV.Messenger.Msn;

namespace ArgusTV.Messenger
{
    [Obfuscation(Exclude = true)]
    public partial class WindowsService : ServiceBase
    {
        public WindowsService()
        {
            InitializeComponent();
        }

        private IMCommands _imCommands = new IMCommands();

        private MsnThread _msnThread;

        protected override void OnStart(string[] args)
        {
            _msnThread = new MsnThread(_imCommands);
            _msnThread.Start();
        }

		protected override void OnStop()
		{
#if !DEBUG
            // Request enough time for a guide to stop being imported.
            RequestAdditionalTime(30 * 60 * 1000);
#endif
            _msnThread.Stop(true);
		}

#if DEBUG
		internal void StartService()
		{
			OnStart(null);
		}

		internal void StopService()
		{
			OnStop();
		}
#endif
    }
}
