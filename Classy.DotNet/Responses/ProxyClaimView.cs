using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Responses
{
    public class ProxyClaimView
    {
        public string Id { get; set; }
        public string ProfileId { get; set; }
        public string ProxyProfileId { get; set; }
        public ProfessionalInfoView ProfessionalInfo { get; set; }
        public IDictionary<string, string> Metadata { get; set; }
        public int Status { get; set; }
    }
}
