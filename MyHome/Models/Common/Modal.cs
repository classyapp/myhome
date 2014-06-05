namespace MyHome.Models.Common
{
    public class Modal
    {
        public string Id { get; private set; }
        public string RemoteUrl { get; private set; }

        public Modal(string id, string remoteUrl)
        {
            Id = id;
            RemoteUrl = remoteUrl;
        }
    }
}