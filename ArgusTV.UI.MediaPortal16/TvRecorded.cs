#region Copyright (C) 2005-2013 Team MediaPortal

/* 
 *	Copyright (C) 2005-2013 Team MediaPortal
 *	http://www.team-mediaportal.com
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

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MediaPortal.Services;
using MediaPortal.Threading;
using MediaPortal.Util;
using MediaPortal.Configuration;

using ArgusTV.DataContracts;
using ArgusTV.ServiceProxy;
using ArgusTV.UI.Process.Recordings;

namespace ArgusTV.UI.MediaPortal
{
    public class TvRecorded : RecordedBase
    {
        protected override string SettingsSection
        {
            get { return "argustv_tvrecorded"; }
        }

        public TvRecorded()
            : base(ChannelType.Television)
        {
            GetID = (int)WindowId.RecordedTv;
        }

        public override void OnAdded()
        {
            // replace g_player's ShowFullScreenWindowTV
            g_Player.ShowFullScreenWindowTV = ShowFullScreenWindowTVHandler;
            g_Player.ShowFullScreenWindowVideo = ShowFullScreenWindowVideoHandler; // singleseaters uses this
            base.OnAdded();
        }

        public override bool Init()
        {
            bool result = Load(GUIGraphicsContext.Skin + @"\ARGUS_RecordedTv2.xml");
            if (result)
            {
                base.Init();
                Utility.CleanupRecordingThumbs();
            }
            return result;
        }

        public override bool IsTv
        {
            get { return true; }
        }

        /// <summary>
        /// This function replaces g_player.ShowFullScreenWindowTV
        /// </summary>
        ///<returns></returns>        
        private static bool ShowFullScreenWindowTVHandler()
        {
            TvHome.SetRecordingChaptersAndJumpPoints();
            if (g_Player.IsTVRecording)
            {
                // watching TV
                if (GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_TVFULLSCREEN)
                    return true;
                Log.Info("TVRecorded: ShowFullScreenWindow switching to fullscreen tv");
                GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_TVFULLSCREEN);
                GUIGraphicsContext.IsFullScreenVideo = true;
                return true;
            }
            return g_Player.ShowFullScreenWindowTVDefault();
        }

        /// <summary>
        /// This function replaces g_player.ShowFullScreenWindowVideo
        /// </summary>
        ///<returns></returns>        
        private static bool ShowFullScreenWindowVideoHandler()
        {
            if (g_Player.IsTVRecording)
            {
                // watching TV
                if (GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_TVFULLSCREEN)
                    return true;
                Log.Info("TVRecorded: ShowFullScreenWindow switching to fullscreen video");
                GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_TVFULLSCREEN);
                GUIGraphicsContext.IsFullScreenVideo = true;
                return true;
            }
            return g_Player.ShowFullScreenWindowVideoDefault();
        }
    }
}
