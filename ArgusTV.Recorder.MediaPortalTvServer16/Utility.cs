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
using System.Collections;

using TvControl;
using TvLibrary.Interfaces;
using Gentle.Framework;

using ArgusTV.DataContracts;

namespace ArgusTV.Recorder.MediaPortalTvServer
{
    internal static class Utility
    {
        public static TvDatabase.TuningDetail FindTuningDetailOnCard(TvDatabase.Channel channel, int cardId)
        {
            try
            {
                CardType cardType = TvServerPlugin.TvController_Type(cardId);
                IList<TvDatabase.TuningDetail> tuningDetails = channel.ReferringTuningDetail();
                foreach (TvDatabase.TuningDetail tuningDetail in tuningDetails)
                {
                    if (CardTunesChannelType(cardType, tuningDetail.ChannelType))
                    {
                        return tuningDetail;
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        public static bool CardTunesChannelType(CardType cardType, int channelType)
        {
            switch (cardType)
            {
                case CardType.Analog:
                    return (channelType == 0);
                case CardType.Atsc:
                    return (channelType == 1);
                case CardType.DvbC:
                    return (channelType == 2);
                case CardType.DvbS:
                    return (channelType == 3);
                case CardType.DvbT:
                    return (channelType == 4);
                case CardType.DvbIP:
                    return (channelType == 7);
            }
            return false;
        }

        public static IChannel FindTuningChannelOnCard(TvDatabase.Channel channel, int cardId)
        {
            IChannel tuningChannel = null;
            try
            {
                List<IChannel> tunings = new TvDatabase.TvBusinessLayer().GetTuningChannelsByDbChannel(channel);
                foreach (IChannel tuning in tunings)
                {
                    if (TvServerPlugin.TvController_CanTune(cardId, tuning))
                    {
                        tuningChannel = tuning;
                        break;
                    }
                }
            }
            catch
            {
                tuningChannel = null;
            }
            return tuningChannel;
        }

        public static List<TGroup> GetAllGroups<TGroup>()
        {
            SqlBuilder sb = new SqlBuilder(StatementType.Select, typeof(TGroup));
            sb.AddOrderByField("groupName");
            SqlResult result = Broker.Execute(sb.GetStatement());
            return (List<TGroup>)ObjectFactory.GetCollection(typeof(TGroup), result, new List<TGroup>());
        }

        /// <summary>
        /// Get all channels in a group, sorted by display name.
        /// </summary>
        /// <param name="channelType">The type of channels in the group.</param>
        /// <param name="groupId">The group ID or -1 for all channels.</param>
        /// <returns>A list containing zero or more channels.</returns>
        public static List<TvDatabase.Channel> GetAllChannelsInGroup(ChannelType channelType, int groupId)
        {
            List<TvDatabase.Channel> channels = new List<TvDatabase.Channel>();

            if (groupId < 0)
            {
                SqlBuilder sb = new SqlBuilder(StatementType.Select, typeof(TvDatabase.Channel));
                if (channelType == ChannelType.Television)
                {
                    sb.AddConstraint(Operator.Equals, "isTv", true);
                }
                else
                {
                    sb.AddConstraint(Operator.Equals, "isRadio", true);
                }
                sb.AddConstraint(Operator.Equals, "visibleInGuide", true);
                sb.AddOrderByField("displayName");
                SqlResult result = Broker.Execute(sb.GetStatement());
                channels = (List<TvDatabase.Channel>)
                    ObjectFactory.GetCollection(typeof(TvDatabase.Channel), result, new List<TvDatabase.Channel>());
            }
            else
            {
                if (channelType == ChannelType.Television)
                {
                    SqlBuilder sb = new SqlBuilder(StatementType.Select, typeof(TvDatabase.GroupMap));
                    sb.AddConstraint(Operator.Equals, "idGroup", groupId);
                    SqlResult result = Broker.Execute(sb.GetStatement());
                    List<TvDatabase.GroupMap> groupMaps = (List<TvDatabase.GroupMap>)
                        ObjectFactory.GetCollection(typeof(TvDatabase.GroupMap), result, new List<TvDatabase.GroupMap>());
                    foreach (TvDatabase.GroupMap groupMap in groupMaps)
                    {
                        TvDatabase.Channel mpChannel = groupMap.ReferencedChannel();
                        if (mpChannel.IsTv)
                        {
                            channels.Add(mpChannel);
                        }
                    }
                }
                else
                {
                    SqlBuilder sb = new SqlBuilder(StatementType.Select, typeof(TvDatabase.RadioGroupMap));
                    sb.AddConstraint(Operator.Equals, "idGroup", groupId);
                    SqlResult result = Broker.Execute(sb.GetStatement());
                    List<TvDatabase.RadioGroupMap> groupMaps = (List<TvDatabase.RadioGroupMap>)
                        ObjectFactory.GetCollection(typeof(TvDatabase.RadioGroupMap), result, new List<TvDatabase.RadioGroupMap>());
                    foreach (TvDatabase.RadioGroupMap groupMap in groupMaps)
                    {
                        TvDatabase.Channel mpChannel = groupMap.ReferencedChannel();
                        if (mpChannel.IsRadio)
                        {
                            channels.Add(mpChannel);
                        }
                    }
                }
                channels.Sort(
                    delegate(TvDatabase.Channel c1, TvDatabase.Channel c2) { return c1.DisplayName.CompareTo(c2.DisplayName); });
            }

            return channels;
        }

        public static List<TvDatabase.Card> GetAllCards()
        {
            List<TvDatabase.Card> cards = new List<TvDatabase.Card>();
            IList<TvDatabase.Card> mediaPortalCards = TvDatabase.Card.ListAll();
            foreach (TvDatabase.Card card in mediaPortalCards)
            {
                if (card.Enabled
                    && !card.DevicePath.Equals("(builtin)", StringComparison.CurrentCultureIgnoreCase))
                {
                    cards.Add(card);
                }
            }
            return cards;
        }

        public static bool IsInSameHybridGroup(TvDatabase.Card card, List<int> otherCardGroupIds)
        {
            if (otherCardGroupIds.Count > 0)
            {
                List<int> cardGroupIds = GetHybridGroupIds(card);
                foreach (int cardGroupId in cardGroupIds)
                {
                    if (otherCardGroupIds.Contains(cardGroupId))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static List<int> GetHybridGroupIds(TvDatabase.Card card)
        {
            List<int> groupIds = new List<int>();
            if (card != null)
            {
                SqlBuilder sb = new SqlBuilder(StatementType.Select, typeof(TvDatabase.CardGroupMap));
                sb.AddConstraint(Operator.Equals, "idCard", card.IdCard);
                SqlResult result = Broker.Execute(sb.GetStatement());
                List<TvDatabase.CardGroupMap> groupMaps = (List<TvDatabase.CardGroupMap>)
                    ObjectFactory.GetCollection(typeof(TvDatabase.CardGroupMap), result, new List<TvDatabase.CardGroupMap>());
                foreach (TvDatabase.CardGroupMap groupMap in groupMaps)
                {
                    groupIds.Add(groupMap.IdCardGroup);
                }
            }
            return groupIds;
        }

        public static bool CardFreeOrUsingSameTransponder(TvDatabase.Card card, TvDatabase.Channel channel)
        {
            return CardFreeOrUsingSameTransponder(card, channel, null);
        }

        public static bool CardFreeOrUsingSameTransponder(TvDatabase.Card card, TvDatabase.Channel channel, IUser userToIgnore)
        {
            IUser[] cardUsers = TvServerPlugin.TvController_GetUsersForCard(card.IdCard);
            if (cardUsers != null)
            {
                TvDatabase.TuningDetail tuning = Utility.FindTuningDetailOnCard(channel, card.IdCard);

                HashSet<int> activeChannels = new HashSet<int>();

                foreach (IUser cardUser in cardUsers)
                {
                    if (userToIgnore == null
                        || cardUser.Name != userToIgnore.Name)
                    {
                        if (!cardUser.Name.Equals("epg", StringComparison.InvariantCultureIgnoreCase))
                        {
                            activeChannels.Add(cardUser.IdChannel);
                            if (!Utility.IsSameTransponder(card.IdCard, tuning, cardUser.IdChannel))
                            {
                                return false;
                            }
                        }
                    }
                }

                return activeChannels.Contains(channel.IdChannel)
                    || card.DecryptLimit == 0
                    || activeChannels.Count < card.DecryptLimit;
            }
            return true;
        }

        public static bool IsSameTransponder(int cardId, TvDatabase.TuningDetail tuning, int otherChannelId)
        {
            TvDatabase.Channel otherChannel = TvDatabase.Channel.Retrieve(otherChannelId);
            if (otherChannel != null)
            {
                return IsSameTransponder(cardId, tuning, otherChannel);
            }
            return false;
        }

        public static bool IsSameTransponder(int cardId, TvDatabase.TuningDetail tuning, TvDatabase.Channel otherChannel)
        {
            TvDatabase.TuningDetail otherTuning = Utility.FindTuningDetailOnCard(otherChannel, cardId);
            if (tuning != null
                && otherTuning != null)
            {
                if (tuning.ChannelType >= 2 && tuning.ChannelType <= 4 // DVB-x channel? ,no Atsc(==1)
                    && tuning.ChannelType == otherTuning.ChannelType
                    && tuning.Frequency == otherTuning.Frequency
                    && tuning.Symbolrate == otherTuning.Symbolrate
                    && tuning.Polarisation == otherTuning.Polarisation
                    && tuning.Bandwidth == otherTuning.Bandwidth
                    && tuning.Modulation == otherTuning.Modulation)
                {
                    return true;
                }
                if (tuning.ChannelType == 0 // Analog
                    && tuning.ChannelType == otherTuning.ChannelType
                    && tuning.IdChannel == otherTuning.IdChannel)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
