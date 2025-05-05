using System.Web;
using System.Web.Optimization;
using System.Web.SessionState;

namespace EDM
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));
            //string pMain = "~/templates/assets/css/main";
            //bundles.Add(new StyleBundle(pMain).Include(
            //    $"{pMain}/app-dark.css",
            //    $"{pMain}/app.css"));
            //string pPages = "~/templates/assets/css/pages";
            //bundles.Add(new StyleBundle(pPages).Include(
            //    $"{pPages}/auth.css",
            //    $"{pPages}/chat.css",
            //    $"{pPages}/datatables.css",
            //    $"{pPages}/dripicons.css",
            //    $"{pPages}/email.css",
            //    $"{pPages}/error.css",
            //    $"{pPages}/filepond.css",
            //    $"{pPages}/fontawesome.css",
            //    $"{pPages}/form-element-select.css",
            //    $"{pPages}/quill.css",
            //    $"{pPages}/rater-js.css",
            //    $"{pPages}/simple-datatables.css",
            //    $"{pPages}/summernote.css",
            //    $"{pPages}/sweetalert2.css",
            //    $"{pPages}/toastify.css"));
            //string pShared = "~/templates/assets/css/shared";
            //bundles.Add(new StyleBundle(pShared).Include(
            //   $"{pShared}/iconly.css"));
            //string pWidgets = "~/templates/assets/css/widgets";
            //bundles.Add(new StyleBundle(pWidgets).Include(
            //   $"{pWidgets}/chat.css",
            //   $"{pWidgets}/todo.css"));
        }
        public void Role(string role)
        {
        }
    }
}
