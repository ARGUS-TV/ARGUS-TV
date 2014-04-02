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
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Globalization;

using TvLibrary.Interfaces;
using TvEngine.Events;
using TvLibrary.Log;
using TvControl;
using Gentle.Framework;

using ArgusTV.DataContracts;
using ArgusTV.Common.Recorders.Utility;
using ArgusTV.Common.Recorders;

namespace ArgusTV.Recorder.MediaPortalTvServer
{
    public class RecordingThread : RecordingThreadBase
    {
        private static object _startRecordingLock = new object();

        private string _suggestedBaseFileName;
        private TvDatabase.Card _recordOnCard;
        private TvDatabase.Channel _channel;

        public RecordingThread(Guid recorderId, string schedulerBaseUrl, CardChannelAllocation channelAllocation,
            DateTime startTimeUtc, DateTime stopTimeUtc, UpcomingProgram recordingProgram, string suggestedBaseFileName,
            TvDatabase.Card recordOnCard, TvDatabase.Channel channel)
            : base(recorderId, schedulerBaseUrl, channelAllocation, startTimeUtc, stopTimeUtc, recordingProgram, true)
        {
            _suggestedBaseFileName = suggestedBaseFileName;
            _recordOnCard = recordOnCard;
            _channel = channel;
        }

        #region Overrides

        protected override TimeSpan? FileSizeCheckerInterval
        {
            get
            {
                return new TimeSpan(0, 2, 0);
            }
        }

        private IUser _tve3User;
        private string _tve3RecordingFileName;

        protected override bool OnPrepareRecording(RecorderCallbackProxy proxy, ref string errorMessage)
        {
            DeleteAllMediaPortalSchedules();

            string userName = String.Format(CultureInfo.InvariantCulture, "ArgusTV{0}", Thread.CurrentThread.ManagedThreadId);
            _tve3User = new User(userName, true, _recordOnCard.IdCard);
            _tve3User.IdChannel = _channel.IdChannel;
            _tve3User.SubChannel = -1;

            bool argusIsRecordingOnCard;
            return EnsureCardFree(true, ref errorMessage, out argusIsRecordingOnCard);
        }

        protected override string OnStartRecording(RecorderCallbackProxy proxy, ref string errorMessage)
        {
            string baseFileName = _suggestedBaseFileName;
            if (String.IsNullOrEmpty(baseFileName))
            {
                baseFileName = FileUtility.BuildRecordingBaseFileName(null, this.RecordingProgram);
            }
            else
            {
                this.UsedSuggestedBaseFileName = true;
            }

            string fileName = Path.Combine(_recordOnCard.RecordingFolder, baseFileName);
            string extension = (_recordOnCard.RecordingFormat == 0) ? ".ts" : ".mpg";
            _tve3RecordingFileName = FileUtility.GetFreeFileName(fileName, extension);
            string tve3RecordingDirectory = Path.GetDirectoryName(_tve3RecordingFileName);
            if (!Directory.Exists(tve3RecordingDirectory))
            {
                Directory.CreateDirectory(tve3RecordingDirectory);
            }

            string uncRecordingFolder = Common.ShareExplorer.GetUncPathForLocalPath(tve3RecordingDirectory);
            if (String.IsNullOrEmpty(uncRecordingFolder))
            {
                errorMessage = "Failed to convert '" + Path.GetDirectoryName(_tve3RecordingFileName) + "' to UNC path, please add required share";
                return null;
            }

            bool argusIsRecordingOnCard;
            if (!EnsureCardFree(false, ref errorMessage, out argusIsRecordingOnCard))
            {
                if (!WaitCardFree(argusIsRecordingOnCard, ref errorMessage))
                {
                    return null;
                }
            }

            IChannel tuningChannel = Utility.FindTuningChannelOnCard(_channel, _recordOnCard.IdCard);
            if (tuningChannel == null)
            {
                errorMessage = "Failed to find tuning details for channel " + _channel.DisplayName;
                return null;
            }

            // Make sure only one thread can tune and start a recording at the same time.
            lock (_startRecordingLock)
            {
                if (TvServerPlugin.TvController_Tune(ref _tve3User, tuningChannel, _channel.IdChannel) != TvResult.Succeeded)
                {
                    errorMessage = "Failed to tune to channel " + _channel.DisplayName;
                    return null;
                }

                if (TvServerPlugin.TvController_StartRecording(ref _tve3User, ref _tve3RecordingFileName) != TvResult.Succeeded)
                {
                    errorMessage = "TV Server failed to start recording on channel " + _channel.DisplayName;
                    return null;
                }
            }

            return Path.Combine(uncRecordingFolder, Path.GetFileName(_tve3RecordingFileName));
        }

        protected override bool OnCheckRecordingActive()
        {
            // IMPORTANT: disabled the code below, for some reason this is *not* working!
            // If _tve3User is still null, we are in the prepare phase, but we also
            // consider this as good, the file will (as far as we know) still be recorded.
            //return (_tve3User == null || TvServerPlugin.TvController.IsRecording(ref _tve3User));
            return true;
        }

        protected override bool OnStopRecording(RecorderCallbackProxy proxy, bool abort)
        {
            if (_tve3User != null)
            {
                if (StopMediaPortalRecording(_tve3User))
                {
                    _tve3User = null;
                    return true;
                }
            }
            return false;
        }

        protected override void OnError()
        {
            if (_tve3User != null
                && TvServerPlugin.TvController_IsRecording(ref _tve3User))
            {
                StopMediaPortalRecording(_tve3User);
            }
        }

        protected override void OnWriteLog(TraceEventType severity, string message)
        {
            if (severity == TraceEventType.Error)
            {
                Log.Error(message);
            }
            else
            {
                Log.Write(message);
            }
        }

        #endregion

        #region Private Methods

        private bool EnsureCardFree(bool allowOtherArgusUser, ref string errorMessage, out bool argusIsRecordingOnCard)
        {
            argusIsRecordingOnCard = false;
            IUser[] cardUsers = TvServerPlugin.TvController_GetUsersForCard(_recordOnCard.IdCard);
            if (cardUsers != null)
            {
                TvDatabase.TuningDetail tuning = Utility.FindTuningDetailOnCard(_channel, _recordOnCard.IdCard);
                foreach (IUser cardUser in cardUsers)
                {
                    if (!cardUser.Name.Equals("epg", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (cardUser.Name.StartsWith("ArgusTV"))
                        {
                            if (!allowOtherArgusUser
                                && !Utility.IsSameTransponder(_recordOnCard.IdCard, tuning, cardUser.IdChannel))
                            {
                                // Seems another ARGUS TV user is using this card, but on a different
                                // transponder!  Normally this should never happen, but in rare conditions
                                // we need to be able to handle this.
                                errorMessage = "Card is in use by previous recording";
                                argusIsRecordingOnCard = true;
                                return false;
                            }
                        }
                        else
                        {
                            IUser tmpUser = cardUser;
                            if (TvServerPlugin.TvController_IsRecording(ref tmpUser))
                            {
                                if (!TvServerPlugin.TvController_StopRecording(ref tmpUser))
                                {
                                    errorMessage = "Failed to stop recording on channel " + _channel.DisplayName;
                                    return false;
                                }
                            }
                            else if (TvServerPlugin.TvController_IsTimeShifting(ref tmpUser))
                            {
                                if (!Utility.IsSameTransponder(_recordOnCard.IdCard, tuning, tmpUser.IdChannel))
                                {
                                    if (!TvServerPlugin.TvController_StopTimeShifting(ref tmpUser, TvStoppedReason.RecordingStarted))
                                    {
                                        errorMessage = "Failed to stop timeshifting on channel " + _channel.DisplayName;
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        private bool WaitCardFree(bool argusIsRecordingOnCard, ref string errorMessage)
        {
            bool cardFree = false;
            if (argusIsRecordingOnCard)
            {
                DateTime maxWaitTime = DateTime.Now.AddSeconds(15);
                while (!cardFree
                    && DateTime.Now < maxWaitTime)
                {
                    Thread.Sleep(500);
                    cardFree = EnsureCardFree(false, ref errorMessage, out argusIsRecordingOnCard);
                }
            }
            return cardFree;
        }

        private static void DeleteAllMediaPortalSchedules()
        {
            try
            {
                foreach (TvDatabase.Schedule schedule in TvDatabase.Schedule.ListAll())
                {
                    schedule.Delete();
                }
            }
            catch { }
        }

        private static bool StopMediaPortalRecording(IUser tve3User)
        {
            for (int count = 0; count < 60; count++)
            {
                try
                {
                    if (TvServerPlugin.TvController_StopRecording(ref tve3User))
                    {
                        return true;
                    }
                }
                catch { }
                Thread.Sleep(250);
            }
            return false;
        }

        #endregion
    }
}
