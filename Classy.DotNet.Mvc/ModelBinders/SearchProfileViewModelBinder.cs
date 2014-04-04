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
                string city, country = null;
                ParseCityAndCountry(filters, out city, out country);
                model.Country = country;
                model.City = city;
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

        private void ParseCityAndCountry(IList<string> filters, out string city, out string country)
        {
            country = null;
            city = null;

            // parse country
            var countries = Localizer.GetList("supported-countries");
            foreach (var val in filters)
            {
                if (countries.Any(x => x.Value.ToLower() == val))
                {
                    country = countries.Single(x => x.Value.ToLower() == val).Value;
                    break;
                }
            }
            if (country != null) filters.Remove(country.ToLower());

            // parse city
            var cities = Localizer.GetList("supported-cities");
            var lCountry = country;
            if (!string.IsNullOrEmpty(lCountry)) cities = cities.Where(x => x.ParentValue == lCountry.ToUpper());
            foreach (var val in filters)
            {
                if (cities.Any(x => x.Value.ToLower() == val))
                {
                    var cityItem = cities.SingleOrDefault(x => x.Value.ToLower() == val);
                    if (cityItem != null)
                    {
                        city = cityItem.Value;
                        country = cityItem.ParentValue.ToUpper();
                    }
                    break;
                }
            }
            if (city != null) filters.Remove(city.ToLower());
        }

        private string ParseQuery(IList<string> filters)
        {
            return filters.Count() > 0 ? filters[0] : null;
        }
    }
}
