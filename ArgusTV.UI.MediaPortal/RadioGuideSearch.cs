#region Copyright (C) 2005-2012 Team MediaPortal

/* 
 *	Copyright (C) 2005-2012 Team MediaPortal
 *	http://www.team-mediaportal.com
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

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MediaPortal.Services;
using MediaPortal.Threading;
using MediaPortal.Util;
using MediaPortal.Configuration;

using ArgusTV.DataContracts;
using ArgusTV.ServiceAgents;
using ArgusTV.ServiceContracts;

namespace ArgusTV.UI.MediaPortal
{
    public class RadioGuideSearch : GuideSearchBase
    {
        public RadioGuideSearch()
            : base(ChannelType.Radio)
        {
            GetID = (int)WindowId.RadioGuideSearch;
        }

        protected override string SettingsSection
        {
            get { return "argustv_radioguidesearch"; }
        }

        public override bool Init()
        {
            return Load(GUIGraphicsContext.Skin + @"\ARGUS_RadioGuideSearch2.xml");
        }
    }
}
