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
                model.Country = ParseCountry(filters);
                if (!string.IsNullOrEmpty(model.Country)) model.City = ParseCity(filters, model.Country);
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

        private string ParseCountry(IList<string> filters)
        {
            var countries = Localizer.GetList("supported-countries");
            string country = null;
            foreach(var val in filters)
            {
                if (countries.Any(x => x.Value.ToLower() == val))
                {
                    country = countries.Single(x => x.Value.ToLower() == val).Value;
                    break;
                }
            }
            if (country != null) filters.Remove(country.ToLower());
            return country;
        }

        private string ParseCity(IList<string> filters, string country)
        {
            var cities = Localizer.GetList("supported-cities");
            string city = null;
            foreach (var val in filters)
            {
                if (cities.Any(x => x.Value.ToLower() == val))
                {
                    var cityItem = cities.SingleOrDefault(x => x.Value.ToLower() == val && x.ParentValue == country.ToUpper());
                    if (cityItem != null) city = cityItem.Value;
                    break;
                }
            }
            if (city != null) filters.Remove(city.ToLower());
            return city;
        }

        private string ParseQuery(IList<string> filters)
        {
            return filters.Count() > 0 ? filters[0] : null;
        }
    }
}
