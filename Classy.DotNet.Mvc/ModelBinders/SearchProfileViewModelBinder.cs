using Classy.DotNet.Mvc.Controllers;
using Classy.DotNet.Mvc.Localization;
using Classy.DotNet.Mvc.ViewModels.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Classy.DotNet.Mvc.ModelBinders
{
    public class SearchProfileViewModelBinder<TProMetadata> : DefaultModelBinder
        where TProMetadata : IMetadata<TProMetadata>, new()
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            SearchProfileViewModel<TProMetadata> model = base.BindModel(controllerContext, bindingContext) as SearchProfileViewModel<TProMetadata>;
            var filtersValue = bindingContext.ValueProvider.GetValue("filters");
            if (filtersValue != null && !string.IsNullOrEmpty(filtersValue.RawValue as string))
            {
                var filters = ((string)filtersValue.RawValue).Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                filters.ForEach((x) => { x = x.ToLower(); });
                model.Category = ParseCategory(filters);
                model.Location = ParseLocation(filters);
                model.Name = ParseQuery(filters);
            }
            return model;
        }

        private string ParseCategory(IList<string> filters)
        {
            var categories = Localizer.GetList("professional-categories");
            string category = null;
            foreach(var val in filters)
            {
                if (categories.Any(x => x.Value.ToLower() == val))
                {
                    category = categories.Single(x => x.Value.ToLower() == val).Value;
                }
            }
            if (category != null) filters.Remove(category.ToLower());
            return category;
        }

        private string ParseLocation(IList<string> filters)
        {
            var locations = Localizer.GetList("supported-countries");
            string location = null;
            foreach(var val in filters)
            {
                if (locations.Any(x => x.Value.ToLower() == val))
                {
                    location = locations.Single(x => x.Value.ToLower() == val).Value;
                    break;
                }
            }
            if (location != null) filters.Remove(location.ToLower());
            return location;
        }

        private string ParseQuery(IList<string> filters)
        {
            return filters.Count() > 0 ? filters[0] : null;
        }
    }
}
