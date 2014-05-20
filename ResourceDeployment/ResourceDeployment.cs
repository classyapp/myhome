using Classy.DotNet.Mvc.Localization;
using Classy.DotNet.Responses;
using Classy.DotNet.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyHome.Deployment
{
    // THIS IS NOT A TEST:
    // this class does actual deployment of new resources to the environment where it is running
    public class ResourceDeployment
    {
        private DeploymentSettings Settings { get; set; }

        public ResourceDeployment(DeploymentSettings settings)
        {
            // read settings
            Settings = settings;
            Console.WriteLine(Settings.ToString());
        }

        public void DeployNewResources()
        {
            var missingTranslations = new Dictionary<string, IList<string>>();

            // supported cultures
            var supportedCultures = GetSupportedCulturesAtTargetEndpoint();
            Console.WriteLine("Supported cultures: " + String.Join(",", supportedCultures));

            // deployment logic
            Console.WriteLine("Deploying new reources");
            try 
            {
                // get all resource manifest files from the 
                var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources");
                Console.WriteLine(path);
                var dir = new DirectoryInfo(path);
                var resourceManifests = dir.GetFiles("*.resm");
                foreach(var rm in resourceManifests)
                {
                    // ignore files that were already deployed to target database
                    if (1 == 2) /* file was alreday deployed to target database */
                    {
                        Console.WriteLine(string.Format("Found {0}... ignoring - already deployed", rm.FullName));
                        continue;
                    }

                    // deploy all resources in manifest file
                    Console.WriteLine(string.Format("Found {0}... deploying", rm.FullName));
                    var manifest = GetManifestFromFile(rm);
                    
                    // throw if any resource is missing a description or value
                    var firstMissingDescription = manifest.Resources.FirstOrDefault(x => string.IsNullOrEmpty(x.Description));
                    if (firstMissingDescription != null) throw new ResourceMissingFieldException(firstMissingDescription.Key, "Description");
                    var firstMissingValues = manifest.Resources.FirstOrDefault(x => x.Values == null);
                    if (firstMissingValues != null) throw new ResourceMissingFieldException(firstMissingValues.Key, "Values");

                    // throw if resource any contains a value for an unsupported culture
                    var firstUnsupported = manifest.Resources.FirstOrDefault(x => x.Values.Keys.Any(y => !supportedCultures.Contains(y)));
                    if (firstUnsupported != null)
                    {
                        Console.WriteLine("first unsupported: key => " + firstUnsupported.Key + ", values => " + string.Join(";", firstUnsupported.Values.Keys));
                        throw new ResourceValuesContainsInvalidCultureException(firstUnsupported.Key);
                    }

                    foreach (var resource in manifest.Resources)
                    {
                        Console.WriteLine(string.Format("\tResource: {0}", resource.Key));

                        // create the resource in the target database
                        var resourceAtTarget = GetResourceAtTargetEndpoint(resource.Key);
                        if (resourceAtTarget == null)
                        {
                            CreateResourceAtTargetEndpoint(resource);
                        }
                        else
                        {
                            if (Settings.OverwriteExistingResourceValues)
                            {
                                Console.WriteLine("\t\tAlready exists, overwriting");
                                SetResourceValuesAtTargetEndpoint(resource.Key, resource.Values);
                            }
                            else Console.WriteLine("\t\tAlready exists, will only update missing translations, if any");
                        }

                        // check for missing translations
                        if (resourceAtTarget == null || resourceAtTarget.Values == null)
                        {
                            missingTranslations.Add(resource.Key, supportedCultures.ToList());
                        }
                        else if (supportedCultures.Any(x => !resourceAtTarget.Values.Keys.Contains(x)))
                        {
                            missingTranslations.Add(resource.Key, supportedCultures.Where(x => !resourceAtTarget.Values.Keys.Contains(x)).ToList());
                        }

                        // if some supported cultures are missing values, copy from remote source
                        if (missingTranslations.ContainsKey(resource.Key) && Settings.CopyMissingResourcesFromRemoteDatabase)
                        {
                            Console.WriteLine(string.Format("\t\tTrying to copy missing translations from source", resource.Key));
                            var resourceAtSource = GetResourceAtSourceEndpoint(resource.Key);
                            var values = new Dictionary<string, string>();
                            var missingCultures = missingTranslations[resource.Key];
                            foreach(var missingCulture in missingCultures)
                            {
                                if (resourceAtSource != null && resourceAtSource.Values.Any(x => x.Key == missingCulture))
                                {
                                    values.Add(missingCulture, resourceAtSource.Values.Single(x => x.Key == missingCulture).Value);
                                    missingTranslations[resource.Key].Remove(missingCulture);
                                    if (missingTranslations[resource.Key].Count == 0) missingTranslations.Remove(resource.Key);
                                }
                            }
                            SetResourceValuesAtTargetEndpoint(resource.Key, values);
                        }
                    }
                }

                // deployment response
                if (missingTranslations.Count > 0 && Settings.BuildFailsIfMissingTranslations)
                    throw new ResourceMissingTranslationsException(FormatMissingTranslationsMessage(missingTranslations));
            }
            catch(Exception ex)
            {
                // deployment failed
                throw ex;
            }
        }

        /// <summary>
        /// format the missing translations into a readable message
        /// </summary>
        /// <param name="missingTranslations"></param>
        /// <returns>a readable message</returns>
        private string FormatMissingTranslationsMessage(Dictionary<string, IList<string>> missingTranslations)
        {
            var formattedMessage = new StringBuilder("Missing translations found.\r\n");
            foreach(var mt in missingTranslations)
            {
                formattedMessage.AppendLine(string.Format("Key: '{0}' is missing translations in {1}", mt.Key, string.Join(",", mt.Value.ToArray())));
            }
            return formattedMessage.ToString();
        }

        /// <summary>
        /// read a resource manifest file into an object
        /// </summary>
        /// <param name="f">file to read</param>
        /// <returns>a ResourceManifest object</returns>
        private ResourceManifest GetManifestFromFile(FileInfo f)
        {
            var resmFile = File.OpenText(f.FullName);
            var resmContent = resmFile.ReadToEnd();
            resmFile.Close();
            resmFile.Dispose();
            var manifest = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceManifest>(resmContent);
            return manifest;
        }

        private WebClient GetWebClient()
        {
            var client = new WebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            client.Headers.Add(HttpRequestHeader.Accept, "application/json");
            client.Headers.Add("X-Classy-Env", string.Format("{{\"AppId\":\"{0}\"}}", Settings.AppId));
            client.Headers.Add("Authorization", "Basic RGVwbG95bWVudFVzZXI6ZDNQbDB5TDFrZUFCMHNT"); // username: DeploymentUser, password: d3Pl0yL1keAB0sS
            return client;
        }

        private void CreateResourceAtTargetEndpoint(Resource resource)
        {
            try
            {
                var client = GetWebClient();
                var url = string.Concat(Settings.TargetApiEndpoint.Trim('/'), "/resource");
                var resourceJson = client.UploadString(url, Newtonsoft.Json.JsonConvert.SerializeObject(new { Key = resource.Key, Values = resource.Values, Description = resource.Description }));
            }
            catch
            {
                throw;
            }
        }

        private LocalizationResourceView SetResourceValuesAtTargetEndpoint(string key, IDictionary<string, string> values)
        {
            try
            {
                var client = GetWebClient();
                var url = string.Format(string.Concat(Settings.TargetApiEndpoint.Trim('/'), "/resource/{0}"), key);
                var resourceJson = client.UploadString(url, string.Concat("{\"Values\":", Newtonsoft.Json.JsonConvert.SerializeObject(values), "}"));
                return Newtonsoft.Json.JsonConvert.DeserializeObject<LocalizationResourceView>(resourceJson);
            }
            catch 
            {
                throw;
            }
        }

        private LocalizationResourceView GetResourceAtSourceEndpoint(string key)
        {
            try
            {
                var client = GetWebClient();
                var url = string.Format(string.Concat(Settings.SourceApiEndpoint.Trim('/'), "/resource/{0}"), key);
                var resourceJson = client.DownloadString(url);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<LocalizationResourceView>(resourceJson);
            }
            catch
            {
                throw;
            }
        }

        private LocalizationResourceView GetResourceAtTargetEndpoint(string key)
        {
            try
            {
                var client = GetWebClient();
                var url = string.Format(string.Concat(Settings.TargetApiEndpoint.Trim('/'), "/resource/{0}"), key);
                var resourceJson = client.DownloadString(url);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<LocalizationResourceView>(resourceJson);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get the supported cultures from tge target api
        /// </summary>
        /// <returns>an array of string values of supported cultures</returns>
        private string[] GetSupportedCulturesAtTargetEndpoint()
        {
            try
            {
                var client = GetWebClient();
                var url = string.Concat(Settings.TargetApiEndpoint.Trim('/'), "/resource/list/supported-cultures");
                var json = client.DownloadString(url);
                dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                IList<string> cultures = new List<string>();
                foreach (dynamic item in obj.ListItems)
                {
                    cultures.Add((string)item.Value.Value);
                }
                return cultures.ToArray();
            }
            catch
            {
                throw;
            }
        }

    }
}
