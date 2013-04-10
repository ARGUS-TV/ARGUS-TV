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
using System.Text;

namespace ArgusTV.Common
{
    [Flags]
    public enum OperatingSystem
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown = 0x00000000,

        /// <summary>
        /// 32-Bit version of windows.
        /// </summary>
        Windows_32Bit = 0x00000001,

        /// <summary>
        /// 64-Bit version of windows.
        /// </summary>
        Windows_64Bit = 0x00000002,

        /// <summary>
        /// Win32s.
        /// </summary>
        WindowsWin32s = 0x00010000,

        /// <summary>
        /// Windows 95.
        /// </summary>
        Windows95 = 0x00020000,

        /// <summary>
        /// Windows 98.
        /// </summary>
        Windows98 = 0x00040000,

        /// <summary>
        /// Windows ME.
        /// </summary>
        WindowsME = 0x00080000,

        /// <summary>
        /// Windows NT.
        /// </summary>
        WindowsNT = 0x00100000,

        /// <summary>
        /// Windows 2000.
        /// </summary>
        Windows2000 = 0x00200000,

        /// <summary>
        /// Windows XP.
        /// </summary>
        WindowsXP = 0x00400000,

        /// <summary>
        /// Windows CE.
        /// </summary>
        WindowsCE = 0x00800000,

        /// <summary>
        /// Windows 2003.
        /// </summary>
        Windows2003 = 0x001000000,

        /// <summary>
        /// Windows Vista.
        /// </summary>
        WindowsVista = 0x002000000,

        /// <summary>
        /// Windows 7.
        /// </summary>
        Windows7 = 0x004000000,

        /// <summary>
        /// Windows 98 SE.
        /// </summary>
        Windows98_SE = 0x00000004,

        /// <summary>
        /// Windows NT 3.5.1.
        /// </summary>
        WindowsNT_351 = 0x00000008,

        /// <summary>
        /// Windows NT 4.0.
        /// </summary>
        WindowsNT_40 = 0x00000010,

        /// <summary>
        /// Windows 2000 Service Pack 1.
        /// </summary>
        Windows2000_SP1 = 0x00000020,

        /// <summary>
        /// Windows 2000 Service Pack 2.
        /// </summary>
        Windows2000_SP2 = 0x00000040,

        /// <summary>
        /// Windows 2000 Service Pack 3.
        /// </summary>
        Windows2000_SP3 = 0x00000080,

        /// <summary>
        /// Windows 2000 Service Pack 4.
        /// </summary>
        Windows2000_SP4 = 0x00000100,

        /// <summary>
        /// Windows XP Service Pack 1.
        /// </summary>
        WindowsXP_SP1 = 0x00000200,

        /// <summary>
        /// Windows XP Service Pack 2.
        /// </summary>
        WindowsXP_SP2 = 0x00000400,

        /// <summary>
        /// Windows XP Service Pack 3.
        /// </summary>
        WindowsXP_SP3 = 0x00000800,

        /// <summary>
        /// Windows Vista Service Pack 1.
        /// </summary>
        WindowsVista_SP1 = 0x00001000,

        /// <summary>
        /// Windows Vista Service Pack 2.
        /// </summary>
        WindowsVista_SP2 = 0x00002000,

        /// <summary>
        /// Windows 7 Service Pack 1.
        /// </summary>
        Windows7_SP1 = 0x00004000,

        /// <summary>
        /// All architecture flags combined.
        /// </summary>
        AllArchitectures = Windows_32Bit | Windows_64Bit,

        /// <summary>
        /// All Windows Release flags combined.
        /// </summary>
        AllReleases =
            WindowsWin32s | Windows95 | Windows98 | WindowsME | WindowsNT |
            Windows2000 | WindowsXP | WindowsCE | Windows2003 | WindowsVista |
            Windows7,

        /// <summary>
        /// All windows Edition and Service Pack flags combined.
        /// </summary>
        AllEditionServicePacks =
            Windows98_SE | WindowsNT_351 | WindowsNT_40 |
            Windows2000_SP1 | Windows2000_SP2 | Windows2000_SP3 | Windows2000_SP4 |
            WindowsXP_SP1 | WindowsXP_SP2 | WindowsXP_SP3 |
            WindowsVista_SP1 | WindowsVista_SP2 |
            Windows7_SP1,

        /// <summary>
        /// A later version, yet unknown by this enumeration.
        /// </summary>
        LaterVersion = 0x10000000
    }
}
