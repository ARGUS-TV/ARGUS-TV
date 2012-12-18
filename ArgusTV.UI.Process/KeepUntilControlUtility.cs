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
using System.Data;
using System.Globalization;
using System.Text;

using ArgusTV.DataContracts;

namespace ArgusTV.UI.Process
{
    public static class KeepUntilControlUtility
    {
        public const string TextColumnName = "Text";
        public const string ValueColumnName = "Value";

        private static int[] _daysValues = new int[] { 7, 14, 31, 62, 92, 183, 365 };
        private static int[] _episodesValues = new int[] { 1, 2, 3, 5, 10, 15, 20, 25, 50, 75, 100 };

        public static DataTable CreateValueTable(KeepUntilMode mode, int? currentValue)
        {
            DataTable result = new DataTable();
            result.Columns.Add(KeepUntilControlUtility.TextColumnName, typeof(string));
            result.Columns.Add(KeepUntilControlUtility.ValueColumnName, typeof(int));

            if (mode == KeepUntilMode.NumberOfDays
                || mode == KeepUntilMode.NumberOfEpisodes
                || mode == KeepUntilMode.NumberOfWatchedEpisodes)
            {
                bool currentValueAdded = !currentValue.HasValue;
                int[] values = (mode == KeepUntilMode.NumberOfDays ) ? _daysValues : _episodesValues;
                foreach (int value in values)
                {
                    if (!currentValueAdded
                        && currentValue.Value <= value)
                    {
                        if (currentValue.Value != value)
                        {
                            AddValueToTable(result, currentValue.Value);
                        }
                        currentValueAdded = true;
                    }
                    AddValueToTable(result, value);
                }
                if (!currentValueAdded)
                {
                    AddValueToTable(result, currentValue.Value);
                }
            }

            return result;
        }

        public static int GetIndexToSelect(DataTable table, int? value)
        {
            int selectIndex = 0;
            if (value.HasValue)
            {
                int index = 0;
                foreach (DataRow row in table.Rows)
                {
                    if (value.Value <= (int)row[KeepUntilControlUtility.ValueColumnName])
                    {
                        selectIndex = index;
                        break;
                    }
                    index++;
                }
            }
            return selectIndex;
        }

        private static void AddValueToTable(DataTable result, int currentValue)
        {
            result.Rows.Add(currentValue.ToString(CultureInfo.CurrentCulture), currentValue);
        }

        public static KeepUntilMode IndexToMode(int index)
        {
            switch (index)
            {
                case 0: return KeepUntilMode.UntilSpaceIsNeeded;
                case 1: return KeepUntilMode.NumberOfDays;
                case 2: return KeepUntilMode.NumberOfEpisodes;
                case 3: return KeepUntilMode.NumberOfWatchedEpisodes;
                case 4: return KeepUntilMode.Forever;

            }
            return KeepUntilMode.UntilSpaceIsNeeded;
        }

        public static int ModeToIndex(KeepUntilMode mode)
        {
            switch (mode)
            {
                case KeepUntilMode.UntilSpaceIsNeeded: return 0;
                case KeepUntilMode.NumberOfDays: return 1;
                case KeepUntilMode.NumberOfEpisodes: return 2;
                case KeepUntilMode.NumberOfWatchedEpisodes: return 3;
                case KeepUntilMode.Forever: return 4;
            }
            return 0;
        }
    }
}
