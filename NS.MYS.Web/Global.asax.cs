using IdentitySample.Models;
using NS.MYS.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace IdentitySample
{
    // Note: For instructions on enabling IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=301868
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        void Session_Start(object sender, EventArgs e)
        {
            // your code here, it will be executed upon session start
        }

        public void Session_OnEnd()
        {
            if (Session["photos"] != null)
            {
                foreach (Photo photo in (List<Photo>)Session["photos"])
                {
                    System.IO.File.Delete(Path.Combine(Server.MapPath("~/UploadedFiles"), photo.PhotoId));
                }
            }

            if (Session["photosEdit"] != null)
            {
                foreach (Photo photo in (List<Photo>)Session["photosEdit"])
                {
                    System.IO.File.Delete(Path.Combine(Server.MapPath("~/UploadedFiles"), photo.PhotoId));
                }
            }
        }
    }
}
