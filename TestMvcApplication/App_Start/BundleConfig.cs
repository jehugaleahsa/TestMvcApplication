using System;
using System.Web.Optimization;

namespace TestMvcApplication
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/base").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery.validate.js",
                "~/Scripts/jquery-ui-{version}.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/knockout-{version}.js"
            ));
            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                "~/Content/bootstrap.css"
            ));
            bundles.Add(new StyleBundle("~/Content/themes/base/jquery-ui").IncludeDirectory("~/Content/themes/base/", "jquery-ui.*"));
            bundles.IgnoreList.Clear();
        }
    }
}