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

namespace ArgusTV.Messenger
{
    internal class IMBotMessage
    {
        private const string _defaultFont = "Courier New";
        private const string _defaultFixedWidthFont = "Courier New";
        private const string _moreText = "More...";

        private readonly System.Drawing.Color _defaultTextColor = System.Drawing.Color.Navy;

        public static readonly System.Drawing.Color ErrorColor = System.Drawing.Color.Red;

        public IMBotMessage()
        {
            this.Font = _defaultFont;
            this.TextColor = _defaultTextColor;
        }

        public IMBotMessage(bool fixedWidth)
        {
            this.Font = fixedWidth ? _defaultFixedWidthFont : _defaultFont;
            this.TextColor = _defaultTextColor;
        }

        public IMBotMessage(string text)
            : this()
        {
            this.BodyText = text;
        }

        public IMBotMessage(string text, System.Drawing.Color textColor)
            : this(text)
        {
            this.TextColor = textColor;
        }

        public IMBotMessage(string text, bool fixedWidth)
            : this(fixedWidth)
        {
            this.BodyText = text;
        }

        public IMBotMessage(string text, bool fixedWidth, System.Drawing.Color textColor)
            : this(text, fixedWidth)
        {
            this.TextColor = textColor;
        }

        public string Font { set; get; }

        public System.Drawing.Color TextColor { set; get; }

        public string BodyText { set; get; }

        public string Footer { set; get; }

        public bool AddMore { set; get; }

        public string MessageText
        {
            get
            {
                if (String.IsNullOrEmpty(this.Footer))
                {
                    return this.BodyText;
                }
                StringBuilder text = new StringBuilder(this.BodyText);
                text.AppendLine();
                text.Append(this.Footer);
                if (this.AddMore)
                {
                    text.AppendLine();
                    text.Append(_moreText);
                }
                return text.ToString();
            }
        }

        public int TotalLength
        {
            get
            {
                return this.BodyText.Length + this.FooterLength;
            }
        }

        public int FooterLength
        {
            get
            {
                if (!String.IsNullOrEmpty(this.Footer))
                {
                    return Environment.NewLine.Length + this.Footer.Length;
                }
                return 0;
            }
        }

        public MSNPSharp.TextMessage ToMsnMessage()
        {
            MSNPSharp.TextMessage message = new MSNPSharp.TextMessage(this.MessageText);
            message.Font = this.Font;
            message.Color = this.TextColor;
            return message;
        }
    }
}
