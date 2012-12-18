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
    [Serializable]
    public class ChannelProgramsList : SortableBindingList<ChannelProgramView>
    {
        public ChannelProgramsList(ChannelProgram[] programs)
            : base(CreateViewList(programs))
        {
        }

        private static IEnumerable<ChannelProgramView> CreateViewList(ChannelProgram[] programs)
        {
            List<ChannelProgramView> viewList = new List<ChannelProgramView>();
            foreach (ChannelProgram program in programs)
            {
                viewList.Add(new ChannelProgramView(program));
            }
            return viewList;
        }


        protected override int ComparePropertyValues<TItem>(System.ComponentModel.PropertyDescriptor prop, TItem t1, TItem t2)
        {
            if (prop.Name == "ProgramTimes")
            {
                ChannelProgramView i1 = t1 as ChannelProgramView;
                ChannelProgramView i2 = t2 as ChannelProgramView;
                return System.Collections.Comparer.Default.Compare(i1.StartTime, i2.StartTime);
            }
            return base.ComparePropertyValues<TItem>(prop, t1, t2);
        }
    }
}
