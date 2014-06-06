using System.Collections.Generic;

namespace Classy.DotNet.Models.LogActivity
{
    public class LogActivityResponse
    {
        public string UserId { get; set; }
        public string Activity { get; set; }
        public string ObjectId { get; set; }
        public Dictionary<string, string> Metadata { get; set; }

        public LogActivityResponse()
        {
            Metadata = new Dictionary<string, string>();
        }
    }

    public class LogActivity<T> where T : ILogActivityMetadata<T>, new()
    {
        public string UserId { get; set; }
        public string Activity { get; set; }
        public string ObjectId { get; set; }
        public T Metadata { get; set; }

        public LogActivity()
        {
            Metadata = new T();
        }
    }

    public interface ILogActivityMetadata<T>
    {
        T DeserializeMetadata(Dictionary<string, string> metadata);
        Dictionary<string, string> SerializeMetadata();
    }
}