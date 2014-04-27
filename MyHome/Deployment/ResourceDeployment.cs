using Classy.DotNet.Mvc.Localization;
using Classy.DotNet.Responses;
using Classy.DotNet.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    [TestClass]
    // THIS IS NOT A TEST:
    // this class does actual deployment of new resources to the environment where it is running
    public class ResourceDeployment
    {
        public DeploymentSettings Settings { get; private set; }

        [TestInitialize]
        public void Initialize()
        {
            // read settings
            Trace.WriteLine("Reading deployment settings");
            Settings = GetDeploymentSettings();
            Trace.WriteLine(Settings.ToString());
        }

        [TestMethod]
        public void DeployNewResources()
        {
            var missingTranslations = new Dictionary<string, IList<string>>();

            // supported cultures
            var supportedCultures = GetSupportedCulturesAtTargetEndpoint();

            // deployment logic
            Trace.WriteLine("Deploying new reources");
            try 
            {
                // get all resource manifest files from the 
                var dir = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Deployment", "Resources"));
                var resourceManifests = dir.GetFiles("*.resm");
                foreach(var rm in resourceManifests)
                {
                    // ignore files that were already deployed to target database
                    if (1 == 2) /* file was alreday deployed to target database */
                    {
                        Trace.WriteLine(string.Format("Found {0}... ignoring - already deployed", rm.FullName));
                        continue;
                    }

                    // deploy all resources in manifest file
                    Trace.WriteLine(string.Format("Found {0}... deploying", rm.FullName));
                    var manifest = GetManifestFromFile(rm);
                    
                    // throw if any resource is missing a description or value
                    var firstMissingDescription = manifest.Resources.FirstOrDefault(x => string.IsNullOrEmpty(x.Description));
                    if (firstMissingDescription != null) throw new ResourceMissingFieldException(firstMissingDescription.Key, "Description");
                    var firstMissingValues = manifest.Resources.FirstOrDefault(x => x.Values == null);
                    if (firstMissingValues != null) throw new ResourceMissingFieldException(firstMissingValues.Key, "Values");

                    // throw if resource any contains a value for an unsupported culture
                    var firstUnsupported = manifest.Resources.FirstOrDefault(x => !supportedCultures.Any(y => !x.Values.Keys.Contains(y)));
                    if (firstUnsupported != null)
                    {
                        throw new ResourceValuesContainsInvalidCultureException(firstUnsupported.Key);
                    }

                    foreach (var resource in manifest.Resources)
                    {
                        Trace.WriteLine(string.Format("\tResource: {0}", resource.Key));

                        // create the resource in the target database
                        var resourceAtTarget = GetResourceAtTargetEndpoint(resource.Key);
                        if (resourceAtTarget != null && Settings.OverwriteExistingResourceValues)
                            SetResourceValuesAtTargetEndpoint(resource.Key, resource.Values);

                        // if some translations are still missing, add t missing translations dictionary
                        if (supportedCultures.Any(x => !resourceAtTarget.Values.Keys.Contains(x)))
                        {
                            missingTranslations.Add(resource.Key, supportedCultures.Where(x => !resourceAtTarget.Values.Keys.Contains(x)).ToList());
                        }

                        // if some supported cultures are missing values, copy from remote source
                        if (missingTranslations.ContainsKey(resource.Key) && Settings.CopyMissingResourcesFromRemoteDatabase)
                        {
                            Trace.WriteLine(string.Format("\t\tTrying to copy missing translations from source", resource.Key));
                            var resourceAtSource = GetResourceAtSourceEndpoint(resource.Key);
                            var values = new Dictionary<string, string>();
                            foreach(var missingCulture in missingTranslations[resource.Key])
                            {
                                if (resourceAtSource.Values.Any(x => x.Key == missingCulture))
                                {
                                    values.Add(missingCulture, resourceAtSource.Values.Single(x => x.Key == missingCulture).Value);
                                    missingTranslations[resource.Key].Remove(missingCulture);
                                    if (missingTranslations[resource.Key].Count == 0) missingTranslations.Remove(resource.Key);
                                }
                            }
                        }
                    }
                }

                // deployment successful
                Assert.IsTrue(missingTranslations.Count == 0 && Settings.BuildFailsIfMissingTranslations, FormatMissingTranslationsMessage(missingTranslations));
            }
            catch(Exception ex)
            {
                // deployment failed
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// format the missing translations into a readable message
        /// </summary>
        /// <param name="missingTranslations"></param>
        /// <returns>a readable message</returns>
        private string FormatMissingTranslationsMessage(Dictionary<string, IList<string>> missingTranslations)
        {
            var formattedMessage = new StringBuilder("Missing translations found.");
            foreach(var mt in missingTranslations)
            {
                formattedMessage.AppendLine(string.Format("Key: '{0}' is missing translations in {1}", mt.Key, string.Join(",", mt.Value.ToArray())));
            }
            return formattedMessage.ToString();
        }

        /// <summary>
        /// get the settings for the deployment
        /// </summary>
        /// <returns>a DeploymentSettings object</returns>
        private DeploymentSettings GetDeploymentSettings()
        {
            var appSettings = ConfigurationManager.AppSettings;
            var settings = new DeploymentSettings();
            settings.BuildFailsIfMissingTranslations = Convert.ToBoolean(appSettings["Classy.Deployment:BuildFailsIfMissingTranslations"]);
            settings.CopyMissingResourcesFromRemoteDatabase = Convert.ToBoolean(appSettings["Classy.Deployment:CopyMissingResourcesFromRemoteDatabase"]);
            settings.SourceApiEndpoint = appSettings["Classy.Deployment:SourceApiEndpoint"];
            settings.TargetApiEndpoint = appSettings["Classy.Deployment:TargetApiEndpoint"];
            settings.OverwriteExistingResourceValues = Convert.ToBoolean(appSettings["Classy.Deployment:OverwriteExistingResourceValues"]);
            return settings;
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
            client.Headers.Add("X-Classy-Env", "{\"AppId\":\"v1.0\"}");
            client.Headers.Add("Authorization", "Basic RGVwbG95bWVudFVzZXI6ZDNQbDB5TDFrZUFCMHNT"); // username: DeploymentUser, password: d3Pl0yL1keAB0sS
            return client;
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
