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
using System.Text;

using ArgusTV.DataContracts;
using ArgusTV.ServiceProxy;

namespace ArgusTV.UI.Process.SearchGuide
{
    public class SearchGuideController
    {
        private SearchGuideModel _model;

        public SearchGuideController(SearchGuideModel model)
        {
            _model = model;
        }

        public void Initialize()
        {
        }

        public void SetChannelType(ChannelType channelType)
        {
            _model.ChannelType = channelType;
            ClearSearchText();
        }

        public void ClearSearchText()
        {
            _model.SearchText = String.Empty;
            _model.Titles = new List<string>();
            ClearCurrentTitle();
        }

        public void SearchTitles(string searchText)
        {
            _model.SearchText = searchText.Trim();
            if (String.IsNullOrEmpty(_model.SearchText))
            {
                ClearSearchText();
            }
            else
            {
                _model.Titles = Proxies.SchedulerService.GetTitlesByPartialTitle(_model.ChannelType, _model.SearchText);
                ClearCurrentTitle();
            }
        }

        public void ClearCurrentTitle()
        {
            _model.CurrentTitle = String.Empty;
            _model.AllUpcomingGuidePrograms.Clear();
            _model.CurrentTitlePrograms = new ChannelProgramsList(new List<ChannelProgram>());
        }

        public void GetProgramsForTitle(string title)
        {
            _model.CurrentTitle = title;
            RefreshAllUpcomingPrograms();
            _model.CurrentTitlePrograms = new ChannelProgramsList(Proxies.SchedulerService.SearchGuideByTitle(_model.ChannelType, _model.CurrentTitle, false));
        }

        public void RefreshAllUpcomingPrograms()
        {
            var upcomingRecordings = Proxies.ControlService.GetAllUpcomingRecordings(UpcomingRecordingsFilter.All, true);
            var upcomingAlerts = Proxies.SchedulerService.GetUpcomingGuidePrograms(ScheduleType.Alert, true);
            var upcomingSuggestions = Proxies.SchedulerService.GetUpcomingGuidePrograms(ScheduleType.Suggestion, true);
            _model.AllUpcomingGuidePrograms = new UpcomingGuideProgramsDictionary(upcomingRecordings, upcomingAlerts, upcomingSuggestions);
        }
    }
}
