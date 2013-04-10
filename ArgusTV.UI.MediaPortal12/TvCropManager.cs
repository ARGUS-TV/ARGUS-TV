#region Copyright (C) 2005-2013 Team MediaPortal

// Copyright (C) 2005-2013 Team MediaPortal
// http://www.team-mediaportal.com
// 
// MediaPortal is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// MediaPortal is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with MediaPortal. If not, see <http://www.gnu.org/licenses/>.

#endregion

#region usings

using System;
using MediaPortal.Configuration;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MediaPortal.Profile;

#endregion

namespace ArgusTV.UI.MediaPortal
{
    internal class TvCropManager
    {
        #region Ctor/Dtor

        public TvCropManager()
        {
            g_Player.PlayBackStarted += new g_Player.StartedHandler(g_Player_PlayBackStarted);
            Log.Info("TvCropManager: Started");
        }

        ~TvCropManager()
        {
            Log.Info("TvCropManager: Stopped");
            g_Player.PlayBackStarted -= new g_Player.StartedHandler(g_Player_PlayBackStarted);
        }

        #endregion

        #region Serialisation

        public static CropSettings CropSettings
        {
            get
            {
                using (Settings xmlreader = new MPSettings())
                {
                    CropSettings cropSettings = new CropSettings(
                      xmlreader.GetValueAsInt("tv", "croptop", 0),
                      xmlreader.GetValueAsInt("tv", "cropbottom", 0),
                      xmlreader.GetValueAsInt("tv", "cropleft", 0),
                      xmlreader.GetValueAsInt("tv", "cropright", 0)
                      );
                    return cropSettings;
                }
            }
            set
            {
                CropSettings cropSettings = value;
                using (Settings xmlwriter = new MPSettings())
                {
                    xmlwriter.SetValue("tv", "croptop", cropSettings.Top);
                    xmlwriter.SetValue("tv", "cropbottom", cropSettings.Bottom);
                    xmlwriter.SetValue("tv", "cropleft", cropSettings.Left);
                    xmlwriter.SetValue("tv", "cropright", cropSettings.Right);
                }
                Log.Info("TvCropManager.SendCropMessage(): {0}, {1}, {2}, {3}", cropSettings.Top, cropSettings.Bottom,
                         cropSettings.Left, cropSettings.Right);
                GUIWindowManager.SendThreadMessage(new GUIMessage(GUIMessage.MessageType.GUI_MSG_PLANESCENE_CROP, 0, 0, 0, 0, 0,
                                                                  cropSettings));
            }
        }

        #endregion

        #region Player events

        /// <summary>
        /// Gets called by g_Player when playback of media has started.
        /// This handles cropping timeshifted TV and recordings.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filename"></param>
        private void g_Player_PlayBackStarted(g_Player.MediaType type, string filename)
        {
            if (type == g_Player.MediaType.TV || type == g_Player.MediaType.Recording)
            {
                try
                {
                    if (CropSettings.Top > 0 || CropSettings.Bottom > 0 || CropSettings.Left > 0 || CropSettings.Right > 0)
                    {
                        Log.Info("TvCropManager.SendCropMessage(): {0}, {1}, {2}, {3}", CropSettings.Top, CropSettings.Bottom,
                                 CropSettings.Left, CropSettings.Right);
                        GUIWindowManager.SendThreadMessage(new GUIMessage(GUIMessage.MessageType.GUI_MSG_PLANESCENE_CROP, 0, 0, 0, 0,
                                                                          0, CropSettings));
                    }
                }
                catch (Exception) { }
            }
        }

        #endregion
    }
}
