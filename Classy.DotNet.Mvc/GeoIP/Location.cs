using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.GeoIP
{
    public class Location
    {
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Location(Newtonsoft.Json.Linq.JToken json)
        {
            if (json != null)
            {
                CountryCode = json["country"].Value<string>("iso_code");
                Country = json["country"]["names"].Value<string>("en");
                Longitude = json["location"].Value<double>("longitude");
                Latitude = json["location"].Value<double>("latitude");
            }
        }
    }
}
