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

using SchedulesDirect;

namespace ArgusTV.GuideImporter.SchedulesDirect.Entities
{
    internal class Program : IEquatable<Program>
    {
        #region Private Members

        private programsProgram _sdProgram;
        private string _genre;
        private List<string> _actors = new List<string>();
        private List<string> _directors = new List<string>();
        #endregion

        #region Constructor 

        public Program(programsProgram sdProgram)
        {
            _sdProgram = sdProgram;
        }
        #endregion

        #region Properties

        public string Id
        {
            get { return _sdProgram.id; }
        }

        public string Title
        {
            get { return _sdProgram.title; }
        }

        public string SubTitle
        {
            get { return _sdProgram.subtitle; }
        }

        public string Description
        {
            get { return _sdProgram.description; }
        }

        public DateTime? OriginalAirDate
        {
            get 
            {
                if (_sdProgram.originalAirDateSpecified)
                    return _sdProgram.originalAirDate;
                return null;
            }
        }

        public string ShowType
        {
            get { return _sdProgram.showType; }
        }

        public double? StarRating
        {
            get 
            {
                double? starRating = null;
                switch (_sdProgram.starRating.ToString())
                {
                    case "Item":
                        starRating = 0.1;
                        break;
                    case "Item1":
                        starRating = 0.25;
                        break;
                    case "Item2":
                        starRating = 0.5;
                        break;
                    case "Item3":
                        starRating = 0.6;
                        break;
                    case "Item4":
                        starRating = 0.75;
                        break;
                    case "Item5":
                        starRating = 0.8;
                        break;
                    case "Item6":
                        starRating = 1;
                        break;
                }
                return starRating;
            }
        }

        public int? EpisodeNumber
		{
            get 
            {
                if (!String.IsNullOrEmpty(_sdProgram.syndicatedEpisodeNumber))
                {
                    int result;
                    if (int.TryParse(_sdProgram.syndicatedEpisodeNumber, out result))
                    {
                        return result;
                    }
                }
                return null; 
            }
		}

        public string Genre
        {
            get { return _genre; }
            set {_genre = value; }
        }

        public string[] Actors
        { 
            get
            {
                return _actors.ToArray();
            }
        }

        public string[] Directors
        { 
            get
            {
                return _directors.ToArray();
            }
        }
        #endregion

        #region Public Methods

        public void AddActor(string actor)
        {
            if (!String.IsNullOrEmpty(actor))
            {
                _actors.Add(actor);
            }
        }

        public void AddDirector(string director)
        {
            if (!String.IsNullOrEmpty(director))
            {
                _directors.Add(director);
            }
        }
        #endregion

        #region IEquatable<Program> Members

        public bool Equals(Program other)
        {
            return Id.Equals(other.Id);
        }
        #endregion
    }
}
