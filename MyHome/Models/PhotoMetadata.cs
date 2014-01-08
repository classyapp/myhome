using Classy.DotNet.Mvc.Controllers;
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
            list.Add("Room", Room);
            list.Add("Style", Style);
            list.Add("CopyrightMessage", CopyrightMessage);
            return list;
        }

        public PhotoMetadata FromDictionary(IDictionary<string, string> metadata)
        {
            var output = new PhotoMetadata();
            if (metadata.ContainsKey("Room")) Room = metadata["Room"];
            if (metadata.ContainsKey("Style")) Style = metadata["Style"];
            if (metadata.ContainsKey("CopyrightMessage")) CopyrightMessage = metadata["CopyrightMessage"];
            return output;
        }
    
        public string FilterMatch(string[] filters)
        {
            switch(filters.Count())
            {
                case 0:
                default:
                    return null;
                case 1:
                    // is it a room?
                    Room = MatchRoom(filters[0]);
                    // if not, is it a style? if not a room and not a style, its a tag.
                    return (string.IsNullOrEmpty(Room) && string.IsNullOrEmpty(Style = MatchStyle(filters[0]))) ? filters[0] : null; 
                case 2:
                    Room = MatchRoom(filters[0]); // if more than one filter, first one is a room. if not, its a tag and break
                    if (string.IsNullOrEmpty(Room)) return filters[0];
                    else // done with room match, second one is a style or its a tag
                    {
                        Style = MatchStyle(filters[1]); 
                        return string.IsNullOrEmpty(Style) ? filters[1] : null; 
                    }
                case 3:
                    // if more than one filter, first one is a room. if not, its a tag and break
                    if (string.IsNullOrEmpty(Room = MatchRoom(filters[0]))) return filters[0];
                    // second filter is style, or a tag and break
                    if (string.IsNullOrEmpty(Style = MatchStyle(filters[1]))) return filters[1];
                    return filters[2];
            }
        }

        private string MatchStyle(string p)
        {
            return p == "eclectic" ? p : null;
        }

        private string MatchRoom(string p)
        {
            return p == "kitchen" ? p : null;
        }


        public string GetSlug()
        {
            var slug = string.Empty;
            if (!string.IsNullOrEmpty(Room))
            {
                slug = Room;
                if (!string.IsNullOrEmpty(Style))
                {
                    slug = string.Concat(slug, "/", Style);
                }
                return slug;
            }
            else return null;

        }
    }
}