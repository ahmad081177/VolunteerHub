using System;
using System.IO;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;

namespace VolunteerHub
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Ensure upload directories exist
            EnsureDirectory(Server.MapPath("~/Uploads/ProfileImages"));
            EnsureDirectory(Server.MapPath("~/Uploads/WorkspaceLogos"));

            // Initialize database tables if not yet created
            DAL.DatabaseInitializer.Initialize();
        }

        private static void EnsureDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}