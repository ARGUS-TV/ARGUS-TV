#region Copyright (C) 2007-2013 ARGUS TV
/*
 *	Copyright (C) 2007-2013 ARGUS TV
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
 */

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using MediaPortal.GUI.Library;

namespace ArgusTV.UI.MediaPortal
{
    internal static class WindowId
    {
        public static int TvHome
        {
            get { return (int)GUIWindow.Window.WINDOW_TV; }
        }

        public static int RadioHome
        {
            get { return (int)GUIWindow.Window.WINDOW_RADIO; }
        }

        public static int TvGuide
        {
            get { return (int)GUIWindow.Window.WINDOW_TVGUIDE; }
        }

        public static int RadioGuide
        {
            get { return (int)GUIWindow.Window.WINDOW_RADIO_GUIDE; }
        }

        public static int TvGuideDialog
        {
            get { return (int)GUIWindow.Window.WINDOW_DIALOG_TVGUIDE; }
        }

        public static int ProgramInfo
        {
            get { return (int)GUIWindow.Window.WINDOW_TV_PROGRAM_INFO; }
        }

        public static int RecordedTv
        {
            get { return (int)GUIWindow.Window.WINDOW_RECORDEDTV; }
        }

        public static int Teletext
        {
            get { return (int)GUIWindow.Window.WINDOW_TELETEXT; }
        }

        public const int UpcomingTvPrograms = 49848;
        public const int UpcomingRadioPrograms = 49847;
        public const int ActiveRecordings = 49849;
        public const int RecordedTvInfo = 49846;
        public const int TvGuideSearch = 49850;
        public const int RadioGuideSearch = 49851;
        public const int RecordedRadio = 49852;
        public const int ManualShedule = 49853;
        public const int SettingsHome = 49854;
        public const int ServerSettings = 49855;
        public const int ChannelManagment = 49856;
        public const int ClientSettings = 49857;
        public const int TunerSettings = 49858;
    }
}
