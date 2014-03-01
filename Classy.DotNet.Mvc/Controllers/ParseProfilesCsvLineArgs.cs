using Classy.DotNet.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classy.DotNet.Mvc.Controllers
{
    public class ParseProfilesCsvLineArgs<TProMetadata>
    {
        public bool IsHeaderLine { get; set; }
        public string[] LineValues { get; set; }
        public int LineCount { get; set; }
        public ProfessionalInfoView ProfessionalInfo { get; set; }
        public TProMetadata Metadata { get; set; }
    }
}
