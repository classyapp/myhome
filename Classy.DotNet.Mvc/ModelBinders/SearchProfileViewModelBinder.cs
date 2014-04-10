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
                string city, country, countryCode = null;
                ParseCityAndCountry(filters, out city, out country, out countryCode);
                model.Country = country;
                model.CountryCode = countryCode;
                model.City = city;
                model.Name = ParseQuery(filters);
            }
            else
            {
                if (!string.IsNullOrEmpty(model.City))
                {
                    string[] cityParts = model.City.Split(',');
                    var countries = Localizer.GetList("supported-countries");
                    if (cityParts.Length > 1)
                    {
                        if (countries.Any(x => x.Text.ToLower() == cityParts[cityParts.Length - 1].Trim().ToLower()))
                        {
                            model.Country = countries.Single(x => x.Text.ToLower() == cityParts[cityParts.Length - 1].Trim().ToLower()).Value;
                        }
                    }
                    else
                    {
                        model.Country = null;
                    }
                    model.City = cityParts.Length > 1 ? string.Join(",", cityParts.Take(cityParts.Length - 1)).Trim() : model.City.Trim();
                }
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
            }
            if (country != null)
            {
                filters.Remove(countryCode.ToLower());

                // parse city
                var cities = Localizer.GetCitiesByCountryCode(countryCode);
                foreach (var val in filters)
                {
                    if (cities.Any(x => x.ToLower() == val))
                    {
                        city = cities.Single(x => x.ToLower() == val);
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
