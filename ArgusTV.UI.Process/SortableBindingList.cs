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
using System.ComponentModel;
using System.Reflection;
using System.Collections;

namespace ArgusTV.UI.Process
{
    [Serializable]
    public class SortableBindingList<T> : BindingList<T>
    {
        #region Constructors

        public SortableBindingList()
        {
        }

        public SortableBindingList(IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }
            foreach (T t in enumerable)
            {
                this.Add(t);
            }
        }

        #endregion

        #region Overriden Methods

        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        private bool _isSorted;

        protected override bool IsSortedCore
        {
            get { return _isSorted; }
        }

        private PropertyDescriptor _propertyDescriptor;

        protected override PropertyDescriptor SortPropertyCore
        {
            get { return _propertyDescriptor; }
        }

        private ListSortDirection _sortDirection;

        protected override ListSortDirection SortDirectionCore
        {
            get { return _sortDirection; }
        }

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            List<T> itemsList = this.Items as List<T>;
            itemsList.Sort(delegate(T t1, T t2)
            {
                _propertyDescriptor = prop;
                _sortDirection = direction;
                _isSorted = true;

                int reverse = (direction == ListSortDirection.Ascending) ? 1 : -1;

                return reverse * ComparePropertyValues(prop, t1, t2);
            });

            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected virtual int ComparePropertyValues<TItem>(PropertyDescriptor prop, TItem t1, TItem t2)
        {
            return Comparer.Default.Compare(prop.GetValue(t1), prop.GetValue(t2));
        }

        protected override void RemoveSortCore()
        {
            this._isSorted = false;
            this._propertyDescriptor = base.SortPropertyCore;
            this._sortDirection = base.SortDirectionCore;
        }

        #endregion
    }
}
