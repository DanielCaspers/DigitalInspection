using System.Web;
using System.Web.Optimization;

namespace DigitalInspection
{
	public class BundleConfig
	{

		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			const string LIB_DIR_PREFIX = "~/Scripts/Library/";

			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
						LIB_DIR_PREFIX + "JQuery/jquery-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/jquery-ajax").Include(
						LIB_DIR_PREFIX + "JQuery-Ajax/jquery.unobtrusive-ajax.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
						LIB_DIR_PREFIX + "JQuery-Validate/jquery.validate*"));

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
						LIB_DIR_PREFIX + "Modernizr/modernizr-*"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
					  LIB_DIR_PREFIX + "Bootstrap/bootstrap.js",
					  LIB_DIR_PREFIX + "Bootstrap/respond.js"));

			bundles.Add(new ScriptBundle("~/bundles/material").Include(
						LIB_DIR_PREFIX + "Bootstrap-Material/material.js",
						LIB_DIR_PREFIX + "Bootstrap-Material/ripples.js"));

			bundles.Add(new ScriptBundle("~/bundles/multiselect").Include(
						LIB_DIR_PREFIX + "Bootstrap-Multiselect/bootstrap-multiselect.js"));

			//bundles.Add(new ScriptBundle("~/bundles/app").IncludeDirectory(
			//			"~/Scripts/App/", "*.js"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
					  "~/Content/bootstrap.css",
					  "~/Content/bootstrap-material-design.css",
					  "~/Content/bootstrap-multiselect.css",
					  "~/Content/ripples.css",
					  "~/Content/site.css"));
		}
	}
}
