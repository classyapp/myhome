using Classy.DotNet.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Services
{
    public class ServiceBase
    {
        internal static string ENDPOINT_BASE_URL = System.Configuration.ConfigurationManager.AppSettings["Classy:EndpointBaseUrl"];

        public ServiceBase()
        {
            ServiceStack.Text.JsConfig.ExcludeTypeInfo = true;
        }
    }
}
