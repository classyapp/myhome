using Classy.DotNet.Responses;
using Classy.DotNet.Security;
using ServiceStack.Text;

namespace Classy.DotNet.Services
{
    public class SettingsService : ServiceBase
    {
        private readonly string GET_APP_SETTINGS_URL = ENDPOINT_BASE_URL + "/app/settings";

        public AppSettingsResponse GetAppSettings()
        {
            var client = ClassyAuth.GetWebClient(true);
            string json = client.DownloadString(GET_APP_SETTINGS_URL);
            return json.FromJson<AppSettingsResponse>();
        }
    }
}
