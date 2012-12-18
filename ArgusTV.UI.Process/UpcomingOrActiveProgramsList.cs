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
using System.Text;
using ArgusTV.DataContracts;

namespace ArgusTV.UI.Process
{
    public class UpcomingOrActiveProgramsList : SortableBindingList<UpcomingOrActiveProgramView>
    {
        public UpcomingOrActiveProgramsList(ActiveRecording[] recordings)
            : base(CreateViewList(recordings))
        {
        }

        public UpcomingOrActiveProgramsList(UpcomingRecording[] recordings)
            : base(CreateViewList(recordings))
        {
        }

        public UpcomingOrActiveProgramsList(UpcomingProgram[] programs)
            : base(CreateViewList(programs))
        {
        }

        private static IEnumerable<UpcomingOrActiveProgramView> CreateViewList(ActiveRecording[] recordings)
        {
            List<UpcomingOrActiveProgramView> viewList = new List<UpcomingOrActiveProgramView>();
            foreach (ActiveRecording recording in recordings)
            {
                viewList.Add(new UpcomingOrActiveProgramView(recording));
            }
            return viewList;
        }

        private static IEnumerable<UpcomingOrActiveProgramView> CreateViewList(UpcomingRecording[] recordings)
        {
            List<UpcomingOrActiveProgramView> viewList = new List<UpcomingOrActiveProgramView>();
            foreach (UpcomingRecording recording in recordings)
            {
                viewList.Add(new UpcomingOrActiveProgramView(recording));
            }
            return viewList;
        }

        private static IEnumerable<UpcomingOrActiveProgramView> CreateViewList(UpcomingProgram[] programs)
        {
            List<UpcomingOrActiveProgramView> viewList = new List<UpcomingOrActiveProgramView>();
            foreach (UpcomingProgram program in programs)
            {
                viewList.Add(new UpcomingOrActiveProgramView(program));
            }
            return viewList;
        }

        public UpcomingProgram FindProgramById(Guid programId)
        {
            foreach (UpcomingOrActiveProgramView view in this)
            {
                if (view.UpcomingProgram.UpcomingProgramId == programId)
                {
                    return view.UpcomingProgram;
                }
            }
            return null;
        }

        public void RemoveByProgramId(Guid programId)
        {
            foreach (UpcomingOrActiveProgramView view in this)
            {
                if (view.UpcomingProgram.UpcomingProgramId == programId)
                {
                    this.Remove(view);
                    break;
                }
            }
        }

        public void RemoveActiveRecordings(ActiveRecording[] activeRecordings)
        {
            foreach (ActiveRecording activeRecording in activeRecordings)
            {
                RemoveByProgramId(activeRecording.Program.UpcomingProgramId);
            }
        }

        public void ApplyScheduleFilter(Guid scheduleId)
        {
            if (scheduleId != Guid.Empty)
            {
                List<UpcomingOrActiveProgramView> deleteItems = new List<UpcomingOrActiveProgramView>();
                foreach (UpcomingOrActiveProgramView view in this)
                {
                    if (view.UpcomingProgram.ScheduleId != scheduleId)
                    {
                        deleteItems.Add(view);
                    }
                }
                foreach (UpcomingOrActiveProgramView deleteItem in deleteItems)
                {
                    this.Remove(deleteItem);
                }
            }
        }

        protected override int ComparePropertyValues<TItem>(System.ComponentModel.PropertyDescriptor prop, TItem t1, TItem t2)
        {
            if (prop.Name == "ChannelName")
            {
                UpcomingOrActiveProgramView u1 = t1 as UpcomingOrActiveProgramView;
                UpcomingOrActiveProgramView u2 = t2 as UpcomingOrActiveProgramView;
                if (u1.UpcomingProgram.Channel.LogicalChannelNumber.HasValue)
                {
                    if (u2.UpcomingProgram.Channel.LogicalChannelNumber.HasValue)
                    {
                        return u1.UpcomingProgram.Channel.LogicalChannelNumber.Value.CompareTo(u2.UpcomingProgram.Channel.LogicalChannelNumber.Value);
                    }
                    return -1;
                }
                else if (u2.UpcomingProgram.Channel.LogicalChannelNumber.HasValue)
                {
                    return 1;
                }
                return u1.ChannelName.CompareTo(u2.ChannelName);
            }
            else if (prop.Name == "ProgramTimes")
            {
                UpcomingOrActiveProgramView u1 = t1 as UpcomingOrActiveProgramView;
                UpcomingOrActiveProgramView u2 = t2 as UpcomingOrActiveProgramView;
                return System.Collections.Comparer.Default.Compare(u1.StartTime, u2.StartTime);
            }
            return base.ComparePropertyValues<TItem>(prop, t1, t2);
        }
    }
}
