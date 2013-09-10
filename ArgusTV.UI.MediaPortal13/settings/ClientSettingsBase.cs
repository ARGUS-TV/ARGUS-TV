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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

using MediaPortal.GUI.Library;
using MediaPortal.Dialogs;
using MediaPortal.Profile;
using Action = MediaPortal.GUI.Library.Action;
using MediaPortal.Util;
using MediaPortal.Player;

using ArgusTV.ServiceAgents;
using ArgusTV.ServiceContracts;
using ArgusTV.DataContracts;

namespace ArgusTV.UI.MediaPortal
{
    public class ClientSettingsBase : GUIWindow
    {
        [SkinControlAttribute(7)] protected GUIButtonControl _testConnectionButton = null;
        [SkinControlAttribute(8)] protected GUIButtonControl _serverNameButton = null;
        [SkinControlAttribute(9)] protected GUIButtonControl _tcpPortButton = null;
        [SkinControlAttribute(10)] protected GUICheckButton _rtspStreamingRecButton = null;
        [SkinControlAttribute(11)] protected GUICheckButton _rtspStreamingTVButton = null;
        [SkinControlAttribute(12)] protected GUICheckButton _autoStreamingButton = null;
        [SkinControlAttribute(13)] protected GUICheckButton _enableWolButton = null;
        [SkinControlAttribute(14)] protected GUICheckButton _standbyOnHomeButton = null;
        [SkinControlAttribute(15)] protected GUISpinButton _WolTimeoutButton = null;
        [SkinControlAttribute(16)] protected GUICheckButton _autoFullScreenButton = null;
        [SkinControlAttribute(17)] protected GUICheckButton _showChannelNumbersButton = null;
        [SkinControlAttribute(18)] protected GUICheckButton _hideAllChannelsGroupButton = null;
        [SkinControlAttribute(19)] protected GUICheckButton _preferAC3Button = null;
        [SkinControlAttribute(20)] protected GUICheckButton _dvbSubtitlesButton = null;
        [SkinControlAttribute(21)] protected GUICheckButton _teletextSubtitleButton = null;
        [SkinControlAttribute(22)] protected GUICheckButton _recordingNotificationButton = null;
        [SkinControlAttribute(23)] protected GUIFadeLabel _infoLabel = null;
        [SkinControlAttribute(24)] protected GUILabelControl _serverLabel = null;
        [SkinControlAttribute(25)] protected GUILabelControl _portLabel = null;

        ServerSettings _serverSettings = new ServerSettings();
        private bool _isSingleSeat = true;
        private const string _settingSection = "argustv";
        private bool _mpRestartNeeded = false;
        private bool _restartPlayerNeeded = false;
          
        public ClientSettingsBase()
        {
            GetID = WindowId.ClientSettings;
        }

        protected override void OnWindowLoaded()
        {
            base.OnWindowLoaded();
            _testConnectionButton.Label = Utility.GetLocalizedText(TextId.TestConnection);
            _autoFullScreenButton.Label = Utility.GetLocalizedText(TextId.AutoFullscreen);
            _dvbSubtitlesButton.Label = Utility.GetLocalizedText(TextId.DVBSubtitles);
            _teletextSubtitleButton.Label = Utility.GetLocalizedText(TextId.TXTSubtitles);
            _preferAC3Button.Label = Utility.GetLocalizedText(TextId.PreferAC3);
            _hideAllChannelsGroupButton.Label = Utility.GetLocalizedText(TextId.HideAllChannelsGroup);
            _showChannelNumbersButton.Label = Utility.GetLocalizedText(TextId.ShowChannelNrInGuide);
            _recordingNotificationButton.Label = Utility.GetLocalizedText(TextId.RecordingNotifications);
            _standbyOnHomeButton.Label = Utility.GetLocalizedText(TextId.OnlySleepOnHome);
            _rtspStreamingRecButton.Label = Utility.GetLocalizedText(TextId.RTSPForRec);
            _rtspStreamingTVButton.Label = Utility.GetLocalizedText(TextId.RTSPForTv);
            _autoStreamingButton.Label = Utility.GetLocalizedText(TextId.AutoSelectRTSP);
            _WolTimeoutButton.Label = Utility.GetLocalizedText(TextId.WolTimeout);
            _enableWolButton.Label = Utility.GetLocalizedText(TextId.EnableWol);

            if (_infoLabel != null)
            {
                _infoLabel.Label = Utility.GetLocalizedText(TextId.ClientSettingsInfo);
            }
            if (_serverLabel != null)
            {
                _serverLabel.Label = Utility.GetLocalizedText(TextId.Server); 
            }
            if (_portLabel != null)
            {
                _portLabel.Label = Utility.GetLocalizedText(TextId.Port);
            }
        }

        public override bool Init()
        {
            bool result = Load(GUIGraphicsContext.Skin + @"\ARGUS_ClientSettings.xml");
            if (result)
            {
                GUIPropertyManager.SetProperty("#TV.Settings.ArgusClientSettingsName", Utility.GetLocalizedText(TextId.ArgusClientSettings));
                GUIPropertyManager.SetProperty("#TV.Settings.ArgusServerSettingsName", Utility.GetLocalizedText(TextId.ArgusServerSettings));
                GUIPropertyManager.SetProperty("#TV.Settings.ChannelManagerName", Utility.GetLocalizedText(TextId.ChannelManager));
                GUIPropertyManager.SetProperty("#TV.Settings.ArgusSettingsEnabled", "true"); //info for skins
            }
            return result;
        }

        protected override void OnPageLoad()
        {
            base.OnPageLoad();
            _mpRestartNeeded = false;
            _restartPlayerNeeded = false;

            LoadSettings(false);
            _isSingleSeat = Utility.IsThisASingleSeatSetup(_serverSettings.ServerName);
            UpdateButtons();
            GUIPropertyManager.SetProperty("#currentmodule", Utility.GetLocalizedText(TextId.ArgusClientSettings));
        }

        protected override void OnPageDestroy(int new_windowId)
        {
            SaveSettings(false);
            GUIWaitCursor.Hide();

            if (_mpRestartNeeded)
            {
                GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_YES_NO);
                if (dlgYesNo != null)
                {
                    dlgYesNo.Reset();
                    dlgYesNo.SetHeading(927);
                    dlgYesNo.SetLine(1, Utility.GetLocalizedText(TextId.RecommendedToRestartMP));
                    dlgYesNo.SetLine(2, Utility.GetLocalizedText(TextId.RestartMPNow));
                    dlgYesNo.SetDefaultToYes(true);
                    dlgYesNo.DoModal(GetID);

                    if (dlgYesNo.IsConfirmed)
                    {
                        Utility.RestartMP();
                    }
                }
            }
            else if (_restartPlayerNeeded)
            {
                if (g_Player.Playing && (g_Player.IsTV || g_Player.IsRadio || g_Player.IsTVRecording))
                {
                    GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_YES_NO);
                    if (dlgYesNo != null)
                    {
                        dlgYesNo.Reset();
                        dlgYesNo.SetHeading(927);
                        dlgYesNo.SetLine(1, Utility.GetLocalizedText(TextId.RecommendedToStopPlayback));
                        dlgYesNo.SetLine(2, Utility.GetLocalizedText(TextId.StopPlayBackNow));
                        dlgYesNo.SetDefaultToYes(true);
                        dlgYesNo.DoModal(GetID);

                        if (dlgYesNo.IsConfirmed)
                        {
                            g_Player.Stop();
                        }
                    }
                }
            }
            base.OnPageDestroy(new_windowId);
        }

        protected override void OnClicked(int controlId, GUIControl control, Action.ActionType actionType)
        {
            if (control == _testConnectionButton)
            {
                TestConnection();
                UpdateButtons();
            }
            else if (control == _serverNameButton)
            {
                VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                if (keyboard != null)
                {
                    keyboard.Reset();
                    keyboard.IsSearchKeyboard = false;
                    keyboard.Text = _serverNameButton.Label ?? "localhost";
                    keyboard.DoModal(GetID);
                    if (keyboard.IsConfirmed)
                    {
                        _serverNameButton.Label = keyboard.Text.Trim();
                    }
                }
            }
            else if (control == _tcpPortButton)
            {
                VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                if (keyboard != null)
                {
                    int port = ServerSettings.DefaultTcpPort;
                    keyboard.Reset();
                    keyboard.IsSearchKeyboard = false;
                    keyboard.Text = _tcpPortButton.Label ?? port.ToString();
                    keyboard.DoModal(GetID);
                    if (keyboard.IsConfirmed)
                    {
                        if (!Int32.TryParse(keyboard.Text, out port))
                        {
                            port = ServerSettings.DefaultTcpPort;
                        }
                        _tcpPortButton.Label = port.ToString();
                    }
                }
            }
            else if (control == _enableWolButton || control == _autoStreamingButton)
            {
                UpdateButtons();
            }

            if (control == _autoStreamingButton || control == _rtspStreamingTVButton 
                || control == _rtspStreamingRecButton || control == _teletextSubtitleButton
                || control == _dvbSubtitlesButton || control == _preferAC3Button
                || control == _recordingNotificationButton )
            {
                _restartPlayerNeeded = true;
            }
            base.OnClicked(controlId, control, actionType);
        }

        private void LoadSettings(bool onlyServerSettings)
        {
            _serverSettings = PluginMain.LoadServerSettings();
            int timeout = _serverSettings.WakeOnLan.TimeoutSeconds;

            bool valueFoundInList = false;
            for (int i = 0; i <= 100; i++)
            {
                _WolTimeoutButton.AddSpinLabel(i.ToString(), 0);
                if (i == timeout) valueFoundInList = true;
            }

            if (valueFoundInList)
            {
                _WolTimeoutButton.SpinValue = timeout;
            }
            else
            {
                _WolTimeoutButton.AddSpinLabel(timeout.ToString(), 0);
                _WolTimeoutButton.SpinValue = _WolTimeoutButton.SpinMaxValue() - 1;
            }
            
            _serverNameButton.Label = _serverSettings.ServerName;
            _tcpPortButton.Label = _serverSettings.Port.ToString();
            _enableWolButton.Selected = _serverSettings.WakeOnLan.Enabled;

            if (!onlyServerSettings)
            {
                _standbyOnHomeButton.Selected = PluginMain.NoClientStandbyWhenNotHome;
                _autoStreamingButton.Selected = PluginMain.AutoStreamingMode;
                _rtspStreamingTVButton.Selected = PluginMain.PreferRtspForLiveTv;
                _rtspStreamingRecButton.Selected = PluginMain.PlayRecordingsOverRtsp;

                using (Settings xmlreader = new MPSettings())
                {
                    _recordingNotificationButton.Selected = xmlreader.GetValueAsBool("mytv", "enableRecNotifier", false);
                    _autoFullScreenButton.Selected = xmlreader.GetValueAsBool("mytv", "autofullscreen", true);
                    _showChannelNumbersButton.Selected = xmlreader.GetValueAsBool("mytv", "showchannelnumber", false);
                    _hideAllChannelsGroupButton.Selected = xmlreader.GetValueAsBool("mytv", "hideAllChannelsGroup", false);
                    _dvbSubtitlesButton.Selected = xmlreader.GetValueAsBool("tvservice", "dvbbitmapsubtitles", false);
                    _teletextSubtitleButton.Selected = xmlreader.GetValueAsBool("tvservice", "dvbttxtsubtitles", false);
                    _preferAC3Button.Selected = xmlreader.GetValueAsBool("tvservice", "preferac3", false);
                }
            }
        }

        private void UpdateButtons()
        {
            _WolTimeoutButton.IsEnabled = _enableWolButton.Selected;
            _rtspStreamingTVButton.Disabled = _autoStreamingButton.Selected;
            _rtspStreamingRecButton.Disabled = _autoStreamingButton.Selected;

            if (_autoStreamingButton.Selected)
            {
                _rtspStreamingTVButton.Selected = !_isSingleSeat;
                _rtspStreamingRecButton.Selected = false;
            }
        }

        private void SaveSettings(bool onlyServerSettings)
        {
            using (Settings xmlwriter = new MPSettings())
            {
                xmlwriter.SetValue(_settingSection, TvHome.SettingName.Server, _serverSettings.ServerName);
                xmlwriter.SetValue(_settingSection, TvHome.SettingName.TcpPort, _serverSettings.Port);
                xmlwriter.SetValue(_settingSection, TvHome.SettingName.MacAddresses, _serverSettings.WakeOnLan.MacAddresses);
                xmlwriter.SetValue(_settingSection, TvHome.SettingName.IPAddress, _serverSettings.WakeOnLan.IPAddress);
                xmlwriter.SetValueAsBool(_settingSection, TvHome.SettingName.UseWakeOnLan, _serverSettings.WakeOnLan.Enabled);
                xmlwriter.SetValue(_settingSection, TvHome.SettingName.WakeOnLanTimeoutSeconds, _serverSettings.WakeOnLan.TimeoutSeconds);

                if (!onlyServerSettings)
                {
                    xmlwriter.SetValueAsBool(_settingSection, TvHome.SettingName.NoClientStandbyWhenNotHome, _standbyOnHomeButton.Selected);
                    xmlwriter.SetValueAsBool(_settingSection, TvHome.SettingName.AvoidRtspForLiveTv, !_rtspStreamingTVButton.Selected);
                    xmlwriter.SetValueAsBool(_settingSection, TvHome.SettingName.PlayRecordingsOverRtsp, _rtspStreamingRecButton.Selected);
                    xmlwriter.SetValueAsBool(_settingSection, TvHome.SettingName.AutoStreamingMode, _autoStreamingButton.Selected);
                    xmlwriter.SetValueAsBool(_settingSection, TvHome.SettingName.IsSingleSeat, Utility.IsThisASingleSeatSetup(_serverSettings.ServerName));

                    xmlwriter.SetValueAsBool("mytv", "enableRecNotifier", _recordingNotificationButton.Selected);
                    xmlwriter.SetValueAsBool("mytv", "autofullscreen", _autoFullScreenButton.Selected);
                    xmlwriter.SetValueAsBool("mytv", "showchannelnumber", _showChannelNumbersButton.Selected);
                    xmlwriter.SetValueAsBool("mytv", "hideAllChannelsGroup", _hideAllChannelsGroupButton.Selected);
                    xmlwriter.SetValueAsBool("tvservice", "dvbbitmapsubtitles", _dvbSubtitlesButton.Selected);
                    xmlwriter.SetValueAsBool("tvservice", "dvbttxtsubtitles", _teletextSubtitleButton.Selected);
                    xmlwriter.SetValueAsBool("tvservice", "preferac3", _preferAC3Button.Selected);
                }
            }

            PluginMain.ClearCachedBooleanSettings();
            if (PluginMain.Navigator != null)
            {
                PluginMain.Navigator.Reload();
            }

            TvHome.SettingChanged();
            HomeBase.OnSettingChanged();
        }

        private bool TestConnection()
        {
            bool wasConnected = false;
            bool portChanged = false;
            bool serverChanged = false;
            string errorMessage = string.Empty;

            PluginMain.ForceNoConnection = true;
            if (ServiceChannelFactories.IsInitialized)
            {
                wasConnected = true;
                if (_serverNameButton.Label != ServiceChannelFactories.ServerSettings.ServerName)
                {
                    serverChanged = true;
                }
                if (Int32.Parse(_tcpPortButton.Label) != ServiceChannelFactories.ServerSettings.Port)
                {
                    portChanged = true;
                }
            }

            _serverSettings.ServerName = _serverNameButton.Label;
            _serverSettings.Transport = ServiceTransport.NetTcp;
            _serverSettings.Port = Int32.Parse(_tcpPortButton.Label);
            _serverSettings.WakeOnLan.Enabled = _enableWolButton.Selected;
            _serverSettings.WakeOnLan.TimeoutSeconds = Int32.Parse(_WolTimeoutButton.SpinLabel);
            _isSingleSeat = Utility.IsThisASingleSeatSetup(_serverSettings.ServerName);

            if (Utility.InitialiseServerSettings(_serverSettings, out errorMessage))
            {
                GUIDialogOK pDlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_OK);
                if (pDlgOK != null)
                {
                    pDlgOK.Reset();
                    pDlgOK.SetHeading(Utility.GetLocalizedText(TextId.Information));
                    pDlgOK.SetLine(1, Utility.GetLocalizedText(TextId.ConnectionSucceeded));
                    pDlgOK.SetLine(2, "Single seat:" + _isSingleSeat);
                    pDlgOK.DoModal(this.GetID);
                }

                SaveSettings(true);
                PluginMain.ForceNoConnection = false;

                if (wasConnected && (serverChanged || portChanged))
                {
                    //we switched between 2 different servers, we suggest to restart mp.
                    _mpRestartNeeded = true;
                }
                return true;
            }
            else
            {
                if (wasConnected)
                {
                    //load settings that worked before the test button was pressed.
                    LoadSettings(true);
                    UpdateButtons();

                    string error;
                    if (Utility.InitialiseServerSettings(_serverSettings, out error))
                    {
                        PluginMain.ForceNoConnection = false;
                    }
                }

                GUIDialogOK pDlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_OK);
                if (pDlgOK != null)
                {
                    pDlgOK.Reset();
                    pDlgOK.SetHeading(Utility.GetLocalizedText(TextId.Error));
                    pDlgOK.SetLine(1, Utility.GetLocalizedText(TextId.ConnectionFailed));
                    pDlgOK.SetLine(2, errorMessage);
                    pDlgOK.DoModal(this.GetID);
                }
            }
            return false;
        }
    }
}
