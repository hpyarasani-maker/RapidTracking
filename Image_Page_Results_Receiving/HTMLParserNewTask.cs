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
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Threading.Tasks;

namespace Image_Page_Results_Receiving
{
    class HTMLParserNewTask
    {
        string myDate = DateTime.Today.ToString("yyyy-MM-dd");
        public event KeywordDone OnKeywordDone;        
        //public delegate void KeywordDone(string value);
        Thread t1;
        public HTMLParserNewTask()
        {
            t1 = new Thread(new ThreadStart(StartProcess))
            {
                Name = "74_1"                
            };
            t1.Start();
        }

        private async void StartProcess()
        {
            string url = "https://seresults.azurewebsites.net/api/callback74images/"; // 74 desktop page urls 
            //string url = "https://seresults.azurewebsites.net/api/callbackrapidtrackingimagedesktop/"; // image urls //402 mobile images links
            //string url = "https://seresults.azurewebsites.net/api/callbackimages/"; //401 desktop image links
            //string url = "https://seresults.azurewebsites.net/api/callbacknews/";      // news 140 and 382 Mobile Image Page Links


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
                        response = await client.GetStringAsync(ul); //06-08-2024  //.Result;
                        if (response != "null")
                            await DoProcess(response);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("# EXCEPTION #  " + ex.Message);
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
            int seid = 0;
            try
            {
                //string username = "gpidatametrics";
                //string password = "sdV5X3fcX6";
                string username = "piapp";
                string password = "b5FCvgkjxx";

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
                    ArrayList arRes = new ArrayList();

                    try
                    {                        
                        // Image Links 401 Desktop and 402 Mobile image links
                        //SearchProperties sp = SearchParamsImageUrls.searches.Where(s => s.locale == hl && s.device == device && s.geo_location == gl && s.tbm == "isch").SingleOrDefault();

                        // Page Links 74 Desktop PageLinks and 382 Mobile Page Links
                        SearchProperties sp = SearchParamsPageUrls.searches.Where(s => s.locale == hl && s.device == device && s.geo_location == gl && s.tbm == "isch").SingleOrDefault();

                        // Google News
                        //SearchProperties sp = SearchParamsImageUrls.searches.Where(s => s.locale == hl && s.device == device && s.geo_location == gl && s.tbm == "nws").SingleOrDefault();

                        seid = sp.seid;
                    
                        JObject obj = JObject.Parse(response);
                        StringBuilder sb = new StringBuilder();

                        var cont = obj["results"];
                        foreach (JObject jo in cont)
                        {
                            sb.Append(jo["content"].Value<string>());
                        }
                        //File.WriteAllText(@"c:\inetpub\wwwroot\html\" + jobid + "_" + HttpUtility.UrlDecode(kw) + ".html", sb.ToString(), Encoding.UTF8);

                        //Un Comment for Image Links 401 and 402
                        //if (device == "desktop_chrome") //401 Desktop Image Links
                        //    arRes = ImagesPatternDesktop401(sb.ToString(), "ImageLinks");
                        //else //402 Mobile Image Links
                        //    arRes = ImagesPatternMobile402(sb.ToString(), "ImageLinks");


                        //Un comment for Page Links Seid 74 and 382
                        if (device == "desktop_chrome") //74 Desktop Page Links
                            arRes = ImagesPatternDesktop74(sb.ToString(), "PageLinks");
                        else //382 Mobile Page Links
                            arRes = ImagesPatternMobile382(sb.ToString(), "PageLinks");


                        // Google News
                        //if (device == "desktop_chrome")
                        //    arRes = NewsPattern(sb.ToString());

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    if (seid > 0)
                      await ProcessResults(arRes, kw, seid, jobid);

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
                       await ProcessError(kw, seid, jobid);
                    }
                    finally { }
                }
                OnKeywordDone.Invoke("Error:  seid: " + seid + ",  keyword: " + kw + ",  jobid: " + jobid + "\r\n\t" + ex.Message);
            }
        }

        private async Task ProcessError(string kw, int seid, string jobid)
        {
            string qry = "insert into dashboard_dataerrors (date, name, seid, jobid) values(Convert(varchar(10),'" + myDate + "',103), N'" +
                kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "' )";

            using (SqlConnection con = new SqlConnection(strConn().Result))
            {
                try
                {
                    con.Open();
                    using (SqlCommand comm = new SqlCommand(qry, con))
                    {
                        comm.CommandTimeout = 0;
                         await comm.ExecuteNonQueryAsync();
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
                string fileName = @"C:\Inetpub\wwwroot\ServerIP_Callback.xml";
                // You'll need to put the correct path to your xml file here
                xml.Load(fileName);

                // Select a specific node
                XmlNode node = xml.SelectSingleNode("ConnectionString/con");
                // Get its value
                string name = node.InnerText;

                return await Task.FromResult<string>(name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task ProcessResults(ArrayList alRes, string kw, int seid, string jobid)
        {
            if (alRes.Count < 1)
            {
                Console.WriteLine("There is no result for " + kw);
                throw new Exception("No Results");
            }
            else
            {
                Console.WriteLine("Processing the keyword: " + kw);

                string tname = t1.Name;

                string path = @"C:\Inetpub\wwwroot\oxycallback_" + tname + ".xml";

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
                            writer.WriteString(alRes[i].ToString());
                            writer.WriteEndElement();
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
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                if (!string.IsNullOrEmpty(result))
                {
                    try
                    {
                        if (alRes.Count > 20)
                        {
                           await SendXmlToAPI(path);
                           await InsertDashBoardData_Callback(kw, seid, alRes, jobid);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        private async Task SendXmlToAPI(string path)
        {
            string submitURL = readAPI();

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

                HttpWebResponse response = (HttpWebResponse)await httpWReq.GetResponseAsync(); //06-08-2024
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
                string fileName = @"C:\Inetpub\wwwroot\ServerIP_Callback.xml";
                // You'll need to put the correct path to your xml file here
                xml.Load(fileName);

                // Select a specific node
                XmlNode node = xml.SelectSingleNode("ConnectionString/apiNew");
                // Get its value
                string name = node.InnerText;

                return name;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task InsertDashBoardData_Callback(string kw, int seid, ArrayList alRes, string jobid, bool lt20 = false)
        {
            string qry = "insert into dashboard_data(date, name, seid, jobid, count, url) " +
                    "values(Convert(varchar(10),'" + myDate + "',103),N'" + kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "', " + alRes.Count + ", N'" + alRes[0].ToString().Replace("'", "''") + "')";

            try
            {
                using (SqlConnection con = new SqlConnection(strConn().Result))
                {
                    con.Open();
                    using (SqlCommand comm = new SqlCommand(qry, con))
                    {
                        comm.CommandTimeout = 0;
                        await comm.ExecuteNonQueryAsync();
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

        private ArrayList NewsPattern(string htmlsource)
        {
            ArrayList googleList = new ArrayList();
            try
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlsource);
                ArrayList alDup = new ArrayList();

                HtmlNodeCollection node = doc.DocumentNode.SelectNodes("//a[@class='top NQHJEb dfhHve']|.//div[@class='dbsr']/a|.//div[@class='ZINbbc xpd O9g5cc uUPGi']/div[@class='kCrYT'][1]/a|.//div[@class='CC9D1e']/a|.//div[@class='mCCq8b dbsr']/a|.//a[@class='WlydOe']");
                if(node != null)
                {
                    foreach (HtmlNode links in node)
                    {
                        try
                        {
                            //HtmlNode a = links.SelectSingleNode(".//a");
                            string urls = links.Attributes["href"].Value.Replace("/url?q=", ""); ///url?esrc=s&amp;q=&amp;rct=j&amp;sa=U&amp;url=
                            if (urls.StartsWith("http") || urls.StartsWith("https"))
                            {
                                int indx = urls.LastIndexOf("http://");
                                if (indx < 0)
                                {
                                    indx = urls.LastIndexOf("https://");
                                }
                                urls = urls.Remove(0, indx);
                                if (!urls.Contains("jobalert.ie"))
                                    alDup.Add(HttpUtility.HtmlDecode(urls));
                            }
                        }
                        catch { continue; }
                    }
                    foreach (string s in alDup)
                    {
                        string s1 = s;
                        int index = s1.IndexOf("&sa=U");
                        if (googleList.Contains(s1) || string.IsNullOrEmpty(s1)) continue;
                        googleList.Add(s1);
                    }

                    if (googleList.Count > 100)
                    {
                        googleList.RemoveRange(100, googleList.Count - 100);
                    }
                }
            }
            catch { }
            return googleList;
        }

        private ArrayList ImagesPatternDesktop401(string htmlsource, string urlType)
        {
            ArrayList googleList = new ArrayList();
            try
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlsource);
                ArrayList alDup = new ArrayList();

                string pattern1 = @"\],\[""(.*?).jpg"",";

                Regex rx = new Regex(pattern1, RegexOptions.IgnoreCase);
                MatchCollection mc = rx.Matches(htmlsource);

                foreach (Match m in mc)
                {
                    string url = "http" + m.Groups[1].Value;
                    int indx = url.LastIndexOf("http://");
                    if (indx < 0)
                    {
                        indx = url.LastIndexOf("https://");
                    }
                    url = url.Remove(0, indx);
                    if (url.StartsWith("http") || url.StartsWith("https"))
                    {
                        if (!url.Contains("htm") || !url.Contains(",null"))
                            if (!url.Contains("jpg"))
                            {
                                url += ".jpg";
                            }
                        alDup.Add(HttpUtility.HtmlDecode(url));
                    }
                }
                foreach (string s in alDup)
                {
                    if (googleList.Contains(s) || string.IsNullOrEmpty(s)) continue;
                    if (s.Contains(",null")) continue;
                    googleList.Add(s);
                }
                if (googleList.Count > 100)
                {
                    googleList.RemoveRange(100, googleList.Count - 100);
                }
            }
            catch { }
            return googleList;
        }

        private ArrayList ImagesPatternMobile402(string htmlsource, string urlType)
        {
            ArrayList googleList = new ArrayList();
            try
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlsource);
                ArrayList alDup = new ArrayList();

                string pattern1 = @"\],\[""(.*?).jpg"",";

                Regex rx = new Regex(pattern1, RegexOptions.IgnoreCase);
                MatchCollection mc = rx.Matches(htmlsource);

                foreach (Match m in mc)
                {
                    string url = "http" + m.Groups[1].Value;
                    int indx = url.LastIndexOf("http://");
                    if (indx < 0)
                    {
                        indx = url.LastIndexOf("https://");
                    }
                    url = url.Remove(0, indx);
                    if (url.StartsWith("http") || url.StartsWith("https"))
                    {
                        if (!url.Contains("htm") || !url.Contains(",null"))
                            if (!url.Contains("jpg"))
                            {
                                url += ".jpg";
                            }
                        alDup.Add(HttpUtility.HtmlDecode(url));
                    }
                }
                foreach (string s in alDup)
                {
                    if (googleList.Contains(s) || string.IsNullOrEmpty(s)) continue;
                    if (s.Contains(",null")) continue;
                    googleList.Add(s);
                }
                if (googleList.Count > 100)
                {
                    googleList.RemoveRange(100, googleList.Count - 100);
                }
            }
            catch { }
            return googleList;
        }

        private ArrayList ImagesPatternDesktop74(string htmlsource, string urlType)
        {
            ArrayList googleList = new ArrayList();
            try
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlsource);
                ArrayList alDup = new ArrayList();

                HtmlNodeCollection node = doc.DocumentNode.SelectNodes(".//div[@class='juwGPd BwPElf OCzgxd']/a[@class='EZAeBe']");

                if (node != null)
                {
                    if (urlType == "PageLinks") 
                    {
                        foreach (HtmlNode links in node)
                        {
                            try
                            {
                                string url = links.Attributes["href"].Value.Replace("/url?q=", "");

                                if (url.StartsWith("http") || url.StartsWith("https"))
                                {
                                    int indx = url.LastIndexOf("http://");
                                    if (indx < 0)
                                    {
                                        indx = url.LastIndexOf("https://");
                                    }
                                    url = url.Remove(0, indx);
                                    if (url.Contains("&amp;sa="))
                                        url = url.Remove(url.IndexOf("&amp;sa="));
                                    if (url.Contains("&sa="))
                                        url = url.Remove(url.IndexOf("&sa="));
                                    alDup.Add(HttpUtility.HtmlDecode(url));
                                }
                            }
                            catch { continue; }
                        }
                    }
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
            catch { }
            return googleList;
        }

        private ArrayList ImagesPatternMobile382(string htmlsource, string urlType)
        {
            ArrayList googleList = new ArrayList();
            try
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlsource);
                ArrayList alDup = new ArrayList();

                HtmlNodeCollection node = doc.DocumentNode.SelectNodes(".//div[@class='kb0PBd cvP2Ce']/a[@class='LBcIee']");

                if (node != null)
                {
                    if (urlType == "PageLinks") 
                    {
                        foreach (HtmlNode links in node)
                        {
                            try
                            {
                                string url = links.Attributes["href"].Value.Replace("/url?q=", "");
                                if (url.StartsWith("http") || url.StartsWith("https"))
                                {
                                    int indx = url.LastIndexOf("http://");
                                    if (indx < 0)
                                    {
                                        indx = url.LastIndexOf("https://");
                                    }
                                    url = url.Remove(0, indx);
                                    if (url.Contains("&amp;sa="))
                                        url = url.Remove(url.IndexOf("&amp;sa="));
                                    if (url.Contains("&sa="))
                                        url = url.Remove(url.IndexOf("&sa="));
                                    alDup.Add(HttpUtility.HtmlDecode(url));
                                }
                            }
                            catch { continue; }
                        }
                    }
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
            catch { }
            return googleList;
        }
    }
}

