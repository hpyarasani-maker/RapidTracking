using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace RapidTrackingSingleThread
{
    public class MissingKeywordsJob
    {

        public static async Task ExecuteMissingKeywordsJob(int number)
        {
            try
            {
                // Delete keywords
                ProcessDB("Exec [dbo].[DeletetrackingKeywords5]");

                // Download keywords
                await DownloadRemainingKeywords();

                // Running sql job
                await RunSqlJob(number);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private static async Task RunSqlJob(int number)
        {
            await Task.Delay(number);
            try
            {
                using (SqlConnection DbConn = new SqlConnection(Common.ReadConnection()))
                {                    
                    SqlCommand ExecJob = new SqlCommand();
                    ExecJob.CommandType = CommandType.StoredProcedure;
                    ExecJob.CommandText = "msdb.dbo.sp_start_job";
                    ExecJob.Parameters.AddWithValue("Delete_DB_Data_Sending", "TestJob");
                    ExecJob.Connection = DbConn; 
           
                    DbConn.Open();
                    using (ExecJob)
                    {
                        ExecJob.ExecuteNonQuery();

                    }
                    MessageBox.Show("Job is sucessful");
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

        private static async Task DownloadRemainingKeywords()
        {
            string myDate = DateTime.Today.ToString("yyyy-MM-dd");
            //string myDate = "2022-07-06"; //change the previous date

            string url = "https://incoming.pi-datametrics.com/provider-api/tracking/get-required-searches?date=" + myDate + "&remaining-only=true"; //true means only remaining and false means all keywords

            string authInfo = "pisoftware" + ":" + "r00t123456";
            Uri uri = new Uri(url);
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            httpWebRequest.Headers["Authorization"] = "Basic " + authInfo;
            XmlDocument doc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings { CheckCharacters = false };
            //MessageBox.Show("Downloading keywords...");
            using (HttpWebResponse response = (HttpWebResponse) await httpWebRequest.GetResponseAsync())
            using (XmlReader reader = XmlReader.Create(response.GetResponseStream(), settings))
            {
                try
                {
                    doc.Load(reader);
                    reader.Close();

                    StringBuilder qry = new StringBuilder();
                    XmlNodeList seid = doc.GetElementsByTagName("searchEngineId");
                    XmlNodeList kwd = doc.GetElementsByTagName("keyword");

                    var dt = new DataTable();
                    dt.Columns.Add("seid");
                    dt.Columns.Add("name");

                    for (var i = 0; i < kwd.Count; i++)
                    {
                        dt.Rows.Add(seid[i].InnerText, kwd[i].InnerText);
                        Console.WriteLine(i.ToString());
                    }
                    if (dt.Rows.Count > 0)
                        using (var sqlBulk = new SqlBulkCopy(Common.buffaloConn())) //bufflao connection
                        {
                            sqlBulk.BulkCopyTimeout = 0;
                            sqlBulk.DestinationTableName = "tracking_keywords5"; 
                            sqlBulk.WriteToServer(dt);
                        }

                    //Console.WriteLine("Keywords downloaded.");
                    //MessageBox.Show("Keywords Downloaded");
                    //Environment.Exit(0);

                }
                catch (SqlException se)
                {
                    string errMsg = "Database Connection is temporarily not working\n" + se.ToString();
                    //Console.WriteLine("SQL Error: " + errMsg);
                    MessageBox.Show("SQL Error: " + errMsg);

                    throw se;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    throw ex;
                }
            }
        }

        private static void ProcessDB(string qry)
        {
            using (SqlConnection con = new SqlConnection(Common.buffaloConn()))
            {
                con.Open();
                using (SqlCommand comm = con.CreateCommand())
                {
                    comm.CommandType = CommandType.Text;
                    comm.CommandText = qry;
                    comm.CommandTimeout = 0;
                    comm.ExecuteNonQuery();
                }
            }
        }

    }
}
