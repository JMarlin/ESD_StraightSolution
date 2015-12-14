using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Dwg = System.Drawing;
using System.Drawing.Imaging;
using System.IO;

/*
 * This is a light abstraction on some of the database operations we need to do
 * in this application, including:
 *     - Simple queries
 *     - Opening image data from the image table
 *     - Checking for the existance of a image in the image table
 *     - Inserting JPEG data into the database
 */
namespace ESD_StraightSolution.App_Code {
    
    public class DBInterface {

        //Store a common reference to the connection string so that we don't have to repeat it later
        private const String connect_string = @"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Database1.mdf';Integrated Security=True";

        //This function abstracts the common action of opening a database connection, executing a SQL
        //query, and returning the data in the form of a DataTable
        public static DataTable doQuery(string query) {

            DataSet ds = new DataSet();
            SqlDataAdapter da;

            //Attempt to complete the query and return an empty table if there are any errors 
            try {
                
                //Create a connection and execute the query into the DataTable
                using (SqlConnection conn = new SqlConnection(connect_string)) {

                    da = new SqlDataAdapter(query, conn);
                    ds.Reset();
                    da.Fill(ds);
                }

                //Return the data table if the query actually returned data
                if(ds.Tables.Count > 0)
                    return ds.Tables[0];
            } catch {

                //Failure mode is a blank table
                return new DataTable();
            }

            //If we didn't get any data, default to returning a blank table
            return new DataTable();
        }


        //This is a simple abstraction for opening image data from the image table in the local database
        //via reference to the name field
        public static Dwg.Bitmap openDBImage(String name) {

            //Get the top matching result for the provided image name
            DataTable dt = doQuery(String.Format("select top(1) data from dbo.images where name = '{0}';", name.Replace("'", "''")));

            //Failure mode is a null bitmap
            if (dt == null || dt.Rows.Count == 0)
                return null;

            //Attempt to decode the returned binary blob into a GDI bitmap, returning a null object on failure
            try {

                //Data column -> byte array -> memory stream -> gdi bitmap
                return new Dwg.Bitmap(new MemoryStream((byte[])dt.Rows[0]["data"]));
            } catch {

                return null;
            }
        }

        //Abstraction for detecting the existance of an existing database image as defined by its name
        public static bool DBImageExists(String dwg_name) {

            DataTable dt =  doQuery(String.Format("select name from dbo.images where name = '{0}';", dwg_name.Replace("'", "''")));

            return !(dt == null || dt.Rows.Count == 0);
        }

        //Abstraction for converting a GDI bitmap into a raw JPEG binary stream and subsequently inserting
        //that data into a new row of the image table
        public static bool insertDBImage(Dwg.Bitmap src_bmp, String dwg_name) {

            //Attempt to execute the query, returning true on success and false on failure
            try {

                //Open an SQL connection, a memory stream for the JPEG data, and finally a
                //SQL command which will process the memory stream
                using (SqlConnection conn = new SqlConnection(connect_string)) {
                    
                    using (MemoryStream memStream = new MemoryStream()) {
                        
                        using (SqlCommand cmd = new SqlCommand(String.Format("insert into dbo.images (name, data) values ('{0}', @bindata)", dwg_name.Replace("'", "''")), conn)) {

                            //Save the bitmap in JPEG format to the memory stream, then insert the byte array of the stream into
                            //the query, and finally attempt to open the connection and execute the query
                            src_bmp.Save(memStream, ImageFormat.Jpeg);
                            cmd.Parameters.Add("@bindata", SqlDbType.VarBinary, -1).Value = memStream.ToArray(); //-1 size on a blob field is equivalent to MAX
                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }

                        //Success signal
                        return true;
                    }
                }
            } catch {

                //Failure signal
                return false;
            }
        }
    }
}