﻿using Classy.DotNet.Mvc.Controllers;
using Classy.DotNet.Mvc.ViewModels.Listing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyHome.Models
{
    public class ProductMetadata : IMetadata<ProductMetadata>
    {
        public IDictionary<string, string> ToDictionary()
        {
            return null;
        }

        public ProductMetadata FromDictionary(IDictionary<string, string> metadata)
        {
            return null;
        }



        public string FilterMatch(string[] filters)
        {
            throw new NotImplementedException();
        }


        public string GetSlug()
        {
            throw new NotImplementedException();
        }
    }
}