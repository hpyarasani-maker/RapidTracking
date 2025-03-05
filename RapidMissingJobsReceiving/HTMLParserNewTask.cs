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
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections;
//using Oxylabs_TrackingComponent;

namespace RapidMissingJobsReceiving
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
                //Name = "All_1"
                Name = "NewSEIDs_2"
                // Name = "Mobile_102_10"
                ///Name = "CommaKeywords_1"
                //Name = "ODesktop_20"
                //Name = "NewMobile_4"
                //Name = "NewComma"
            };
            t1.Start();
        }

        private void StartProcess()
        {


            string url = "http://10.2.0.4/WebApi2/webapi2/api/missing?date=" + myDate; //receiving New SEIDs
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
                        string seid = string.Empty;
                        string keyword = string.Empty;
                        string jobid = string.Empty;
                        if (response != null)
                            try
                            {
                                JArray jo = JArray.Parse(response);
                                seid = jo[0].Value<JObject>().Value<string>("seid");
                                keyword = jo[0].Value<JObject>().Value<string>("name");
                                jobid = jo[0].Value<JObject>().Value<string>("jobid");
                                DoProcess(jobid);
                            }
                            catch(Exception ex) { throw ex; }

                   }
                    catch (Exception ex) { throw ex; }
                }

            }
        }
        
       

    

    private void DoProcess(string jobid)
     {

            //string username = "gpidatametrics";
            //string password = "sdV5X3fcX6";
            string username = "piapp";
            string password = "b5FCvgkjxx";
            string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));

                string infoURL = "http://data.oxylabs.io/v1/queries/" + jobid;
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(infoURL);
                httpWebRequest.Headers["Authorization"] = "Basic " + authInfo;
                HttpWebResponse res1 = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream resStream = res1.GetResponseStream();
                StreamReader reader = new StreamReader(resStream, Encoding.UTF8);
                string response = reader.ReadToEnd();
                resStream.Close();
                res1.Close();
                JObject obj = JObject.Parse(response);
                string status = obj["status"].Value<string>();
                string kw = obj["query"].Value<string>();
                string device = obj["user_agent_type"].Value<string>();
                string hl = obj["locale"].Value<string>();
                string gl = obj["geo_location"].Value<string>();
                string domain = obj["domain"].Value<string>();
                string jb = obj["id"].Value<string>();
                string resURL = obj["_links"][1]["href"].Value<string>();
                
                string seid = "";
                status = obj["status"].Value<string>();
                if (status == "faulted")
                {
                    throw new Exception("status is faulted");
                }
                if (status == "pending") //31-01-2022
                {
                    throw new Exception("status is pending");
                }

            try
            {

                if (status == "done")
                {
                   /// resURL = obj["results_url"].Value<string>()
                    httpWebRequest = (HttpWebRequest)WebRequest.Create(resURL);
                    authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));
                    httpWebRequest.Headers["Authorization"] = "Basic " + authInfo;
                    HttpWebResponse resR = (HttpWebResponse)httpWebRequest.GetResponse();
                    Stream resStreamR = resR.GetResponseStream();
                    StreamReader readerR = new StreamReader(resStreamR, Encoding.UTF8);
                    string resResults = readerR.ReadToEnd();
                    resStreamR.Close();
                    resR.Close();
                    string result = string.Empty;
                    int orgUrls = 0;
                    try
                    {
                        JObject rObj = JObject.Parse(resResults);
                        resResults = rObj["results"][0]["content"].Value<string>();

                        SearchProperties sp = SearchParams.searches.Where(s => s.locale == hl && s.device == device && s.geo_location == gl).SingleOrDefault();
                        seid = sp.seid.ToString();

                       if ( device == "desktop_chrome")
                            result = desktop.ProcessDocument(seid, kw, resResults, out orgUrls);
                        else
                            result = ios.ProcessDocument(seid, kw, resResults, out orgUrls);
                        //File.WriteAllText(@"C:\inetpub\wwwroot\html\" + jobid + "_" + kw + ".html", html, Encoding.UTF8);
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
                //string fileName = @"C:\Inetpub\wwwroot\downloadKeywords.xml";

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

