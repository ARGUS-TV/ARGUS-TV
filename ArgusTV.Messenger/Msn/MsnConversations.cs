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
using System.Threading;

using MSNPSharp;

using ArgusTV.ServiceAgents;

namespace ArgusTV.Messenger.Msn
{
    internal class MsnConversations
    {
        private Dictionary<Guid, MsnConversation> _msnConversations = new Dictionary<Guid, MsnConversation>();
        private MSNPSharp.Messenger _messenger;
        private IMCommands _imCommands;

        public MsnConversations(MSNPSharp.Messenger messenger, IMCommands imCommands)
        {
            _messenger = messenger;
            _imCommands = imCommands;
        }

        public MsnConversation EnsureConversation(Contact contact)
        {
            lock (_msnConversations)
            {
                if (!_msnConversations.ContainsKey(contact.Guid))
                {
                    AddConversation(contact);
                }
                return _msnConversations[contact.Guid];
            }
        }

        public void CloseConversation(Contact contact)
        {
            lock (_msnConversations)
            {
                if (_msnConversations.ContainsKey(contact.Guid))
                {
                    RemoveConversation(contact.Guid);
                }
            }
        }

        public void CloseAllConversations()
        {
            lock (_msnConversations)
            {
                while (_msnConversations.Count > 0)
                {
                    foreach (Guid contactId in _msnConversations.Keys)
                    {
                        RemoveConversation(contactId);
                        break;
                    }
                }
            }
        }

        private Guid RemoveConversation(Guid contactId)
        {
            _msnConversations[contactId].Close();
            _msnConversations.Remove(contactId);
            return contactId;
        }

        private MsnConversation AddConversation(Contact contact)
        {
            MsnConversation msnConversation = new MsnConversation(_messenger, _imCommands, contact);

            if (_msnConversations.ContainsKey(msnConversation.Contact.Guid))
            {
                RemoveConversation(msnConversation.Contact.Guid);
            }
            _msnConversations.Add(msnConversation.Contact.Guid, msnConversation);

            return msnConversation;
        }
    }
}
