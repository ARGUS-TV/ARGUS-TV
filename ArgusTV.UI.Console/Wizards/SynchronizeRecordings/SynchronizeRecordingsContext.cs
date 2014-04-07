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
using System.Net;

using ArgusTV.DataContracts;

namespace ArgusTV.UI.Console.Wizards.SynchronizeRecordings
{
    internal class SynchronizeRecordingsContext
    {
        private Dictionary<string, RecordingSummary> _allRecordings = new Dictionary<string, RecordingSummary>();
        private Dictionary<string, bool> _hostComparisonResults = new Dictionary<string, bool>();
        private List<RecordingSummary> _missingRecordings = new List<RecordingSummary>();
        private List<Recording> _foundRecordings = new List<Recording>();
        private List<Recording> _importRecordings = new List<Recording>();
        private List<RecordingSummary> _deleteRecordings = new List<RecordingSummary>();
        private List<MoveRecording> _moveRecordings = new List<MoveRecording>();
        private List<string> _recordingFolders = new List<string>();

        public List<RecordingSummary> AllRecordings
        {
            get { return new List<RecordingSummary>(_allRecordings.Values); }
        }

        public List<RecordingSummary> MissingRecordings
        {
            get { return _missingRecordings; }
        }

        public List<Recording> FoundRecordings
        {
            get { return _foundRecordings; }
        }

        public List<Recording> ImportRecordings
        {
            get { return _importRecordings; }
        }

        public List<MoveRecording> MoveRecordings
        {
            get { return _moveRecordings; }
        }

        public List<RecordingSummary> DeleteRecordings
        {
            get { return _deleteRecordings; }
        }

        public List<string> RecordingFolders
        {
            get { return _recordingFolders; }
        }

        public void ClearRecordings()
        {
            _allRecordings.Clear();
        }

        public bool AddRecording(RecordingSummary recording)
        {
            string key = GetRecordingKey(recording.RecordingFileName);
            if (key != null)
            {
                if (!_allRecordings.ContainsKey(key))
                {
                    _allRecordings[key] = recording;
                    return true;
                }
            }
            return false;
        }

        public bool ContainsRecording(string recordingFileName)
        {
            string key = GetRecordingKey(recordingFileName);
            if (key != null)
            {
                if (_allRecordings.ContainsKey(key))
                {
                    RecordingSummary recording = _allRecordings[key];
                    string hostName = GetHostFromUncPath(recordingFileName);
                    string recordingHostName = GetHostFromUncPath(recording.RecordingFileName);
                    return HostsAreEqual(hostName, recordingHostName);
                }
            }
            return false;
        }

        public void ClearFoundRecordings()
        {
            _foundRecordings.Clear();
            _importRecordings.Clear();
            _moveRecordings.Clear();
        }

        public void AddFoundRecording(Recording recording, bool importRecording)
        {
            if (importRecording)
            {
                foreach (RecordingSummary missingRecording in _missingRecordings)
                {
                    if (recording.RecordingId == missingRecording.RecordingId)
                    {
                        _missingRecordings.Remove(missingRecording);
                        _moveRecordings.Add(new MoveRecording(missingRecording.RecordingFileName, recording));
                        return;
                    }
                }
            }
            _foundRecordings.Add(recording);
            if (importRecording)
            {
                _importRecordings.Add(recording);
            }
        }

        private bool HostsAreEqual(string hostName, string recordingHostName)
        {
            string key = hostName.ToLowerInvariant() + "|" + recordingHostName.ToLowerInvariant();
            if (!_hostComparisonResults.ContainsKey(key))
            {
                if (!String.IsNullOrEmpty(hostName)
                    && !String.IsNullOrEmpty(recordingHostName))
                {
                    IPAddress[] hostAddresses = Dns.GetHostAddresses(hostName);
                    IPAddress[] recHostAddresses = Dns.GetHostAddresses(recordingHostName);
                    _hostComparisonResults[key] = HostAddressesAreEqual(hostAddresses, recHostAddresses);
                }
                else
                {
                    _hostComparisonResults[key] = false;
                }
            }
            return _hostComparisonResults[key];
        }

        public bool ContainsRecording(RecordingSummary recording)
        {
            return ContainsRecording(recording.RecordingFileName);
        }

        private string GetRecordingKey(string recordingFileName)
        {
            if (recordingFileName.StartsWith(@"\\"))
            {
                int separatorIndex = recordingFileName.IndexOf(@"\", 2);
                if (separatorIndex >= 0)
                {
                    return recordingFileName.Substring(separatorIndex).ToLowerInvariant();
                }
            }
            return null;
        }

        private string GetHostFromUncPath(string path)
        {
            if (path.StartsWith(@"\\"))
            {
                int separatorIndex = path.IndexOf(@"\", 2);
                if (separatorIndex > 2)
                {
                    return path.Substring(2, separatorIndex - 2);
                }
            }
            return null;
        }

        private bool HostAddressesAreEqual(IPAddress[] hostAddresses, IPAddress[] recHostAddresses)
        {
            if (hostAddresses.Length == recHostAddresses.Length)
            {
                foreach (IPAddress ipAddress in hostAddresses)
                {
                    if (!IPAddressesContains(recHostAddresses, ipAddress))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool IPAddressesContains(IPAddress[] recHostAddresses, IPAddress ipAddress)
        {
            foreach (IPAddress recHostAddress in recHostAddresses)
            {
                if (recHostAddress.Equals(ipAddress))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
