﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Classy.DotNet.Mvc.GeoIP;
using MaxMind.Db;
using System.Web;

namespace Classy.DotNet.Mvc.Helpers
{
    public static class IPLocator
    {
        private static MaxMind.Db.Reader dbReader = null;

        static IPLocator()
        {
            dbReader = new MaxMind.Db.Reader(HttpContext.Current.Server.MapPath("~/App_Data/GeoLite2-City.mmdb"), FileAccessMode.Memory);
        }

        public static Location GetLocationByRequestIP()
        {
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip)) 
            { 
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; 
            }

            return new Location(dbReader.Find(ip));
        }
    }
}
