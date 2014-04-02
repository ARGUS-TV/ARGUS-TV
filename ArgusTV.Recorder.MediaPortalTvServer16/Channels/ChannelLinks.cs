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
using System.IO;
using System.Xml.XPath;
using System.Xml;
using System.Collections;
using System.Text;

using System.Threading;

using Gentle.Framework;

using ArgusTV.DataContracts;

namespace ArgusTV.Recorder.MediaPortalTvServer.Channels
{
    internal class ChannelLinks
    {
        private ReaderWriterLockSlim _readerWriterLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private Dictionary<Guid, ChannelLink> _linksById = new Dictionary<Guid, ChannelLink>();
        private Dictionary<int, bool> _autoLinkedMPChannels = new Dictionary<int, bool>();

        private ChannelLinks()
        {
            LoadChannelLinks(true);
        }

        #region Singleton

        private static ChannelLinks Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly ChannelLinks instance = new ChannelLinks();
        }

        #endregion

        public static LinkedMediaPortalChannel GetLinkedMediaPortalChannel(Channel channel)
        {
            bool isAutoLinked;
            bool duplicateChannelsFound;
            return GetLinkedMediaPortalChannel(channel, out isAutoLinked, out duplicateChannelsFound);
        }

        public static LinkedMediaPortalChannel GetLinkedMediaPortalChannel(ChannelType channelType, Guid channelId, string displayName)
        {
            bool isAutoLinked;
            bool duplicateChannelsFound;
            return GetLinkedMediaPortalChannel(channelType, channelId, displayName, out isAutoLinked, out duplicateChannelsFound);
        }

        public static LinkedMediaPortalChannel GetLinkedMediaPortalChannel(Channel channel, out bool isAutoLinked, out bool duplicateChannelsFound)
        {
            return GetLinkedMediaPortalChannel(channel.ChannelType, channel.ChannelId, channel.DisplayName, out isAutoLinked, out duplicateChannelsFound);
        }

        public static LinkedMediaPortalChannel GetLinkedMediaPortalChannel(ChannelType channelType, Guid channelId, string displayName,
            out bool isAutoLinked, out bool duplicateChannelsFound)
        {
            return Instance.GetLinkedMediaPortalChannelInternal(channelType, channelId, displayName, out isAutoLinked, out duplicateChannelsFound);
        }

        public static bool IsAutoLinked(TvDatabase.Channel mpChannel)
        {
            return Instance.IsAutoLinkedInternal(mpChannel);
        }

        public static void SetLinkedMediaPortalChannel(Channel channel, TvDatabase.Channel mpChannel)
        {
            Instance.SetLinkedMediaPortalChannelInternal(channel, mpChannel);
        }

        public static void ClearLinkedMediaPortalChannel(Channel channel)
        {
            Instance.ClearLinkedMediaPortalChannelInternal(channel);
        }

        public static ChannelLink GetChannelLinkForMediaPortalChannel(TvDatabase.Channel channel)
        {
            return Instance.GetChannelLinkForMediaPortalChannelInternal(channel);
        }

        public static void RemoveObsoleteLinks(ChannelType channelType, List<Channel> channels)
        {
            Instance.RemoveObsoleteLinksInternal(channelType, channels);
        }

        public static void Save()
        {
            Instance.SaveChannelLinks();
        }

        #region Serialization

        private string SettingsFileName
        {
            get
            {
                return Path.Combine(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ARGUS TV"),
                    "MediaPortalTvServer.Channels.config");
            }
        }

        private DateTime _lastFileWriteTimeUtc = DateTime.MinValue;

        private void EnsureLoaded()
        {
            LoadChannelLinks(false);
        }

        private void LoadChannelLinks(bool forceLoad)
        {
            string settingsFileName = this.SettingsFileName;
            if (!File.Exists(settingsFileName))
            {
                _readerWriterLock.EnterWriteLock();
                try
                {
                    _linksById.Clear();
                    _autoLinkedMPChannels.Clear();
                    return;
                }
                finally
                {
                    _readerWriterLock.ExitWriteLock();
                }
            }

            DateTime lastWriteTimeUtc = File.GetLastWriteTimeUtc(settingsFileName);
            if (forceLoad
                || lastWriteTimeUtc != _lastFileWriteTimeUtc)
            {
                _readerWriterLock.EnterWriteLock();
                try
                {
                    _lastFileWriteTimeUtc = lastWriteTimeUtc;
                    _linksById.Clear();
                    _autoLinkedMPChannels.Clear();
                    using (StreamReader reader = new StreamReader(settingsFileName, Encoding.UTF8))
                    {
                        XPathDocument xpathDocument = new XPathDocument(reader);
                        XPathNavigator navigator = xpathDocument.CreateNavigator();
                        XPathNodeIterator iterator = navigator.Select("/links/link");
                        while (iterator.MoveNext())
                        {
                            ChannelLink link = ChannelLink.Parse(iterator.Current);
                            if (link != null)
                            {
                                TvDatabase.Channel mpChannel = GetChannelById(link.ChannelType, link.MPChannelId);
                                if (mpChannel == null
                                    || mpChannel.DisplayName != link.MPChannelName)
                                {
                                    bool duplicateChannelsFound;
                                    mpChannel = GetChannelByDisplayName(link.ChannelType, link.MPChannelName, out duplicateChannelsFound);
                                }
                                if (mpChannel != null)
                                {
                                    _linksById.Add(link.ChannelId, link);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    _readerWriterLock.ExitWriteLock();
                }
            }
        }

        private void SaveChannelLinks()
        {
            _readerWriterLock.EnterReadLock();
            try
            {
                string settingsFileName = this.SettingsFileName;
                string settingsDirName = Path.GetDirectoryName(settingsFileName);
                if (!Directory.Exists(settingsDirName))
                {
                    Directory.CreateDirectory(settingsDirName);
                }
                using (XmlTextWriter xmlWriter = new XmlTextWriter(this.SettingsFileName, Encoding.UTF8))
                {
                    xmlWriter.Formatting = Formatting.Indented;
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("links");
                    foreach (ChannelLink link in _linksById.Values)
                    {
                        link.WriteToXml(xmlWriter);
                    }
                    xmlWriter.WriteEndElement();
                }
                _lastFileWriteTimeUtc = File.GetLastWriteTimeUtc(settingsFileName);
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }

        #endregion

        #region Private Methods

        private LinkedMediaPortalChannel GetLinkedMediaPortalChannelInternal(ChannelType channelType, Guid channelId, string displayName, out bool isAutoLinked, out bool duplicateChannelsFound)
        {
            EnsureLoaded();

            _readerWriterLock.EnterReadLock();
            try
            {
                isAutoLinked = false;
                duplicateChannelsFound = false;

                if (_linksById.ContainsKey(channelId))
                {
                    ChannelLink link = _linksById[channelId];
                    return new LinkedMediaPortalChannel(link.MPChannelId, link.MPChannelName);
                }

                TvDatabase.Channel channel = GetChannelByDisplayName(channelType, displayName,
                    out duplicateChannelsFound);
                if (channel != null)
                {
                    isAutoLinked = true;
                    _autoLinkedMPChannels[channel.IdChannel] = true;
                    return new LinkedMediaPortalChannel(channel.IdChannel, channel.DisplayName);
                }

                return null;
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }

        private ChannelLink GetChannelLinkForMediaPortalChannelInternal(TvDatabase.Channel channel)
        {
            EnsureLoaded();

            _readerWriterLock.EnterReadLock();
            try
            {
                foreach (ChannelLink link in _linksById.Values)
                {
                    if (link.MPChannelId == channel.IdChannel
                        && link.MPChannelName == channel.DisplayName)
                    {
                        return link;
                    }
                }
                return null;
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }

        public bool IsAutoLinkedInternal(TvDatabase.Channel mpChannel)
        {
            EnsureLoaded();

            _readerWriterLock.EnterReadLock();
            try
            {
                return _autoLinkedMPChannels.ContainsKey(mpChannel.IdChannel);
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }

        public void SetLinkedMediaPortalChannelInternal(Channel channel, TvDatabase.Channel mpChannel)
        {
            EnsureLoaded();

            _readerWriterLock.EnterWriteLock();
            try
            {
                ClearLinkedMediaPortalChannel(channel);
                ChannelLink link = new ChannelLink(channel.ChannelType, channel.ChannelId, channel.DisplayName, mpChannel.IdChannel, mpChannel.DisplayName);
                _linksById.Add(link.ChannelId, link);
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }
        }

        public void ClearLinkedMediaPortalChannelInternal(Channel channel)
        {
            EnsureLoaded();

            _readerWriterLock.EnterWriteLock();
            try
            {
                if (_linksById.ContainsKey(channel.ChannelId))
                {
                    _linksById.Remove(channel.ChannelId);
                }
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }
        }

        private void RemoveObsoleteLinksInternal(ChannelType channelType, List<Channel> channels)
        {
            EnsureLoaded();

            _readerWriterLock.EnterWriteLock();
            try
            {
                List<Guid> obsoleteLinks = new List<Guid>();
                foreach (Guid channelId in _linksById.Keys)
                {
                    if (_linksById[channelId].ChannelType == channelType
                        && !ChannelListContains(channels, channelId))
                    {
                        obsoleteLinks.Add(channelId);
                    }
                }
                foreach (Guid channelId in obsoleteLinks)
                {
                    _linksById.Remove(channelId);
                }
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }
        }

        private TvDatabase.Channel GetChannelByDisplayName(ChannelType channelType, string displayName, out bool duplicateChannelsFound)
        {
            return InternalGetChannelBy(channelType, "displayName", displayName, out duplicateChannelsFound);
        }

        private TvDatabase.Channel GetChannelById(ChannelType channelType, int id)
        {
            bool duplicateChannelsFound;
            return InternalGetChannelBy(channelType, "idChannel", id, out duplicateChannelsFound);
        }

        private TvDatabase.Channel InternalGetChannelBy(ChannelType channelType, string columnName, object value, out bool duplicateChannelsFound)
        {
            SqlBuilder sb = new SqlBuilder(StatementType.Select, typeof(TvDatabase.Channel));
            sb.AddConstraint(Operator.Equals, columnName, value);
            sb.AddConstraint(Operator.Equals, "visibleInGuide", true);
            if (channelType == ChannelType.Television)
            {
                sb.AddConstraint(Operator.Equals, "isTv", true);
            }
            else
            {
                sb.AddConstraint(Operator.Equals, "isRadio", true);
            }
            SqlResult result = Broker.Execute(sb.GetStatement());
            if (result.Rows.Count == 1)
            {
                duplicateChannelsFound = false;
                IList channels = ObjectFactory.GetCollection(typeof(TvDatabase.Channel), result);
                return channels[0] as TvDatabase.Channel;
            }
            duplicateChannelsFound = (result.Rows.Count > 0);
            return null;
        }

        private static bool ChannelListContains(List<Channel> channels, Guid channelId)
        {
            foreach (Channel channel in channels)
            {
                if (channel.ChannelId == channelId)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
