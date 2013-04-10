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
using System.Data.OleDb;

using ArgusTV.DataContracts;
using ArgusTV.GuideImporter.Interfaces;

namespace ArgusTV.GuideImporter.ClickFinder
{
    internal class DbReader
    {
        #region Private Members

        private bool _useShortDesc = false;
        private bool _addShortCritic = false;
        #endregion

        #region Public Methods

        public List<GuideProgram> GetGuideDataByClickFinderChannel(ImportGuideChannel clickFinderChannel)
        {            
            string sqlStatement = @"SELECT  Bezeichnung as Channel,
                                     Sendungen.Beginn as StartTime, 
                                     Sendungen.Ende as EndTime, 
                                     Sendungen.Titel as Title, 
                                     Sendungen.Originaltitel as SubTitle, 
                                     Sendungen.KurzBeschreibung as ShortDescription, 
                                     SendungenDetails.Beschreibung as Description, 
                                     Sendungen.Kategorietext, 
                                     Sendungen.Genre as Category, 
                                     Sendungen.KzWiederholung as IsRepeat, 
                                     Sendungen.Kz16zu9 as ImageFormat16to9,
                                     Sendungen.Kurzkritik as ShortCritic,
                                     Sendungen.Bewertungen as Rating,  
                                     Sendungen.Bewertung as StarRating, 
                                     Sendungen.Regie as Director, 
                                     SendungenDetails.Darsteller as Actors
                                    FROM Sender, Sendungen, SendungenDetails
                                    WHERE Sender.SenderKennung=Sendungen.SenderKennung AND Sendungen.Pos=SendungenDetails.Pos AND Sender.Bezeichnung ='{0}'  
                                     AND Sendungen.Ende>=Now() AND Sender.GueltigAb < Now() AND Sender.GueltigBis > Now() 
                                    ORDER BY Sender.Bezeichnung, Sendungen.Beginn, Sendungen.Ende";

            #region SQL without time filter
            /*
            string sqlStatement = @"SELECT  Bezeichnung as Channel,
                                     Sendungen.Beginn as StartTime, 
                                     Sendungen.Ende as EndTime, 
                                     Sendungen.Titel as Title, 
                                     Sendungen.Originaltitel as SubTitle, 
                                     Sendungen.KurzBeschreibung as ShortDescription, 
                                     SendungenDetails.Beschreibung as Description, 
                                     Sendungen.Kategorietext as Category, 
                                     Sendungen.Genre, 
                                     Sendungen.KzWiederholung as IsRepeat, 
                                     Sendungen.Kz16zu9 as ImageFormat16to9, 
                                     Sendungen.Kurzkritik as ShortCritic, 
                                     Sendungen.Bewertungen as Rating,  
                                     Sendungen.Bewertung as StarRating, 
                                     Sendungen.Regie as Director, 
                                     SendungenDetails.Darsteller as Actors
                                    FROM Sender, Sendungen, SendungenDetails
                                    WHERE Sender.SenderKennung=Sendungen.SenderKennung AND Sendungen.Pos=SendungenDetails.Pos AND Sender.Bezeichnung ='{0}'  
                                    ORDER BY Sender.Bezeichnung, Sendungen.Beginn, Sendungen.Ende";
            */
            #endregion

            sqlStatement = string.Format(sqlStatement, clickFinderChannel.ChannelName);
            
            List<GuideProgram> guideProgramList = new List<GuideProgram>();
            //try
            //{
                using (OleDbConnection dbConnection = new OleDbConnection(ConfigInstance.Current.ClickFinderConnectionString))
                {
                    dbConnection.Open();
                    using (OleDbCommand dbCommand = new OleDbCommand(sqlStatement, dbConnection))
                    {
                        using (OleDbDataReader dbReader = dbCommand.ExecuteReader())
                        {
                            while (dbReader.Read())
                            {
                                GuideProgram guideProgram = FillGuideProgram(dbReader);
                                if (guideProgram != null)
                                {
                                    guideProgramList.Add(guideProgram);
                                }
                            }
                        }
                    }
                }
            //}
            //catch { }

            return guideProgramList;
        }

        public static bool SafeIsValidConnectionString(string clickFinderConnectionString)
        {
            try
            {
                return IsValidConnectionString(clickFinderConnectionString);
            }
            catch { }
            
            return false;
        }

        public static bool IsValidConnectionString(string clickFinderConnectionString)
        {
            bool isValid = false;
            
            string sqlSelect = "SELECT Sender.Bezeichnung as channelName, Sender.ID as externalId FROM Sender ORDER BY Sender.Bezeichnung;";

            using (OleDbConnection dbConnection = new OleDbConnection(clickFinderConnectionString))
            {
                dbConnection.Open();
                using (OleDbCommand dbCommand = new OleDbCommand(sqlSelect, dbConnection))
                {
                    dbCommand.ExecuteReader();
                    isValid = true;
                }
            }
            return isValid;
        }

        public List<ImportGuideChannel> GetAllImportChannels()
        {
            List<ImportGuideChannel> channelList = new List<ImportGuideChannel>();

            string sqlSelect = "SELECT Sender.Bezeichnung as channelName, Sender.ID as externalId FROM Sender ORDER BY Sender.Bezeichnung;";

            //try
            //{
                using (OleDbConnection dbConnection = new OleDbConnection(ConfigInstance.Current.ClickFinderConnectionString))
                {
                    dbConnection.Open();
                    using (OleDbCommand dbCommand = new OleDbCommand(sqlSelect, dbConnection))
                    {
                        OleDbDataReader dbReader = dbCommand.ExecuteReader();
                        while (dbReader.Read())
                        {
                            string channelName = dbReader["channelName"].ToString();
                            string externalId = dbReader["externalId"].ToString();
                            if(String.IsNullOrEmpty(externalId))
                            {
                                externalId = channelName;
                            }
                            ImportGuideChannel channel = new ImportGuideChannel(channelName, externalId);
                            channelList.Add(channel);
                        }
                    }
                }
            //}
            //catch {}

            return channelList;
        }
        #endregion

        #region Private Methods

        private GuideProgram FillGuideProgram(OleDbDataReader dataReader)
        {
            GuideProgram guideProgram = new GuideProgram();
            //try
            //{
                guideProgram.StartTime = DateTime.Parse(dataReader["StartTime"].ToString());
                guideProgram.StartTimeUtc = guideProgram.StartTime.ToUniversalTime();
                guideProgram.StopTime = DateTime.Parse(dataReader["EndTime"].ToString());
                guideProgram.StopTimeUtc = guideProgram.StopTime.ToUniversalTime();

                if (guideProgram.StartTimeUtc >= guideProgram.StopTimeUtc)
                {
                    return null;
                }

                guideProgram.Title = dataReader["Title"].ToString();
                string subTitle = dataReader["SubTitle"].ToString();
                if (!String.IsNullOrEmpty(subTitle) && !subTitle.Equals(guideProgram.Title))
                {
                    guideProgram.SubTitle = subTitle;
                }

                if (_useShortDesc)
                {
                    string shortDescription = dataReader["ShortDescription"].ToString().Replace("<br>", " ");
                    guideProgram.Description = TrimAndCutString(shortDescription, 3999);
                }
                else
                {
                    string description = dataReader["Description"].ToString().Replace("<br>", " ");
                    guideProgram.Description = TrimAndCutString(description, 4000);
                }

                guideProgram.Actors = BuildCredits(dataReader["Actors"].ToString());
                guideProgram.Directors = BuildCredits(dataReader["Director"].ToString());

                if (!String.IsNullOrEmpty(dataReader["Rating"].ToString()))
                {
                    guideProgram.Rating = BuildRating(dataReader["Rating"].ToString());               
                }
                if (_addShortCritic && !String.IsNullOrEmpty(dataReader["ShortCritic"].ToString()))
                {
                    guideProgram.Rating = guideProgram.Rating + ", " + dataReader["ShortCritic"].ToString();
                }
                if (!String.IsNullOrEmpty(guideProgram.Rating))
                {
                    guideProgram.Rating = TrimAndCutString(guideProgram.Rating, 49);
                }

                guideProgram.StarRating = 0;
                try
                {
                    guideProgram.StarRating = Convert.ToSingle(dataReader["StarRating"].ToString()) / 6;
                }
                catch { }

                if (dataReader["ImageFormat16to9"] != null && (bool)dataReader["ImageFormat16to9"])
                {
                    guideProgram.Flags |= GuideProgramFlags.WidescreenAspectRatio;
                }

                guideProgram.IsRepeat = false;
                if (dataReader["IsRepeat"] != null && (bool)dataReader["IsRepeat"] )
                {
                    guideProgram.IsRepeat = true;
                }
                guideProgram.Category = TrimAndCutString(dataReader["Category"].ToString(), 49);
            //}
            //catch { }

            return guideProgram;
        }

        private string[] BuildCredits(string credits)
        {
            List<string> result = new List<string>();
            string[] persons = credits.Split(';');
            foreach (string person in persons)
            {
                string credit = person.Trim();
                if (!String.IsNullOrEmpty(credit))
                {
                    result.Add(credit);
                }
            }
            return result.ToArray();
        }

        private string BuildRating(string incomingRating)
        {
            string result = String.Empty;
            if (!String.IsNullOrEmpty(incomingRating))
            {
                string[] ratings = incomingRating.Split(';');
                if (ratings.Length > 0)
                {
                    List<string> acceptedRatings = new List<string>();
                    foreach (string rating in ratings)
                    {
                        string[] tmp = rating.Split('=');
                        if (tmp.Length == 2)
                        {
                            tmp[0] = tmp[0].Trim();
                            tmp[1] = tmp[1].Trim();

                            if(!tmp[1].Equals("0") && !acceptedRatings.Contains(tmp[0]))
                            {
                                acceptedRatings.Add(tmp[0]);

                                if (result.Length > 0)
                                {
                                    result = result + ",";
                                }
                                result = result + rating;
                            }
                        }
                    }
                }
                else if (!incomingRating.Trim().EndsWith("=0"))
                {
                    result = incomingRating;
                }
            }
            return result;
        }

        private string TrimAndCutString(string content, int trimLength)
        {
            if (!String.IsNullOrEmpty(content))
            {
                content = content.Trim();
                if (trimLength < content.Length)
                {
                    content = content.Substring(0, trimLength);
                }
            }
            return content;
        }
        #endregion
    }
}
