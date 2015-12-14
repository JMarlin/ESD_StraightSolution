using System;
using System.IO;
using System.Data;
using Dwg = System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/*
 *     This is the backing code for the main (and basically only) page in this application.
 * It contains the loading event for the page which does the handling of the loaded images 
 * as requested by the spec and also contains helper functions to do the in-memory image
 * scaling and conversion of the GDI bitmaps this application uses into inline resources.
 *     It also calls the window manager to generate a new unique window entry to be used by
 * the polling client script for each page load so that we can handle writing the fs image
 * back to the hard drive on window close.
 */

namespace ESD_StraightSolution {

    public partial class _Default : Page {

        //This helper function creates a new bitmap and uses its GDI context
        //to blit the passed bitmap into itself at the area scale percentage specified
        protected Dwg.Bitmap scaleBmp(Dwg.Bitmap src_bmp, int scale_pct) {

            //If we're not passed anything, don't return anything
            if (src_bmp == null)
                return null;

            //Calculate the scaled side lengths based on the area scale, 
            double scale_factor = Math.Sqrt((double)scale_pct/100.00);
            int scaled_w = (int)((double)src_bmp.Width * scale_factor);
            int scaled_h = (int)((double)src_bmp.Height * scale_factor);


            //Be sure to not truncate decimals down to a zero-sized image
            scaled_w = scaled_w < 1 ? 1 : scaled_w;
            scaled_h = scaled_h < 1 ? 1 : scaled_h;

            //Generate the scaled result bitmap
            Dwg.Bitmap ret_bmp = new Dwg.Bitmap(scaled_w, scaled_h);

            //Get the new GDI context and blit the source bitmap into it
            using (Dwg.Graphics g = Dwg.Graphics.FromImage(ret_bmp))
                g.DrawImage(src_bmp, 0, 0, scaled_w, scaled_h);

            return ret_bmp;
        }

        //This helper function takes a GDI bitmap, serializes it to binary JPEG
        //data in a memory stream, and then creates a base64 inline image resource
        //string to be written to the page
        protected String BitmapToEmbedded(Dwg.Bitmap src_bmp) {

            //If we're passed nothing, return nothing
            if (src_bmp == null)
                return "";

            //Create a memory stream into which we'll write the JPEG encoded image data
            var memStream = new MemoryStream();

            //Write the data to the memory stream, then convert its byte array to a base64 
            //string and prepend the inline data header
            src_bmp.Save(memStream, ImageFormat.Jpeg);
            return "data:image/jpeg;base64," + Convert.ToBase64String(memStream.ToArray());
        }

        //On page load, we process the images, pass the generated inline image data to the
        //template engine, and finally register the page as a new unique client window
        protected void Page_Load(object sender, EventArgs e) {

            //Create objects for the two bitmaps we'll be rendering from
            Dwg.Bitmap original_bmp = null;
            Dwg.Bitmap dboriginal_bmp = null;

            //First, assume that we loaded the image from the fs at app start
            original_bmp = Global.disk_image;

            if (original_bmp == null) {

                //If the image wasn't preloaded from the fs at startup, we'll try to load 
                //it from the database. However, if that also fails, we'll update the image
                //labels to inform the user of this travesty.
                if ((original_bmp = App_Code.DBInterface.openDBImage(Global.image_name)) == null) {

                    OriginalLabel.Text =
                    SixtyLabel.Text =
                    TwentyFiveLabel.Text =
                    "Could not open image file!";
                }
            }

            //Try to load the database image and update the client's labels if that fails
            if ((dboriginal_bmp = App_Code.DBInterface.openDBImage(Global.dbimage_name)) == null)  {

                DBOriginalLabel.Text =
                DBSixtyLabel.Text =
                DBTwentyFiveLabel.Text =
                "Could not get image from database!";
            }
                
            //Send the encoded inline data to the template engine
            Original.ImageUrl = BitmapToEmbedded(original_bmp);
            Sixty.ImageUrl = BitmapToEmbedded(scaleBmp(original_bmp, 60));
            TwentyFive.ImageUrl = BitmapToEmbedded(scaleBmp(original_bmp, 25));
            DBOriginal.ImageUrl = BitmapToEmbedded(dboriginal_bmp);
            DBSixty.ImageUrl = BitmapToEmbedded(scaleBmp(dboriginal_bmp, 60));
            DBTwentyFive.ImageUrl = BitmapToEmbedded(scaleBmp(dboriginal_bmp, 25));

            //Register this instance of the page before we quit so we can get that
            //window-still-open postback
            WindowID.Value = App_Code.WindowManager.registerNewWindow().ToString();
        }
    }
}