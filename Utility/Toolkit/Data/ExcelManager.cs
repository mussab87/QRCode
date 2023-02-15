using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Core.Logging;

namespace Utility.Toolkit.Data
{
    public static class ExcelManager
    {
        public const string Provider = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES;IMEX=1;';";

        public static DataSet ExcelToDS(string path)
        {
            DataSet ds = null;

            FileTrace.WriteMemberEntry();

            try
            {
                //Create the connection to the excel file using OLEDB
                using (var conn = new OleDbConnection(String.Format(Provider, path)))
                {
                    //Open the connection
                    conn.Open();
                    
                    //var dtSchema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

                    //var sheet1 = dtSchema.Rows[0].Field<string>("TABLE_NAME");

                    //query excel sheet
                    string strExcel = "select * from [sheet1$]";
                    
                    //create data adapter command
                    OleDbDataAdapter myCommand = new OleDbDataAdapter(strExcel, conn);
                    
                    // initialize dataset
                    ds = new DataSet();

                    //call the fill method to get all the columns from excel to data set table
                    myCommand.Fill(ds, "table1");
                }

                //delete the file after importing into excel
                File.Delete(path);
            }
            catch (Exception ex)
            {
                FileTrace.WriteException(ex);                
            }
            FileTrace.WriteMemberExit();

            return ds;
        }

    }
}
