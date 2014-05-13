using System.Collections.Generic;
using System.Web;

namespace Classy.DotNet.Mvc.Config
{
    public class FeatureSwitchProvider
    {
        public const string FeatureSwitchProviderKey = "__FeatureSwitchProviderKey__";

        public static bool IsFeatureOn(string featureName)
        {
            var httpContext = HttpContext.Current;
            if (httpContext == null)
                return false;

            var featuresOn = httpContext.Items[FeatureSwitchProviderKey];
            if (featuresOn == null)
                return false;

            var featuresList = featuresOn as HashSet<string>;
            if (featuresList == null)
                return false;

            return featuresList.Contains(featureName);
        }

        public static void TurnOnFeatures(string[] featureNames)
        {
            var httpContext = HttpContext.Current;
            if (httpContext == null)
                return;

            if (httpContext.Items[FeatureSwitchProviderKey] == null)
                httpContext.Items[FeatureSwitchProviderKey] = new HashSet<string>();

            foreach (var feature in featureNames)
                ((HashSet<string>) httpContext.Items[FeatureSwitchProviderKey]).Add(feature);
        }
    }
}