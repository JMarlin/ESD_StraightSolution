
/*
 *     This client-side code polls Keepalive.aspx in order to tell the server the page
 * is alive in a sort of deadman switch fashion. When the page stops sending its
 * window ID back to the server via that page request after two seconds the server
 * assumes (mostly safaly) that the window has been closed.
 *     It uses jQery for the nice lazy man's ajax and page load event abstraction, but
 * it wouldn't be strictly necessary otherwise
 */

var windowId;

//Send an async postback to the server with the window id the ASP page was generated with
function sendKeepalive() {

    $.ajax({
        type: "GET",
        url: "Keepalive.aspx",
        cache: false,
        contentType: "application/x-www-form-urlencoded; charset=utf-8",
        data: {windowid: windowId}
    });
};

//When the document is loaded, get the window ID value the page was generated with and 
//start the postback polling timer on a 1s interval
$(document).ready(function () {

    windowId = document.getElementById('MainContent_WindowID').value;
    setInterval(sendKeepalive, 1000);
});