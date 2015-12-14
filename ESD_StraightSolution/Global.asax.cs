using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using Dwg = System.Drawing;

/*
 * We're using the global class to initialize the window manager and load the
 * image file from the filesystem (or the database, if it already exists in there)  
*/

namespace ESD_StraightSolution
{

    public class Global : HttpApplication
    {

        //Create a global to store the image loaded from disk as well as easily
        //configureable constants for the names of the disk image and database image
        //(we load the disk image from the fs globally so that its availible for
        //the window close event to write to the database without reloading from disk)
        public static Dwg.Bitmap disk_image;
        public const String image_name = "amnesiac.jpg";
        public const String dbimage_name = "hailtothetheif.jpg";

        void Application_Start(object sender, EventArgs e)
        {
            
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //If the image isn't already in the database, attempt to load the image from the filesystem
            //when the application starts, and leave the image object null if it can't be done
            if (!App_Code.DBInterface.DBImageExists(image_name)) {
                
                try {

                    //Open the file as a stream, then decode it to a GDI bitmap
                    Stream bmp_stream = System.IO.File.Open(Server.MapPath("~/Images/" + image_name), System.IO.FileMode.Open);
                    disk_image = new Dwg.Bitmap(bmp_stream);
                } catch {

                    disk_image = null;
                }
            } else {

                disk_image = null;
            }
            
            //Set up the module that will keep track of open client windows
            App_Code.WindowManager.Init();

            System.Diagnostics.Debug.WriteLine("Application Start");
        }
    }
}