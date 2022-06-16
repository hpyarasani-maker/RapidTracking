using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Threading;
using System.Xml;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Bing_Receiving
{ 
    class HTMLParserNewTask
    {
        string myDate = DateTime.Today.ToString("yyyy-MM-dd");
        public event KeywordDone OnKeywordDone;
        public delegate void KeywordDone(string value);
        public HTMLParserNewTask()
        {
            Thread t1 = new Thread(new ThreadStart(StartProcess))
            {
                Name = "Bing_Mobile_GT20_2"
            };
            t1.Start();
        }

        private void StartProcess()
        {

            //string url = "http://previous.azurewebsites.net/api/callbackbingcomma/";
            //string url = "http://previous.azurewebsites.net/api/callbackbingdesktop/";  // bing desktop
            string url = "http://previous.azurewebsites.net/api/callbackbingmobile/"; //Bing mobile

            // WebClient client = new WebClient();
            Uri uri = new Uri(url);
            using (var client = new HttpClient())
            {
                while (true)
                {
                    /* try
                     {
                         string response="";
                         client.Encoding = Encoding.UTF8;
                         response = client.DownloadString(url);
                         if (response != "null")
                             DoProcess(response);
                     }
                     catch (Exception ex)
                         {
                             Console.WriteLine("# EXCEPTION #  " + ex.Message);
                         }*/

                    //client.BaseAddress = new Uri(url);
                    try
                    {
                        string response = "";
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        response = client.GetStringAsync(uri).Result;
                        if (response != "null")
                            DoProcess(response);
                    }

                    catch (Exception)
                    {

                        //throw new ArgumentException(message: ex.Message.ToString(), paramName: "response");
                        //OnKeywordDone.Invoke("Error:" + ex.Message.ToString());
                    }
                        
                    finally {
                       
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
            int seid=0;

            StringBuilder sb = new StringBuilder();

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
                    HttpWebResponse res = (HttpWebResponse) httpWebRequest.GetResponse();
                    Stream resStream = res.GetResponseStream();
                    StreamReader reader = new StreamReader(resStream, Encoding.UTF8);
                    string response = reader.ReadToEnd();
                    resStream.Close();
                    res.Close();
                    ArrayList arRes = new ArrayList();
                    try
                    {
                        SearchProperties sp = SearchParams.searches.Where(s => s.domain == domain && s.device == device && s.geo_location == gl).SingleOrDefault();
                        seid = sp.seid;
                        JObject obj = JObject.Parse(response);
                        var cont = obj["results"];//[0]["content"];
                        //int i = 0;
                        foreach (JObject jo in cont)
                        {
                            response = jo["content"].Value<string>();
                            sb.Append(response); 
                            //i++;
                            //ArrayList addURLs = new ArrayList();
                            //response = jo["content"].Value<string>(); //["content"].Value<string>();

                            ////System.IO.File.WriteAllText(@"c:\inetpub\wwwroot\html\" + kw + "_"+i+".html", response, Encoding.UTF8);
                            //if (device == "desktop")
                            //    addURLs = DesktopPattern(response);
                            //else
                            //    addURLs= MobilePattern(response);
                            //foreach(string str in addURLs)
                            //{
                            //    if(!arRes.Contains(str))
                            //    arRes.Add(str);
                            //}
                        }
                        if (device == "desktop")
                        {
                            //File.WriteAllText(@"C:\inetpub\wwwroot\html\" + jobid + "_" + kw + ".html", sb.ToString(), Encoding.UTF8);

                            arRes = DesktopPattern(sb.ToString()); 
                        }
                        else
                        {
                            //File.WriteAllText(@"C:\inetpub\wwwroot\html\" + jobid + "_" + kw + ".html", sb.ToString(), Encoding.UTF8);
                            arRes = MobilePattern(sb.ToString()); 
                        }

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                    if (arRes.Count > 100)                    
                        arRes.RemoveRange(100, arRes.Count - 100);
                    
                    if (seid > 0)                        
                        ProcessResults(arRes, kw, seid, jobid );

                    OnKeywordDone.Invoke(seid + ": " + kw + ": " + arRes.Count);
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Result Request: " + ex.Message);
                if (!string.IsNullOrEmpty(jobid))
                {
                    try
                    {
                        ProcessError(kw, seid, jobid);
                    }
                    finally { }
                }
                OnKeywordDone.Invoke("Error:  seid: " + seid + ",  keyword: " + kw + ",  jobid: " + jobid + "\r\n\t" + ex.Message);                
            }
        }

        private void ProcessError(string kw, int seid, string jobid)
        {
            //string qry = "update dashboard_data set status = 'Error' where name='" + kw.Replace("'", "''") + "' and seid = " + seid + " and jobid='" + jobid + "'";
            string qry = "insert into dashboard_dataerrors (date, name, seid, jobid) values(Convert(varchar(10),'" + myDate + "',103), N'" + 
                kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "' )";

            using (SqlConnection con = new SqlConnection(strConn()))
            {
                try
                {
                    con.Open();
                    using (SqlCommand comm = new SqlCommand(qry, con))
                    {
                        comm.CommandTimeout = 0;
                        comm.ExecuteNonQuery();
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

       public string strConn()
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
        private void ProcessResults(ArrayList alRes, string kw, int seid, string jobid)
        {          
            //string qry = "";

            if (alRes.Count < 1)
            {
                Console.WriteLine("There is no result for " + kw);
                throw new Exception("No Results") ;
            }
            else
            {
                Console.WriteLine("Processing the keyword: " + kw);

                string tname = Thread.CurrentThread.Name;

                string path = @"C:\Inetpub\wwwroot\oxycallback" + tname + ".xml" ;     
                
                string result = string.Empty;
                try
                {
                    MemoryStream stream = new MemoryStream();
                    using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8))
                    {
                        writer.Formatting = System.Xml.Formatting.Indented;
                        writer.Indentation = 2;
                        writer.WriteStartDocument();

                        writer.WriteStartElement("", "searchResults", "");

                        writer.WriteStartElement("", "searchResult", "");
                        writer.WriteStartAttribute("searchEngineId");
                        writer.WriteString(seid.ToString());
                        writer.WriteStartAttribute("keyword");
                        writer.WriteString(kw);
                        writer.WriteStartAttribute("date");
                        writer.WriteString(myDate);
                        string c = string.Empty;
                        int k;
                                       
                        for (int i = 0; i < alRes.Count; i++)
                        {
                            k = i + 1;
                            c = k.ToString();

                            writer.WriteStartElement("", "url", "");
                            writer.WriteStartAttribute("position");
                            writer.WriteString(c);
                            writer.WriteEndAttribute();
                            string dURL = alRes[i].ToString();
                            writer.WriteString(dURL);
                            writer.WriteEndElement();
                            //Insert100DashBoardData(myDate, kw, seid.ToString(),k, alRes[i].ToString());
                            //qry += "insert into dashboard_yandex(date,name,seid,position,url)values(Convert(varchar(10),'" + myDate + "',103),N'" + kw.Replace("'", "''") + "'," + seid + ",'" + c + "',N'" + dURL.ToString().Replace("'", "''") + "')";
                        }
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndDocument();

                        writer.Flush();
                        writer.Flush();
                        //writer.Close();
                        Encoding utf = Encoding.UTF8;
                        result = utf.GetString(stream.GetBuffer(), 0, (int)stream.Length);
                        stream.Close();
                    }

                    if (!string.IsNullOrEmpty(result))
                    {
                        StreamWriter sw = new StreamWriter(path, false);
                        sw.Write(result);
                        sw.Close();
                    }
                    if (!string.IsNullOrEmpty(result))
                    {
                        try
                        {
                            if (alRes.Count > 50)
                            {
                                SendXmlToAPI(path);
                                //Insert100DashBoardData(qry); //storing 100 URLs
                                InsertDashBoardData_Callback(kw, seid, alRes.Count, alRes[0].ToString(), jobid);
                            }
                            //else
                            //{
                            //   InsertDashBoardData_Callback( kw, seid, alRes, jobid, true);
                            //}
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }               
            }
        }

        private void SendXmlToAPI(string path)
        {             
            string submitURL = readAPI();

            //string user = "pi-tracking";
            //string pwd = "ipseo2001";

            string user = "pisoftware";
            string pwd = "r00t123456";

            try
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(submitURL);
                //httpWReq = (HttpWebRequest)WebRequest.Create(submitURL);
                httpWReq.UseDefaultCredentials = true;
                httpWReq.PreAuthenticate = true;
                httpWReq.Credentials = CredentialCache.DefaultCredentials;

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


                Stream stream = httpWReq.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                string s = response.ToString();
                StreamReader reader = new StreamReader(response.GetResponseStream());

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
                        errorMsg += "\r\n" + reader.ReadToEnd();
                    }                    
                }
                throw new Exception(errorMsg);
            }
        }

        private string GetTextFromXMLFile(string file)
        {
            StreamReader reader = new StreamReader(file);
            string ret = reader.ReadToEnd();
            reader.Close();
            return ret;
        }

        public string readAPI()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                string fileName = @"C:\Inetpub\wwwroot\Callback_TrackingTrending.xml";
                // You'll need to put the correct path to your xml file here
                xml.Load(fileName);

                // Select a specific node
                XmlNode node = xml.SelectSingleNode("ConnectionString/apiSubmit");
                // Get its value
                string name = node.InnerText;

                return name;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void Insert100DashBoardData(string qry)
        {
            //string strInsert = "insert into dashboard_yandex(date,name,seid,position,url)values(Convert(varchar(10),'" + ddate + "',103),N'" + kwd.Replace("'", "''") + "'," + seid + ",'"+ position +"',N'" + url.Replace("'", "''") + "')";

            try
            {
                using (SqlConnection con = new SqlConnection(strConn()))
                {
                    con.Open();
                    using (SqlCommand comm = new SqlCommand(qry, con))
                    {
                        comm.CommandTimeout = 0;
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

        private void InsertDashBoardData_Callback( string kw, int seid, int cnt, string alRes, string jobid)
        {
            string qry = "insert into dashboard_data(date, name, seid, jobid, count, url) " +
                    "values(Convert(varchar(10),'" + myDate + "',103),N'" + kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "', " + cnt + ", N'" + alRes.Replace("'", "''") + "')";

            try
            {
                using (SqlConnection con = new SqlConnection(strConn()))
                {
                    con.Open();
                    using (SqlCommand comm = new SqlCommand(qry, con))
                    {
                        comm.CommandTimeout = 0;
                        comm.ExecuteNonQuery();

                        

                        //if (lt20)
                        //{
                        //    string url = alRes.Count > 0 ? alRes[0].ToString() : "";
                        //    qry = "exec [InsertLessthan20] '" + myDate + "',N'" + kw.Replace("'", "''") + "'," + seid + ",N'" + url.Replace("'", "''") + "'," + alRes.Count + ",'" + jobid + "'";
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



        
        private ArrayList MobilePattern(string htmlsource)
        {
            ArrayList googleList = new ArrayList();
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlsource);
                ArrayList alDup = new ArrayList();

                //HtmlNodeCollection node = doc.DocumentNode.SelectNodes("//ol[@id='b_results']/li[@class='b_algo']");
                HtmlNodeCollection node = doc.DocumentNode.SelectNodes(".//ol[@id='b_results']/li[@class='b_algo']/div[@class='b_algoheader']|//ol[@id='b_results']/li[@class='b_algo']|//div[@class='b_algoheader']|//div[@class='b_algoheader b_removeline12px']|//div[@class='b_algoheader b_removeline8px']");

                foreach (HtmlNode links in node)
                {
                    try
                    {
                        HtmlNode a = links.SelectSingleNode(".//a");
                        string urls = a.Attributes["href"].Value;
                        if (urls.StartsWith("http") || urls.StartsWith("https"))
                        {
                            int indx = urls.LastIndexOf("http://");
                            if (indx < 0)
                            {
                                indx = urls.LastIndexOf("https://");
                            }
                            urls = urls.Remove(0, indx);
                            if (urls.Contains("www.bing.com/ck/a"))
                            {
                                alDup.Add(redirecturls(new Uri(urls)).Result);
                            }
                            alDup.Add(HttpUtility.HtmlDecode(urls));
                        }
                    }
                    catch { continue; }
                }
                foreach (string s in alDup)
                {
                    if (googleList.Contains(s) || string.IsNullOrEmpty(s)) continue;
                    googleList.Add(s);
                }

                if (googleList.Count > 100)
                {
                    googleList.RemoveRange(100, googleList.Count - 100);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("No pattern match,  " + ex.Message);
            }
            return googleList;
        }  

        private ArrayList DesktopPattern( string htmlsource)
        {
            ArrayList googleList = new ArrayList();
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlsource);
                ArrayList alDup = new ArrayList();

                //HtmlNodeCollection node = doc.DocumentNode.SelectNodes("//ol[@id='b_results']/li[@class='b_algo']");
                HtmlNodeCollection node = doc.DocumentNode.SelectNodes("//ol[@id='b_results']/li[@class='b_algo']|//ol[@id='b_results']/li[@class='b_algo']/h2|//div[@class='b_algoheader']|//ol[@id='b_results']//li[@class='b_algo']/div[@class='b_title']/h2|//li[@class='b_algo']/h2"); //seid = 5

                foreach (HtmlNode links in node)
                {
                    try
                    {
                        HtmlNode a = links.SelectSingleNode(".//a");
                        string urls = a.Attributes["href"].Value;
                        if (urls.StartsWith("http") || urls.StartsWith("https"))
                        {
                            int indx = urls.LastIndexOf("http://");
                            if (indx < 0)
                            {
                                indx = urls.LastIndexOf("https://");
                            }
                            urls = urls.Remove(0, indx);
                            if (urls.Contains("www.bing.com/ck/a"))
                            {
                                alDup.Add(redirecturls(new Uri(urls)).Result);
                            }
                            alDup.Add(HttpUtility.HtmlDecode(urls));
                        }
                    }
                    catch { continue; }
                }

                foreach (string s in alDup)
                {
                    if (googleList.Contains(s)|| string.IsNullOrEmpty(s)) continue;
                    googleList.Add(s);
                }

                if (googleList.Count > 100)
                {
                    googleList.RemoveRange(100, googleList.Count - 100);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("No pattern match,  " + ex.Message);
            }
            return googleList;
        }
        public async Task<string> redirecturls(Uri url)
        {
            string redirectedUrl = null;
            try
            {
                var a = new HttpClientHandler()
                {
                    AllowAutoRedirect = false
                };
                using (HttpClient client = new HttpClient(a))
                using (HttpResponseMessage response = await client.GetAsync(url))
                using (HttpContent content = response.Content)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Found)
                    {
                        HttpResponseHeaders headers = response.Headers;
                        if (headers != null && headers.Location != null)
                        {
                            redirectedUrl = headers.Location.AbsoluteUri;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("No url match,  " + ex.Message);
            }
            return redirectedUrl;
        }
    }
}

