using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Responses
{
    public enum MediaFileType
    {
        Image = 1 << 0,
        File = 2
    }

    public class MediaFileView
    {
        public string Key { get; set; }
        public string ContentType { get; set; }
        public string Url { get; set; }
        //public IList<MediaFileThumbnail> Thumbnails { get; set; }
    }
}
