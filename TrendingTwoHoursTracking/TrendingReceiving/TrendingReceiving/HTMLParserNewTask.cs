using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace TrendingReceiving
{
    class HTMLParserNewTask
    {
        Desktop desktop ;
        iOS ios ;
        string statusCode = string.Empty;
        string myDate = DateTime.Today.ToString("yyyy-MM-dd");
        public event KeywordDone OnKeywordDone;
        double apitime, dbtime;    // 31-03-2020
        Thread t1; //14-08-2024
        public HTMLParserNewTask()
        {
            desktop = new Desktop();
            ios = new iOS();

            t1 = new Thread(new ThreadStart(StartProcess))//14-08-2024
            {
                Name = "twm_1"
            };
            t1.Start();
        }

        private async void StartProcess()//14-08-2024
        {
            //string url = "https://seresults.azurewebsites.net/api/callbacktrendingdesktop/";       // Desktop
            string url = "https://seresults.azurewebsites.net/api/callbacktrendingmobile/";       // Mobile


            Uri ul = new Uri(url);
            string username = "pisoftware";
            string password = "Pi*Soft74UBXi";
            using (var client = new HttpClient())
            {
                client.BaseAddress = ul;//12-05-2024
                while (true)
                {
                    try
                    {
                        string response = "";
                        client.DefaultRequestHeaders.Clear();
                        string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));//12-05-2024
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);//12-05-2024
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        response = await client.GetStringAsync(ul);//14-08-2024
                        if (response != "null")
                            await DoProcess(response);//14-08-2024
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine("# EXCEPTION #  " + ex.Message);
                    }
                }
            }
        }

        private async Task DoProcess(string resp)//14-08-2024
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
                SearchProperties sp = SearchParams.searches.Where(s => s.locale == hl && s.device == device && s.geo_location == gl).SingleOrDefault();
                seid = sp.seid.ToString();//22-01-2025
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
                    var startTime = System.Diagnostics.Stopwatch.StartNew();//08-11-2023
                    //ServicePointManager.Expect100Continue = true;
                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    string resURL = job["results_url"].Value<string>();
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(resURL);
                    string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));
                    httpWebRequest.Headers["Authorization"] = "Basic " + authInfo;
                    HttpWebResponse res = (HttpWebResponse)await httpWebRequest.GetResponseAsync();//14-08-2024
                    Stream resStream = res.GetResponseStream();
                    StreamReader reader = new StreamReader(resStream, Encoding.UTF8);
                    response =await reader.ReadToEndAsync();//14-08-2024
                    resStream.Close();
                    res.Close();
                    totalTime = Convert.ToDouble(startTime.ElapsedMilliseconds) / 1000;//08-11-2023
                    string result = string.Empty;
                    int orgUrls = 0;

                    //15-04-2020
                    apitime = 0.0;
                    dbtime = 0.0;

                    try
                    {
                        JObject obj = JObject.Parse(response);
                        response = obj["results"][0]["content"].Value<string>();

                        if (device == "desktop_chrome")
                            result = desktop.ProcessDocument(seid, kw, response, out orgUrls);
                        else
                            result = ios.ProcessDocument(seid, kw, response, out orgUrls);
                    }
                    catch (Exception ex )
                    {
                        throw ex;
                    }

                    if (!string.IsNullOrEmpty(seid))
                        await ProcessResults(result, kw, seid, jobid, orgUrls);//14-08-2024

                    // OnKeywordDone.Invoke(seid + ":  " + kw + ",  " + orgUrls);
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
                        await ProcessError(kw, seid, jobid, isOldPage);//14-08-2024
                    }
                    finally { }
                }
                OnKeywordDone.Invoke("Error:  seid: " + seid + ",  keyword: " + kw + ",  jobid: " + jobid + "\r\n\t" + ex.Message + "^" + statusCode + "^" + apitime + "^" + dbtime + "^" + totalTime);//22-01-2025
            }
        }

        private async Task ProcessError(string kw, string seid, string jobid, bool isOldPage)//14-08-2024
        {
            string dt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            string qry = "insert into dashboard_dataerrors (date, name, seid, jobid) values('" + dt + "', N'" +
                        kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "' )";

            string qryOld = "insert into dashboard_oldgooglepage (date, keyword, seid, jobid) values('" + DateTime.Now + "', N'" +
                kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "' )";

            using (SqlConnection con = new SqlConnection(await strConn()))
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
                            await comm.ExecuteNonQueryAsync();//14-08-2024
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

        public async Task<string> strConn()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                string fileName = @"C:\Inetpub\wwwroot\TrendingLiveAPI.xml";

                // You'll need to put the correct path to your xml file here
                xml.Load(fileName);

                // Select a specific node
                XmlNode node = xml.SelectSingleNode("TrendingAPI/con");

                // Get its value
                string name = node.InnerText;

                return await Task.FromResult<string>(name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task ProcessResults(string result, string kw, string seid, string jobid, int urlcount)//14-08-2024
        {                           
            if (string.IsNullOrEmpty(result))
            {
                throw new Exception("No result."); 
            }

            try
            {
                if (urlcount > 20)
                {
                    DateTime st = DateTime.Now;
                    await SendXmlToAPI(seid, kw, result);//14-08-2024
                    DateTime ed = DateTime.Now;
                    apitime = (ed - st).TotalSeconds;

                    DateTime st1 = DateTime.Now;
                    await SendToDB(seid, kw, result, urlcount);//14-08-2024
                    DateTime ed1 = DateTime.Now;
                    dbtime = (ed1 - st1).TotalSeconds;
                    //end of 31-03-2020
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }      

        private async Task SendXmlToAPI(string seid, string kw, string res)//14-08-2024
        {
            string tname = t1.Name;//14-08-2024
            string path = @"C:\Inetpub\wwwroot\oxycallback_" + tname + ".xml";

            XmlDocument xd = new XmlDocument();
            res = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + res;
            xd.LoadXml(res);
            xd.Save(path);
                 

            string submitURL = await readAPI();

            string user = "pisoftware";
            string pwd = "r00t123456";
            try
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(submitURL);
                httpWReq.UseDefaultCredentials = true;
                httpWReq.PreAuthenticate = true;
                httpWReq.Credentials = CredentialCache.DefaultCredentials;

                Encoding encoding = new UTF8Encoding();
                string postData = await GetTextFromXMLFile(path);//14-08-2024
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

                Stream stream = await httpWReq.GetRequestStreamAsync(); //14-08-2024
                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse response = (HttpWebResponse)await httpWReq.GetResponseAsync();//14-08-2024
                //string s = response.ToString();
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
                    errorMsg = string.Format("API Error: StatusCode {0}", httpResponse.StatusCode);

                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        errorMsg += "\r\n" + await reader.ReadToEndAsync();//14-08-2024
                    }
                }                    

                throw new Exception(errorMsg);

            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        private async Task<string> GetTextFromXMLFile(string file)//14-08-2024
        {
            StreamReader reader = new StreamReader(file);
            string ret = await reader.ReadToEndAsync();//14-08-2024
            reader.Close();
            return await Task.FromResult<string>(ret);
        }

        public async Task<string> readAPI()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                string fileName = @"C:\Inetpub\wwwroot\TrendingLiveAPI.xml";

                // You'll need to put the correct path to your xml file here
                xml.Load(fileName);

                // Select a specific node
                XmlNode node = xml.SelectSingleNode("TrendingAPI/submitapi");
                // Get its value
                string name = node.InnerText;

                return await Task.FromResult<string>(name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task SendToDB(string seid, string keyword, string xml, int urlcount)//14-08-2024
        {
            //string qry = "Insert into TrendingXmlResults(date, seid, keyword, xmldata) values('" + DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") + "', " + seid + ", N'" + keyword.Replace("'", "''") + "', N'" + xml.Replace("'", "''") + "') ";
            //qry += "Update [dbo].[Keywords] set status=1 where seid=" + seid + " and keyword=N'" + keyword.Replace("'", "''") + "'; ";

            try
            {
                using (SqlConnection con = new SqlConnection(await strConn()))
                {
                    con.Open();

                    using (SqlCommand comm = con.CreateCommand())
                    {
                        comm.CommandTimeout = 0;
                        comm.CommandType = CommandType.StoredProcedure;
                        comm.CommandText = "Insert_TrendingXMLResults";
                        comm.Parameters.Add("Date", SqlDbType.DateTime).Value = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
                        comm.Parameters.Add("Seid", SqlDbType.Int).Value = seid;
                        comm.Parameters.Add("Keyword", SqlDbType.NVarChar).Value = keyword; 
                        comm.Parameters.Add("XmlData", SqlDbType.Xml).Value = xml.Replace("'", "''");

                        await comm.ExecuteNonQueryAsync();//14-08-2024
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

