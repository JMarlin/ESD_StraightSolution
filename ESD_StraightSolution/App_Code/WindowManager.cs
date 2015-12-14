using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/*     This is a subsystem for keeping track of open instances of pages on the site.
 * It accomplishes this in combination with the content of window_close_handler.js
 * and Keepalive.aspx, the first of which causes the loaded page to generate 
 * regular 1s requests for the latter. 
 *     When a page wants to use this feature to detect whether it has open instances
 * it uses this code to register a new window instance, which creates a window ID
 * and puts that ID into a hash table, the content of each entry being a DateTime object
 * which is updated to the current time each time that client window causes a
 * Keepalive.aspx request
 *     Finally, when this code is initialized it starts a timer which cycles every two
 * seconds through all of the current entries in the table of window ids and, for each
 * window which is detected to not have responded in over 2s, it fires a window close
 * function
 * 
 *     For the moment, this is kind of overkill for the task at hand of causing an event
 * to occur server-side when the user window is closed, but would allow for future expansion
 * into more sensical uses of such functionality which would probably want to keep close 
 * events tied to their generating client page instances. Also, this polling method
 * is pretty much the only definitive way to detect such an event since firing a page request
 * on a javascript unload event is almost completely guaranteed to fail and waiting for 
 * an ASP session to time out (default 20 minutes of no server requests) doesn't seem to
 * fit the spirit of the phrase 'when a window closes' -- not only would it be too slow, but
 * it would be per user session and not per window.
 */
  
namespace ESD_StraightSolution.App_Code {

    public class WindowManager {

        //We need a dictionary for the window instances, a counter to keep track of the last
        //assigned ID, and a timer to do the regular window check
        private static Dictionary<int, DateTime> session_counter;
        private static int next_windowid;
        private static System.Timers.Timer session_timer;

        //This function gets fired whenever a window close event is detected. For now, it is
        //statically programmed to insert the image loaded from the filesystem into the DB
        //but could be set up in the future to do something more interesting per window ID
        private static void handleWindowClose(int window_id) {

            //Do some logging just so we can see that the polling method is working
            System.Diagnostics.Debug.WriteLine("Window {0} closed.", window_id);

            //Check to see if the image already exists in the DB and, if not, insert it. 
            //Finally, log an error in the case that the insertion fails.
            if (!DBInterface.DBImageExists(Global.image_name))
                if (!DBInterface.insertDBImage(Global.disk_image, Global.image_name))
                    System.Diagnostics.Debug.WriteLine("There was an error when trying to insert image '{0}' into the database.", Global.image_name);
        }

        //This should be called from the page which will be polled from the client, and
        //will update the client's window instance in the table
        public static void refreshWindow(int window_id) {

            //Do nothing if there's no such active window_id int the table
            if (session_counter.ContainsKey(window_id)) {

                //Update the entry and log to show that the polling method is working
                session_counter[window_id] = DateTime.Now;
                System.Diagnostics.Debug.WriteLine("Window {0} refreshed.", window_id);
            }
        }

        //This should be called on page draw by the page which wants to keep track of its open instances
        //It increments the ID, creates a new table entry for it, and returns the new window ID to be
        //passed to the new client
        public static int registerNewWindow() {

            //Log this event to show that the polling method is working
            System.Diagnostics.Debug.WriteLine("Created window {0}.", next_windowid);
            
            //Create the new entry, return its ID, and increment the ID counter
            session_counter.Add(next_windowid, DateTime.Now);
            return next_windowid++;
        }

        //This is called every 2s from the timer and scans the window instance table for stale handles
        private static void checkPageLifetimes(Object source, System.Timers.ElapsedEventArgs e) {

            List<int> delete_keys = new List<int>();

            //Log this event to show that the polling method is working
            System.Diagnostics.Debug.WriteLine("Checking window statuses.");

            //Iterate through the open window instances and run the window close function on any
            //that are over 2s stale. Also add each stale window to a list to be removed from the table
            foreach (KeyValuePair<int, DateTime> val in session_counter) {

                if (val.Value < DateTime.Now.Add(new TimeSpan(0, 0, -2))) {

                    handleWindowClose(val.Key);
                    delete_keys.Add(val.Key);
                }
            }

            //Remove the stale handles from the table
            foreach (int val in delete_keys)
                session_counter.Remove(val);
        }

        //Set the default values for this module: Set up the window table, init the 
        //id counter, and start the timer
        public static void Init() {

            session_counter = new Dictionary<int, DateTime>();
            next_windowid = 1;
            session_timer = new System.Timers.Timer(2000);
            session_timer.Elapsed += checkPageLifetimes;
            session_timer.AutoReset = true;
            session_timer.Start();
        }
    }
}