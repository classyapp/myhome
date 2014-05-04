using MyHome.Deployment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployment
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() < 6) return;

            var deploy = new ResourceDeployment(new DeploymentSettings
            {
                AppId = args[0],
                TargetApiEndpoint = args[1],
                SourceApiEndpoint = args[2],
                OverwriteExistingResourceValues = Convert.ToBoolean(args[3]),
                CopyMissingResourcesFromRemoteDatabase = Convert.ToBoolean(args[4]), 
                BuildFailsIfMissingTranslations = Convert.ToBoolean(args[5])
            });

            deploy.DeployNewResources();
        }
    }
}
