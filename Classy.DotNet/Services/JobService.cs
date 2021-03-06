﻿using System;
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
        private readonly string GET_JOB_ERRORS_URL = ENDPOINT_BASE_URL + "/job/{0}/errors"; 

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

        public string GetJobErrors(string id)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(GET_JOB_ERRORS_URL, id);
                string errors = client.DownloadString(url);
                return errors;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }
    }
}
