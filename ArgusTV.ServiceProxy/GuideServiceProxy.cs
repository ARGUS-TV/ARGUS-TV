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
using System.Net;
using System.Net.Http;

using ArgusTV.DataContracts;

namespace ArgusTV.ServiceProxy
{
    /// <summary>
    /// Service to control/query all aspects of TV guide data.
    /// </summary>
    public partial class GuideServiceProxy : RestProxyBase
    {
        /// <summary>
        /// Constructs a channel to the service.
        /// </summary>
        internal GuideServiceProxy()
            : base("Guide")
        {
        }

        /// <summary>
        /// Add a new guide channel.
        /// </summary>
        /// <param name="xmltvId">The XMLTV ID of the new channel.</param>
        /// <param name="name">The name of the channel.</param>
        /// <param name="channelType">The channel type of the channel to add.</param>
        /// <returns>The ID of the new channel.</returns>
        public Guid AddChannel(string xmltvId, string name, ChannelType channelType)
        {
            var request = NewRequest(HttpMethod.Post, "NewChannel");
            request.AddBody(new
            {
                XmltvId = xmltvId,
                Name = name,
                ChannelType = channelType
            });
            var result = Execute<ChannelIdResult>(request);
            return result.GuideChannelId;
        }

        /// <summary>
        /// Delete a guide channel.  Any channels that are linked to this guide channel will have their
        /// link broken.  All guide programs in the channel will also be deleted.
        /// </summary>
        /// <param name="guideChannelId">The ID of the guide channel to delete.</param>
        public void DeleteChannel(Guid guideChannelId)
        {
            var request = NewRequest(HttpMethod.Post, "DeleteChannel/{0}", guideChannelId);
            Execute(request);
        }

        /// <summary>
        /// Save a modified or new guide channel.  A new guide channel is recognized by a Guid.Empty ID.
        /// </summary>
        /// <param name="channel">The guide channel to save.</param>
        /// <returns>The saved guide channel.</returns>
        public GuideChannel SaveChannel(GuideChannel channel)
        {
            var request = NewRequest(HttpMethod.Post, "SaveChannel");
            request.AddBody(channel);
            return Execute<GuideChannel>(request);
        }

        /// <summary>
        /// Ensure a guide channel exists.
        /// </summary>
        /// <param name="xmltvId">The XMLTV ID of the new channel.</param>
        /// <param name="name">The name of the channel.</param>
        /// <param name="channelType">The channel type of the channel to ensure.</param>
        /// <returns>The ID of the guide channel.</returns>
        public Guid EnsureChannelExists(string xmltvId, string name, ChannelType channelType)
        {
            var request = NewRequest(HttpMethod.Post, "EnsureChannelExists");
            request.AddBody(new
            {
                XmltvId = xmltvId,
                Name = name,
                ChannelType = channelType
            });
            var result = Execute<ChannelIdResult>(request);
            return result.GuideChannelId;
        }

        /// <summary>
        /// Get all guide channels.
        /// </summary>
        /// <param name="channelType">The channel type of the channels to retrieve.</param>
        /// <returns>An array containing zero or more guide channels.</returns>
        public List<GuideChannel> GetAllChannels(ChannelType channelType)
        {
            var request = NewRequest(HttpMethod.Post, "Channels/{0}", channelType);
            return Execute<List<GuideChannel>>(request);
        }

        /// <summary>
        /// Get a guide channel by its XMLTV ID.
        /// </summary>
        /// <param name="xmlTvId">The XMLTV ID.</param>
        /// <returns>The guide channel, or null if not found.</returns>
        public GuideChannel GetChannelByXmlTvId(string xmlTvId)
        {
            var request = NewRequest(HttpMethod.Post, "Channel/ByXmlTvId");
            request.AddBody(new
            {
                XmltvId = xmlTvId
            });
            return Execute<GuideChannel>(request);
        }

        /// <summary>
        /// Get a guide channel by its name.
        /// </summary>
        /// <param name="name">The name of the channel.</param>
        /// <returns>The guide channel, or null if not found.</returns>
        public GuideChannel GetChannelByName(string name)
        {
            var request = NewRequest(HttpMethod.Post, "Channel/ChannelByName");
            request.AddBody(new
            {
                Name = name
            });
            return Execute<GuideChannel>(request);
        }

        /// <summary>
        /// Import a new program into the guide.
        /// </summary>
        /// <param name="guideProgram">The program to import.</param>
        /// <param name="source">The source of the program.</param>
        /// <returns>The ID of the imported program.</returns>
        public Guid ImportProgram(GuideProgram guideProgram, GuideSource source)
        {
            var request = NewRequest(HttpMethod.Post, "ImportNewProgram");
            request.AddBody(new
            {
                Program = guideProgram,
                Source = source
            });
            var result = Execute<GuideProgramIdResult>(request);
            return result.GuideProgramId;
        }

        /// <summary>
        /// Import several new progams into the guide.
        /// </summary>
        /// <param name="guidePrograms">An array containing all programs to import.</param>
        /// <param name="source">The source of the programs.</param>
        public void ImportPrograms(IEnumerable<GuideProgram> guidePrograms, GuideSource source)
        {
            var request = NewRequest(HttpMethod.Post, "ImportPrograms");
            request.AddBody(new
            {
                Programs = guidePrograms,
                Source = source
            });
            Execute(request);
        }

        /// <summary>
        /// Get a guide programs by its ID.
        /// </summary>
        /// <param name="guideProgramId">The ID of the guide program.</param>
        /// <returns>The requested guide program, or null if it wasn't found.</returns>
        public GuideProgram GetProgramById(Guid guideProgramId)
        {
            var request = NewRequest(HttpMethod.Get, "Program/{0}", guideProgramId);
            return Execute<GuideProgram>(request);
        }

        /// <summary>
        /// Get all guide programs on the given channels, between the given lower and upper time.
        /// </summary>
        /// <param name="guideChannelIds">An array containing all guide channel IDs.</param>
        /// <param name="lowerTime">Return programs that end after this time.</param>
        /// <param name="upperTime">Return programs that start before this time.</param>
        /// <returns>An array containing zero or more guide programs.</returns>
        public List<GuideProgramSummary> GetChannelsProgramsBetween(IEnumerable<Guid> guideChannelIds, DateTime lowerTime, DateTime upperTime)
        {
            var request = NewRequest(HttpMethod.Post, "ChannelsPrograms");
            request.AddBody(new
            {
                GuideChannelIds = guideChannelIds,
                LowerTime = lowerTime,
                UpperTime = upperTime
            });
            return Execute<List<GuideProgramSummary>>(request);
        }

        /// <summary>
        /// Get all guide programs on the given channel, between the given lower and upper time.
        /// </summary>
        /// <param name="guideChannelId">The guide channel ID.</param>
        /// <param name="lowerTime">Return programs that end after this time.</param>
        /// <param name="upperTime">Return programs that start before this time.</param>
        /// <returns>An array containing zero or more guide programs.</returns>
        public List<GuideProgramSummary> GetChannelProgramsBetween(Guid guideChannelId, DateTime lowerTime, DateTime upperTime)
        {
            var request = NewRequest(HttpMethod.Get, "Programs/{0}/{1}/{2}", guideChannelId, lowerTime, upperTime);
            return Execute<List<GuideProgramSummary>>(request);
        }

        /// <summary>
        /// Get all available categories currently in the guide.
        /// </summary>
        /// <returns>An array containing zero or more categories.</returns>
        public List<string> GetAllCategories()
        {
            var request = NewRequest(HttpMethod.Get, "Categories");
            return Execute<List<string>>(request);
        }

        /// <summary>
        /// Delete old guide programs (everything before yesterday).
        /// </summary>
        public void DeleteOldPrograms()
        {
            var request = NewRequest(HttpMethod.Post, "DeleteOldPrograms");
            Execute(request);
        }

        /// <summary>
        /// Delete all guide programs.
        /// </summary>
        public void DeleteAllPrograms()
        {
            var request = NewRequest(HttpMethod.Post, "DeleteAllPrograms");
            Execute(request);
        }

        /// <summary>
        /// Notify ARGUS TV that a guide import has started. Call this before doing one or more ImportPrograms() calls.
        /// </summary>
        public void StartGuideImport()
        {
            var request = NewRequest(HttpMethod.Post, "StartGuideImport");
            Execute(request);
        }

        /// <summary>
        /// Notify ARGUS TV that the guide import has ended.  The NewGuideData event will be sent to all listeners.
        /// </summary>
        public void EndGuideImport()
        {
            var request = NewRequest(HttpMethod.Post, "EndGuideImport");
            Execute(request);
        }

        private class ChannelIdResult
        {
            public Guid GuideChannelId { get; set; }
        }

        private class GuideProgramIdResult
        {
            public Guid GuideProgramId { get; set; }
        }
    }
}
