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

using ArgusTV.DataContracts;

namespace ArgusTV.Common.Recorders.Utility
{
    public class RecordingThreadCollection<T> : Dictionary<Guid, T>
        where T : RecordingThreadBase
    {
        public bool StartNewThread(T thread)
        {
            lock (this.SyncLock)
            {
                T existingThread = FindByProgram(thread.RecordingProgram);
                if (existingThread == null)
                {
                    this[thread.RecordingProgram.UpcomingProgramId] = thread;
                    HandleConsecutiveThread(thread);
                    thread.Start();
                    return true;
                }
            }
            return false;
        }

        public bool ValidateAndUpdateRecording(UpcomingProgram recordingProgram, DateTime stopTimeUtc, out bool threadNotFound)
        {
            return ValidateAndUpdateRecording(recordingProgram.UpcomingProgramId, stopTimeUtc, out threadNotFound);
        }

        public bool ValidateAndUpdateRecording(Guid programId, DateTime stopTimeUtc, out bool threadNotFound)
        {
            lock (this.SyncLock)
            {
                T thread = FindByProgramId(programId);
                if (thread != null)
                {
                    threadNotFound = false;
                    thread.StopTimeUtc = stopTimeUtc;
                    return thread.IsRecording;
                }
            }
            threadNotFound = true;
            return false;
        }

        public bool StopThreadAsync(UpcomingProgram recordingProgram)
        {
            return StopThreadAsync(recordingProgram.UpcomingProgramId);
        }

        public bool StopThreadAsync(Guid programId)
        {
            lock (this.SyncLock)
            {
                T thread = FindByProgramId(programId);
                if (thread != null)
                {
                    StopThreadAsync(thread);
                    return true;
                }
            }
            return false;
        }

        public void StopThreadAsync(T thread)
        {
            thread.Stop(false);
            Remove(thread.RecordingProgram.UpcomingProgramId);
        }

        private object _syncLock = new object();

        public object SyncLock
        {
            get { return _syncLock; }
        }

        public T FindByProgram(UpcomingProgram recordingProgram)
        {
            return FindByProgramId(recordingProgram.UpcomingProgramId);
        }

        public T FindByProgramId(Guid programId)
        {
            CleanupCompleted();
            if (ContainsKey(programId))
            {
                return this[programId];
            }
            return null;
        }

        private void CleanupCompleted()
        {
            List<Guid> ids = new List<Guid>(this.Keys);
            foreach (Guid id in ids)
            {
                if (this[id].ThreadState == System.Threading.ThreadState.Stopped
                    || this[id].ThreadState == System.Threading.ThreadState.Aborted)
                {
                    this.Remove(id);
                }
            }
        }

        private void HandleConsecutiveThread(T newThread)
        {
            foreach (T thread in this.Values)
            {
                if (thread.ChannelAllocation.CardId == newThread.ChannelAllocation.CardId
                    && thread.ChannelAllocation.ChannelId == newThread.ChannelAllocation.ChannelId
                    && thread.StopTimeUtc <= newThread.StartTimeUtc)
                {
                    newThread.WaitForThreadToComplete = thread;
                    break;
                }
            }
        }
    }
}
