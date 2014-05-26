using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Classy.DotNet.Responses;
using Classy.DotNet.Security;
using ServiceStack.Text;

namespace Classy.DotNet.Services
{
    public class JobService : ServiceBase
    {
        private readonly string GET_JOBS_STATUS_URL = ENDPOINT_BASE_URL + "/jobs/{0}";

        public IList<JobView> GetJobsStatus(string profileId)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(GET_JOBS_STATUS_URL, profileId);
                var jobsJson = client.DownloadString(url);
                var jobs = jobsJson.FromJson<IList<JobView>>();
                return jobs;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }
    }
}
