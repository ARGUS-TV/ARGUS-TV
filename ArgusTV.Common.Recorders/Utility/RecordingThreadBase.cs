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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using ArgusTV.DataContracts;
using ArgusTV.Common.Threading;

namespace ArgusTV.Common.Recorders.Utility
{
    public abstract class RecordingThreadBase : WorkerThread
    {
        private string _schedulerBaseUrl;
        private FileSizeChecker _fileSizeChecker;
        private bool _okToRenameRecordedFiles;

        public RecordingThreadBase(Guid recorderId, string schedulerBaseUrl, CardChannelAllocation channelAllocation,
            DateTime startTimeUtc, DateTime stopTimeUtc, UpcomingProgram recordingProgram, bool okToRenameRecordedFiles)
            : base("Record")
        {
            _recorderId = recorderId;
            _schedulerBaseUrl = schedulerBaseUrl;
            _channelAllocation = channelAllocation;
            _startTimeUtc = startTimeUtc;
            _stopTimeUtc = stopTimeUtc;
            _recordingProgram = recordingProgram;
            _okToRenameRecordedFiles = okToRenameRecordedFiles;
        }

        #region Public Properties

        public string RecordingFileName { get; private set; }

        public string ActualRecordingFileName { get; set; }

        public virtual bool IsRecording
        {
            get
            {
                return (OnCheckRecordingActive() && OnCheckFileSize());
            }
        }

        private CardChannelAllocation _channelAllocation;

        public CardChannelAllocation ChannelAllocation
        {
            get { return _channelAllocation; }
        }

        private DateTime _startTimeUtc;

        public DateTime StartTimeUtc
        {
            get { return _startTimeUtc; }
        }

        private DateTime _stopTimeUtc;

        public DateTime StopTimeUtc
        {
            get { lock (this) { return _stopTimeUtc; } }
            set { lock (this) { _stopTimeUtc = value; } }
        }

        private UpcomingProgram _recordingProgram;

        public UpcomingProgram RecordingProgram
        {
            get { return _recordingProgram; }
        }

        private RecordingThreadBase _waitForThreadToComplete;

        public RecordingThreadBase WaitForThreadToComplete
        {
            get { return _waitForThreadToComplete; }
            set { _waitForThreadToComplete = value; }
        }

        #endregion

        #region Abstract/Virtual Methods/Properties

        virtual protected TimeSpan CheckRecordingActiveInterval
        {
            get { return new TimeSpan(0, 0, 30); }
        }

        virtual protected TimeSpan? FileSizeCheckerInterval
        {
            get { return new TimeSpan(0, 1, 0); }
        }

        private bool _usedSuggestedBaseFileName;

        virtual protected bool UsedSuggestedBaseFileName
        {
            set { _usedSuggestedBaseFileName = value; }
            get { return _usedSuggestedBaseFileName; }
        }

        abstract protected bool OnPrepareRecording(RecorderCallbackProxy callbackProxy, ref string errorMessage);

        abstract protected string OnStartRecording(RecorderCallbackProxy callbackProxy, ref string errorMessage);

        virtual protected bool OnCheckRecordingActive()
        {
            return true;
        }

        virtual protected bool OnCheckFileSize()
        {
            // If _fileSizeChecker is still null, we are in the prepare phase, but we also
            // consider this as good, the file will (as far as we know) still be recorded.
            return (_fileSizeChecker == null || _fileSizeChecker.Check());
        }

        abstract protected bool OnStopRecording(RecorderCallbackProxy callbackProxy, bool abort);

        abstract protected void OnWriteLog(TraceEventType severity, string message);

        abstract protected void OnError();

        virtual protected void OnThreadEnding()
        {
        }

        #endregion

        #region Protected Methods

        private Guid _recorderId;

        protected Guid RecorderId
        {
            get { return _recorderId; }
        }

        protected void WriteLog(string message, params object[] args)
        {
            WriteLog(TraceEventType.Information, message, args);
        }

        protected void WriteLog(TraceEventType severity, string message, params object[] args)
        {
            OnWriteLog(severity, String.Format(CultureInfo.CurrentCulture, message, args));
        }

        protected override void Run()
        {
            var callbackProxy = new RecorderCallbackProxy(_schedulerBaseUrl);

            try
            {
                SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_AWAYMODE_REQUIRED);

                if (this.StopTimeUtc <= DateTime.UtcNow || _startTimeUtc >= this.StopTimeUtc)
                {
                    CallStartRecordingFailed(callbackProxy, String.Format(CultureInfo.InvariantCulture,
                        "Recording of {0} at {1} for {2} has invalid timing parameters.",
                        _channelAllocation.ChannelName, _startTimeUtc.ToLocalTime().ToShortTimeString(), this.StopTimeUtc.Subtract(_startTimeUtc)));
                    return;
                }

                if (_waitForThreadToComplete != null)
                {
                    _waitForThreadToComplete.Join();
                }

                bool aborted = false;
                _usedSuggestedBaseFileName = !_okToRenameRecordedFiles;

                // First of all make sure the recorder is tuned to the correct channel.
                string errorMessage = null;
                if (!OnPrepareRecording(callbackProxy, ref errorMessage))
                {
                    CallStartRecordingFailed(callbackProxy, errorMessage);
                    OnError();
                    aborted = true;
                }

                DateTime actualStartTimeUtc = DateTime.MaxValue;

                if (!aborted)
                {
                    // Now wait for the actual start-time
                    try
                    {
                        TimeSpan delay = _startTimeUtc.AddSeconds(-1) - DateTime.UtcNow;
                        if (delay.TotalMilliseconds > 0)
                        {
                            aborted = this.StopThreadEvent.WaitOne((int)delay.TotalMilliseconds, false);
                        }
                        if (!aborted)
                        {
                            errorMessage = null;
                            this.RecordingFileName = OnStartRecording(callbackProxy, ref errorMessage);
                            if (String.IsNullOrEmpty(this.RecordingFileName))
                            {
                                CallStartRecordingFailed(callbackProxy, errorMessage);
                                OnError();
                                aborted = true;
                            }
                            else
                            {
                                this.ActualRecordingFileName = ShareExplorer.TryConvertUncToLocal(this.RecordingFileName);
                                actualStartTimeUtc = DateTime.UtcNow;
                                CallAddNewRecording(callbackProxy, actualStartTimeUtc);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        CallStartRecordingFailed(callbackProxy, e.Message);
                        OnError();
                        aborted = true;
                    }
                }

                if (!aborted)
                {
                    TimeSpan? checkerInterval = this.FileSizeCheckerInterval;
                    if (!checkerInterval.HasValue)
                    {
                        checkerInterval = TimeSpan.MaxValue;
                    }
                    _fileSizeChecker = new FileSizeChecker(this.ActualRecordingFileName, checkerInterval.Value);

                    while (!aborted && DateTime.UtcNow < this.StopTimeUtc)
                    {
                        TimeSpan interval = this.CheckRecordingActiveInterval;
                        TimeSpan timeToEnd = this.StopTimeUtc.AddMilliseconds(1) - DateTime.UtcNow;
                        if (timeToEnd < interval)
                        {
                            interval = timeToEnd;
                        }

                        aborted = this.StopThreadEvent.WaitOne(interval, false);
                    }

                    if (aborted)
                    {
                        WriteLog(TraceEventType.Warning, "RecordingThread [{0}]: Aborted", _recordingProgram.CreateProgramTitle());
                    }

                    if (!OnStopRecording(callbackProxy, aborted))
                    {
                        WriteLog(TraceEventType.Error, "RecordingThread [{0}]: Failed to stop recording", _recordingProgram.CreateProgramTitle());
                        try
                        {
                            callbackProxy.LogMessage(this.RecorderId, LogSeverity.Error, "Failed to stop recording " + _recordingProgram.CreateProgramTitle());
                        }
                        catch { }
                    }

                    CallEndRecording(callbackProxy, actualStartTimeUtc, DateTime.UtcNow);
                }
            }
            catch (Exception ex)
            {
                WriteLog(TraceEventType.Error, "RecordingThread [{0}]: {1}", _recordingProgram.CreateProgramTitle(), ex.Message);
                try
                {
                    callbackProxy.LogMessage(this.RecorderId, LogSeverity.Error, "Exception: " + ex.Message);
                }
                catch { }
                OnError();
            }
            finally
            {
                SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
            }

            OnThreadEnding();
        }

        #endregion

        #region Private Methods

        private void CallAddNewRecording(RecorderCallbackProxy callbackProxy, DateTime actualStartTimeUtc)
        {
            WriteLog("RecordingThread [{0}]: Calling AddNewRecording()", _recordingProgram.CreateProgramTitle());
            try
            {
                callbackProxy.AddNewRecording(_recordingProgram, actualStartTimeUtc, this.RecordingFileName);
            }
            catch (Exception ex)
            {
                WriteLog(TraceEventType.Error, "RecordingThread [{0}]: {1}", _recordingProgram.CreateProgramTitle(), ex.Message);
            }
        }

        private void CallStartRecordingFailed(RecorderCallbackProxy callbackProxy, string reason)
        {
            WriteLog("RecordingThread [{0}]: Calling StartRecordingFailed(Reason=\"{1}\")", _recordingProgram.CreateProgramTitle(), reason);
            try
            {
                callbackProxy.StartRecordingFailed(_channelAllocation, _recordingProgram, reason);
            }
            catch (Exception ex)
            {
                WriteLog(TraceEventType.Error, "RecordingThread [{0}]: {1}", _recordingProgram.CreateProgramTitle(), ex.Message);
            }
        }

        private void CallEndRecording(RecorderCallbackProxy callbackProxy, DateTime actualStartTimeUtc, DateTime actualStopTimeUtc)
        {
            bool isPartial = (actualStartTimeUtc > _recordingProgram.StartTimeUtc.AddSeconds(30))
                || (actualStopTimeUtc < _recordingProgram.StopTimeUtc.AddSeconds(-30));
            WriteLog("RecordingThread [{0}]: Calling EndRecording(IsPartial={1})", _recordingProgram.CreateProgramTitle(), isPartial);
            try
            {
                callbackProxy.EndRecording(this.RecordingFileName, actualStopTimeUtc, isPartial, !_usedSuggestedBaseFileName);
            }
            catch (Exception ex)
            {
                WriteLog(TraceEventType.Error, "RecordingThread [{0}]: {1}", _recordingProgram.CreateProgramTitle(), ex.Message);
            }
        }

        #endregion

        #region P/Invoke

        [FlagsAttribute]
        private enum EXECUTION_STATE : uint
        {
            ES_SYSTEM_REQUIRED = 0x00000001,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000
        }

        [DllImport("Kernel32.DLL", CharSet = CharSet.Auto, SetLastError = true)]
        private extern static EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE state);

        #endregion
    }
}
