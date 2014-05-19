using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classy.DotNet.Responses;

namespace Classy.DotNet.Mvc.Controllers
{
    public class ListingCommentEventArgs
    {
        public CommentView Comment { get; set; }
        public ListingView Listing { get; set; }
    }
}
