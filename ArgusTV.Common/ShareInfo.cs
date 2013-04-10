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
 *
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ArgusTV.Common
{
	#region Share Type

	[Flags]
	public enum ShareType
	{
		Disk = 0,
		Printer = 1,
		Device = 2,
		IPC = 3,
		Special = -2147483648, // 0x80000000,
	}
	#endregion

	public class ShareInfo
	{
		#region Private Members

		private string _serverName;
		private string _netName;
		private string _path;
		private string _remark;
		private ShareType _shareType;
		#endregion

		public ShareInfo(string serverName, string netName, string path, ShareType shareType, string remark)
		{
			if (shareType == ShareType.Special && netName == "IPC$")
			{
				shareType |= ShareType.IPC;
			}
			_serverName = serverName;
			_netName = netName;
			_path = path;
			_shareType = shareType;
			_remark = remark;
		}

		#region Properties

		public string ServerName
		{
			get { return _serverName; }
		}

		public string NetName
		{
			get { return _netName; }
		}

		public string Path
		{
			get { return _path; }
		}

		public ShareType ShareType
		{
			get { return _shareType; }
		}

		public string Remark
		{
			get { return _remark; }
		}

		/// <summary>
		/// Returns true if this is a file system share
		/// </summary>
		public bool IsFileSystem
		{
			get
			{
				// Shared device
				if ((_shareType & ShareType.Device) != 0)
				{
					return false;
				}
				// IPC share
				if ((_shareType & ShareType.IPC) != 0)
				{
					return false;
				}
				// Shared printer
				if ((_shareType & ShareType.Printer) != 0)
				{
					return false;
				}
				// Standard disk share
				if ((_shareType & ShareType.Special) == 0)
				{
					return true;
				}
				// Special disk share (e.g. C$)
				if (ShareType.Special == _shareType && null != _netName && 0 != _netName.Length)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Get the root of a file system (disk) based share
		/// </summary>
		public DirectoryInfo Root
		{
			get
			{
				if (IsFileSystem)
				{
					if (String.IsNullOrEmpty(_serverName))
					{
						if (String.IsNullOrEmpty(_path))
						{
							return new DirectoryInfo(ToString());
						}
						else
						{
							return new DirectoryInfo(_path);
						}
					}
					else
					{
						return new DirectoryInfo(ToString());
					}
				}
				return null;
			}
		}
		#endregion

		#region Overrides

		/// <summary>
		/// Returns the path to this share
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if (String.IsNullOrEmpty(_serverName))
			{
				return string.Format(@"\\{0}\{1}", Environment.MachineName, _netName);
			}
			return string.Format(@"\\{0}\{1}", _serverName, _netName);
		}
		#endregion

		#region Public Methods

		/// <summary>
		/// Returns true if this share matches the local path
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public bool MatchesPath(string path)
		{
            if (String.IsNullOrEmpty(path) || !IsFileSystem)
            {
                return false;
            }
            if (path.StartsWith(_path, StringComparison.CurrentCultureIgnoreCase))
            {
                int index = _path.Length;
                if (_path.EndsWith(@"\"))
                {
                    index--;
                }
                return (path.Length == _path.Length) || (path[index] == System.IO.Path.DirectorySeparatorChar);
            }
            return false;
        }
		#endregion
	}
}

