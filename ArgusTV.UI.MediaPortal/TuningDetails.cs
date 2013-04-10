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

using System;
using System.Collections.Generic;

using MediaPortal.GUI.Library;
using MediaPortal.Player;

using ArgusTV.DataContracts;
using ArgusTV.DataContracts.Tuning;
using ArgusTV.ServiceAgents;
using ArgusTV.ServiceContracts;

namespace ArgusTV.UI.MediaPortal
{
    public class TuningDetails : GUIInternalWindow
    {
        public TuningDetails()
        {
            GetID = (int)Window.WINDOW_TV_TUNING_DETAILS;
        }

        private ControlServiceAgent _tvControlAgent;
        public IControlService ControlAgent
        {
            get
            {
                if (_tvControlAgent == null)
                {
                    _tvControlAgent = new ControlServiceAgent();
                }
                return _tvControlAgent;
            }
        }

        #region Overrides

        public override bool Init()
        {
            bool bResult = Load(GUIGraphicsContext.Skin + @"\ARGUS_TuningDetails.xml");
            return bResult;
        }

        protected override void OnPageDestroy(int new_windowId)
        {
            g_Player.PlayBackStopped -= new global::MediaPortal.Player.g_Player.StoppedHandler(OnPlayBackStopped);
            g_Player.PlayBackEnded -= new global::MediaPortal.Player.g_Player.EndedHandler(OnPlayBackEnded);
            g_Player.PlayBackChanged -= new global::MediaPortal.Player.g_Player.ChangedHandler(OnPlayBackChanged);

            if (_tvControlAgent != null)
            {
                _tvControlAgent.Dispose();
            }
            base.OnPageDestroy(new_windowId);
        }

        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            if (!g_Player.Playing || !g_Player.IsTV)
            {
                GUIWindowManager.CloseCurrentWindow();
                return;
            }

            g_Player.PlayBackStopped += new global::MediaPortal.Player.g_Player.StoppedHandler(OnPlayBackStopped);
            g_Player.PlayBackEnded += new global::MediaPortal.Player.g_Player.EndedHandler(OnPlayBackEnded);
            g_Player.PlayBackChanged += new global::MediaPortal.Player.g_Player.ChangedHandler(OnPlayBackChanged);

            LiveStream _livestream = PluginMain.Navigator.LiveStream;
            Channel _channel = PluginMain.Navigator.CurrentChannel;

            if (g_Player.Playing && g_Player.IsTV && _livestream != null && _channel != null
                && PluginMain.IsConnected())
            {
                ServiceTuning _serviceTuning = ControlAgent.GetLiveStreamTuningDetails(_livestream);

                if (_livestream.RtspUrl.StartsWith("rtsp://", StringComparison.CurrentCultureIgnoreCase))
                    GUIPropertyManager.SetProperty("#TV.TuningDetails.RTSPURL.value", _livestream.RtspUrl);
                else
                    GUIPropertyManager.SetProperty("#TV.TuningDetails.RTSPURL.value", Utility.GetLocalizedText(TextId.UnavailableText));


                if (g_Player.CurrentFile.StartsWith("rtsp:", StringComparison.InvariantCultureIgnoreCase))
                    GUIPropertyManager.SetProperty("#TV.TuningDetails.StreamingMode.value", "RTSP");
                else
                    GUIPropertyManager.SetProperty("#TV.TuningDetails.StreamingMode.value", "UNC");


                if (g_Player.EnableSubtitle)
                {
                    if ((g_Player.SubtitleStreams > 0 /*|| g_Player.SupportsCC*/) && g_Player.CurrentSubtitleStream >= 0)
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.SubtitleStream.value", g_Player.SubtitleLanguage(g_Player.CurrentSubtitleStream));
                    else
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.SubtitleStream.value", Utility.GetLocalizedText(TextId.UnavailableText));
                }
                else
                {
                    GUIPropertyManager.SetProperty("#TV.TuningDetails.SubtitleStream.value", Utility.GetLocalizedText(TextId.Off));
                }


                GUIPropertyManager.SetProperty("#TV.TuningDetails.RTSPURL.label", "RTSP URL");
                GUIPropertyManager.SetProperty("#TV.TuningDetails.SubtitleStream.label", Utility.GetLocalizedText(TextId.CurrentSubtitle));
                GUIPropertyManager.SetProperty("#TV.TuningDetails.StreamingMode.label", Utility.GetLocalizedText(TextId.StreamingMode));
                GUIPropertyManager.SetProperty("#TV.TuningDetails.AudioType.label", Utility.GetLocalizedText(TextId.AudioType));
                GUIPropertyManager.SetProperty("#TV.TuningDetails.AudioType.value", g_Player.AudioType(g_Player.CurrentAudioStream));
                GUIPropertyManager.SetProperty("#TV.TuningDetails.AudioLanguage.label", Utility.GetLocalizedText(TextId.AudioLanguage));
                GUIPropertyManager.SetProperty("#TV.TuningDetails.AudioLanguage.value", g_Player.AudioLanguage(g_Player.CurrentAudioStream));
                GUIPropertyManager.SetProperty("#TV.TuningDetails.VideoResolution.label", Utility.GetLocalizedText(TextId.VideoResolution));
                GUIPropertyManager.SetProperty("#TV.TuningDetails.VideoResolution.value", g_Player.Width.ToString() + "x" + g_Player.Height.ToString());
                GUIPropertyManager.SetProperty("#TV.TuningDetails.ChannelName.label", Utility.GetLocalizedText(TextId.Channel));
                GUIPropertyManager.SetProperty("#TV.TuningDetails.ChannelName.value", _serviceTuning.Name);
                GUIPropertyManager.SetProperty("#TV.TuningDetails.Frequency.label", Utility.GetLocalizedText(TextId.Frequency));
                GUIPropertyManager.SetProperty("#TV.TuningDetails.Frequency.value", _serviceTuning.Frequency.ToString());
                GUIPropertyManager.SetProperty("#TV.TuningDetails.Provider.label", Utility.GetLocalizedText(TextId.Provider));
                GUIPropertyManager.SetProperty("#TV.TuningDetails.Provider.value", _serviceTuning.ProviderName);
                GUIPropertyManager.SetProperty("#TV.TuningDetails.ChannelType.label", Utility.GetLocalizedText(TextId.Type));
                GUIPropertyManager.SetProperty("#TV.TuningDetails.SignalLevel.label", Utility.GetLocalizedText(TextId.SignalLevel));
                GUIPropertyManager.SetProperty("#TV.TuningDetails.SignalQuality.label", Utility.GetLocalizedText(TextId.SignalQuality));
                             
                switch (_serviceTuning.CardType)
                {
                    case CardType.Analog:
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.ChannelType.value", "Analog");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Provider.value", Utility.GetLocalizedText(TextId.Unknown));
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail1.label", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail1.value", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail2.label", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail2.value", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail3.label", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail3.value", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail4.label", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail4.value", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail5.label", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail5.value", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail6.label", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail6.value", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail7.label", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail7.value", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail8.label", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail8.value", "");
                        break;

                    case CardType.Atsc:
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.ChannelType.value", "ATSC");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Provider.value", Utility.GetLocalizedText(TextId.Unknown));
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail1.label", Utility.GetLocalizedText(TextId.Encrypted));
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail1.value", _serviceTuning.IsFreeToAir ? Utility.GetLocalizedText(TextId.No) : Utility.GetLocalizedText(TextId.Yes));
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail2.label", Utility.GetLocalizedText(TextId.Modulation));
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail2.value", _serviceTuning.Modulation.ToString());
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail3.label", "Symbol Rate");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail3.value", _serviceTuning.SymbolRate.ToString());
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail4.label", "Service ID");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail4.value", _serviceTuning.SID.ToString());
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail5.label", "Transport ID");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail5.value", _serviceTuning.TSID.ToString());
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail6.label", "InnerFecRate");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail6.value", _serviceTuning.InnerFecRate.ToString());
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail7.label", "Physical Channel");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail7.value", _serviceTuning.PhysicalChannel.ToString());
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail8.label", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail8.value", "");
                        break;

                    case CardType.DvbC:
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.ChannelType.value", "DVB-C");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail1.label", Utility.GetLocalizedText(TextId.Encrypted));
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail1.value", _serviceTuning.IsFreeToAir ? Utility.GetLocalizedText(TextId.No) : Utility.GetLocalizedText(TextId.Yes));
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail2.label", Utility.GetLocalizedText(TextId.Modulation));
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail2.value", _serviceTuning.Modulation.ToString());
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail3.label", "Symbol Rate");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail3.value", _serviceTuning.SymbolRate.ToString());
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail4.label", "Service ID");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail4.value", _serviceTuning.SID.ToString());
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail5.label", "Network ID");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail5.value", _serviceTuning.ONID.ToString());
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail6.label", "Transport ID");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail6.value", _serviceTuning.TSID.ToString());
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail7.label", "InnerFecRate");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail7.value", _serviceTuning.InnerFecRate.ToString());
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail8.label", "OuterFecRate");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail8.value", _serviceTuning.OuterFecRate.ToString());
                        break;

                    case CardType.DvbS:
                        if (_serviceTuning.IsDvbS2)
                            GUIPropertyManager.SetProperty("#TV.TuningDetails.ChannelType.value", "DVB-S2");
                        else
                            GUIPropertyManager.SetProperty("#TV.TuningDetails.ChannelType.value", "DVB-S");

                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail1.label", Utility.GetLocalizedText(TextId.SatellitePosition));
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail1.value", _serviceTuning.OrbitalPosition.ToString());
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail2.label", Utility.GetLocalizedText(TextId.Encrypted));
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail2.value", _serviceTuning.IsFreeToAir ? Utility.GetLocalizedText(TextId.No) : Utility.GetLocalizedText(TextId.Yes));
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail3.label", Utility.GetLocalizedText(TextId.Modulation));
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail3.value", _serviceTuning.Modulation.ToString());
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail4.label", Utility.GetLocalizedText(TextId.Polarisation));
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail4.value", _serviceTuning.SignalPolarisation.ToString());
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail5.label", "Symbol Rate");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail5.value", _serviceTuning.SymbolRate.ToString());
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail6.label", "Service ID");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail6.value", _serviceTuning.SID.ToString());
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail7.label", "Network ID");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail7.value", _serviceTuning.ONID.ToString());
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail8.label", "Transport ID");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail8.value", _serviceTuning.TSID.ToString());
                        break;

                    case CardType.DvbT:
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.ChannelType.value", "DVB-T");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail1.label", Utility.GetLocalizedText(TextId.Encrypted));
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail1.value", _serviceTuning.IsFreeToAir ? Utility.GetLocalizedText(TextId.No) : Utility.GetLocalizedText(TextId.Yes));
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail2.label", Utility.GetLocalizedText(TextId.Modulation));
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail2.value", _serviceTuning.Modulation.ToString());
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail3.label", Utility.GetLocalizedText(TextId.Bandwidth));
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail3.value", _serviceTuning.Bandwidth.ToString());
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail4.label", "Service ID");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail4.value", _serviceTuning.SID.ToString());
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail5.label", "Network ID");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail5.value", _serviceTuning.ONID.ToString());
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail6.label", "Transport ID");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail6.value", _serviceTuning.TSID.ToString());
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail7.label", "InnerFecRate");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail7.value", _serviceTuning.InnerFecRate.ToString());
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail8.label", Utility.GetLocalizedText(TextId.TransmissionMode));
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail8.value", _serviceTuning.TransmissionMode.ToString());
                        break;

                    default:
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.ChannelType.value", Utility.GetLocalizedText(TextId.Unknown));
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Provider.value", Utility.GetLocalizedText(TextId.Unknown));
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail1.label", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail1.value", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail2.label", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail2.value", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail3.label", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail3.value", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail4.label", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail4.value", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail5.label", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail5.value", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail6.label", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail6.value", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail7.label", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail7.value", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail8.label", "");
                        GUIPropertyManager.SetProperty("#TV.TuningDetails.Detail8.value", "");
                        break;
                }
            }
        }

        private DateTime _updateTimer = DateTime.MinValue;
        public override void Process()
        {
            TimeSpan ts = DateTime.Now - _updateTimer;
            if (ts.TotalMilliseconds < 500)
            {
                return;
            }
            _updateTimer = DateTime.Now;

            LiveStream _livestream = PluginMain.Navigator.LiveStream;
            if (_livestream != null && g_Player.Playing)
            {
                ServiceTuning _serviceTuning = ControlAgent.GetLiveStreamTuningDetails(_livestream);
                GUIPropertyManager.SetProperty("#TV.TuningDetails.SignalLevel.value", _serviceTuning.SignalStrength.ToString());
                GUIPropertyManager.SetProperty("#TV.TuningDetails.SignalQuality.value", _serviceTuning.SignalQuality.ToString());
                _livestream = null;
            }
        }

        #endregion

        #region Playback Events

        private void OnPlayBackChanged(global::MediaPortal.Player.g_Player.MediaType type, int stoptime, string filename)
        {
            GUIWindowManager.ActivateWindow(WindowId.TvHome);
        }

        private void OnPlayBackStopped(global::MediaPortal.Player.g_Player.MediaType type, int stoptime, string filename)
        {
            GUIWindowManager.ActivateWindow(WindowId.TvHome);
        }

        private void OnPlayBackEnded(global::MediaPortal.Player.g_Player.MediaType type, string filename)
        {
            GUIWindowManager.ActivateWindow(WindowId.TvHome);
        }

        #endregion
    }
}
