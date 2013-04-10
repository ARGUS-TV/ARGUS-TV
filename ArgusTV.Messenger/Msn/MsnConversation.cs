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
using System.Threading;

using MSNPSharp;

using ArgusTV.ServiceAgents;

namespace ArgusTV.Messenger.Msn
{
    internal class MsnConversation : IMBotConversation
    {
        private MSNPSharp.Messenger _messenger;

        public MsnConversation(MSNPSharp.Messenger messenger, IMCommands imCommands, Contact contact)
            : base(imCommands)
        {
            _messenger = messenger;
            _contact = contact;
        }

        protected override int MaxMessageLength
        {
            get { return 1000; }
        }

        private Contact _contact;

        public Contact Contact
        {
            get { return _contact; }
        }

        public void Close()
        {
        }

        private TextMessage _sentTextMessage;
        private DateTime _sentTextMessageTimeout;

        public void SendTextMessage(TextMessage textMessage)
        {
            _sentTextMessage = textMessage;
            _sentTextMessageTimeout = DateTime.Now.AddSeconds(90);
            _contact.SendMessage(textMessage);
        }

        public void HandleIncomingMessage(TextMessage textMessage)
        {
            _sentTextMessage = null;

            IMBotMessage reply = HandleIncomingMessage(textMessage.Text);

            TextMessage message = reply.ToMsnMessage();
            _contact.SendMessage(message);
        }
    }
}
