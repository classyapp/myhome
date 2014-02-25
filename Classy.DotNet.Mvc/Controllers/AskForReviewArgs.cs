using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classy.DotNet.Responses;

namespace Classy.DotNet.Mvc.Controllers
{
    public class AskForReviewArgs<TProMetadata>
    {
        public IList<string> Emails { get; set; }
        public string Message { get; set; }
        public ProfileView Profile { get; set; }
        public TProMetadata Metadata { get; set; }
    }
}
