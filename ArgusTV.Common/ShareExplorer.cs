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
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace ArgusTV.Common
{
	public class ShareExplorer
	{
		#region Interop

		#region Constants

		private const int NO_ERROR = 0;
		private const int ERROR_ACCESS_DENIED = 5;
		private const int ERROR_WRONG_LEVEL = 124;
		private const int ERROR_MORE_DATA = 234;
		private const int ERROR_NOT_CONNECTED = 2250;
		private const int UNIVERSAL_NAME_INFO_LEVEL = 1;
		private const int MAX_SI50_ENTRIES = 20;
		#endregion

		#region Structures

		/// <summary>Unc name</summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct UNIVERSAL_NAME_INFO
		{
			[MarshalAs(UnmanagedType.LPTStr)]
			public string lpUniversalName;
		}

		/// <summary>Share information, NT, level 2</summary>
		/// <remarks>
		/// Requires admin rights to work. 
		/// </remarks>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct SHARE_INFO_2
		{
			[MarshalAs(UnmanagedType.LPWStr)]
			public string NetName;
			public ShareType ShareType;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string Remark;
			public int Permissions;
			public int MaxUsers;
			public int CurrentUsers;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string Path;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string Password;
		}

		/// <summary>Share information, NT, level 1</summary>
		/// <remarks>
		/// Fallback when no admin rights.
		/// </remarks>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct SHARE_INFO_1
		{
			[MarshalAs(UnmanagedType.LPWStr)]
			public string NetName;
			public ShareType ShareType;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string Remark;
		}

		/// <summary>Share information, Win9x</summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		private struct SHARE_INFO_50
		{
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
			public string NetName;
			public byte bShareType;
			public ushort Flags;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string Remark;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string Path;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
			public string PasswordRW;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
			public string PasswordRO;
			public ShareType ShareType
			{
				get { return (ShareType)((int)bShareType & 0x7F); }
			}
		}

		/// <summary>Share information level 1, Win9x</summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		protected struct SHARE_INFO_1_9x
		{
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
			public string NetName;
			public byte Padding;
			public ushort bShareType;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string Remark;
			public ShareType ShareType
			{
				get { return (ShareType)((int)bShareType & 0x7FFF); }
			}
		}
		#endregion

		#region Functions

		/// <summary>Get a UNC name</summary>
		[DllImport("mpr", CharSet = CharSet.Auto)]
		private static extern int WNetGetUniversalName(string lpLocalPath, int dwInfoLevel, ref UNIVERSAL_NAME_INFO lpBuffer, ref int lpBufferSize);

		/// <summary>Get a UNC name</summary>
		[DllImport("mpr", CharSet = CharSet.Auto)]
		private static extern int WNetGetUniversalName(string lpLocalPath, int dwInfoLevel, IntPtr lpBuffer, ref int lpBufferSize);

		/// <summary>Enumerate shares (NT)</summary>
		[DllImport("netapi32", CharSet = CharSet.Unicode)]
		private static extern int NetShareEnum(string lpServerName, int dwLevel, out IntPtr lpBuffer, int dwPrefMaxLen, out int entriesRead, out int totalEntries, ref int hResume);

		/// <summary>Free the buffer (NT)</summary>
		[DllImport("netapi32")]
		private static extern int NetApiBufferFree(IntPtr lpBuffer);

		/// <summary>Enumerate shares (9x)</summary>
		[DllImport("svrapi", CharSet = CharSet.Ansi)]
		private static extern int NetShareEnum([MarshalAs(UnmanagedType.LPTStr)] string lpServerName, int dwLevel, IntPtr lpBuffer, ushort cbBuffer, out ushort entriesRead, out ushort totalEntries);
		#endregion

		#endregion

		#region Public Static Methods

		public static List<ShareInfo> GetShareInfo()
		{
			return GetShareInfo(Environment.MachineName, GetAllShareTypes());
		}

		public static List<ShareInfo> GetShareInfo(ShareType shareType)
		{
			IList<ShareType> shareTypeList = new List<ShareType>();
			shareTypeList.Add(shareType);
			return GetShareInfo(Environment.MachineName, shareTypeList);
		}

		public static List<ShareInfo> GetShareInfo(IList<ShareType> shareTypes)
		{
			return GetShareInfo(Environment.MachineName, shareTypes);
		}

		public static List<ShareInfo> GetShareInfo(string serverName)
		{
			return GetShareInfo(serverName, GetAllShareTypes());
		}

		public static List<ShareInfo> GetShareInfo(string serverName, ShareType shareType)
		{
			IList<ShareType> shareTypeList = new List<ShareType>();
			shareTypeList.Add(shareType);
			return GetShareInfo(serverName, shareTypeList);
		}

		public static List<ShareInfo> GetShareInfo(string serverName, IList<ShareType> shareTypes)
		{
			if (!String.IsNullOrEmpty(serverName) && !ComputerInfo.IsWindows2KUp)
			{
				serverName = serverName.ToUpper();
				// On NT4, 9x and Me, server has to start with "\\"
				if (!serverName.StartsWith(@"\\"))
				{
					serverName = serverName + @"\\";
				}
			}

			if (ComputerInfo.IsWindowsNT)
			{
				return GetShareInfoNT(serverName, shareTypes);
			}
			return GetShareInfo9x(serverName, shareTypes);
		}

		public static bool IsValidLocalFilePath(string fileName)
		{
			if (String.IsNullOrEmpty(fileName) || fileName.Length <= 2)
			{
				return false;
			}

			char drive = char.ToUpper(fileName[0]);
			if ('A' > drive || drive > 'Z')
			{
				return false;
			}
			else if (Path.VolumeSeparatorChar != fileName[1])
			{
				return false;
			}
			else if (Path.DirectorySeparatorChar != fileName[2])
			{
				return false;
			}
			return true;
		}

		public static bool IsLocalShare(string shareName)
		{
			if (!String.IsNullOrEmpty(shareName))
			{
				string shareServerName = GetServerNameFromShareName(shareName);
				if (String.Compare(shareServerName, Environment.MachineName, true) == 0)
				{
					DirectoryInfo dirInfo = new DirectoryInfo(shareName);
					if (dirInfo.Exists && dirInfo.Root != null)
					{
						List<ShareInfo> localShares = GetShareInfo(ShareType.Disk);
						foreach (ShareInfo shareInfo in localShares)
						{
							if (shareInfo.Root != null)
							{
								if (String.Compare(shareInfo.Root.FullName, 0, dirInfo.FullName, 0, shareInfo.Root.FullName.Length, true) == 0)
								{
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}

		public static string GetServerNameFromShareName(string shareName)
		{
			if (!String.IsNullOrEmpty(shareName) && shareName.StartsWith(@"\\"))
			{
				string[] parts = shareName.Substring(2).Split('\\');
				if (parts.Length > 0)
				{
					return parts[0];
				}
			}
			return String.Empty;
		}

		public static string GetUncPathForLocalPath(string fileName)
		{
            if (String.IsNullOrEmpty(fileName))
            {
                return String.Empty;
            }
            if (fileName.StartsWith(@"\\"))
            {
                return fileName;
            }

            fileName = Path.GetFullPath(fileName);
            if (!IsValidLocalFilePath(fileName) || !Directory.Exists(fileName))
            {
                return String.Empty;
            }

            int nRet = 0;
            UNIVERSAL_NAME_INFO rni = new UNIVERSAL_NAME_INFO();
            int bufferSize = Marshal.SizeOf(rni);

            nRet = WNetGetUniversalName(fileName, UNIVERSAL_NAME_INFO_LEVEL, ref rni, ref bufferSize);

            if (nRet == ERROR_MORE_DATA)
            {
                IntPtr pBuffer = Marshal.AllocHGlobal(bufferSize); ;
                try
                {
                    nRet = WNetGetUniversalName(fileName, UNIVERSAL_NAME_INFO_LEVEL, pBuffer, ref bufferSize);
                    if (NO_ERROR == nRet)
                    {
                        rni = (UNIVERSAL_NAME_INFO)Marshal.PtrToStructure(pBuffer, typeof(UNIVERSAL_NAME_INFO));
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(pBuffer);
                }
            }

            switch (nRet)
            {
                case NO_ERROR:
                    return rni.lpUniversalName;

                case ERROR_NOT_CONNECTED:
                    //Local file-name
                    List<ShareInfo> shareInfoList = GetShareInfo();
                    if (shareInfoList != null)
                    {
                        ShareInfo shareInfo = FindBestShareInfoMatch(shareInfoList, fileName);
                        if (shareInfo != null)
                        {
                            // TODO : (Steph) verify what this ugly code is doing ...
                            string path = shareInfo.Path;
                            if (!String.IsNullOrEmpty(path))
                            {
                                int index = path.Length;
                                if (Path.DirectorySeparatorChar != path[path.Length - 1])
                                {
                                    index++;
                                }

                                if (index < fileName.Length)
                                {
                                    fileName = fileName.Substring(index);
                                }
                                else
                                {
                                    fileName = String.Empty;
                                }
                                return Path.Combine(shareInfo.ToString(), fileName);
                            }
                        }
                    }
                    return String.Empty;

                default:
                    return String.Empty;
            }
        }

		public static ShareInfo GetShareInfoForLocalPath(string fileName)
		{
			if (String.IsNullOrEmpty(fileName))
			{
				return null;
			}

			fileName = Path.GetFullPath(fileName);
			if (!IsValidLocalFilePath(fileName))
			{
				return null;
			}

			List<ShareInfo> shareInfoList = GetShareInfo();
			if (shareInfoList != null)
			{
				return FindBestShareInfoMatch(shareInfoList, fileName);
			}
			return null;
		}

        public static string TryConvertUncToLocal(string pathToConvert)
        {
            if (pathToConvert.StartsWith("file:", StringComparison.InvariantCultureIgnoreCase))
            {
                Uri uri = new Uri(pathToConvert);
                if (uri.IsUnc)
                {
                    pathToConvert = uri.LocalPath;
                }
            }
            if (pathToConvert.StartsWith(@"\\"))
            {
                try
                {
                    // remove the "\\" from the UNC path and split the path
                    string path = pathToConvert.Replace(@"\\", "");
                    string[] uncParts = path.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                    if (uncParts.Length >= 2)
                    {
                        if (uncParts[0] == "localhost"
                            || uncParts[0].Equals(Dns.GetHostName(), StringComparison.InvariantCultureIgnoreCase)
                            || IsLocalMachine(uncParts[0]))
                        {
                            ManagementScope scope = new ManagementScope(@"\\" + uncParts[0] + @"\root\cimv2");
                            SelectQuery query = new SelectQuery("Select * From Win32_Share Where Name = '" + uncParts[1] + "'");

                            string localPath = String.Empty;
                            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
                            using (var collection = searcher.Get())
                            {
                                foreach (ManagementObject obj in collection)
                                {
                                    localPath = obj["path"].ToString();
                                    break;
                                }
                            }

                            // Append any additional folders to the local path name
                            if (uncParts.Length > 2)
                            {
                                for (int i = 2; i < uncParts.Length; i++)
                                {
                                    localPath = localPath.EndsWith(@"\") ? localPath + uncParts[i] : localPath + @"\" + uncParts[i];
                                }
                            }

                            pathToConvert = localPath;
                        }
                    }
                }
                catch { }
            }
            return pathToConvert;
        }

		#endregion

		#region Private Static Methods

		private static IList<ShareType> GetAllShareTypes()
		{
			IList<ShareType> shareTypes = new List<ShareType>();
			shareTypes.Add(ShareType.Device);
			shareTypes.Add(ShareType.Device);
			shareTypes.Add(ShareType.Disk);
			shareTypes.Add(ShareType.IPC);
			shareTypes.Add(ShareType.Printer);
			shareTypes.Add(ShareType.Special);
			return shareTypes;
		}

		private static List<ShareInfo> GetShareInfoNT(string serverName, IList<ShareType> shareTypes)
		{
			int level = 2;
			int entriesRead, totalEntries, nRet, hResume = 0;
			IntPtr pBuffer = IntPtr.Zero;
			List<ShareInfo> shareInfoList = new List<ShareInfo>();

			try
			{
				nRet = NetShareEnum(serverName, level, out pBuffer, -1, out entriesRead, out totalEntries, ref hResume);

				if (nRet == ERROR_ACCESS_DENIED)
				{
					//Need admin for level 2, drop to level 1
					level = 1;
					nRet = NetShareEnum(serverName, level, out pBuffer, -1, out entriesRead, out totalEntries, ref hResume);
				}

				if (nRet == NO_ERROR && entriesRead > 0)
				{
					Type t = (level == 2) ? typeof(SHARE_INFO_2) : typeof(SHARE_INFO_1);
					int offset = Marshal.SizeOf(t);

					for (int i = 0, lpItem = pBuffer.ToInt32(); i < entriesRead; i++, lpItem += offset)
					{
						IntPtr pItem = new IntPtr(lpItem);
						if (level == 1)
						{
							SHARE_INFO_1 shareInfo = (SHARE_INFO_1)Marshal.PtrToStructure(pItem, t);
							if (shareTypes.Contains(shareInfo.ShareType))
							{
								shareInfoList.Add(new ShareInfo(serverName, shareInfo.NetName, string.Empty, shareInfo.ShareType, shareInfo.Remark));
							}
						}
						else
						{
							SHARE_INFO_2 shareInfo = (SHARE_INFO_2)Marshal.PtrToStructure(pItem, t);
							if (shareTypes.Contains(shareInfo.ShareType))
							{
								shareInfoList.Add(new ShareInfo(serverName, shareInfo.NetName, shareInfo.Path, shareInfo.ShareType, shareInfo.Remark));
							}
						}
					}
				}
			}
			finally
			{
				// Clean up buffer allocated by system
				if (IntPtr.Zero != pBuffer)
				{
					NetApiBufferFree(pBuffer);
				}
			}
			return shareInfoList;
		}

		private static List<ShareInfo> GetShareInfo9x(string serverName, IList<ShareType> shareTypes)
		{
			int level = 50;
			int nRet = 0;
			ushort entriesRead, totalEntries;
			List<ShareInfo> shareInfoList = new List<ShareInfo>();

			Type t = typeof(SHARE_INFO_50);
			int size = Marshal.SizeOf(t);
			ushort cbBuffer = (ushort)(MAX_SI50_ENTRIES * size);
			//On Win9x, must allocate buffer before calling API
			IntPtr pBuffer = Marshal.AllocHGlobal(cbBuffer);

			try
			{
				nRet = NetShareEnum(serverName, level, pBuffer, cbBuffer, out entriesRead, out totalEntries);

				if (nRet == ERROR_WRONG_LEVEL)
				{
					level = 1;
					t = typeof(SHARE_INFO_1_9x);
					size = Marshal.SizeOf(t);
					nRet = NetShareEnum(serverName, level, pBuffer, cbBuffer, out entriesRead, out totalEntries);
				}

				if (nRet == NO_ERROR || nRet == ERROR_MORE_DATA)
				{
					for (int i = 0, lpItem = pBuffer.ToInt32(); i < entriesRead; i++, lpItem += size)
					{
						IntPtr pItem = new IntPtr(lpItem);

						if (level == 1)
						{
							SHARE_INFO_1_9x shareInfo = (SHARE_INFO_1_9x)Marshal.PtrToStructure(pItem, t);
							if (shareTypes.Contains(shareInfo.ShareType))
							{
								shareInfoList.Add(new ShareInfo(serverName, shareInfo.NetName, string.Empty, shareInfo.ShareType, shareInfo.Remark));
							}
						}
						else
						{
							SHARE_INFO_50 shareInfo = (SHARE_INFO_50)Marshal.PtrToStructure(pItem, t);
							if (shareTypes.Contains(shareInfo.ShareType))
							{
								shareInfoList.Add(new ShareInfo(serverName, shareInfo.NetName, shareInfo.Path, shareInfo.ShareType, shareInfo.Remark));
							}
						}
					}
				}
			}
			finally
			{
				//Clean up buffer
				Marshal.FreeHGlobal(pBuffer);
			}
			return shareInfoList;
		}

		private static ShareInfo FindBestShareInfoMatch(List<ShareInfo> shareInfoList, string fileName)
		{
			ShareInfo matched = null;
			foreach (ShareInfo shareInfo in shareInfoList)
			{
                if (shareInfo.IsFileSystem
                    && shareInfo.ShareType != ShareType.Special
                    && shareInfo.MatchesPath(fileName))
                {
					//Store first match
					if (matched == null)
					{
						matched = shareInfo;
					}
					// better match ? if so keep it
					else if (matched.Path.Length < shareInfo.Path.Length)
					{
						if (ShareType.Disk == shareInfo.ShareType || ShareType.Disk != matched.ShareType)
						{
							matched = shareInfo;
						}
					}
				}
			}
			return matched;
		}

        private static bool IsLocalMachine(string host)
        {
            try
            {
                IPAddress[] hostAddressess = Dns.GetHostAddresses(host);
                IPAddress[] localAddresses = Dns.GetHostAddresses(Dns.GetHostName());
                return localAddresses.Any(la => hostAddressess.Any(ha => ha.ToString() == la.ToString()));
            }
            catch
            {
                return false;
            }
        }

		#endregion
	}
}
