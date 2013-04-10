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
using System.Collections;
using System.Globalization;
using System.IO;
using MediaPortal.GUI.Library;
using MediaPortal.Util;
using Action = MediaPortal.GUI.Library.Action;

using ArgusTV.ServiceAgents;
using ArgusTV.DataContracts;

namespace ArgusTV.UI.MediaPortal
{
  public class TvZapOsd : GUIInternalWindow
  {
    [SkinControl(35)] protected GUILabelControl lblCurrentChannel = null;
    [SkinControl(36)] protected GUITextControl lblOnTvNow = null;
    [SkinControl(37)] protected GUITextControl lblOnTvNext = null;
    [SkinControl(100)] protected GUILabelControl lblCurrentTime = null;
    [SkinControl(101)] protected GUILabelControl lblStartTime = null;
    [SkinControl(102)] protected GUILabelControl lblEndTime = null;
    [SkinControl(39)] protected GUIImage imgRecIcon = null;
    [SkinControl(10)] protected GUIImage imgTvChannelLogo = null;
    [SkinControl(38)] protected GUILabelControl lblZapToCannelNo = null;

    private bool m_bNeedRefresh = false;
    private DateTime m_dateTime = DateTime.Now;
    private string channelName = "";
    private string channelNr = "";
    private Guid channelId = Guid.Empty;
    private Channel channel = null;
    private string m_lastError = string.Empty;

    public string LastError
    { 
      get {return m_lastError;}
      set { m_lastError = value;}
    }

    public TvZapOsd()
    {
      GetID = (int)Window.WINDOW_TVZAPOSD;
    }

    public override bool IsTv
    {
      get { return true; }
    }

    public override bool Init()
    {
      bool bResult = Load(GUIGraphicsContext.Skin + @"\tvZAPOSD.xml");
      GetID = (int)Window.WINDOW_TVZAPOSD;
      return bResult;
    }

    public override bool SupportsDelayedLoad
    {
      get { return false; }
    }

    public override void Render(float timePassed)
    {
      UpdateProgressBar();
      Get_TimeInfo(); // show the time elapsed/total playing time
      base.Render(timePassed); // render our controls to the screen
    }

    public override void OnAction(Action action)
    {
      switch (action.wID)
      {
        case Action.ActionType.ACTION_SHOW_OSD:
          {
            return;
          }

        case Action.ActionType.ACTION_NEXT_CHANNEL:
          {
            OnNextChannel();
            return;
          }

        case Action.ActionType.ACTION_PREV_CHANNEL:
          {
            OnPreviousChannel();
            return;
          }

        case Action.ActionType.ACTION_CONTEXT_MENU:
          {
            if (action.wID == Action.ActionType.ACTION_CONTEXT_MENU)
            {
              TvFullScreen tvWindow = (TvFullScreen)GUIWindowManager.GetWindow((int)Window.WINDOW_TVFULLSCREEN);
              tvWindow.OnAction(new Action(Action.ActionType.ACTION_SHOW_OSD, 0, 0));
              tvWindow.OnAction(action);
            }
            return;
          }
      }
      base.OnAction(action);
    }

    public override bool OnMessage(GUIMessage message)
    {
      switch (message.Message)
      {
        case GUIMessage.MessageType.GUI_MSG_WINDOW_INIT:
          //m_lastError = message.Label; // custom Text to show
          break;
      }
      return base.OnMessage(message);
    }

    protected override void OnPageDestroy(int newWindowId)
    {
      Log.Debug("zaposd pagedestroy");
      Dispose();
      base.OnPageDestroy(newWindowId);
      GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(100000 + newWindowId));
    }

    protected override void OnPageLoad()
    {
      Log.Debug("zaposd pageload");
      AllocResources();
      ResetAllControls(); // make sure the controls are positioned relevant to the OSD Y offset
      m_bNeedRefresh = false;
      m_dateTime = DateTime.Now;
      channelNr = GetChannelNumber();
      SetChannelIdAndName();
      SetCurrentChannelLogo();
      base.OnPageLoad();
      GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(100000 + GetID));
    }

    private void Get_TimeInfo()
    {
      string strTime = channelName;
      GuideProgram prog = PluginMain.GetCurrentForChannel(channel);
      if (prog != null)
      {
        strTime = String.Format("{0}-{1}",
                                prog.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                                prog.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));
      }
      if (lblCurrentTime != null)
      {
        lblCurrentTime.Label = strTime;
      }
    }

    public override void ResetAllControls()
    {
      bool bOffScreen = false;
      int iCalibrationY = GUIGraphicsContext.OSDOffset;
      int iTop = GUIGraphicsContext.OverScanTop;
      int iMin = 0;

      foreach (CPosition pos in _listPositions)
      {
        pos.control.SetPosition((int)pos.XPos, (int)pos.YPos + iCalibrationY);
      }
      foreach (CPosition pos in _listPositions)
      {
        GUIControl pControl = pos.control;

        int dwPosY = pControl.YPosition;
        if (pControl.IsVisible)
        {
          if (dwPosY < iTop)
          {
            int iSize = iTop - dwPosY;
            if (iSize > iMin)
            {
              iMin = iSize;
            }
            bOffScreen = true;
          }
        }
      }
      if (bOffScreen)
      {
        foreach (CPosition pos in _listPositions)
        {
          GUIControl pControl = pos.control;
          int dwPosX = pControl.XPosition;
          int dwPosY = pControl.YPosition;
          if (dwPosY < (int)100)
          {
            dwPosY += Math.Abs(iMin);
            pControl.SetPosition(dwPosX, dwPosY);
          }
        }
      }
      base.ResetAllControls();
    }

    public override bool NeedRefresh()
    {
      if (m_bNeedRefresh)
      {
        m_bNeedRefresh = false;
        return true;
      }
      return false;
    }

    private void OnPreviousChannel()
    {
      Log.Debug("zaposd: OnPreviousChannel()");
      if (!PluginMain.Navigator.IsLiveStreamOn)
      {
        return;
      }
      PluginMain.Navigator.ZapToPreviousChannel(true);
      channelNr = "";
      SetChannelIdAndName();
      SetCurrentChannelLogo();
      m_dateTime = DateTime.Now;
    }

    private void OnNextChannel()
    {
      Log.Debug("zaposd: OnNextChannel()");
      if (!PluginMain.Navigator.IsLiveStreamOn)
      {
          return;
      }
      PluginMain.Navigator.ZapToNextChannel(true);
      channelNr = "";
      SetChannelIdAndName();
      SetCurrentChannelLogo();
      m_dateTime = DateTime.Now;
    }

    public void UpdateChannelInfo()
    {
      channelNr = GetChannelNumber();
      SetChannelIdAndName();
      SetCurrentChannelLogo();
    }

    private void SetCurrentChannelLogo()
    {
      string strLogo = string.Empty;
      if (channelId != Guid.Empty && channelName != string.Empty)
      {
          using (SchedulerServiceAgent tvSchedulerAgent = new SchedulerServiceAgent())
          {
              strLogo = Utility.GetLogoImage(channelId, channelName, tvSchedulerAgent);
          }
      }

      if (string.IsNullOrEmpty(strLogo))                         
      {
        if (imgTvChannelLogo != null)
        {
          imgTvChannelLogo.IsVisible = false;
        }
      }
      else
      {
        if (imgTvChannelLogo != null)
        {
          imgTvChannelLogo.SetFileName(strLogo);
          m_bNeedRefresh = true;
          imgTvChannelLogo.IsVisible = true;
        }        
      }
      ShowPrograms();
    }

    private void SetChannelIdAndName()
    {
        channel = PluginMain.Navigator.ZapChannel ?? PluginMain.Navigator.CurrentChannel;
        if (channel != null)
        {
            channelId = channel.ChannelId;
            channelName = channel.DisplayName;
        }
        else
        {
            channelId = Guid.Empty;
            channelName = string.Empty;
        }
    }

    private string GetChannelNumber()
    {
      int zapChannelNr = PluginMain.Navigator.ZapChannelNr;
      if (zapChannelNr<0)
      {
        return "";
      }
      return zapChannelNr.ToString();
    }

    private void ShowPrograms()
    {
      if (lblOnTvNow != null)
      {
        lblOnTvNow.EnableUpDown = false;
        lblOnTvNow.Clear();
      }
      if (lblOnTvNext != null)
      {
        lblOnTvNext.EnableUpDown = false;
        lblOnTvNext.Clear();
      }

      // Set recorder status
      if (imgRecIcon != null)
      {
          ActiveRecording activeRecording;
          imgRecIcon.IsVisible = PluginMain.IsChannelRecording( channelId, out activeRecording);
      }

      if (lblCurrentChannel != null)
      {
        lblCurrentChannel.Label = channelName;
      }
      if (lblZapToCannelNo != null)
      {
        lblZapToCannelNo.Label = channelNr;
        lblZapToCannelNo.Visible = !string.IsNullOrEmpty(channelNr);
      }

      if (m_lastError != string.Empty)
      {
          lblOnTvNow.Label = m_lastError;// first line in "NOW"
          m_lastError = string.Empty;

          if (lblStartTime != null)
          {
              lblStartTime.Label = String.Empty;
          }
          if (lblEndTime != null)
          {
              lblEndTime.Label = String.Empty;
          }
          if (lblCurrentTime != null)
          {
              lblCurrentTime.Label = String.Empty;
          }
      }
      else
      {
        GuideProgram prog = PluginMain.GetCurrentForChannel(channel);
        if (prog != null)
        {
          string strTime = String.Format("{0}-{1}",
                                         prog.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                                         prog.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));

          if (lblCurrentTime != null)
          {
            lblCurrentTime.Label = strTime;
          }

          if (lblOnTvNow != null)
          {
            lblOnTvNow.Label = prog.Title;
          }
          if (lblStartTime != null)
          {
            strTime = String.Format("{0}", prog.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));
            lblStartTime.Label = strTime;
          }
          if (lblEndTime != null)
          {
            strTime = String.Format("{0} ", prog.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));
            lblEndTime.Label = strTime;
          }

          // next program
          prog = PluginMain.GetNextForChannel(channel);
          if (prog != null)
          {
            if (lblOnTvNext != null)
            {
              lblOnTvNext.Label = prog.Title;
            }
          }
        }
        else
        {
          lblOnTvNow.Label = GUILocalizeStrings.Get(736); // no epg for this channel

          if (lblStartTime != null)
          {
            lblStartTime.Label = String.Empty;
          }
          if (lblEndTime != null)
          {
            lblEndTime.Label = String.Empty;
          }
          if (lblCurrentTime != null)
          {
            lblCurrentTime.Label = String.Empty;
          }
        }
      }
      UpdateProgressBar();
    }

    private void UpdateProgressBar()
    {
      double fPercent = 0.0;
      GuideProgram prog = PluginMain.GetCurrentForChannel(channel);
      if (prog != null)
      {
          string strTime = String.Format("{0}-{1}",
                                         prog.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                                         prog.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));

          TimeSpan ts = prog.StopTime - prog.StartTime;
          double iTotalSecs = ts.TotalSeconds;
          ts = DateTime.Now - prog.StartTime;
          double iCurSecs = ts.TotalSeconds;
          fPercent = ((double)iCurSecs) / ((double)iTotalSecs);
          fPercent *= 100.0d;
      }
      GUIPropertyManager.SetProperty("#TV.View.Percentage", fPercent.ToString());
    }
  }
}