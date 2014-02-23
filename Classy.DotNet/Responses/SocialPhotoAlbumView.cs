using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Responses
{
    public class SocialPhotoView
    {
        public string Id { get; set; }
        public string Url { get; set; }
    }
    public class SocialPhotoAlbumView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Count { get; set; }
        public List<SocialPhotoView> Photos { get; set; }
    }
}
