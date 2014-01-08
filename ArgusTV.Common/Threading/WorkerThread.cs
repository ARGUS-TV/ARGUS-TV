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
using System.Threading;

namespace ArgusTV.Common.Threading
{
    public abstract class WorkerThread
    {
        private Thread _thread;

        #region Constructors

        protected WorkerThread(string name)
            : this(name, ThreadPriority.Normal, ApartmentState.MTA)
        {
        }

        protected WorkerThread(string name, ThreadPriority priority)
            : this(name, priority, ApartmentState.MTA)
        {
        }

        protected WorkerThread(string name, ThreadPriority priority, ApartmentState apartmentState)
        {
            _thread = new Thread(new ThreadStart(this.InternalRun));
            _thread.Name = name;
            _thread.Priority = priority;
            _thread.SetApartmentState(apartmentState);
        }

        #endregion

        #region Public

        private ManualResetEvent _threadEndedEvent;

        public ManualResetEvent ThreadEndedEvent
        {
            get { return _threadEndedEvent; }
        }

        public ThreadState ThreadState
        {
            get { return _thread.ThreadState; }
        }

        public virtual void Start()
        {
            _threadEndedEvent = new ManualResetEvent(false);
            _stopThreadEvent = new ManualResetEvent(false);
            _thread.Start();
        }

        public virtual void Stop(bool waitForExit, int millisecondsTimeout)
        {
            lock (this)
            {
                if (this.StopThreadEvent != null)
                {
                    this.StopThreadEvent.Set();
                }
                else
                {
                    waitForExit = false;
                }
            }
            if (waitForExit
                && _thread.ThreadState != ThreadState.Stopped)
            {
                _thread.Join(millisecondsTimeout);
            }
        }

        public virtual void Stop(bool waitForExit)
        {
            Stop(waitForExit, Timeout.Infinite);
        }

        public virtual void Abort()
        {
            _thread.Abort();
        }

        public virtual void Join()
        {
            _thread.Join();
        }

        public virtual bool Join(int millisecondsTimeout)
        {
            return _thread.Join(millisecondsTimeout);
        }

        #endregion

        #region Protected

        private ManualResetEvent _stopThreadEvent;

        protected ManualResetEvent StopThreadEvent
        {
            get { return _stopThreadEvent; }
        }

        protected abstract void Run();

        #endregion

        #region Private

        private void InternalRun()
        {
            try
            {
                Run();
            }
            finally
            {
                lock (this)
                {
                    _stopThreadEvent.Close();
                    _stopThreadEvent = null;
                }
                _threadEndedEvent.Set();
            }
        }

        #endregion
    }
}
