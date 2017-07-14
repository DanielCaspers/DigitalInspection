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
			const string APP_DIR_PREFIX = "~/Scripts/App/Shared/Services";

			bundles.Add(
				new ScriptBundle("~/bundles/Library").Include(
					LIB_DIR_PREFIX + "JQuery/jquery-{version}.js",
					LIB_DIR_PREFIX + "JQuery-Ajax/jquery.unobtrusive-ajax.js",
					LIB_DIR_PREFIX + "JQuery-DirtyForms/jquery.dirtyforms.min.js",
					LIB_DIR_PREFIX + "JQuery-DirtyForms/jquery.dirtyforms.dialogs.bootstrap.js",
					LIB_DIR_PREFIX + "JQuery-Validate/jquery.validate*",
					// Use the development version of Modernizr to develop with and learn from. Then, when you're
					// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
					LIB_DIR_PREFIX + "Modernizr/modernizr-*",
					LIB_DIR_PREFIX + "Bootstrap/bootstrap.js",
					LIB_DIR_PREFIX + "Bootstrap/respond.js",
					LIB_DIR_PREFIX + "DataTables/jquery.dataTables.min.js",
					LIB_DIR_PREFIX + "DataTables/dataTables.select.min.js",
					LIB_DIR_PREFIX + "Bootstrap-Material/material.js",
					LIB_DIR_PREFIX + "Bootstrap-Material/ripples.js",
					LIB_DIR_PREFIX + "Bootstrap-Multiselect/bootstrap-multiselect.js"
				)
			);



			bundles.Add(new ScriptBundle("~/bundles/App").IncludeDirectory(
						APP_DIR_PREFIX, "*.service.js"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
					  "~/Content/bootstrap.css",
					  "~/Content/bootstrap-material-design.css",
					  "~/Content/bootstrap-multiselect.css",
					  "~/Content/material-icons.css",
					  "~/Content/ripples.css",
					  "~/Content/site.css"));
		}
	}
}
