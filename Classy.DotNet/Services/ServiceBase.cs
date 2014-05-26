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
