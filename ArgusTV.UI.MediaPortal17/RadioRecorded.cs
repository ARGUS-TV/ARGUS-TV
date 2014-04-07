#region Copyright (C) 2005-2013 Team MediaPortal

/* 
 *	Copyright (C) 2005-2013 Team MediaPortal
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
using ArgusTV.ServiceProxy;
using ArgusTV.UI.Process.Recordings;

namespace ArgusTV.UI.MediaPortal
{
    public class RadioRecorded : RecordedBase
    {
        protected override string SettingsSection
        {
            get { return "argustv_radiorecorded"; }
        }

        public RadioRecorded()
            : base(ChannelType.Radio)
        {
            GetID = (int)WindowId.RecordedRadio;
        }

        public override void OnAdded()
        {
            base.OnAdded();
        }

        public override bool Init()
        {
            bool result = Load(GUIGraphicsContext.Skin + @"\ARGUS_RecordedRadio2.xml");
            if (result)
            {
                base.Init();
            }
            return result;
        }

        public override bool IsTv
        {
            get { return true; }
        }
    }
}
