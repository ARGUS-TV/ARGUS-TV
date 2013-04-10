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
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Diagnostics;

using SchedulesDirect;

using ArgusTV.Common.Logging;
using ArgusTV.DataContracts;
using ArgusTV.GuideImporter.Interfaces;
using ArgusTV.GuideImporter.SchedulesDirect.Entities;
using System.Text.RegularExpressions;

namespace ArgusTV.GuideImporter.SchedulesDirect
{
    public class GuideImporter : IGuideImportPlugin
    {
        #region Private Members

        private const string _name = "SchedulesDirect";
        private const string _description = "Imports Schedules Direct guide data into ARGUS TV";

        private string _installationPath;
        #endregion

        #region IGuideImport Members

        public string Name
        {
            get 
            {
                if (!String.IsNullOrEmpty(ConfigInstance.Current.PluginName))
                {
                    return ConfigInstance.Current.PluginName;
                }
                return _name; 
            }
        }

        public string Description
        {
            get { return _description; }
        }

        public string InstallationPath
        {
            get { return _installationPath; }
            set 
            { 
                _installationPath = value;
                ConfigInstance.Load(ConfigFileName);
            }
        }

        public string ConfigFileName
        {
            get
            {
                return Path.Combine(InstallationPath, "ArgusTV.GuideImporter.SchedulesDirect.dll.config");
            }
        }

        public void ShowConfigurationDialog(Form parentDialog)
        {
            ConfigurationForm configurationForm = new ConfigurationForm();
            configurationForm.ShowDialog(parentDialog);
        }

        public bool IsConfigured()
        {
            return !String.IsNullOrEmpty(ConfigInstance.Current.SDUserName) && !String.IsNullOrEmpty(ConfigInstance.Current.SDPassword);
        }

        public List<ImportGuideChannel> GetAllImportChannels(bool reload, ProgressCallback progressCallback, FeedbackCallback feedbackCallback)
        {
            if (reload)
            {
                GiveFeedback(feedbackCallback, "Calling SchedulesDirect WebService ...");
                xtvdResponse response = CallSchedulesDirectWebService(ConvertStartDateTimeToString(DateTime.Now), ConvertEndDateTimeToString(DateTime.Now));
                if (response != null)
                {
                    GiveFeedback(feedbackCallback, "Processing retrieved SchedulesDirect channels ...");

                    Dictionary<int, ImportGuideChannel> tvGuideChannels = BuildImportGuideChannelDictionary(response, ConfigInstance.Current.ChannelNameFormat);
                    GuideChannelStore.Save(AvailableChannelsConfigFile, new List<ImportGuideChannel>(tvGuideChannels.Values));
                }
            }

            List<ImportGuideChannel> availableGuideChannels = GuideChannelStore.Load(AvailableChannelsConfigFile);
            return availableGuideChannels;
        }

        public void PrepareImport(FeedbackCallback feedbackCallback, KeepImportServiceAliveCallback keepImportServiceAliveCallback)
        {          
        }

        public void Import(List<ImportGuideChannel> skipChannels, ImportDataCallback importDataCallback, ProgressCallback progressCallback, FeedbackCallback feedbackCallback, KeepImportServiceAliveCallback keepImportServiceAliveCallback)
        {
            keepImportServiceAliveCallback();
            DateTime todeey = DateTime.Now;

            DateTime startDate = todeey;
            DateTime endDate = todeey.AddDays(ConfigInstance.Current.NrOfDaysToImport);

            if (ConfigInstance.Current.UpdateMode == Config.UpDateMode.NextDayOnly)
            {
                endDate = startDate;
                DateTime lastImportedDay = ConvertIntAsDateTime(ConfigInstance.Current.LastImportedDay);

                if (lastImportedDay > startDate)
                {
                    Logger.Write(FormatForLogger(DateTime.Now.ToLongTimeString() + " : Calling Import for updates for one day : " + startDate.ToLongTimeString()));

                    Import(skipChannels, importDataCallback, progressCallback, feedbackCallback, keepImportServiceAliveCallback, ConvertStartDateTimeToString(startDate, true), ConvertEndDateTimeToString(startDate.AddHours(25), true));
                    Logger.Write(FormatForLogger(DateTime.Now.ToLongTimeString() + " : End of one day Import"));

                    startDate = lastImportedDay;
                }
                
                endDate = todeey.AddDays(ConfigInstance.Current.NrOfDaysToImport);
                if (endDate > startDate)                
                {
                    Logger.Write(FormatForLogger(DateTime.Now.ToLongTimeString() + " : Calling Import for new data : " + startDate.ToLongTimeString()));

                    Import(skipChannels, importDataCallback, progressCallback, feedbackCallback, keepImportServiceAliveCallback, ConvertStartDateTimeToString(startDate), ConvertEndDateTimeToString(endDate));
                }
            }
            else
            {
                Import(skipChannels, importDataCallback, progressCallback, feedbackCallback, keepImportServiceAliveCallback, ConvertStartDateTimeToString(startDate), ConvertEndDateTimeToString(endDate));
            }
            Logger.Write(FormatForLogger(DateTime.Now.ToLongTimeString() + " : End of new data Import"));

            ConfigInstance.Current.LastImportedDay = ConvertDateTimeAsDateInt(endDate);
            ConfigInstance.Save();
        }
        #endregion

        private void Import(List<ImportGuideChannel> skipChannels, 
                           ImportDataCallback importDataCallback, 
                           ProgressCallback progressCallback, 
                           FeedbackCallback feedbackCallback, 
                           KeepImportServiceAliveCallback keepImportServiceAliveCallback,
                           string startDate, string endDate )
        {
            GiveFeedback(feedbackCallback, "Calling SchedulesDirect WebService ...");

            if (progressCallback != null)
            {
                progressCallback(0);
            }
            
            keepImportServiceAliveCallback();

            xtvdResponse response = CallSchedulesDirectWebService(startDate, endDate);
            if (response != null)
            {
                GiveFeedback(feedbackCallback, "Processing SchedulesDirect data ...");

                //DataDumper.DumpResponse(response, @"d:\steph\temp\response.txt");
                keepImportServiceAliveCallback();

                // Process results, create dictionaries for easier access

                // Stations
                Dictionary<int, ImportGuideChannel> guideChannels = BuildImportGuideChannelDictionary(response, ConfigInstance.Current.ChannelNameFormat);
                GuideChannelStore.Save(AvailableChannelsConfigFile, new List<ImportGuideChannel>(guideChannels.Values));

                // Programs
                Dictionary<string, Program> programs = BuildProgramsDictionary(response);

                // Add genres to programs
                AddGenresToPrograms(response, programs);

                // Add actors and directors to programs
                AddActorsAndDirectorsToPrograms(response, programs);

                // Start the real import :
                List<GuideProgram> guideData = new List<GuideProgram>();
                ImportGuideChannel currentGuideChannel = new ImportGuideChannel(String.Empty, String.Empty);
                int importedCount = 0;
                int procentDone = 0;

                foreach (schedulesSchedule schedule in response.xtvd.schedules)
                {
                    if (guideChannels.ContainsKey(schedule.station) && !skipChannels.Contains(guideChannels[schedule.station]))
                    {
                        if (!currentGuideChannel.ExternalId.Equals(schedule.station.ToString()))
                        {
                            if (guideData.Count > 0)
                            {
                                GiveFeedback(feedbackCallback, String.Format("Importing channel {0}, {1} schedule(s).", currentGuideChannel.ChannelName, guideData.Count));

                                importDataCallback(currentGuideChannel, ChannelType.Television, guideData.ToArray(), ConfigInstance.Current.UpdateChannelNames);
                                guideData = new List<GuideProgram>();
                            }
                            currentGuideChannel = guideChannels[schedule.station];
                        }
                        if (programs.ContainsKey(schedule.program))
                        {
                            GuideProgram guideProgram = FillGuideProgram(schedule, programs[schedule.program]);
                            if (guideProgram != null)
                            {
                                guideData.Add(guideProgram);
                            }
                            else
                            {
                                Logger.Write(FormatForLogger("FillGuideProgram returned null for schedule program : " + schedule.program));
                            }
                        }
                        else
                        {
                            Logger.Write(FormatForLogger("FillGuideProgram : unknown schedule program : " + schedule.program));
                        }
                    }
                    importedCount++;

                    int currentProcentDone = ((100 * importedCount) / response.xtvd.schedules.Length);
                    if (currentProcentDone != procentDone)
                    {
                        procentDone = currentProcentDone;
                        if (progressCallback != null)
                        {
                            progressCallback(procentDone);
                        }
                        else if ((procentDone % 5) == 0)
                        {
                            Logger.Write(FormatForLogger(String.Format("Progress : {0} % done.", procentDone)));
                        }
                    }
                }
                if (guideData.Count > 0)
                {
                    importDataCallback(currentGuideChannel, ChannelType.Television, guideData.ToArray(), ConfigInstance.Current.UpdateChannelNames);
                }
            }
        }

        private xtvdResponse CallSchedulesDirectWebService(string startTime, string endTime)
        {
            try
            {
                xtvdWebService xtvdWebService = new xtvdWebService();
                xtvdWebService.EnableDecompression = true;
                xtvdWebService.UserAgent = String.Format("ARGUS TV Version {0}", Constants.ProductVersion);
                xtvdWebService.Credentials = new NetworkCredential(ConfigInstance.Current.SDUserName, ConfigInstance.Current.SDPassword);
                //xtvdWebService.Timeout = 2000;
                //#if DEBUG
                // xtvdWebService.Credentials = new NetworkCredential("argustv", "password");
                //#endif
                Logger.Write(TraceEventType.Verbose, FormatForLogger("Begin call to WS"));
                xtvdResponse response = xtvdWebService.download(startTime, endTime);
                Logger.Write(TraceEventType.Verbose, FormatForLogger("End call to WS"));

                return response;
            }
            catch (Exception ex)
            {
                Logger.Write(FormatForLogger(String.Format("Exception calling scheduleDirect webservice, message : {0} .", ex.Message)));
            }
            return null;
        }

        private string AvailableChannelsConfigFile
        {
            get
            {
                return Path.Combine(InstallationPath, "AvailableChannels.config");
            }
        }

        private Dictionary<int, int> BuildGuideChannelLcnDictionary(xtvdResponse response)
        {
            Dictionary<int, int> guideChannelLcns = new Dictionary<int, int>();

            foreach (lineupsLineup lineUps in response.xtvd.lineups)
            {
                foreach (lineupsLineupMap lineUpMap in lineUps.map)
                {
                    if (!guideChannelLcns.ContainsKey(lineUpMap.station))
                    {
                        int lcn;
                        if (int.TryParse(lineUpMap.channel, out lcn))
                        {
                            guideChannelLcns.Add(lineUpMap.station, lcn);
                        }
                    }
                }
            }
            return guideChannelLcns;
        }

        private Dictionary<int, ImportGuideChannel> BuildImportGuideChannelDictionary(xtvdResponse response, string channelNameFormat)
        {
            // StationLCNs
            Dictionary<int, int> guideChannelsLcn = BuildGuideChannelLcnDictionary(response);

            Dictionary<int, ImportGuideChannel> guideChannels = new Dictionary<int, ImportGuideChannel>();
            foreach (stationsStation station in response.xtvd.stations)
            {
                if (!guideChannels.ContainsKey(station.id))
                {
                    ImportGuideChannel importGuideChannel = new ImportGuideChannel();
                    importGuideChannel.ExternalId = station.id.ToString();

                    string channelName = channelNameFormat.ToLowerInvariant();

                    channelName = channelName.Replace("{callsign}", station.callSign);
                    channelName = channelName.Replace("{name}", station.name);
                    channelName = channelName.Replace("{affiliate}", station.affiliate);
                    if (guideChannelsLcn.ContainsKey(station.id))
                    {
                        importGuideChannel.LogicalChannelNumber = guideChannelsLcn[station.id];
                        //string lcn3Digits = String.Format("{0:D3}", guideChannelsLcn[station.id]);
                        channelName = channelName.Replace("{logicalchannelnumber}", guideChannelsLcn[station.id].ToString());
                    }
                    else
                    {
                        channelName = channelName.Replace("{logicalchannelnumber}", String.Empty);                    
                    }
                    importGuideChannel.ChannelName = channelName;

                    guideChannels.Add(station.id, importGuideChannel); 
                }
            }
            return guideChannels;
        }

        private Dictionary<string, Program> BuildProgramsDictionary(xtvdResponse response)
        {
            Dictionary<string, Program> programs = new Dictionary<string, Program>();
            foreach (programsProgram program in response.xtvd.programs)
            {
                if (!programs.ContainsKey(program.id) )
                {
                    programs.Add(program.id, new Program(program));
                }
            }
            return programs;
        }

        private void AddGenresToPrograms(xtvdResponse response, Dictionary<string, Program> programs)
        {
            foreach (genresProgramGenre genresProgramGenre in response.xtvd.genres)
            {
                if (programs.ContainsKey(genresProgramGenre.program))
                {
                    genresProgramGenreGenre[] programGenres = genresProgramGenre.genre;
                    int highestRelevance = -1;

                    for (int i = programGenres.Length - 1; i >= 0; i--)
                    {
                        if (programGenres[i].relevance > highestRelevance && !String.IsNullOrEmpty(programGenres[i].@class))
                        {
                            highestRelevance = programGenres[i].relevance;
                            programs[genresProgramGenre.program].Genre = programGenres[i].@class;
                        }
                    }
                }
            }        
        }

        private void AddActorsAndDirectorsToPrograms(xtvdResponse response, Dictionary<string, Program> programs)
        {
            foreach (productionCrewCrew productionCrew in response.xtvd.productionCrew)
            {
                if (programs.ContainsKey(productionCrew.program))
                {
                    try
                    {
                        foreach (crewMember crewMember in productionCrew.member)
                        {
                            if (String.IsNullOrEmpty(crewMember.role))
                                continue;

                            if (crewMember.role.Equals("Actor", StringComparison.InvariantCultureIgnoreCase))
                            {
                                string actorName = String.Format("{0} {1}", crewMember.givenname, crewMember.surname);
                                programs[productionCrew.program].AddActor(actorName.Trim());
                            }
                            else if (crewMember.role.Equals("Director", StringComparison.InvariantCultureIgnoreCase))
                            {
                                string directorName = String.Format("{0} {1}", crewMember.givenname, crewMember.surname);
                                programs[productionCrew.program].AddDirector(directorName.Trim());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(FormatForLogger(String.Format("Exception in AddActorsAndDirectorsToPrograms, message : {0} .", ex.Message)));
                    }
                }
            }
        }

        private GuideProgram FillGuideProgram(schedulesSchedule schedule, Program program)
        {
            GuideProgram guideProgram = new GuideProgram();
            try
            {
                guideProgram.StartTime = schedule.time;
                guideProgram.StartTimeUtc = schedule.time.ToUniversalTime();

                TimeSpan duration = ParseDuration(schedule.duration);
                guideProgram.StopTime = guideProgram.StartTime.Add(duration);
                guideProgram.StopTimeUtc = guideProgram.StartTimeUtc.Add(duration);

                if (guideProgram.StartTimeUtc >= guideProgram.StopTimeUtc)
                {
                    return null;
                }
                AdaptGuideProgramTimeToLocalTimeZone(guideProgram);

                guideProgram.Title = program.Title;
                guideProgram.SubTitle = program.SubTitle;
                guideProgram.Description = TrimAndCutString(program.Description, 3999);

                guideProgram.Actors = program.Actors;
                guideProgram.Directors = program.Directors;

                if (schedule.tvRatingSpecified)
                {
                    guideProgram.Rating = schedule.tvRating.ToString();
                }
                
                guideProgram.StarRating = program.StarRating;

                guideProgram.IsRepeat = false;
                guideProgram.IsPremiere = false;
                if(schedule.hdtv )
                    guideProgram.Flags |= GuideProgramFlags.HighDefinition;

                // If the tvScheduleItem says it is a repeat or new, we believe them
                // MV = movies, EP = episode, SH = show, SP = sports
                if (schedule.repeat || schedule.@new )
                {
                    guideProgram.IsPremiere = schedule.@new;
                    guideProgram.IsRepeat = !guideProgram.IsPremiere;
                }
                else if(program.Id.StartsWith("EP") && 
                        program.OriginalAirDate.HasValue &&
                        guideProgram.StartTime.Subtract(program.OriginalAirDate.Value).TotalDays > 60 )
                {                    
                    guideProgram.IsRepeat = true;
                }
                else if(program.Id.StartsWith("SH") &&
                        program.OriginalAirDate.HasValue &&
                        guideProgram.StartTime.Subtract(program.OriginalAirDate.Value).TotalDays > 60 &&
                        program.ShowType != null && (program.ShowType.Equals("Short Film") || program.ShowType.Equals("Special")))                
                {
                    guideProgram.IsRepeat = true;
                }
                else if (program.Id.StartsWith("SP")) // Sports -> always new.
                {
                    guideProgram.IsPremiere = true;
                }

                if (!ParseEpisodeAndSeriesIds(program.Id, guideProgram))
                {
                    guideProgram.EpisodeNumber = program.EpisodeNumber;                
                }

                if (guideProgram.EpisodeNumber.HasValue)
                {
                    guideProgram.EpisodeNumberDisplay = guideProgram.EpisodeNumber.ToString();
                }

                guideProgram.Category = TrimAndCutString(program.Genre, 49);
                
                guideProgram.PreviouslyAiredTime = program.OriginalAirDate;
            }
            catch(Exception ex)
            {
                Logger.Write(TraceEventType.Error, "FillGuideProgram exception : " + ex.Message);
            }

            return guideProgram;
        }

        private TimeSpan ParseDuration(string duration)
        {
            TimeSpan timespan = new TimeSpan();

            //Pattern = PT00H30M
            Regex durationExpression = new Regex("^PT([0-9]{2})H([0-9]{2})M$");
            Match match = durationExpression.Match(duration);

            if (match.Success && match.Groups.Count == 3 )            
            {
                int hours = 0;
                int minutes = 0;
                
                if (int.TryParse(match.Groups[1].Value, out hours) && int.TryParse(match.Groups[2].Value, out minutes))
                {
                    timespan = new TimeSpan(hours, minutes, 0);
                }
            }
            return timespan;
        }

        // For shows beginning with EP, the next 8 digits represent the series ID, with the last 4 digits representing the episode id.
        private bool ParseEpisodeAndSeriesIds(string programId, GuideProgram guideProgram)
        {
            guideProgram.SeriesNumber = null;
            guideProgram.EpisodeNumber = null;

            bool hasEpisodeAndSeriesInfo = false;
            if(programId.StartsWith("EP") && programId.Length >= 12 )
            {
                string seriesIdString = programId.Substring(2, 8);
                string episodeIdString = programId.Substring(programId.Length - 4);

                int tmp;
                if(int.TryParse(seriesIdString, out tmp))
                {
                    guideProgram.SeriesNumber = tmp;
                }                
                if(int.TryParse(episodeIdString, out tmp))
                {
                    guideProgram.EpisodeNumber = tmp;
                }                                
                hasEpisodeAndSeriesInfo = true;
            }
            return hasEpisodeAndSeriesInfo;
        }

        private int ConvertDateTimeAsDateInt(DateTime dateTime)
        {
            return dateTime.Year * 10000 + dateTime.Month * 100 + dateTime.Day;
        }

        private string ConvertStartDateTimeToString(DateTime startDate)
        {
            return ConvertDateTimeToString(startDate, 0, 0, 0);
        }

        private string ConvertStartDateTimeToString(DateTime startDate, bool includeTime)
        {
            if (includeTime)
            {
                return ConvertDateTimeToString(startDate, startDate.Hour, startDate.Minute, startDate.Second);              
            }
            return ConvertDateTimeToString(startDate, 0, 0, 0); 
        }

        private string ConvertEndDateTimeToString(DateTime endDate)
        {
            return ConvertDateTimeToString(endDate, 23, 59, 59);
        }

        private string ConvertEndDateTimeToString(DateTime endDate, bool includeTime)
        {
            if (includeTime)
            {
                return ConvertDateTimeToString(endDate, endDate.Hour, endDate.Minute, endDate.Second);
            }
            return ConvertDateTimeToString(endDate, 23, 59, 59);
        }

        private string ConvertDateTimeToString(DateTime date, int hour, int minute, int second)
        {
            return String.Format("{0:yyyy-MM-dd}T{1:D2}:{2:D2}:{3:D2}Z", date, hour, minute, second);
        }

        private DateTime ConvertIntAsDateTime(int dateTime)
        { 
            int year = dateTime / 10000;
            int month = (dateTime % (year * 10000)) / 100;
            int day = (dateTime % (year * 10000)) - (month * 100);
            return new DateTime(year, month, day);
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

        private void AdaptGuideProgramTimeToLocalTimeZone(GuideProgram guideProgram)
        {
            guideProgram.StartTime = AdjustGmtToLocal(guideProgram.StartTime);
            guideProgram.StopTime = AdjustGmtToLocal(guideProgram.StopTime);
        }

        private DateTime AdjustGmtToLocal(DateTime dateTime)
        {
            // Get local timezone for target date.
            TimeSpan localOffset = dateTime.ToLocalTime() - dateTime;

            // Get specified timezone offset, for SchedulesDirect this 0 (GMT time).
            TimeSpan offset = TimeSpan.Zero;

            // Adjust timezone by the difference.
            dateTime += localOffset - offset;

            return dateTime;
        }

        private void GiveFeedback(FeedbackCallback feedbackCallback, string message)
        {
            Logger.Write(FormatForLogger(message));

            if(feedbackCallback != null)
            {
                feedbackCallback(message);
            }
        }

        private string FormatForLogger(string message)
        {
            return String.Format("{0} {1}", Name, message);
        }
    }   
}