using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Responses
{
    public class PostReviewResponse : BaseDeleteResponse
    {
        public ReviewView Review { get; set; }
        public ProfileView RevieweeProfile { get; set; }
        public ProfileView ReviewerProfile { get; set; }
    }
}
