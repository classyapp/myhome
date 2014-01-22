using Classy.DotNet.Mvc.Controllers;
using Classy.DotNet.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyHome.Models
{
    public class PhotoMetadata : IMetadata<PhotoMetadata>
    {
        public string Room { get; set; }
        public string Style { get; set; }
        public string CopyrightMessage { get; set; }

        public IDictionary<string, string> ToDictionary()
        {
            var list = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(Room)) list.Add("Room", Room);
            if (!string.IsNullOrEmpty(Style)) list.Add("Style", Style);
            if (!string.IsNullOrEmpty(CopyrightMessage)) list.Add("CopyrightMessage", CopyrightMessage);
            return list;
        }

        public PhotoMetadata FromDictionary(IDictionary<string, string> metadata)
        {
            var output = new PhotoMetadata();
            if (metadata.ContainsKey("Room")) output.Room = metadata["Room"];
            if (metadata.ContainsKey("Style")) output.Style = metadata["Style"];
            if (metadata.ContainsKey("CopyrightMessage")) output.CopyrightMessage = metadata["CopyrightMessage"];
            return output;
        }
    
        public void ParseSearchFilters(string[] filters, out string keyword, ref LocationView location)
        {
            switch(filters.Count())
            {
                case 0:
                default:
                    keyword = null;
                    break;
                case 1:
                    // is it a room?
                    Room = MatchRoom(filters[0]);
                    // if not, is it a style? if not a room and not a style, its a tag.
                    keyword = (string.IsNullOrEmpty(Room) && string.IsNullOrEmpty(Style = MatchStyle(filters[0]))) ? filters[0] : null;
                    break;
                case 2:
                    Room = MatchRoom(filters[0]); // if more than one filter, first one is a room. if not, its a tag and break
                    if (string.IsNullOrEmpty(Room)) keyword = filters[0];
                    else // done with room match, second one is a style or its a tag
                    {
                        Style = MatchStyle(filters[1]); 
                        keyword = string.IsNullOrEmpty(Style) ? filters[1] : null; 
                    }
                    break;
                case 3:
                    // if more than one filter, first one is a room. if not, its a tag and break
                    if (string.IsNullOrEmpty(Room = MatchRoom(filters[0]))) keyword = filters[0];
                    // second filter is style, or a tag and break
                    if (string.IsNullOrEmpty(Style = MatchStyle(filters[1]))) keyword = filters[1];
                    keyword = filters[2];
                    break;
            }
        }

        private string MatchStyle(string p)
        {
            // TODO: create a real list of styles
            return new string[] {"eclectic","modern","contemporary","classic"}.Contains(p) ? p : null;
        }

        private string MatchRoom(string p)
        {
            // TODO: create a real list of rooms
            return new string[] { "kitchen", "bedroom", "bath", "living-room" }.Contains(p) ? p : null;
        }


        public string GetSearchFilterSlug(string keyword, LocationView location)
        {
            // pare metadata props
            var slug = string.Empty;
            if (!string.IsNullOrEmpty(Room))
            {
                slug = Room;
                if (!string.IsNullOrEmpty(Style))
                {
                    slug = string.Concat(slug, "/", Style);
                }
            }
            // add location
            if (location != null) { }
            // add keyword
            if (!string.IsNullOrEmpty(keyword)) slug = string.Concat(slug, string.IsNullOrEmpty(slug) ? "" : "/", keyword);
            return slug;
        }
    }
}