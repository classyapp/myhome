using Classy.DotNet.Responses;
using System.Security.Principal;

namespace Classy.DotNet.Security
{
    public class ClassyIdentity : IIdentity
    {
        public string AuthenticationType { get { return "classy API"; } }

        public bool IsAuthenticated { get; set; }

        public string Name { get; set; }

        public ProfileView Profile { get; set; }
    }
}