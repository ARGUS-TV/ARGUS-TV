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
using System.Xml;
using System.Xml.Serialization;

namespace ArgusTV.Batch.ProcessCommand
{
    #region Tags

    [Serializable]
    [XmlRoot(ElementName = "tags")]
    public class Tags
    {
        private List<SimpleTag> _simpleTags = new List<SimpleTag>();

        [XmlArray("tag")]
        [XmlArrayItem("SimpleTag", typeof(SimpleTag))]
        public List<SimpleTag> SimpleTags
        {
            get {return _simpleTags; }
        }
    }
    #endregion

    #region SimpleTag

    [Serializable()]
    public class SimpleTag
    {
        private string _name;
        private string _value;

        public SimpleTag() {}

        public SimpleTag(string name, string value)
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