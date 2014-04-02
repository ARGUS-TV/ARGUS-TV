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

namespace ArgusTV.Common.Recorders.Utility
{
	public class FileSizeChecker
	{
        private const int _maximumErrorCount = 1;

		#region Private Members

		private string _fileName;
		private TimeSpan _checkInterval;
		private long _lastSize = 0;
		private DateTime _lastCheckTime = DateTime.MaxValue;
        private int _errorCount = 0;
		private bool _initialized;

		#endregion

        #region Constructor

        public FileSizeChecker(string fileName, TimeSpan checkInterval)
		{
			_fileName = fileName;
			_checkInterval = checkInterval;
        }

        #endregion

        #region Public Members

        public bool Check()
		{
			return Check(false);
		}

		public bool Check(bool skipNonExistingFile)
		{
            lock (this)
            {
                Initialize();
                FileInfo fileInfo = new FileInfo(_fileName);
                if (fileInfo.Exists)
                {
                    // If we have checked too recently,
                    // or the file has actually grown -- return success.
                    if (fileInfo.Length > _lastSize)
                    {
                        _lastCheckTime = DateTime.Now;
                        _lastSize = fileInfo.Length;
                        _errorCount = 0;
                        return true;
                    }
                    else
                    {
                        if (DateTime.Now < _lastCheckTime.Add(_checkInterval))
                        {
                            return true;
                        }
                    }
                    // Otherwise, increase the error count,
                    // and if the maximum of errors is reached -- return failure.
                    if (++_errorCount > _maximumErrorCount)
                    {
                        return false;
                    }
                    return true;
                }
                return skipNonExistingFile;
            }
		}


		public void Reset()
		{
            lock (this)
            {
                _initialized = false;
            }
		}

		#endregion

		#region Private Members

		private void Initialize()
		{
			if (!_initialized)
			{
				_lastCheckTime = DateTime.Now;
				_initialized = true;
			}
		}

		#endregion
	}
}
