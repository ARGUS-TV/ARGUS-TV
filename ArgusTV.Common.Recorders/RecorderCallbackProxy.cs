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
using ArgusTV.DataContracts;
using ArgusTV.Common.Recorders.Utility;
using System.Net.Http;

namespace ArgusTV.Common.Recorders
{
    public class RecorderCallbackProxy : RestProxyBase
    {
        public RecorderCallbackProxy(string callbackBaseUrl)
            : base (callbackBaseUrl)
        {
        }

        public void RegisterRecorder(Guid recorderId, string name, string version)
        {
            var request = NewRequest(HttpMethod.Post, "RecorderCallback/RegisterRecorder/{0}", recorderId);
            request.AddBody(new
            {
                Name = name,
                Version = version
            });
            Execute(request);
        }

        public void LogMessage(Guid recorderId, LogSeverity severity, string message)
        {
            var request = NewRequest(HttpMethod.Post, "RecorderCallback/LogMessage");
            request.AddBody(new
            {
                RecorderId = recorderId,
                Severity = severity,
                Message = message
            });
            ExecuteAsync(request);
        }

        public void AddNewRecording(UpcomingProgram recordingProgram, DateTime recordingStartTimeUtc, string recordingFileName)
        {
            var request = NewRequest(HttpMethod.Post, "RecorderCallback/Recording/New");
            request.AddBody(new
            {
                RecordingProgram = recordingProgram,
                RecordingStartTimeUtc = recordingStartTimeUtc,
                RecordingFileName = recordingFileName
            });
            Execute(request);
        }

        public void EndRecording(string recordingFileName, DateTime recordingStopTimeUtc, bool isPartialRecording, bool okToMoveFile)
        {
            var request = NewRequest(HttpMethod.Put, "RecorderCallback/Recording/End");
            request.AddBody(new
            {
                RecordingFileName = recordingFileName,
                RecordingStopTimeUtc = recordingStopTimeUtc,
                IsPartialRecording = isPartialRecording,
                OkToMoveFile = okToMoveFile
            });
            Execute(request);
        }

        public void StartRecordingFailed(CardChannelAllocation channelAllocation, UpcomingProgram recordingProgram, string reason)
        {
            var request = NewRequest(HttpMethod.Put, "RecorderCallback/Recording/StartFailed");
            request.AddBody(new
            {
                Allocation = channelAllocation,
                RecordingProgram = recordingProgram,
                Reason = reason
            });
            Execute(request);
        }

        public void LiveStreamAborted(LiveStream abortedStream, LiveStreamAbortReason reason, UpcomingProgram program)
        {
            var request = NewRequest(HttpMethod.Put, "RecorderCallback/LiveStream/Aborted");
            request.AddBody(new
            {
                AbortedStream = abortedStream,
                Reason = reason,
                Program = program
            });
            Execute(request);
        }

        public GuideProgram GetGuideProgramById(Guid guideProgramId)
        {
            var request = NewRequest(HttpMethod.Get, "Guide/Program/{0}", guideProgramId);
            return Execute<GuideProgram>(request);
        }

        public Recording GetRecordingByFileName(string recordingFileName)
        {
            var request = NewRequest(HttpMethod.Post, "Control/RecordingByFile");
            request.AddBody(new
            {
                RecordingFileName = recordingFileName
            });
            return Execute<Recording>(request);
        }

        public void DeleteRecording(string recordingFileName, bool deleteRecordingFile)
        {
            var request = NewRequest(HttpMethod.Delete, "Control/RecordingByFile");
            request.AddParameter("deleteRecordingFile", deleteRecordingFile);
            request.AddBody(new
            {
                RecordingFileName = recordingFileName
            });
            Execute(request);
        }
    }
}
