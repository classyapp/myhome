using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Responses
{
    [Flags]
    public enum ListingUpdateFields
    {
        None = 0,
        Title = 1,
        Content = 2,
        Pricing = 4,
        ContactInfo = 8,
        SchedulingTemplate = 16,
        Metadata = 32,
        Hashtags = 64
    }
}
