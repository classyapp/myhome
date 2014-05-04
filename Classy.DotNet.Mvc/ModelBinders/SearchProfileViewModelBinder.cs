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
                // split filters url
                var filters = ((string)filtersValue.RawValue).Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                filters.ForEach((x) => { x = x.ToLower().Trim(); });
                // try parseing category
                model.Category = ParseCategory(filters);
                // try parsing city and country. fallback to selected country for user, or default country.
                string city, country, countryCode = null;
                ParseCityAndCountry(filters, out city, out country, out countryCode);
                model.CountryCode = countryCode;
                model.Country = country; 
                model.City = city;
                // unparsed part is considered a search query
                model.Name = ParseQuery(filters);
            }
            return model;
        }

        private string ParseCategory(IList<string> filters)
        {
            var categories = Localizer.GetList("professional-categories");
            string category = null;
            foreach (var val in filters)
            {
                if (categories.Any(x => x.Value.ToLower() == val))
                {
                    category = categories.Single(x => x.Value.ToLower() == val).Value;
                }
            }
            if (category != null) filters.Remove(category.ToLower());
            return category;
        }

        private void ParseCityAndCountry(IList<string> filters, out string city, out string country, out string countryCode)
        {
            country = null;
            city = null;
            countryCode = null;

            // parse country
            var countries = Localizer.GetList("supported-countries");
            foreach (var val in filters)
            {
                if (countries.Any(x => x.Value.ToLower() == val))
                {
                    var entry = countries.Single(x => x.Value.ToLower() == val);
                    country = entry.Text;
                    countryCode = entry.Value;
                    break;
                }
                else if (countries.Any(x => x.Text.ToLower() == val))
                {
                    var entry = countries.Single(x => x.Text.ToLower() == val);
                    country = entry.Text;
                    countryCode = entry.Value;
                    break;
                }
            }
            if (country != null)
            {
                filters.Remove(countryCode.ToLower());
                filters.Remove(country.ToLower());

                // parse city
                var cities = Localizer.GetCitiesByCountryCode(countryCode);
                foreach (var val in filters)
                {
                    if (cities.Any(x => x.ToLower() == val))
                    {
                        city = cities.First(x => x.ToLower() == val);
                        break;
                    }
                }
                if (city != null) filters.Remove(city.ToLower());
            }
        }

        private string ParseQuery(IList<string> filters)
        {
            return filters.Count() > 0 ? filters[0] : null;
        }
    }
}
