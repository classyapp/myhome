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
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.Name == "Contacts" && propertyDescriptor.PropertyType == typeof(IList<string>))
            {
                var valueResult = bindingContext.ValueProvider.GetValue(propertyDescriptor.Name);
                if (valueResult != null)
                {
                    var commaSeparated = (string)valueResult.ConvertTo(typeof(string));
                    if (!string.IsNullOrWhiteSpace(commaSeparated))
                    {
                        List<string> vals = new List<string>();
                        commaSeparated.Split(',').ToList().ForEach(x => vals.Add(x.Trim()));

                        propertyDescriptor.SetValue(bindingContext.Model, vals);
                    }
                }
            }
            else
            {
                base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
            }
        }
    }
}
