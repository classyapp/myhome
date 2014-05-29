using System.Net;
using Classy.DotNet.Responses.LogActivity;
using Classy.DotNet.Security;
using ServiceStack.Text;

namespace Classy.DotNet.Services
{
    public class LogActivityService : ServiceBase
    {
        private readonly string LOG_ACTIVITY_LOG = ENDPOINT_BASE_URL + "/log-activity/log";

        public bool WasLogged(string subjectId, string predicate, string objectId)
        {
            try
            {
                var client = ClassyAuth.GetWebClient();
                var data = new {
                    SubjectId = subjectId,
                    Predicate = predicate,
                    ObjectId = objectId
                }.ToJson();
                var listingJson = client.UploadString(LOG_ACTIVITY_LOG, "POST", data);
                var triple = listingJson.FromJson<Triple>();

                return triple.Count > 1;
            }
            catch (WebException ex)
            {
                throw ex.ToClassyException();
            }
        }
    }
}