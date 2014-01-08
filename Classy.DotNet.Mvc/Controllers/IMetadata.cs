using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.Controllers
{
    // marker interface for constraining types passed into generic controllers
    public interface IMetadata<TMetadata> where TMetadata : new()
    {
        IDictionary<string, string> ToDictionary();
        TMetadata FromDictionary(IDictionary<string, string> metadata);
        string FilterMatch(string[] filters);
        string GetSlug();
    }
}
