using System.Web;
using System.Web.Optimization;

namespace MyHome
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js",
                        "~/Scripts/jquery.validate.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/TwitterBootstrapMvcJs.js"));

            bundles.Add(new StyleBundle("~/Content/css/rtl").Include(
                      "~/Content/rtl/bootstrap.css",
                      "~/Content/rtl/site.css"));

            bundles.Add(new StyleBundle("~/Content/css/ltr").Include(
                      "~/Content/ltr/bootstrap.min.css",
                      "~/Content/ltr/site.css"));
        }
    }
}
