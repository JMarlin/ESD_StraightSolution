# ESD_StraightSolution
This application was split into the global.asax, two aspx pages, two helper classes, a single JS support script, and a small smattering of css for flavor. It displays two images each at three different scaling factors. These images are loaded from either from the server's filesystem or a local database and are handled in memory via GDI and finally rendered into the page as base-64 inline resource strings. The application also supports tracking open client windows via an asynchronous polling method and allows for firing a routine, which in this case inserts the image which was loaded from the filesystem into the database, when a client window ceases polling.

##Global.asax
We use the app startup handler in the global class to load our filesystem image (so that we don't have to keep looking for it on every page request, as well as so that it has a common location that can be accessed by both the code which writes that image data to the local database and the main application page) and start the window manager module which is further described under **WindowManager.cs**.

##Default.aspx
The primary aspx page is the Default.aspx and is the only one to serve user content. Its backing C# file handles, on page request, getting the images objects to be displayed from their respective stores, scaling them using GDI, and converting them into page-embedded base-64 encoded resource strings. For its first image file, it attempts to use the image loaded by **Global.asax** on startup, however if that fails it then attempts to find that image in the local database. The second image is always loaded from the database. As a very last step in rendering, it uses **WindowManager.cs** to register the served page as a client window to the server and embed a unique window id into the client for passback when keepalive-polling. This page also imports **window_close_handler.js** which contains the client-side polling script.

##Keepalive.aspx
This page simply makes a call to **WindowManager.cs** when requested which causes the window management system to refresh the timestamp on its internal representation of that client window.

##DBInterface.cs
This class simply acts as a wrapper for the local database, allowing for simplified detect, get, and insert operations on database-stored image data.

##WindowManager.cs
This class encapsulates the functionality which makes our polling-based client window tracking work. When initialized, it starts a timer which scans a table of registered client windows every two seconds. For each window it finds which has an expired last-refresh timestamp, it executes a window-closed routine and then deletes that entry from the list of open windows. It works with **Keepalive.aspx** as well as **window_close_handler.js** to keep that list refreshed.

##window_close_handler.js
This is a simple jQuery-based javascript file which, on page load, retrieves the window id embedded at template render time into the served page, and then initiates a timer which regularly requests **Keepalive.aspx** with that window id to in turn tell **WindowManager.cs** that the page is still open.
