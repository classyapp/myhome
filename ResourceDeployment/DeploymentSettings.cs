using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyHome.Deployment
{
    public class DeploymentSettings
    {
        public string AppId { get; set; }
        public bool OverwriteExistingResourceValues { get; set; }
        public bool CopyMissingResourcesFromRemoteDatabase { get; set; }
        public string SourceApiEndpoint { get; set; }
        public string TargetApiEndpoint { get; set; }
        public bool BuildFailsIfMissingTranslations { get; set; }

        public override string ToString()
        {
            return string.Format("\tAppId: {0}\r\n\tSource API Endpoint: {1}\r\n\tTarget API Endpoint: {2}\r\n\tCopy Missing Resources From Remote Database: {3}\r\n\tBuild Fails If Missing Translations: {4}\r\n\tOverwrite existing resource values: {5}",
                AppId,
                SourceApiEndpoint,
                TargetApiEndpoint,
                CopyMissingResourcesFromRemoteDatabase,
                BuildFailsIfMissingTranslations,
                OverwriteExistingResourceValues);
        }
    }
}