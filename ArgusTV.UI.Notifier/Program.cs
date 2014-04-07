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
using System.Windows.Forms;
using System.Globalization;
using System.Net;
using System.Net.Security;
using System.Threading;

namespace ArgusTV.UI.Notifier
{
    static class Program
    {
        private const string _singleInstanceGuid = "{c3998ca2-bf97-4a7f-a981-90d317e1f459}";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                bool firstInstance;
                using (Mutex singleInstanceMutex = new Mutex(true, _singleInstanceGuid, out firstInstance))
                {
                    if (firstInstance)
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);

                        int port = GetFreeTcpPort(Properties.Settings.Default.EventsNetTcpPort);

                        string eventsServiceBaseUrl = String.Format(CultureInfo.InvariantCulture, "net.tcp://{0}:{1}/ArgusTV.UI.Notifier/",
                            Dns.GetHostName(), port);

                        //
                        // Create the form *before* opening the host for the WCF thread synchronization context!
                        //
                        StatusForm form = new StatusForm();
                        form.EventsServiceBaseUrl = eventsServiceBaseUrl;

                        //ServiceHost recordingEventsHost = EventsListenerService.CreateServiceHost(eventsServiceBaseUrl);
                        //EventsListenerService.StatusForm = form;
                        //recordingEventsHost.Open();

                        try
                        {
                            Application.Run(form);
                        }
                        finally
                        {
                            //recordingEventsHost.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ArgusTV.UI.Notifier", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static int GetFreeTcpPort(int port)
        {
            for (; ; )
            {
                try
                {
                    System.Net.Sockets.TcpListener tcpListener = new System.Net.Sockets.TcpListener(IPAddress.Any, port);
                    tcpListener.Start();
                    tcpListener.Stop();
                    break;
                }
                catch (System.Net.Sockets.SocketException)
                {
                    port--;
                }
            }
            return port;
        }
    }
}
