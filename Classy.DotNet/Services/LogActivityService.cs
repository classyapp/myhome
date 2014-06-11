using System.Net;
using Classy.DotNet.Models.LogActivity;
using Classy.DotNet.Security;
using CsQuery.ExtensionMethods.Internal;
using ServiceStack.Text;

namespace Classy.DotNet.Services
{
    public class LogActivityService : ServiceBase
    {
        private readonly string LOG_ACTIVITY_LOG = ENDPOINT_BASE_URL + "/log-activity/log";

        public LogActivity<T> GetLogActivity<T>(LogActivity<T> logActivity) where T : ILogActivityMetadata<T>, new()
        {
            using (var client = ClassyAuth.GetWebClient())
            {
                var data = new {
                    SubjectId = logActivity.UserId,
                    Predicate = logActivity.Activity,
                    ObjectId = logActivity.ObjectId
                }.ToJson();
                var url = string.Format(LOG_ACTIVITY_LOG + "?SubjectId={0}&Predicate={1}&ObjectId={2}",
                    logActivity.UserId, logActivity.Activity, logActivity.ObjectId);
                var response = client.DownloadString(url);

                if (response.IsNullOrEmpty())
                    return null;

                var parsedResponse = response.FromJson<LogActivityResponse>();
                return new LogActivity<T>
                {
                    UserId = parsedResponse.UserId,
                    Activity = parsedResponse.Activity,
                    ObjectId = parsedResponse.ObjectId,
                    Metadata = logActivity.Metadata.DeserializeMetadata(parsedResponse.Metadata)
                };
            }
        }

        public LogActivity<T> LogActivity<T>(LogActivity<T> logActivity) where T : ILogActivityMetadata<T>, new()
        {
            try
            {
                using (var client = ClassyAuth.GetWebClient())
                {
                    var data = new {
                        SubjectId = logActivity.UserId,
                        Predicate = logActivity.Activity,
                        ObjectId = logActivity.ObjectId,
                        Metadata = logActivity.Metadata != null ? logActivity.Metadata.SerializeMetadata() : null
                    }.ToJson();
                    var response = client.UploadString(LOG_ACTIVITY_LOG, "POST", data);

                    var parsedResponse = response.FromJson<LogActivityResponse>();
                    return new LogActivity<T> {
                        UserId = parsedResponse.UserId,
                        Activity = parsedResponse.Activity,
                        ObjectId = parsedResponse.ObjectId,
                        Metadata = logActivity.Metadata.DeserializeMetadata(parsedResponse.Metadata)
                    };
                }
            }
            catch (WebException ex)
            {
                throw ex.ToClassyException();
            }
        }

        public void LogActivity(string userId, string activity, string objectId)
        {
            try
            {
                using (var client = ClassyAuth.GetWebClient())
                {
                    var data = new {
                        SubjectId = userId,
                        Predicate = activity,
                        ObjectId = objectId
                    }.ToJson();
                    client.UploadString(LOG_ACTIVITY_LOG, "POST", data);
                }
            }
            catch (WebException ex)
            {
                throw ex.ToClassyException();
            }
        }
    }
}