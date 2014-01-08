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

namespace ArgusTV.UI.Process
{
    public class LiveStreamView
    {
        private LiveStream _liveStream;

        public LiveStreamView(LiveStream liveStream)
        {
            _liveStream = liveStream;
        }

        public LiveStream LiveStream
        {
            get { return _liveStream; }
        }

        public string ChannelName
        {
            get { return _liveStream.Channel.DisplayName; }
        }

        public string RtspUrl
        {
            get { return _liveStream.RtspUrl; }
        }

        public DateTime StreamStartedTime
        {
            get { return _liveStream.StreamStartedTime; }
        }

        public string RecorderTuner
        {
            get
            {
                if (_liveStream != null)
                {
                    PluginService recorderTuner = RecorderTunersCache.GetRecorderTunerById(_liveStream.RecorderTunerId);
                    if (recorderTuner == null)
                    {
                        return "?";
                    }
                    if (String.IsNullOrEmpty(_liveStream.CardId))
                    {
                        return recorderTuner.Name;
                    }
                    return recorderTuner.Name + " (" + _liveStream.CardId + ")";
                }
                return String.Empty;
            }
        }
    }
}
