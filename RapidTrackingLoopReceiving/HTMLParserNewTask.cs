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
using System.Collections;
using System.Threading.Tasks;

namespace RapidTrackingLoopReceiving
{
    class HTMLParserNewTask
    {
        Desktop desktop;
        iOS ios;
        string statusCode = string.Empty;
        readonly string myDate = DateTime.Today.ToString("yyyy-MM-dd");

        public event KeywordDone OnKeywordDone;
        double apitime, dbtime;    // 31-03-2020
        Thread t1;
        public HTMLParserNewTask()
        {
            desktop = new Desktop();
            ios = new iOS();

            t1 = new Thread(new ThreadStart(StartProcess))
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

        private async void StartProcess()
        {
            //string url = "https://seresults.azurewebsites.net/api/callbackrapidtrackingdesktop/";  // rapid tracking desktop and all keywords
            //string url = "https://seresults.azurewebsites.net/api/callbackrapidtrackingmobile/";  // rapid tracking mobile
            // string url = "https://seresults.azurewebsites.net/api/callbackrapidtrackingcommakeywords/";  // rapid tracking comma keywords
            //string url = "https://seresults.azurewebsites.net/api/callbackrapidtrackingmobilehotel/";  // rapid tracking mobile
            //string url = "https://seresults.azurewebsites.net/api/callbackrapidTrackingnewdesktop/"; //Desktop new keywords
            //string url = "https://seresults.azurewebsites.net/api/callbackrapidTrackingnewmobile/"; //mobile new keywords
            // string url = "https://seresults.azurewebsites.net/api/callbackrapidtrackingnewcommakeywords/";  // new comma keywords

            //string url = "https://seresults.azurewebsites.net/api/callbackuk503desktoptemp/"; //receiving New SEIDs
            //string url = "https://seresults.azurewebsites.net/api/trackingtrending/";
            //string url = "https://seresults.azurewebsites.net/api/callbackuk58desktop/";       // 58
            //string url = "https://seresults.azurewebsites.net/api/callbackuk106mobile/";      // 106
            //string url = "https://seresults.azurewebsites.net/api/callbackus1desktop/";     // 1
            //string url = "https://seresults.azurewebsites.net/api/callbackus102mobile/";      // 102
            string url = "https://seresults.azurewebsites.net/api/callbackotherdesktop/";  // Loop Receiving other desktop
            //string url = "https://seresults.azurewebsites.net/api/callbackothermobile/";   // other mobiles

            //string url = "https://seresults.azurewebsites.net/api/callbackimagesdesktop/";       // images desktop
            //string url = "https://seresults.azurewebsites.net/api/callbackimagesmobile/";      // images mobilse
            //string url = "https://seresults.azurewebsites.net/api/callback74images/";
            //string url = "https://previous.azurewebsites.net/api/callbackrapidtrackingmobile/";  // rapid tracking mobile
            //string url = "https://previous.azurewebsites.net/api/callbackrapidtrackingcommakeywords/";  // rapid tracking comma keywords
            //string url = "https://previous.azurewebsites.net/api/callbackrapidtrackingdesktop/";  // rapid tracking other desktop
            //string url = "https://previous.azurewebsites.net/api/callbackuk58desktop/";       // 58 sending for previous date

            Uri ul = new Uri(url);
            string username = "pisoftware";
            string password = "Pi*Soft74UBXi";
            using (var client = new HttpClient())
            {
                //WebClient client = new WebClient();
                client.BaseAddress = ul;//12-05-2024
                while (true)
                {
                    /*try
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
                    }*/
                    try
                    {
                        string response = "";
                        client.DefaultRequestHeaders.Clear();
                        string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));//12-05-2024
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);//12-05-2024
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        response = client.GetStringAsync(ul).Result;
                        if (response != "null")
                            await DoProcess(response);
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

        private async Task DoProcess(string resp)
        {
            JObject job = JObject.Parse(resp);
            string status = job["status"].Value<string>();
            string kw = job["query"].Value<string>();
            string device = job["user_agent_type"].Value<string>();
            string hl = job["locale"].Value<string>();
            string gl = job["geo_location"].Value<string>();
            string domain = job["domain"].Value<string>();
            string jobid = job["id"].Value<string>();
            double totalTime = 0;//22-01-2025
            string seid = "";
            SearchProperties sp = SearchParams.searches.Where(s => s.locale == hl && s.device == device && s.geo_location == gl).SingleOrDefault();
            seid = sp.seid.ToString();
            int count = 0;
            string resx = string.Empty;
            ArrayList result = new ArrayList();
            try
            {
                string username = string.Empty;//04-03-2025
                string password = string.Empty;
                if (sp.device == "mobile_android")
                {
                    username = "piapp";
                    password = "b5FCvgkjxx";
                }
                else if (sp.device == "desktop_chrome")
                {
                    username = "piapp-aio";
                    password = "4gvfnA+aBYpBNs37";
                }//04-03-2025

                if (status == "done")
                {
                    //ServicePointManager.Expect100Continue = true;
                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var startTime = System.Diagnostics.Stopwatch.StartNew();//08-11-2023
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
                    startTime.Stop();//08-11-2023
                    totalTime = Convert.ToDouble(startTime.ElapsedMilliseconds) / 1000;//08-11-2023
                    string rest = string.Empty;//12-05-2025
                    int orgUrls = 0;
                    try
                    {
                        //JObject obj = JObject.Parse(response);
                        //response = obj["results"][0]["content"].Value<string>();


                        JObject obj = JObject.Parse(response);
                        var contents = obj["results"];

                        for (int x = 0; x < contents.Count(); x++)
                        {
                            try
                            {
                                response = contents[x]["content"].Value<string>();
                                if (device == "desktop_chrome")
                                    (rest, orgUrls) = await desktop.ProcessDocument(seid, kw, jobid, response);//12-05-2025//30-10-2024
                                else
                                    (rest, orgUrls) = await ios.ProcessDocument(seid, kw, jobid, response);//12-05-2025//30-10-2024
                                count += orgUrls;
                                result.Add(rest);//12-05-2025
                            }
                            catch { }
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    // 31-03-2020
                    apitime = 0.0;
                    dbtime = 0.0;
                    int k = 0;
                    XmlDocument xmlDoc = new XmlDocument();
                    foreach (string xml in result)
                    {
                        if (string.IsNullOrEmpty(xml.Trim())) continue;//02-05-2022
                        if (k == 0)
                        {
                            k++;
                            xmlDoc.LoadXml(xml);
                            continue;
                        }
                        XmlDocument xmlDoc1 = new XmlDocument();
                        xmlDoc1.LoadXml(xml);

                        XmlNode node = xmlDoc.SelectSingleNode(".//section[@col='main']");
                        XmlNode node1 = xmlDoc1.SelectSingleNode(".//section[@col='main']");

                        foreach (XmlNode nd in node1.ChildNodes)
                        {
                            XmlNode imported = xmlDoc.ImportNode(nd, true);
                            node.AppendChild(imported);
                        }
                    }
                    resx = xmlDoc.InnerXml;
                    if (!string.IsNullOrEmpty(resx))
                    {
                       
                        //if (count > 20)
                        //{
                            await ProcessResults(resx, kw, seid, jobid, count);
                        //}
                        //if(count <= 20)
                        //{
                        //    await SendToDB(seid, kw, resx, jobid, count);
                        //}
                        bool aio = resx.Contains("<block type=\"aiOverview\">");//16-02-2025
                        if (aio && device == "mobile_android") // inserting true value//16-02-2025
                            await InsertAIO_Keyword(kw, seid, aio);//16-02-2025
                    }
                    //if (!string.IsNullOrEmpty(seid))
                    //    for (int i = 0; i < result.Count; i++)
                    //    {
                    //        ProcessResults(result[i].ToString(), kw, seid, jobid, orgUrls);
                    //    }

                    OnKeywordDone.Invoke(seid + ":  " + kw + ",  " + count + "^" + statusCode + "^" + apitime + "^" + dbtime + "^" + totalTime);    // 31-03-2020
                }
                if (status == "faulted")
                {
                    statusCode = status;//22-01-2025
                    throw new Exception("Status is faulted");
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
                        if (ex.Message == "faulted")
                            isOldPage = false;
                        await ProcessError(kw, seid, jobid,ex.Message, isOldPage);
                    }
                    finally { }
                }
                OnKeywordDone.Invoke("Error:  seid: " + seid + ",  keyword: " + kw + ",  jobid: " + jobid + "\r\n\t" + ex.Message + "^" + statusCode + "^" + apitime + "^" + dbtime + "^" + totalTime);    // 31-03-2020                
            }
        }

        private async Task ProcessError(string kw, string seid, string jobid, string message,bool isOldPage)
        {
            string qry = "insert into dashboard_dataerrors (date, name, seid, jobid,message) values(Convert(varchar(10),'" + myDate + "',103), N'" +
                  kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "',N'" + message.Replace("'", "''") + "' )";//09-11-2024

            string qryOld = "insert into dashboard_oldgooglepage (date, keyword, seid, jobid,message) values('" + DateTime.Now + "', N'" +
                 kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "',N'" + message.Replace("'", "''") + "'  )";//09-11-2024

            using (SqlConnection con = new SqlConnection(await StrConn()))
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
        private async Task InsertAIO_Keyword(string kw, string seid, bool aio)//16-02-2025
        {
            try
            {
                using (SqlConnection con = new SqlConnection(await StrConn()))
                {
                    con.Open();
                    using (SqlCommand comm = con.CreateCommand())
                    {
                        comm.CommandTimeout = 0;
                        comm.CommandType = CommandType.StoredProcedure;
                        comm.CommandText = "Insert_AIO_Keywords";
                        comm.Parameters.Add("Seid", SqlDbType.Int).Value = seid;
                        comm.Parameters.Add("Name", SqlDbType.NVarChar).Value = kw;
                        comm.Parameters.Add("AIO", SqlDbType.Bit).Value = aio;
                        await comm.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex)
            {
                string errorMessage = $"Database Error in Insert_AIO_Keywords: \r\n{ex.Message}";
                throw new Exception(errorMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }//16-02-2025

        public async Task<string> StrConn()
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

                return await Task.FromResult<string>(name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task ProcessResults(string result, string kw, string seid, string jobid, int urlcount)
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
                    await SendXmlToAPI(seid, kw, result);
                    DateTime ed = DateTime.Now;
                    apitime = (ed - st).TotalSeconds;
                }
                DateTime st1 = DateTime.Now;
                await SendToDB(seid, kw, result, jobid, urlcount);
                DateTime ed1 = DateTime.Now;
                dbtime = (ed1 - st1).TotalSeconds;
                //end of 31-03-2020
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task SendXmlToAPI(string seid, string kw, string res)
        {
            string tname = t1.Name;
            string path = @"C:\Inetpub\wwwroot\rapidtracking_" + tname + ".xml";

            XmlDocument xd = new XmlDocument();
            res = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + res;
            xd.LoadXml(res);
            xd.Save(path);


            string submitURL = await ReadAPI();

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
                string postData = await GetTextFromXMLFile(path);
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

                Stream stream = await httpWReq.GetRequestStreamAsync();
                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse response = (HttpWebResponse)await httpWReq.GetResponseAsync();
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

        private async Task<string> GetTextFromXMLFile(string file)
        {
            StreamReader reader = new StreamReader(file);
            string ret = reader.ReadToEnd();
            reader.Close();
            return await Task.FromResult<string>(ret);
        }

        public async Task<string> ReadAPI()
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

                return await Task.FromResult<string>(name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task SendToDB(string seid, string keyword, string xml, string jobid, int urlcount)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(await StrConn()))
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
                        comm.Parameters.Add("Received", SqlDbType.VarChar).Value = "Normal Loop Receive"; //14-04-2025
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

