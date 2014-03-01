﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classy.DotNet.Responses;
using Classy.DotNet.Services;

namespace Classy.DotNet.Responses
{
    public class AppView
    {
        public static int PageSize { get; set; }
        public static int PagesCount { get; set; }
        public static string DefaultProfileImage { get; set; }

        static AppView()
        {
            var service = new SettingsService();
            AppSettingsResponse settings = service.GetAppSettings();
            PageSize = settings.PageSize;
            PagesCount = settings.PagesCount;
            DefaultProfileImage = settings.DefaultProfileImage;
        }
    }
}