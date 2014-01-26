﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;
using Classy.DotNet.Responses;
using Classy.DotNet.Security;

namespace Classy.DotNet.Services
{
    public class LocalizationService : ServiceBase
    {
        private readonly string GET_RESOURCE_KEYS_URL = ENDPOINT_BASE_URL + "/resource/keys";
        private readonly string RESOURCE_URL = ENDPOINT_BASE_URL + "/resource/{0}";
        private readonly string LIST_RESOURCE_URL = ENDPOINT_BASE_URL + "/resource/list/{0}";

        public string[] GetResourceKeys()
        {
            try
            {
                var client = ClassyAuth.GetWebClient();
                var resourceJson = client.DownloadString(GET_RESOURCE_KEYS_URL);
                var resourceKeys = resourceJson.FromJson<string[]>();
                return resourceKeys;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public LocalizationResourceView GetResourceByKey(string key)
        {
            try
            {
                var client = ClassyAuth.GetWebClient();
                var url = string.Format(RESOURCE_URL, key);
                var resourceJson = client.DownloadString(url);
                var resource = resourceJson.FromJson<LocalizationResourceView>();
                return resource;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public LocalizationListResourceView GetListResourceByKey(string key)
        {
            try
            {
                var client = ClassyAuth.GetWebClient();
                var url = string.Format(LIST_RESOURCE_URL, key);
                var resourceJson = client.DownloadString(url);
                var resource = resourceJson.FromJson<LocalizationListResourceView>();
                return resource;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public LocalizationResourceView SetResourceValues(string key, IDictionary<string, string> values)
        {
            try
            {
                var client = ClassyAuth.GetWebClient();
                var url = string.Format(RESOURCE_URL, key);
                var resourceJson = client.UploadString(url, string.Concat("{\"Values\":", values.ToJson()));
                var resource = resourceJson.FromJson<LocalizationResourceView>();
                return resource;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }
    }
}
