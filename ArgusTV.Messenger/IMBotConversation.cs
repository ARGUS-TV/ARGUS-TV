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
using System.Linq;
using System.Text;

using ArgusTV.ServiceAgents;

namespace ArgusTV.Messenger
{
    internal abstract class IMBotConversation
    {
        private IMCommands _imCommands;
        private List<IMBotMessage> _queuedMessages = new List<IMBotMessage>();

        public Dictionary<string, object> Session = new Dictionary<string, object>();

        public IMBotConversation(IMCommands imCommands)
        {
            _imCommands = imCommands;
        }

        protected abstract int MaxMessageLength { get; }

        protected IMBotMessage HandleIncomingMessage(string text)
        {
            DoKeepAlive();

            if (text.Equals("more", StringComparison.CurrentCultureIgnoreCase)
                || text.Equals("m", StringComparison.CurrentCultureIgnoreCase))
            {
                return GetNextQueuedMessage();
            }
            else
            {
                IMBotMessage reply = _imCommands.ProcessIMCommand(this, text);

                int chunks = 1 + (int)Math.Floor(reply.TotalLength / (decimal)this.MaxMessageLength);
                if (chunks > 1)
                {
                    _queuedMessages = CreateQueuedMessages(reply);
                    reply = GetNextQueuedMessage();
                }

                return reply;
            }
        }

        #region Queued Messages

        private List<IMBotMessage> CreateQueuedMessages(IMBotMessage message)
        {
            List<IMBotMessage> queuedMessages = new List<IMBotMessage>();

            StringBuilder chunkBuilder = new StringBuilder();
            int index = 0;
            while (index < message.BodyText.Length)
            {
                int nextIndex = message.BodyText.IndexOf(Environment.NewLine, index);
                int lineLength;
                if (nextIndex < 0)
                {
                    nextIndex = message.BodyText.Length;
                }
                else
                {
                    nextIndex += Environment.NewLine.Length;
                }
                lineLength = nextIndex - index;
                if (chunkBuilder.Length + message.FooterLength + lineLength > this.MaxMessageLength)
                {
                    chunkBuilder.Remove(chunkBuilder.Length - Environment.NewLine.Length, Environment.NewLine.Length);
                    AppendQueuedMessage(queuedMessages, message, chunkBuilder, true);
                    chunkBuilder = new StringBuilder();
                }
                chunkBuilder.Append(message.BodyText, index, lineLength);
                index = nextIndex;
            }
            AppendQueuedMessage(queuedMessages, message, chunkBuilder, false);

            return queuedMessages;
        }

        private void AppendQueuedMessage(List<IMBotMessage> queuedMessages, IMBotMessage message, StringBuilder chunkBuilder, bool addMore)
        {
            if (chunkBuilder.Length > 0)
            {
                IMBotMessage queuedMessage = new IMBotMessage(chunkBuilder.ToString());
                queuedMessage.Font = message.Font;
                queuedMessage.TextColor = message.TextColor;
                queuedMessage.Footer = message.Footer;
                queuedMessage.AddMore = addMore;
                queuedMessages.Add(queuedMessage);
            }
        }

        private IMBotMessage GetNextQueuedMessage()
        {
            IMBotMessage result = null;
            if (_queuedMessages.Count > 0)
            {
                result = _queuedMessages[0];
                _queuedMessages.RemoveAt(0);
            }
            else
            {
                result = new IMBotMessage("There is no more.", System.Drawing.Color.Red);
            }
            return result;
        }

        #endregion

        #region Keep Alive

        private static void DoKeepAlive()
        {
            try
            {
                // Inform the local system we need it.
                SetThreadExecutionState(EXECUTION_STATE.ES_SYSTEM_REQUIRED);
                // Tell the server we need it.
                using (CoreServiceAgent coreAgent = new CoreServiceAgent())
                {
                    coreAgent.KeepServerAlive();
                }
            }
            catch { }
        }

        [FlagsAttribute]
        private enum EXECUTION_STATE : uint
        {
            ES_SYSTEM_REQUIRED = 0x00000001,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000
        }

        [System.Runtime.InteropServices.DllImport("Kernel32.DLL", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        private extern static EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE state);

        #endregion
    }
}
