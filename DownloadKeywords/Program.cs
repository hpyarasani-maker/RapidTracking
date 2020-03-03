using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DownloadKeywords
{
    class Program
    {
        //private static string strConn = "Data Source=82.136.46.2;User ID=sa;Password = Brisbane007;initial catalog = Buffalo";
        private static string StrConn()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                string fileName = @"C:\Inetpub\wwwroot\downloadKeywords.xml";
                //string fileName = @"C:\Inetpub\wwwroot\ServerIP_Callback.xml";

                // You'll need to put the correct path to your xml file here
                xml.Load(fileName);

                // Select a specific node
                XmlNode node = xml.SelectSingleNode("download/con");
                //XmlNode node = xml.SelectSingleNode("ConnectionString/con");

                // Get its value
                string name = node.InnerText;

                return name;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static void Main(string[] args)
        {
            Console.Title = "Tracking Trending Download Keywords";
            GetKeywordsBatch();
        }

        private static void GetKeywordsBatch()
        {
            try
            {
                string qry = "Exec [dbo].[DeleteKeywords] ";
                ProcessDB(qry);
            }
            catch (SqlException se)
            {
                Console.WriteLine("SQL Error: " + se.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            string myDate = DateTime.Today.ToString("yyyy-MM-dd");

            string url = "https://incoming.pi-datametrics.com/provider-api/tracking/get-required-searches?date=" + myDate + "&remaining-only=false";


            string authInfo = "pisoftware" + ":" + "r00t123456";
            StringBuilder stringBuilder = new StringBuilder();
            string value = string.Empty;
            Uri uri = new Uri(url);
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            httpWebRequest.Headers["Authorization"] = "Basic " + authInfo;
            XmlDocument doc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings { CheckCharacters = false };
            Console.WriteLine("Downloading keywords...");
            using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
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
                        using (var sqlBulk = new SqlBulkCopy(StrConn()))
                        {
                            sqlBulk.BulkCopyTimeout = 0;
                            sqlBulk.DestinationTableName = "tracking_keywords";
                            sqlBulk.WriteToServer(dt);
                        }

                    Console.WriteLine("Keywords downloaded.");

                    Environment.Exit(0);

                }
                catch (SqlException se)
                {
                    string errMsg = "Database Connection is temporarily not working\n" + se.ToString();
                    Console.WriteLine("SQL Error: " + errMsg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }


        }

        private static void ProcessDB(string qry)
        {
            using (SqlConnection con = new SqlConnection(StrConn()))
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
