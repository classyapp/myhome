using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Classy.DotNet.Mvc.ViewModels.Profiles
{
    public enum CatalogTemplateType
    {
        HomeLab,
        Amazon
    }

    public class UploadProductCatalogViewModel
    {
        public CatalogTemplateType CatalogTemplateType { get; set; }
        public bool OverwriteListings { get; set; }
        public bool UpdateImages { get; set; }
    }
}