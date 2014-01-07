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

        public PhotoMetadata FromStringArray(string[] strings)
        {
            if (strings == null) return null;
            switch (strings.Count())
            {
                case 0:
                default:
                    return null;
                case 1:
                    return new PhotoMetadata
                    {
                        Room = strings[0]
                    };
                case 2:
                    return new PhotoMetadata
                    {
                        Room = strings[0],
                        Style = strings[1]
                    };
            }
        }


        public string ToSlug()
        {
            var slug = string.Empty;
            if (!string.IsNullOrEmpty(Room))
            {
                slug = Room;
                if (!string.IsNullOrEmpty(Style)) slug = string.Concat(slug, "/", Style);
            }
            return slug;
        }
    }
}