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
//using Oxylabs_TrackingComponent;

namespace Oxylabs_BulkKeywords
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
                //Name = "All_5"
                // Name = "Mobile_102_10"
                ///Name = "CommaKeywords_1"
                Name = "58_16"
            };
            t1.Start();
        }

        private void StartProcess()
        {
           // string url = "http://seresults.azurewebsites.net/api/callbackrapidtrackingdesktop/";  // rapid tracking desktop and all keywords
           //string url = "http://seresults.azurewebsites.net/api/callbackrapidtrackingmobile/";  // rapid tracking mobile
           // string url = "http://seresults.azurewebsites.net/api/callbackrapidtrackingcommakeywords/";  // rapid tracking comma keywords
            //string url = "http://seresults.azurewebsites.net/api/callbackrapidtrackingmobilehotel/";  // rapid tracking mobile
            //string url = "http://seresults.azurewebsites.net/api/callbackuk503desktoptemp/";
            //string url = "http://seresults.azurewebsites.net/api/trackingtrending/";

            string url = "http://seresults.azurewebsites.net/api/callbackuk58desktop/";       // 58
            //string url = "http://seresults.azurewebsites.net/api/callbackuk106mobile/";      // 106
            //string url = "http://seresults.azurewebsites.net/api/callbackus1desktop/";     // 1
            //string url = "http://seresults.azurewebsites.net/api/callbackus102mobile/";      // 102
            //string url = "http://seresults.azurewebsites.net/api/callbackotherdesktop/";  // other desktop
            //string url = "http://seresults.azurewebsites.net/api/callbackothermobile/";   // other mobiles

            //string url = "http://seresults.azurewebsites.net/api/callbackimagesdesktop/";       // images desktop
            //string url = "http://seresults.azurewebsites.net/api/callbackimagesmobile/";      // images mobilse
            //string url = "http://seresults.azurewebsites.net/api/callback74images/";
            //string url = "http://previous.azurewebsites.net/api/callbackrapidtrackingmobile/";  // rapid tracking mobile
            //string url = "http://previous.azurewebsites.net/api/callbackrapidtrackingcommakeywords/";  // rapid tracking comma keywords
            //string url = "http://previous.azurewebsites.net/api/callbackrapidtrackingdesktop/";  // rapid tracking other desktop

            Uri ul = new Uri(url);
            WebClient client = new WebClient();
            while (true)
            {
                try
                {
                    statusCode = string.Empty;
                    string response = string.Empty;
                    client.Encoding = Encoding.UTF8;

                    response = client.DownloadString(url);
                    if (response != "null")
                        DoProcess(response);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("# EXCEPTION #  " + ex.Message);
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
                string fileName = @"C:\Inetpub\wwwroot\Callback_TrackingTrending.xml";
                //string fileName = @"C:\Inetpub\wwwroot\ServerIP_Callback.xml";

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
                    DateTime st = DateTime.Now;
                    SendXmlToAPI(seid, kw, result);
                    DateTime ed = DateTime.Now;
                    apitime = (ed - st).TotalSeconds;
                }
                DateTime st1 = DateTime.Now;
                SendToDB(seid, kw, result, jobid, urlcount);
                DateTime ed1 = DateTime.Now;
                dbtime = (ed1 - st1).TotalSeconds;
                //end of 31-03-2020
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SendXmlToAPI(string seid, string kw, string res)
        {
            string tname = Thread.CurrentThread.Name;
            string path = @"C:\Inetpub\wwwroot\rapidtracking_" + tname + ".xml";

            XmlDocument xd = new XmlDocument();
            res = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + res;
            xd.LoadXml(res);
            xd.Save(path);


            string submitURL = ReadAPI();

            string user = "pisoftware";
            string pwd = "r00t123456";
            try
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(submitURL);
                httpWReq.UseDefaultCredentials = true;
                httpWReq.PreAuthenticate = true;
                httpWReq.Credentials = CredentialCache.DefaultCredentials;

                httpWReq.CookieContainer = new CookieContainer();

                Encoding encoding = new UTF8Encoding();
                string postData = GetTextFromXMLFile(path);
                byte[] data = encoding.GetBytes(postData);

                httpWReq.ProtocolVersion = HttpVersion.Version11;
                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/x-www-form-urlencoded"; //charset=UTF-8";  


                string auth = string.Format("{0}:{1}", user, pwd);
                string enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(auth));
                string cred = string.Format("{0} {1}", "Basic", enc);


                httpWReq.Headers[HttpRequestHeader.Authorization] = cred;
                httpWReq.ContentLength = data.Length;
                //httpWReq.Timeout = 0;

                Stream stream = httpWReq.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                //statusCode = response.StatusCode.ToString();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    reader.Close();
                    response.Close();
                    throw new Exception(response.StatusCode + ": " + response.StatusDescription);
                }
                String xmlResponse = "";
                String temp = null;
                while ((temp = reader.ReadLine()) != null)
                {
                    xmlResponse += temp;
                }
                reader.Close();
                response.Close();
            }
            catch (WebException ex)
            {
                string errorMsg = string.Empty;
                using (WebResponse response = ex.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    statusCode = httpResponse.StatusCode.ToString();
                    errorMsg = string.Format("API Error: StatusCode {0}", statusCode);

                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        errorMsg += "\r\n" + reader.ReadToEnd();
                    }
                }

                throw new Exception(errorMsg);

            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        private string GetTextFromXMLFile(string file)
        {
            StreamReader reader = new StreamReader(file);
            string ret = reader.ReadToEnd();
            reader.Close();
            return ret;
        }

        public string ReadAPI()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                string fileName = @"C:\Inetpub\wwwroot\Callback_TrackingTrending.xml";
                //string fileName = @"C:\Inetpub\wwwroot\ServerIP_Callback.xml";

                // You'll need to put the correct path to your xml file here
                xml.Load(fileName);

                // Select a specific node
                XmlNode node = xml.SelectSingleNode("ConnectionString/apiSubmit");
                //XmlNode node = xml.SelectSingleNode("ConnectionString/apiNew");
                // Get its value
                string name = node.InnerText;

                return name;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                        comm.Parameters.Add("Date", SqlDbType.DateTime).Value = myDate;
                        comm.Parameters.Add("Name", SqlDbType.NVarChar).Value = keyword; //.Replace("'", "''");
                        comm.Parameters.Add("Seid", SqlDbType.Int).Value = seid;
                        comm.Parameters.Add("JobId", SqlDbType.NVarChar).Value = jobid;
                        comm.Parameters.Add("Count", SqlDbType.Int).Value = urlcount;
                        comm.Parameters.Add("XmlData", SqlDbType.Xml).Value = xml.Replace("'", "''");

                        comm.ExecuteNonQuery();

                        if (urlcount < 20)
                        {
                            string qry = "exec [InsertLessthan20] '" + myDate + "',N'" + keyword.Replace("'", "''") + "'," + seid + ",N''," + urlcount + ",'" + jobid + "'";
                            comm.CommandText = qry;
                            comm.CommandType = CommandType.Text;
                            comm.ExecuteNonQuery();
                        }
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

