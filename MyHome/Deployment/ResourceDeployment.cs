using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployment
{
    public class Resource
    {
        public string Key { get; set; }
    }
    public class ResourceManifest 
    {
        public IList<Resource> Resources { get; set;}
    }

    [TestClass]
    // THIS IS NOT A TEST:
    // this class does actual deployment of new resources to the environment where it is running
    public class ResourceDeployment
    {
        [TestMethod]
        public void DeployNewResources()
        {
            Trace.WriteLine("Deploying new reources");

            try 
            {
                var resourceManifests = Directory.GetFiles("Resources", "*.resm");
                foreach(var rm in resourceManifests)
                {
                    Trace.WriteLine(string.Format("Found {0}... deploying", rm));
                
                    var manifest = GetManifestFromFile(rm); 
                }
                Assert.IsTrue(1 == 1);
            }
            catch
            {
                Assert.IsTrue(1 == 2);
            }
        }

        private ResourceManifest GetManifestFromFile(string fileName)
        {
            var resmFile = File.OpenText(fileName);
            var resmContent = resmFile.ReadToEnd();
            resmFile.Close();
            resmFile.Dispose();
            var manifest = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceManifest>(resmContent);
            return manifest;
        }
    }
}
