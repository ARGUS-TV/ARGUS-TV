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
using System.IO;
using System.Diagnostics;

using SchedulesDirect;

namespace ArgusTV.GuideImporter.SchedulesDirect
{
    internal static class DataDumper
    {
        public static void DumpResponse(xtvdResponse response, string fileName)
        {
            string fileNamePrefix = fileName.Substring(0, fileName.LastIndexOf('.'));
            string fileExtension = Path.GetExtension(fileName);
            
            WritePrograms(response, String.Format("{0}{1}.{2}", fileNamePrefix, "_programs", fileExtension) );
            WriteSchedules(response, String.Format("{0}{1}.{2}", fileNamePrefix, "_schedules", fileExtension));
            WriteStations(response, String.Format("{0}{1}.{2}", fileNamePrefix, "_stations", fileExtension));
            WriteGenres(response, String.Format("{0}{1}.{2}", fileNamePrefix, "_genres", fileExtension));
            WriteLineUps(response, String.Format("{0}{1}.{2}", fileNamePrefix, "_lineups", fileExtension));
        }

        #region Programs

        private static void WritePrograms(xtvdResponse response, string filename)
        {
            Debug.WriteLine(String.Format("Writing programs to : {0}", filename));
            File.Delete(filename);

            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.WriteLine("Id;Title;SubTitle;StarRating;ShowType;Series;ColorCodeSpecified;ColorCode;MpaaRatingSpecified;MpaaRating;OriginalAirDateSpecified;OriginalAirDate;SyndicatedEpisodeNumber;Year;Description");
                try
                {
                    foreach (programsProgram programsProgram in response.xtvd.programs)
                    {
                        sw.WriteLine(String.Format("'{0}';'{1}';'{2}';'{3}';'{4}';'{5}';'{6}';'{7}';'{8}';'{9}';'{10}';'{11}';'{12}';'{13}';'{14}';",
                                                programsProgram.id,
                                                String.IsNullOrEmpty(programsProgram.title) ? "-" : CleanContent(programsProgram.title),
                                                String.IsNullOrEmpty(programsProgram.subtitle) ? "-" : CleanContent(programsProgram.subtitle),
                                                String.IsNullOrEmpty(programsProgram.starRating.ToString()) ? "-" : programsProgram.starRating.ToString(),
                                                String.IsNullOrEmpty(programsProgram.showType) ? "-" : programsProgram.showType,
                                                String.IsNullOrEmpty(programsProgram.series) ? "-" : programsProgram.series,
                                                programsProgram.colorCodeSpecified.ToString(),
                                                programsProgram.colorCode.ToString(),
                                                programsProgram.mpaaRatingSpecified.ToString(),
                                                programsProgram.mpaaRating.ToString(),
                                                programsProgram.originalAirDateSpecified.ToString(),
                                                programsProgram.originalAirDate.ToString(),
                                                String.IsNullOrEmpty(programsProgram.syndicatedEpisodeNumber) ? "-" : programsProgram.syndicatedEpisodeNumber,
                                                String.IsNullOrEmpty(programsProgram.year) ? "-" : programsProgram.year,
                                                String.IsNullOrEmpty(programsProgram.description) ? "-" : CleanContent(programsProgram.description)));

                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(String.Format("An exception occured : {0}", ex.Message));
                }
            }
            Debug.WriteLine("Done writing programs");
        }
        #endregion

        #region Schedules

        private static void WriteSchedules(xtvdResponse response, string filename)
        {
            Debug.WriteLine(String.Format("Writing schedules to : {0}", filename));
            File.Delete(filename);
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.WriteLine("Program;Station;CloseCaptionedSpecified;CloseCaptioned;DolbySpecified;Dolby;Duration;EiSpecified;Ei;HDTVSpecified;HDTV;NewSpecified;New;Part;RepeatSpecified;Repeat;StereoSpecified;Stereo;SubtitledSpecified;Subtitled;TvRatingSpecified;TvRating;Time");
                try
                {
                    foreach (schedulesSchedule schedulesSchedule in response.xtvd.schedules)
                    {
                        sw.WriteLine(String.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20};{21};{22}",
                                                String.IsNullOrEmpty(schedulesSchedule.program) ? "-" : schedulesSchedule.program,
                                                schedulesSchedule.station,
                                                schedulesSchedule.closeCaptionedSpecified.ToString(),
                                                schedulesSchedule.closeCaptioned.ToString(),
                                                schedulesSchedule.dolbySpecified.ToString(),
                                                schedulesSchedule.dolby.ToString(),
                                                String.IsNullOrEmpty(schedulesSchedule.duration) ? "-" : schedulesSchedule.duration,
                                                schedulesSchedule.eiSpecified.ToString(),
                                                schedulesSchedule.ei.ToString(),
                                                schedulesSchedule.hdtvSpecified.ToString(),
                                                schedulesSchedule.hdtv.ToString(),
                                                schedulesSchedule.newSpecified.ToString(),
                                                schedulesSchedule.@new.ToString(),
                                                schedulesSchedule.part != null ? schedulesSchedule.part.number.ToString() + "/" + schedulesSchedule.part.total.ToString() : "-",
                                                schedulesSchedule.repeatSpecified.ToString(),
                                                schedulesSchedule.repeat.ToString(),
                                                schedulesSchedule.stereoSpecified.ToString(),
                                                schedulesSchedule.stereo.ToString(),
                                                schedulesSchedule.subtitledSpecified.ToString(),
                                                schedulesSchedule.subtitled.ToString(),
                                                schedulesSchedule.tvRatingSpecified.ToString(),
                                                schedulesSchedule.tvRating.ToString(),
                                                schedulesSchedule.time.ToLocalTime()));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(String.Format("An exception occured : {0}", ex.Message));
                }
            }
            Debug.WriteLine("Done writing schedules");
        }
        #endregion

        #region Stations

        private static void WriteStations(xtvdResponse response, string filename)
        {
            Debug.WriteLine(String.Format("Writing stations to : {0}", filename));
            File.Delete(filename);
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.WriteLine("Id;Name;FccChanNbr;CallSign;Affiliate");
                try
                {
                    foreach (stationsStation station in response.xtvd.stations)
                    {
                        sw.WriteLine(String.Format("{0};{1};{2};{3};{4}",
                                                station.id,
                                                String.IsNullOrEmpty(station.name) ? "-" : station.name,
                                                String.IsNullOrEmpty(station.fccChannelNumber) ? "-" : station.fccChannelNumber,
                                                String.IsNullOrEmpty(station.callSign) ? "-" : station.callSign,
                                                String.IsNullOrEmpty(station.affiliate) ? "-" : station.affiliate));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(String.Format("An exception occured : {0}", ex.Message));
                }
            }
            Debug.WriteLine("Done writing stations");
        }
        #endregion

        #region Genres

        private static void WriteGenres(xtvdResponse response, string filename)
        {
            Debug.WriteLine(String.Format("Writing genres to : {0}", filename));
            File.Delete(filename);
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.WriteLine("Program;GenreClass;GenreRelevance");
                try
                {
                    foreach (genresProgramGenre genresProgramGenre in response.xtvd.genres)
                    {
                        genresProgramGenreGenre[] genreGenres = genresProgramGenre.genre;
                        foreach (genresProgramGenreGenre genresProgramGenreGenre in genreGenres)
                        {
                            sw.WriteLine(String.Format("{0};{1};{2}",
                                                    String.IsNullOrEmpty(genresProgramGenre.program) ? "-" : genresProgramGenre.program,
                                                    String.IsNullOrEmpty(genresProgramGenreGenre.@class) ? "-" : genresProgramGenreGenre.@class,
                                                    genresProgramGenreGenre.relevance));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(String.Format("An exception occured : {0}", ex.Message));
                }
            }
            Debug.WriteLine("Done writing genres");
        }
        #endregion

        #region LineUps

        private static void WriteLineUps(xtvdResponse response, string filename)
        {
            Debug.WriteLine("LineUps :");
            File.Delete(filename);
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.WriteLine("Id;Name;UserLineupName;Location;Type;Device;PostalCode");
                try
                {
                    foreach (lineupsLineup lineupsLineup in response.xtvd.lineups)
                    {
                        sw.WriteLine(String.Format("{0};{1}",
                                                String.IsNullOrEmpty(lineupsLineup.id) ? "-" : lineupsLineup.id,
                                                String.IsNullOrEmpty(lineupsLineup.name) ? "-" : lineupsLineup.name,
                                                String.IsNullOrEmpty(lineupsLineup.userLineupName) ? "-" : lineupsLineup.userLineupName,
                                                String.IsNullOrEmpty(lineupsLineup.location) ? "-" : lineupsLineup.location,
                                                String.IsNullOrEmpty(lineupsLineup.type.ToString()) ? "-" : lineupsLineup.type.ToString(),
                                                String.IsNullOrEmpty(lineupsLineup.device) ? "-" : lineupsLineup.device,
                                                String.IsNullOrEmpty(lineupsLineup.postalCode) ? "-" : lineupsLineup.postalCode));

                        sw.WriteLine("LineUpMaps :");
                        sw.WriteLine("Station;Channel;ChannelMinor");

                        foreach (lineupsLineupMap lineupsLineupMap in lineupsLineup.map)
                        {
                            sw.WriteLine(String.Format("{0};{1};{2}",
                                                lineupsLineupMap.station,
                                                String.IsNullOrEmpty(lineupsLineupMap.channel) ? "-" : lineupsLineupMap.channel,
                                                String.IsNullOrEmpty(lineupsLineupMap.channelMinor) ? "-" : lineupsLineupMap.channelMinor));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(String.Format("An exception occured : {0}", ex.Message));
                }
            }
            Debug.WriteLine("Done writing LineUps");
        }
        #endregion

        private static string CleanContent(string content)
        {
            string result = content.Replace(';', ' ');
            result = result.Replace(@"'", " ");
            return result;
        }
    }
}
