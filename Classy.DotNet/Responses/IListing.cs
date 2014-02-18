using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Responses
{
    public interface IListing
    {
        string Id { get; set; }
        string ProfileId { get; set; }
        string ListingType { get; set; }
    }
}
