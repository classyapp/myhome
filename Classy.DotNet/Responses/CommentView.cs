using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Responses
{
    public class CommentView
    {
        public CommentView() { }
        //
        public string Id { get; set; }
        public string ProfileId { get; set; }
        public string Content { get; set; }
        //
        public ProfileView Profile { get; set; }
    }
}
