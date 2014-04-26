using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployment
{
    [TestClass]
    // THIS IS NOT A TEST:
    // this class does actual deployment of new resources to the environment where it is running
    public class ResourceDeployment
    {
        [TestMethod]
        public void DeployNewResources()
        {
            Trace.WriteLine("deploying new reources");
            Assert.IsTrue(1 == 1);
        }
    }
}
