using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Classy.DotNet.Mvc.ModelBinders
{
    public class CommaSeparatedToList : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueResult == null)
                return null;
            var commaSeparated = (string)valueResult.ConvertTo(typeof(string));
            if (string.IsNullOrWhiteSpace(commaSeparated))
                return null;

            List<string> vals = new List<string>();
            commaSeparated.Split(',').ToList().ForEach(x => vals.Add(x.Trim()));
            return vals.AsEnumerable<string>();
        }
    }
}
