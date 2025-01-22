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
using System.Threading.Tasks;
//using Oxylabs_TrackingComponent;

namespace AIOReceiving
{
    class HTMLParserNewTask
    {
        Desktop desktop;
        iOS ios;
        string statusCode = string.Empty;
        readonly string myDate = DateTime.Today.ToString("yyyy-MM-dd");

        public event KeywordDone OnKeywordDone;//30-10-2024
        double apitime, dbtime;    // 31-03-2020
        Thread t1; //06-08-2024
        public HTMLParserNewTask()
        {
            desktop = new Desktop();
            desktop.OnKeywordDone += Desktop_OnKeywordDone;//30-10-2024
            ios = new iOS();
            ios.OnKeywordDone += Ios_OnKeywordDone;//30-10-2024
            t1 = new Thread(new ThreadStart(StartProcess))//06-08-2024
            {
                //Name = "AIO_ALll_1"
                Name = "AIO_1"
                ///Name = "AIO_CommaKeywords_1"
            };
            t1.Start();
        }
        private void Ios_OnKeywordDone(string value)//30-10-2024
        {
            OnKeywordDone.Invoke(value);
        }
        private void Desktop_OnKeywordDone(string value)//30-10-2024
        {
            OnKeywordDone.Invoke(value);
        }

        private async void StartProcess() //06-08-2024
        {
            //string url = "https://seresults.azurewebsites.net/api/callbackrapidtrackingcommakeywords/";  // rapid tracking comma keywords
            string url = "https://seresults.azurewebsites.net/api/callbackuk58desktop/";       // 58 AIO Keyword Tracking Desktop and Mobile

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
                        response = await client.GetStringAsync(ul); //06-08-2024  //.Result;
                        if (response != "null")
                            await DoProcess(response); //06-08-2024
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

        private async Task DoProcess(string resp) //06-08-2024
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
            string response = string.Empty; //18-03-2024
            double totalTime = 0;//22-01-2025
            try
            {
                //string username = "gpidatametrics";
                //string password = "sdV5X3fcX6";
                string username = "piapp-aio";
                string password = "4gvfnA+aBYpBNs37";
                SearchProperties sp = SearchParams.searches.Where(s => s.locale == hl && s.device == device && s.geo_location == gl).SingleOrDefault();
                seid = sp.seid.ToString();//22-01-2025
                if (status == "done")
                {
                    var startTime = System.Diagnostics.Stopwatch.StartNew();//08-11-2023
                    //ServicePointManager.Expect100Continue = true;
                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    string resURL = job["results_url"].Value<string>();
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(resURL);
                    string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));
                    httpWebRequest.Headers["Authorization"] = "Basic " + authInfo;
                    HttpWebResponse res = (HttpWebResponse)await httpWebRequest.GetResponseAsync(); //06-08-2024
                    Stream resStream = res.GetResponseStream();
                    StreamReader reader = new StreamReader(resStream, Encoding.UTF8);
                    response = await reader.ReadToEndAsync(); //06-08-2024
                    resStream.Close();
                    res.Close();
                    startTime.Stop();//08-11-2023
                    totalTime = Convert.ToDouble(startTime.ElapsedMilliseconds) / 1000;//08-11-2023
                    string result = string.Empty;
                    int orgUrls = 0;
                    try
                    {
                        JObject obj = JObject.Parse(response);
                        response = obj["results"][0]["content"].Value<string>();

                        //SearchProperties sp = SearchParams.searches.Where(s => s.locale == hl && s.device == device && s.geo_location == gl).SingleOrDefault();
                        //seid = sp.seid.ToString();

                        if (device == "desktop_chrome")
                            result = desktop.ProcessDocument(seid, kw, jobid, response, out orgUrls);//30-10-2024
                        else
                            result = ios.ProcessDocument(seid, kw, jobid, response, out orgUrls);//30-10-2024
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        response = string.Empty;//18-03-2024
                    }

                    // 31-03-2020
                    apitime = 0.0;
                    dbtime = 0.0;

                    if (!string.IsNullOrEmpty(seid))
                        await ProcessResults(result, kw, seid, jobid, orgUrls); //06-08-2024
                    OnKeywordDone.Invoke(seid + ":  " + kw + ",  " + orgUrls + "^" + statusCode + "^" + apitime + "^" + dbtime + "^" + totalTime);//08-11-2023 //31-03-2020
                }
                else if (status == "faulted")//21-01-2025
                {
                    statusCode = status;//22-01-2025
                    throw new Exception("Status is faulted");
                }//21-01-2025
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
                        await ProcessError(kw, seid, jobid,ex.Message.ToString(), isOldPage); //09-11-2024//06-08-2024
                    }
                    finally { }
                }
                OnKeywordDone.Invoke("Error:  seid: " + seid + ",  keyword: " + kw + ",  jobid: " + jobid + "\r\n\t" + ex.Message + "^" + statusCode + "^" + apitime + "^" + dbtime + "^" + totalTime);//22-01-2025
            }
        }

        private async Task ProcessError(string kw, string seid, string jobid,string message, bool isOldPage)//09-11-2024 //06-08-2024
        {
            string qry = "insert into dashboard_dataerrors (date, name, seid, jobid,message) values(Convert(varchar(10),'" + myDate + "',103), N'" +
                  kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "',N'"+message.Replace("'", "''") + "' )";//09-11-2024

            string qryOld = "insert into dashboard_oldgooglepage (date, keyword, seid, jobid,message) values('" + DateTime.Now + "', N'" +
                 kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "',N'" + message.Replace("'", "''") + "'  )";//09-11-2024

            using (SqlConnection con = new SqlConnection(StrConn()))
            {
                try
                {
                    con.Open();
                    using (SqlCommand comm = new SqlCommand(qry, con))
                    {
                        comm.CommandTimeout = 0;
                        await comm.ExecuteNonQueryAsync(); //06-08-2024

                        if (isOldPage)
                        {
                            comm.CommandText = qryOld;
                            await comm.ExecuteNonQueryAsync(); //06-08-2024
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

        private async Task ProcessResults(string result, string kw, string seid, string jobid, int urlcount) //06-08-2024
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
                    await SendXmlToAPI(seid, kw, result); //06-08-2024
                    DateTime ed = DateTime.Now;
                    apitime = (ed - st).TotalSeconds;
                }
                DateTime st1 = DateTime.Now;
                await SendToDB(seid, kw, result, jobid, urlcount);//storing in database table //06-08-2024
                //SendToDB(seid, kw, jobid, urlcount); //creating and storing data in txt file
                DateTime ed1 = DateTime.Now;
                dbtime = (ed1 - st1).TotalSeconds;
                //end of 31-03-2020
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task SendXmlToAPI(string seid, string kw, string res) //06-08-2024
        {
            string tname = t1.Name;//06-08-2024
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
                xd = null;//14-03-2024
                httpWReq.CookieContainer = new CookieContainer();

                Encoding encoding = new UTF8Encoding();
                string postData = await GetTextFromXMLFile(path); //06-08-2024
                byte[] data = encoding.GetBytes(postData);

                httpWReq.ProtocolVersion = HttpVersion.Version11;
                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/x-www-form-urlencoded"; //charset=UTF-8";  


                string auth = string.Format("{0}:{1}", user, pwd);
                string enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(auth));
                string cred = string.Format("{0} {1}", "Basic", enc);


                httpWReq.Headers[HttpRequestHeader.Authorization] = cred;
                httpWReq.ContentLength = data.Length;
                postData = string.Empty;//14-03-2024
                //httpWReq.Timeout = 0;

                Stream stream = await httpWReq.GetRequestStreamAsync(); //06-08-2024
                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse response = (HttpWebResponse)await httpWReq.GetResponseAsync(); //06-08-2024
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
                        errorMsg += "\r\n" + await reader.ReadToEndAsync(); //06-08-2024
                    }
                }

                throw new Exception(errorMsg);

            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        private async Task<string> GetTextFromXMLFile(string file) //06-08-2024
        {
            StreamReader reader = new StreamReader(file);
            string ret = await reader.ReadToEndAsync(); //06-08-2024
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
        private void SendToDB(string seid, string keyword, string jobid, int urlCount)
        {
            string dt = DateTime.Today.ToString("yyyy-MM-dd");
            string path = @"C:\Inetpub\wwwroot\Results" + dt + ".txt";
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(seid + "\t" + keyword + "\t" + jobid + "\t" + urlCount);
                }
            }
            else if (File.Exists(path))
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(seid + "\t" + keyword + "\t" + jobid + "\t" + urlCount);
                }
            }
        }
        private async Task SendToDB(string seid, string keyword, string xml, string jobid, int urlcount) //06-08-2024
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

                        await comm.ExecuteNonQueryAsync(); //06-08-2024

                        //if (urlcount < 20)
                        //{
                        //    string qry = "exec [InsertLessthan20] '" + myDate + "',N'" + keyword.Replace("'", "''") + "'," + seid + ",N''," + urlcount + ",'" + jobid + "'";
                        //    comm.CommandText = qry;
                        //    comm.CommandType = CommandType.Text;
                        //    comm.ExecuteNonQuery();
                        //}
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