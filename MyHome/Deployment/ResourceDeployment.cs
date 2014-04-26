using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public string AppId { get; set; }
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
                var dir = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Deployment", "Resources"));
                var resourceManifests = dir.GetFiles("*.resm");
                foreach(var rm in resourceManifests)
                {
                    Trace.WriteLine(string.Format("Found {0}... deploying", rm.FullName));
                
                    var manifest = GetManifestFromFile(rm); 
                    foreach (var resource in manifest.Resources)
                    {
                        Trace.WriteLine(string.Format("-> {0}", resource.Key));
                    }
                }
                Assert.IsTrue(1 == 1);
            }
            catch(Exception ex)
            {
                Assert.IsTrue(1 == 2, ex.Message);
            }
        }

        private ResourceManifest GetManifestFromFile(FileInfo f)
        {
            var resmFile = File.OpenText(f.FullName);
            var resmContent = resmFile.ReadToEnd();
            resmFile.Close();
            resmFile.Dispose();
            var manifest = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceManifest>(resmContent);
            return manifest;
        }
    }
}
