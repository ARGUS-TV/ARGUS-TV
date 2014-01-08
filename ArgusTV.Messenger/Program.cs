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
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;

using ArgusTV.Common.Logging;

namespace ArgusTV.Messenger
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                //ArgusTV.ServiceImplementation.EventLogger.Initialize();
                Logger.SetLogFilePath("Messenger.log", Properties.Settings.Default.LogLevel);

                WindowsService service = new WindowsService();
#if DEBUG
                System.Threading.Thread.Sleep(5000);
                service.StartService();
                System.Windows.Forms.MessageBox.Show("Click OK to end Messenger service.");
                service.StopService();
#else
                ServiceBase.Run(new ServiceBase[] { service });
#endif
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                //ArgusTV.ServiceImplementation.EventLogger.WriteEntry(ex);
                throw;
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                //ArgusTV.ServiceImplementation.EventLogger.WriteEntry(ex);
            }
        }
    }
}
