using Classy.DotNet.Responses;
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
        /// <summary>
        /// parses strings passed in from the controller into the metadata properties
        /// </summary>
        /// <param name="filters">the strings parsed from the request url by the controller</param>
        /// <returns></returns>
        void ParseSearchFilters(string[] filters, out string keyword, ref LocationView location);
        /// <summary>
        /// constructs a url slug from the metadata properties
        /// </summary>
        /// <returns>the url slug in the format {property1}/{property2}/{property2}/...</returns>
        string GetSearchFilterSlug(string keyword, LocationView location);
    }
}
