using Classy.DotNet.Mvc.Controllers;
using Classy.DotNet.Mvc.Localization;
using Classy.DotNet.Responses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Classy.DotNet.Mvc.Attributes;

namespace MyHome.Models
{
    public class PhotoMetadata : IMetadata<PhotoMetadata>
    {
        [Display(Name = "PhotoMetadata_Room")]
        [Required(ErrorMessage = "PhotoMetadata_Room_Required")]
        public string Room { get; set; }
        [Display(Name = "PhotoMetadata_Style")]
        [Required(ErrorMessage = "PhotoMetadata_Style_Required")]
        public string Style { get; set; }
        [Display(Name = "PhotoMetadata_Copyright")]
        public string CopyrightMessage { get; set; }
        [BooleanRequired(ErrorMessage = "PhotoMetadata_Terms_Required")]
        public bool AgreeToTerms { get; set; }
        public bool IsWebPhoto { get; set; }
        public string Host { get; set; }

        public IDictionary<string, string> ToDictionary()
        {
            var list = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(Room)) list.Add("Room", Room);
            if (!string.IsNullOrEmpty(Style)) list.Add("Style", Style);
            if (!string.IsNullOrEmpty(CopyrightMessage)) list.Add("CopyrightMessage", CopyrightMessage);
            list.Add("IsWebPhoto", IsWebPhoto.ToString());
            if (string.IsNullOrWhiteSpace(Host) && IsWebPhoto && !string.IsNullOrWhiteSpace(CopyrightMessage))
            {
                Uri uri = null;
                if (Uri.TryCreate(CopyrightMessage, UriKind.RelativeOrAbsolute, out uri))
                {
                    list.Add("Host", uri.DnsSafeHost);
                }
                
            } else if (!string.IsNullOrWhiteSpace(Host))
            {
                list.Add("Host", Host);
            }
            return list;
        }

        public PhotoMetadata FromDictionary(IDictionary<string, string> metadata)
        {
            var output = new PhotoMetadata();
            if (metadata.ContainsKey("Room")) output.Room = metadata["Room"];
            if (metadata.ContainsKey("Style")) output.Style = metadata["Style"];
            if (metadata.ContainsKey("CopyrightMessage")) output.CopyrightMessage = metadata["CopyrightMessage"];
            if (metadata.ContainsKey("IsWebPhoto")) output.IsWebPhoto = bool.Parse(metadata["IsWebPhoto"]);
            if (metadata.ContainsKey("Host")) output.Host = metadata["Host"];
            return output;
        }

        public Dictionary<string, string[]> ParseSearchFilters(string[] filters, out string keyword, ref LocationView location)
        {
            var dict = new Dictionary<string, string[]>();
            string[] room = null;
            string[] style = null;
            keyword = null;
            switch(filters.Count())
            {
                case 0:
                default:
                    keyword = null;
                    break;
                case 1:
                    // is it a room?
                    room = MatchRoom(filters[0]);
                    // if not, is it a style?
                    if (room == null)
                    {
                        style = MatchStyle(filters[0]);
                        if (style != null)
                        {
                            dict.Add("Style", style);
                            Style = filters[0];
                        }
                    }
                    else
                    {
                        dict.Add("Room", room);
                        Room = filters[0];
                    }
                    // if not a room and not a style, its a tag.
                    if (room == null && style == null) keyword = filters[0];
                    break;
                case 2:
                    room = MatchRoom(filters[0]); // if more than one filter, first one is a room. if not, its a tag and break
                    if (room == null) keyword = filters[0];
                    else // done with room match, second one is a style or its a tag
                    {
                        dict.Add("Room", room);
                        Room = filters[0];
                        style = MatchStyle(filters[1]);
                        if (style != null)
                        {
                            dict.Add("Style", style);
                            Style = filters[1];
                        }
                        else keyword = filters[1]; 
                    }
                    break;
                case 3:
                    // if more than one filter, first one is a room. if not, its a tag and break
                    room = MatchRoom(filters[0]);
                    if (room == null) keyword = filters[0];
                    else
                    {
                        dict.Add("Room", room);
                        Room = filters[0];
                        // second filter is style, or a tag and break
                        style = MatchStyle(filters[1]);
                        if (style == null) keyword = filters[1];
                        else
                        {
                            dict.Add("Style", style);
                            Style = filters[1];
                            keyword = filters[2];
                        }
                    }
                    break;
            }
            return dict;
        }

        private string[] MatchStyle(string p)
        {
            var styles = Localizer.GetList("room-styles");
            var style = styles.SingleOrDefault(x => x.Value == p);
            return style != null ? new string[] { style.Value } : null;
        }

        private string[] MatchRoom(string p)
        {
            var rooms = Localizer.GetList("rooms");
            var room = rooms.Where(x => x.Value == p).Union(rooms.Where(x => x.ParentValue == p));
            return room.Count() > 0 ? room.Select(x => x.Value).ToArray() : null;
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
            else if (!string.IsNullOrEmpty(Style))
            {
                slug = Style;
            }
            // add location
            if (location != null) { }
            // add keyword
            if (!string.IsNullOrEmpty(keyword)) slug = string.Concat(slug, string.IsNullOrEmpty(slug) ? "" : "/", keyword);
            return slug;
        }


        public IDictionary<string, string> ToTranslationsDictionary()
        {
            IDictionary<string, string> metadata = new Dictionary<string, string>();

            return metadata;
        }
    }
}