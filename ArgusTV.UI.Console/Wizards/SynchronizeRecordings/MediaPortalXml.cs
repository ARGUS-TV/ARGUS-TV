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
using System.Xml;
using System.Xml.Serialization;

namespace ArgusTV.UI.Console.Wizards.SynchronizeRecordings
{
    #region Tags

    [Serializable()]
    [XmlRoot(ElementName="tags")]
    public class MPTags
    {
        private List<MPSimpleTag> _simpleTags = new List<MPSimpleTag>();

        [XmlArray("tag")]
        [XmlArrayItem("SimpleTag", typeof(MPSimpleTag))]
        public List<MPSimpleTag> SimpleTags
        {
            get {return _simpleTags; }
        }

        public string GetValue(string name, string defaultValue)
        {
            foreach (MPSimpleTag simpleTag in _simpleTags)
            {
                if (simpleTag.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return simpleTag.Value;
                }
            }
            return defaultValue;
        }
    }

    #endregion

    #region SimpleTag

    [Serializable()]
    public class MPSimpleTag
    {
        private string _name;
        private string _value;

        public MPSimpleTag() {}

        public MPSimpleTag(string name, string value)
        {
            _name = name;
            _value = value;
        }

        [XmlElement(ElementName = "name")]
        public string Name 
        {
            get{return _name;}
            set{_name = value;}            
        }
        
        [XmlElement(ElementName = "value")]
        public string Value 
        {
            get{return _value;}
            set{_value = value;}            
        }
    }

    #endregion
}