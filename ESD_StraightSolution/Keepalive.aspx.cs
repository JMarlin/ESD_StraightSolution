using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/*
 * This page works with window_close_handler.js on the client side to keep
 * the list of open windows refreshed on the server side. 
 * Very simply, when this page is requested with a window ID, it calls
 * the window manager to make sure that that window's timestamp is 
 * up to date
 */

namespace ESD_StraightSolution {

    public partial class _Keepalive : System.Web.UI.Page {

        //We use this to pass whether the window refresh succeeded back to the client
        //in a more robust application, that information would actually be useful
        public bool refresh_succeeded;

        protected void Page_Load(object sender, EventArgs e) {

            //Try to get the windowid from the postdata and send it to the window manager
            //pass true to the client on success and false to the client on failure
            try {

                App_Code.WindowManager.refreshWindow(int.Parse(Request["windowid"]));
                refresh_succeeded = true;
            } catch {

                refresh_succeeded = false;
            }
        }
    }
}