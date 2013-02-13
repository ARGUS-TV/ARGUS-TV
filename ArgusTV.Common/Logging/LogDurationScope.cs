/*
 *	Copyright (C) 2007-2012 ARGUS TV
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
using System.Linq;
using System.Text;

namespace ArgusTV.Common.Logging
{
    public class LogDurationScope : IDisposable
    {
        private string _name;
        private DateTime _startDateTime;

        public LogDurationScope(string name)
        {
            _name = name;
            _startDateTime = DateTime.Now;
        }

        public static LogDurationScope Start(string name)
        {
            return new LogDurationScope(name);
        }

        #region IDisposable Pattern

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Logger.Verbose("{0}, duration: {1}", _name, (DateTime.Now - _startDateTime).TotalMilliseconds);
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~LogDurationScope()
        {
            Dispose(false);
        }

        #endregion
    }
}
