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
using System.Text;

namespace ArgusTV.Messenger
{
    internal class AddressList : List<string>
    {
        public AddressList(string addresses)
        {
            if (!String.IsNullOrEmpty(addresses))
            {
                string[] parts = addresses.Split(';');
                foreach (string part in parts)
                {
                    string address = part.Trim();
                    if (address.Length > 0)
                    {
                        this.Add(address);
                    }
                }
            }
        }

        public bool ContainsAddress(string address)
        {
            foreach(string mail in this)
            {
                if (String.Equals(mail, address, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public bool AddAddress(string address)
        {
            foreach (string mail in this)
            {
                if (String.Equals(mail, address, StringComparison.CurrentCultureIgnoreCase))
                {
                    return false;
                }
            }
            this.Add(address);
            return true;
        }

        public bool RemoveAddress(string address)
        {
            foreach (string mail in this)
            {
                if (String.Equals(mail, address, StringComparison.CurrentCultureIgnoreCase))
                {
                    this.Remove(mail);
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            foreach (string mail in this)
            {
                if (result.Length > 0)
                {
                    result.Append(';');
                }
                result.Append(mail);
            }
            return result.ToString();
        }
    }
}
