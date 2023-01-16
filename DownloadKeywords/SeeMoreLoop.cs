using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DownloadKeywords
{
    public class SeeMoreLoop
    {
        private static string StrConn()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                string fileName = @"C:\Inetpub\wwwroot\Callback_TrackingTrending.xml";

                // You'll need to put the correct path to your xml file here
                xml.Load(fileName);

                // Select a specific node 
                XmlNode node = xml.SelectSingleNode("ConnectionString/con");

                // Get its value
                string name = node.InnerText;

                return name;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task RunSqlJob(int number)
        {
            await Task.Delay(number);
            try
            {
                using (SqlConnection DbConn = new SqlConnection(StrConn()))
                {
                    SqlCommand ExecJob = new SqlCommand();
                    ExecJob.CommandType = CommandType.StoredProcedure;
                    ExecJob.CommandText = "msdb.dbo.sp_start_job";
                    ExecJob.Parameters.AddWithValue("@job_name", "SeeMore_Loop");
                    ExecJob.Connection = DbConn;

                    DbConn.Open();
                    using (ExecJob)
                    {
                        ExecJob.ExecuteNonQuery();

                    }
                    Console.WriteLine("Job is sucessful");
                    System.Threading.Thread.Sleep(2000);
                }
            }
            catch (SqlException ex)
            {
                string errorMessage = "Database Error: \r\n";
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessage += "Index #" + i + "\n" +
                                     "Message: " + ex.Errors[i].Message + "\n" +
                                     "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                     "Source: " + ex.Errors[i].Source + "\n" +
                                     "Procedure: " + ex.Errors[i].Procedure + "\n" +
                                     "Server: " + ex.Errors[i].Server + "\n";
                }

                throw new Exception(errorMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
