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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

using TvLibrary.Interfaces;
using TvEngine.Events;
using TvLibrary.Log;
using Gentle.Framework;

using ArgusTV.ServiceAgents;
using ArgusTV.DataContracts;
using ArgusTV.Common.Threading;
using ArgusTV.Recorder.MediaPortalTvServer.Channels;

namespace ArgusTV.Recorder.MediaPortalTvServer
{
    internal class DvbEpgThread : WorkerThread
    {
        private List<List<GuideProgram>> _guideProgramsToImport = new List<List<GuideProgram>>();
        private AutoResetEvent _newProgramsToImportEvent;
        private object _guideProgramsToImportLock = new object();

        public DvbEpgThread()
            : base("ArgusTVDvbEpg")
        {
        }

        public void ImportProgramsAsync(List<GuideProgram> guidePrograms)
        {
            if (guidePrograms != null
                && guidePrograms.Count > 0)
            {
                lock (_guideProgramsToImportLock)
                {
                    _guideProgramsToImport.Add(guidePrograms);
                    if (_newProgramsToImportEvent != null)
                    {
                        _newProgramsToImportEvent.Set();
                    }
                }
            }
        }

        private List<GuideProgram> GetProgramsToImport()
        {
            lock (_guideProgramsToImportLock)
            {
                List<GuideProgram> result = null;
                if (_guideProgramsToImport.Count > 0)
                {
                    result = _guideProgramsToImport[0];
                    _guideProgramsToImport.RemoveAt(0);
                }
                return result;
            }
        }

        protected override void Run()
        {
            Thread.Sleep(5 * 1000);

            lock (_guideProgramsToImportLock)
            {
                _newProgramsToImportEvent = new AutoResetEvent(false);
            }
            try
            {
                int interval = 1 * 60 * 1000;
#if DEBUG
                interval = 5000;
#endif

                bool aborted = false;
                while (!aborted)
                {
                    try
                    {
                        List<GuideProgram> guidePrograms = GetProgramsToImport();
                        while (guidePrograms != null)
                        {
                            using (GuideServiceAgent tvGuideAgent = new GuideServiceAgent())
                            {
                                Log.Debug("ArgusTV.Recorder.MediaPortalTvServer: ArgusTVDvbEpg: importing {0} programs into ARGUS TV", guidePrograms.Count);
                                foreach (GuideProgram guideProgram in guidePrograms)
                                {
                                    tvGuideAgent.ImportProgram(guideProgram, GuideSource.DvbEpg);
                                    aborted = this.StopThreadEvent.WaitOne(0, false);
                                    if (aborted)
                                    {
                                        break;
                                    }
                                }
                            }
                            aborted = this.StopThreadEvent.WaitOne(0);
                            if (aborted)
                            {
                                break;
                            }
                            guidePrograms = GetProgramsToImport();
                        }
                        if (!aborted)
                        {
                            aborted = (0 == WaitHandle.WaitAny(new WaitHandle[] { this.StopThreadEvent, _newProgramsToImportEvent }, interval, false));
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("ArgusTVDvbEpg error: {0}", ex.Message);
                        // Delay for a short while and then restart.
                        aborted = this.StopThreadEvent.WaitOne(30 * 1000, false);
                    }
                }
            }
            finally
            {
                lock (_guideProgramsToImportLock)
                {
                    _newProgramsToImportEvent.Close();
                    _newProgramsToImportEvent = null;
                }
            }
        }
    }
}
