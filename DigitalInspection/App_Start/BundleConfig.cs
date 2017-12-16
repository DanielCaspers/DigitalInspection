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
			const string TYPESCRIPT_DIR_PREFIX = "~/Scripts/compiledTS";
			const string STYLE_LIB_DIR_PREFIX = "~/Content/Library/";

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
					LIB_DIR_PREFIX + "JQuery-Bootstrap-Scrolling-Tabs/jquery.scrolling-tabs.min.js",
					LIB_DIR_PREFIX + "DataTables/jquery.dataTables.min.js",
					LIB_DIR_PREFIX + "DataTables/dataTables.select.min.js",
					LIB_DIR_PREFIX + "Bootstrap-Material/material.js",
					LIB_DIR_PREFIX + "Bootstrap-Material/ripples.js",
					LIB_DIR_PREFIX + "Bootstrap-Multiselect/bootstrap-multiselect.js",
					LIB_DIR_PREFIX + "Bootstrap-Select/bootstrap-select.min.js",
					LIB_DIR_PREFIX + "WebcamJS/webcam.min.js"
				)
			);

			bundles.Add(new ScriptBundle("~/bundles/App")
				.IncludeDirectory(TYPESCRIPT_DIR_PREFIX + "/Shared/Services", "*.js")
				.IncludeDirectory(TYPESCRIPT_DIR_PREFIX + "/Views", "*.js")

			//.IncludeDirectory(TYPESCRIPT_DIR_PREFIX, "*.service.js.map")
			//.IncludeDirectory(APP_DIR_PREFIX, "*.service.ts")
			);

			bundles.Add(new StyleBundle("~/Content/css").Include(
					STYLE_LIB_DIR_PREFIX + "bootstrap.css",
					STYLE_LIB_DIR_PREFIX + "bootstrap-material-design.css",
					STYLE_LIB_DIR_PREFIX + "bootstrap-multiselect.css",
					STYLE_LIB_DIR_PREFIX + "bootstrap-select.min.css",
					STYLE_LIB_DIR_PREFIX + "material-icons.css",
					STYLE_LIB_DIR_PREFIX + "ripples.css",
					// Scrolling tabs is not included here because it is imported via SASS into its component
					"~/Content/fonts/materialdesignicons.min.css",
					"~/Content/site.css"));
		}
	}
}
