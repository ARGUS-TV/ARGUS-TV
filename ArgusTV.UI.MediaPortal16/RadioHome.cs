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

using MediaPortal.GUI.Library;
using MediaPortal.Configuration;
using MediaPortal.Player;

using ArgusTV.DataContracts;
using ArgusTV.ServiceProxy;

namespace ArgusTV.UI.MediaPortal
{
    public class RadioHome : HomeBase, ISetupForm, IShowPlugin
    {
        public RadioHome()
            : base(ChannelType.Radio){}

        #region ISetupForm Members

        public string Author()
        {
            return "http://www.argus-tv.com/ (dot-i)";
        }

        public bool CanEnable()
        {
            //TODO
            //disabling radio can also disable TV,so prevent it for now until we find a real solution.
            return false;
        }

        public bool DefaultEnabled()
        {
            return true;
        }

        public string Description()
        {
            return "ARGUS Radio plugin";
        }

        public bool GetHome(out string strButtonText, out string strButtonImage, out string strButtonImageFocus, out string strPictureImage)
        {
            strButtonText = GUILocalizeStrings.Get(665);
            strButtonImage = String.Empty;
            strButtonImageFocus = String.Empty;
            strPictureImage = @"hover_my radio.png";
            return true;
        }

        public int GetWindowId()
        {
            return WindowId.RadioHome;
        }

        public bool HasSetup()
        {
            return false;
        }

        public string PluginName()
        {
            return "ARGUS Radio";
        }

        public void ShowPlugin()
        { }

        #endregion

        #region overrides

        public override bool Init()
        {
            bool result = Load(GUIGraphicsContext.Skin + @"\ARGUS_RadioHome.xml");
            if (result)
            {
                LoadSettings();
                base.Init();
            }
            return result;
        }

        public override void DeInit()
        {
            base.DeInit();
        }

        protected override void OnWindowLoaded()
        {
            base.OnWindowLoaded();
            _tvGuideButton.Label = Utility.GetLocalizedText(TextId.RadioGuide);
            _recordingsButton.Label = Utility.GetLocalizedText(TextId.RecordedRadio);
            _tvOnOffButton.Label = Utility.GetLocalizedText(TextId.RadioOn);
            _teletextButton = null;
        }

        protected override void OnPageDestroy(int new_windowId)
        {
            SaveSettings();
            base.OnPageDestroy(new_windowId);
        }

        public override int GetID
        {
            get { return WindowId.RadioHome; }
            set { }
        }

        #endregion

        #region public methods

        public static void UpdateProgressPercentageBar()
        {
            DoUpdateProgressPercentageBar(ChannelType.Radio,false);
        }

        public static void SetMusicProperties(string chanName, Guid channelId)
        {
            string logo = string.Empty;
            if (channelId != Guid.Empty && chanName != string.Empty)
            {
                logo = Utility.GetLogoImage(channelId, chanName);
            }
            else
            {
                chanName = string.Empty;
            }

            if (string.IsNullOrEmpty(logo))
            {
                logo = "defaultMyRadioBig.png";
            }

             GUIPropertyManager.RemovePlayerProperties();
             GUIPropertyManager.SetProperty("#Play.Current.Thumb", logo);
             GUIPropertyManager.SetProperty("#Play.Current.ArtistThumb", chanName);
             GUIPropertyManager.SetProperty("#Play.Current.Album", chanName);
             GUIPropertyManager.SetProperty("#Play.Current.Title", chanName);
             GUIPropertyManager.SetProperty("#Play.Current.Artist", chanName);
        }

        #endregion

        #region Serialisation

        private static bool _settingsLoaded;
        private void LoadSettings()
        {
            if (!_settingsLoaded)
            {
                _settingsLoaded = true;
            }
        }

        private void SaveSettings()
        { }

        protected override bool AutoTurnOnStream
        {
            get { return false; }
        }

        #endregion

        #region IPlugin Members

        public void Start()
        {}

        public void Stop()
        {}

        #endregion

        #region IShowPlugin Members

        public bool ShowDefaultHome()
        {
            return true;
        }

        #endregion
    }
}
