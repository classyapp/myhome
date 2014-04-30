using System;
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
        private readonly string GET_ALL_RESOURCES_URL = ENDPOINT_BASE_URL + "/resource/all";
        private readonly string CREATE_RESOURCE_URL = ENDPOINT_BASE_URL + "/resource";
        private readonly string RESOURCE_URL = ENDPOINT_BASE_URL + "/resource/{0}";
        private readonly string LIST_RESOURCE_URL = ENDPOINT_BASE_URL + "/resource/list/{0}";
        private readonly string CITIES_RESOURCE_URL = ENDPOINT_BASE_URL + "/resource/list/cities/{0}";

        public string[] GetMissingResources(string culture)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var resourceJson = client.DownloadString(GET_ALL_RESOURCES_URL);
                var resourceKeys = resourceJson.FromJson<IList<LocalizationResourceView>>();
                var noValues = resourceKeys.Where(x => x.Values == null);
                return resourceKeys.Where(x => !x.Values.Any(y => y.Key == culture)).OrderBy(x => x.Key).Select(x => x.Key).ToArray();
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public LocalizationResourceView GetResourceByKey(string key, bool processMarkdown = true)
        {
            try
            {
                var client = ClassyAuth.GetWebClient();
                var url = string.Format(RESOURCE_URL, key);
                var resourceJson = client.DownloadString(string.Concat(url, "?processMarkdown=", processMarkdown.ToString()));
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

        public LocalizationResourceView CreateResource(string key, IDictionary<string, string> values, string description)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(CREATE_RESOURCE_URL, key);
                var resourceJson = client.UploadString(url, new { Key = key, Values = values, Description = description }.ToJson());
                var resource = resourceJson.FromJson<LocalizationResourceView>();
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
                var client = ClassyAuth.GetAuthenticatedWebClient();
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

        public LocalizationListResourceView SetListResourceValue(string key, string culture, string value, string text)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(LIST_RESOURCE_URL, key);
                var resourceJson = client.UploadString(url, string.Concat("{\"ListItems\":", new List<ListItemView> { new ListItemView { Value = value, Text = new Dictionary<string, string> { { culture, text }} } }.ToJson()));
                var listResource = resourceJson.FromJson<LocalizationListResourceView>();
                return listResource;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public IList<string> GetCitiesByCountry(string countryCode)
        {
            try
            {
                var client = ClassyAuth.GetWebClient();
                var url = string.Format(CITIES_RESOURCE_URL, countryCode);
                var resourceJson = client.DownloadString(url);
                var cities = resourceJson.FromJson<IList<string>>();
                return cities;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }
    }
}
