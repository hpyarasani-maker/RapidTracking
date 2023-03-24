using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net.Http;
//using Oxylabs_TrackingComponent;

namespace ReceivingProject
{
    class HTMLParserNewTask
    {
        Desktop desktop;
        iOS ios;
        string statusCode = string.Empty;
        readonly string myDate = DateTime.Today.ToString("yyyy-MM-dd");

        public event KeywordDone OnKeywordDone;
        double apitime, dbtime;    // 31-03-2020

        public HTMLParserNewTask()
        {
            desktop = new Desktop();
            ios = new iOS();

            Thread t1 = new Thread(new ThreadStart(StartProcess))
            {
                Name = "TrackingData_1"

            };
            t1.Start();
        }

        private void StartProcess()
        {
            //string url = "https://seresults.azurewebsites.net/api/trackingDataDesktop/";  // TrackingData Desktop
            //string url = "https://seresults.azurewebsites.net/api/trackingDataMobile/";       // TrackingData Mobile
            //string url = "http://previous.azurewebsites.net/api/callbackus1desktop/";     // Desktop
            string url = "http://previous.azurewebsites.net/api/callbackrapidtrackingdesktop/"; // Mobile.....................................................Controlllers
            Uri ul = new Uri(url);
            using (var client = new HttpClient())
            {
                //WebClient client = new WebClient();
                while (true)
                {
                  
                    try
                    {
                        string response = "";
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        response = client.GetStringAsync(ul).Result;
                        if (response != "null")
                            DoProcess(response);
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine("# EXCEPTION #  " + ex.Message);
                        //throw new ArgumentException(message: ex.Message.ToString(), paramName: "response");
                        //OnKeywordDone.Invoke("Error:" + ex.Message.ToString());
                    }
                }
            }
        }

        private void DoProcess(string resp)
        {
            JObject job = JObject.Parse(resp);
            string status = job["status"].Value<string>();
            string kw = job["query"].Value<string>();
            string device = job["user_agent_type"].Value<string>();
            string hl = job["locale"].Value<string>();
            string gl = job["geo_location"].Value<string>();
            string domain = job["domain"].Value<string>();
            string jobid = job["id"].Value<string>();
            string seid = "";
            try
            {
                string username = "gpidatametrics";
                string password = "sdV5X3fcX6";

                if (status == "done")
                {
                    //ServicePointManager.Expect100Continue = true;
                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    string resURL = job["results_url"].Value<string>();
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(resURL);
                    string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));
                    httpWebRequest.Headers["Authorization"] = "Basic " + authInfo;
                    HttpWebResponse res = (HttpWebResponse)httpWebRequest.GetResponse();
                    Stream resStream = res.GetResponseStream();
                    StreamReader reader = new StreamReader(resStream, Encoding.UTF8);
                    string response = reader.ReadToEnd();
                    resStream.Close();
                    res.Close();
                    string result = string.Empty;
                    int orgUrls = 0;
                    try
                    {
                        JObject obj = JObject.Parse(response);
                        response = obj["results"][0]["content"].Value<string>();

                        SearchProperties sp = SearchParams.searches.Where(s => s.locale == hl && s.device == device && s.geo_location == gl).SingleOrDefault();
                        seid = sp.seid.ToString();

                        if (device == "desktop")
                            result = desktop.ProcessDocument(seid, kw, response, out orgUrls);
                        else
                            result = ios.ProcessDocument(seid, kw, response, out orgUrls);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    // 31-03-2020
                    apitime = 0.0;
                    dbtime = 0.0;

                    if (!string.IsNullOrEmpty(seid))
                        ProcessResults(result, kw, seid, jobid, orgUrls);

                    OnKeywordDone.Invoke(seid + ":  " + kw + ",  " + orgUrls + "^" + statusCode + "^" + apitime + "^" + dbtime);    // 31-03-2020
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Result Request: " + ex.Message);
                if (!string.IsNullOrEmpty(jobid))
                {
                    try
                    {
                        bool isOldPage = false;
                        if (ex.Message == "Old page found.")
                            isOldPage = true;
                        ProcessError(kw, seid, jobid, isOldPage);
                    }
                    finally { }
                }
                OnKeywordDone.Invoke("Error:  seid: " + seid + ",  keyword: " + kw + ",  jobid: " + jobid + "\r\n\t" + ex.Message + "^" + statusCode + "^" + apitime + "^" + dbtime);    // 31-03-2020                
            }
        }

        private void ProcessError(string kw, string seid, string jobid, bool isOldPage)
        {
            string qry = "insert into dashboard_dataerrors (date, name, seid, jobid) values(Convert(varchar(10),'" + myDate + "',103), N'" +
                  kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "' )";

            string qryOld = "insert into dashboard_oldgooglepage (date, keyword, seid, jobid) values('" + DateTime.Now + "', N'" +
                 kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "' )";

            using (SqlConnection con = new SqlConnection(StrConn()))
            {
                try
                {
                    con.Open();
                    using (SqlCommand comm = new SqlCommand(qry, con))
                    {
                        comm.CommandTimeout = 0;
                        comm.ExecuteNonQuery();

                        if (isOldPage)
                        {
                            comm.CommandText = qryOld;
                            comm.ExecuteNonQuery();
                        }
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

        public string StrConn()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                string fileName = @"C:\Inetpub\wwwroot\TrackingData.xml";
                //string fileName = @"C:\Inetpub\wwwroot\downloadKeywords.xml";

                // You'll need to put the correct path to your xml file here
                xml.Load(fileName);

                // Select a specific node
                XmlNode node = xml.SelectSingleNode("ConnectionString/con");
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

        private void ProcessResults(string result, string kw, string seid, string jobid, int urlcount)
        {
            if (string.IsNullOrEmpty(result))
            {
                throw new Exception("No result.");
            }

            try
            {
                // 31-03-2020
                if (urlcount > 20)
                {

                    DateTime st1 = DateTime.Now;
                    //SendToDB(seid, kw, result, jobid, urlcount);
                    DateTime ed1 = DateTime.Now;
                    dbtime = (ed1 - st1).TotalSeconds;
                }
                //end of 31-03-2020
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetTextFromXMLFile(string file)
        {
            StreamReader reader = new StreamReader(file);
            string ret = reader.ReadToEnd();
            reader.Close();
            return ret;
        }

       

        private void SendToDB(string seid, string keyword, string xml, string jobid, int urlcount)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(StrConn()))
                {
                    con.Open();

                    using (SqlCommand comm = con.CreateCommand())
                    {
                        comm.CommandTimeout = 0;
                        comm.CommandType = CommandType.StoredProcedure;
                        comm.CommandText = "Insert_dashboard_data";
                        //comm.CommandText = "Insert_dashboard_dataP"; //previous date
                        comm.Parameters.Add("Date", SqlDbType.DateTime).Value = myDate;
                        comm.Parameters.Add("Name", SqlDbType.NVarChar).Value = keyword; //.Replace("'", "''");
                        comm.Parameters.Add("Seid", SqlDbType.Int).Value = seid;
                        comm.Parameters.Add("JobId", SqlDbType.NVarChar).Value = jobid;
                        comm.Parameters.Add("Count", SqlDbType.Int).Value = urlcount;
                        comm.Parameters.Add("XmlData", SqlDbType.Xml).Value = xml.Replace("'", "''");

                        comm.ExecuteNonQuery();

                      
                    }
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

