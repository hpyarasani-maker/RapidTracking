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

namespace Image_Page_Results_Receiving
{
    class HTMLParserNewTask
    {
        string myDate = DateTime.Today.ToString("yyyy-MM-dd");
        public event KeywordDone OnKeywordDone;

        public HTMLParserNewTask()
        {
            Thread t1 = new Thread(new ThreadStart(StartProcess))
            {
                Name = "M_images_PL"
                //Name = "pages"
            };
            t1.Start();
        }

        private void StartProcess()
        {
            //string url = "https://seresults.azurewebsites.net/api/callbackuk58desktop/";       // 58
            //string url = "https://seresults.azurewebsites.net/api/callbackuk106mobile/";      // 106
            //string url = "https://seresults.azurewebsites.net/api/callbackus1desktop/";     // 1
            //string url = "https://seresults.azurewebsites.net/api/callbackus102mobile/";      // 102
            //string url = "https://seresults.azurewebsites.net/api/callbackotherdesktop/";  // other desktop
            //string url = "https://seresults.azurewebsites.net/api/callbackothermobile/";   // other mobiles

            //string url = "https://seresults.azurewebsites.net/api/callbackrapidtrackingimagedesktop/"; // image urls //402 mobile images links
            //string url = "https://seresults.azurewebsites.net/api/callback74images/"; // 74 desktop page urls 
            //string url = "https://seresults.azurewebsites.net/api/callbackimages/"; //401 desktop image links
            string url = "https://seresults.azurewebsites.net/api/callbacknews/";      // news 140 and 382 Mobile Image Page Links



            WebClient client = new WebClient();
            while (true)
            {
                try
                {
                    string response = "";
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
                        //File.WriteAllText(@"C:\inetpub\wwwroot\html\" + jobid + "_" + kw + ".html", response, Encoding.UTF8);
                        //// Image Links 401 Desktop and 402 Mobile image links
                        //SearchProperties sp = SearchParamsImageUrls.searches.Where(s => s.locale == hl && s.device == device && s.geo_location == gl && s.tbm == "isch").SingleOrDefault();

                        // Page Links 74 Desktop PageLinks and 382 Mobile Page Links
                        SearchProperties sp = SearchParamsPageUrls.searches.Where(s => s.locale == hl && s.device == device && s.geo_location == gl && s.tbm == "isch").SingleOrDefault();

                        //// News
                        //SearchProperties sp = SearchParamsImageUrls.searches.Where(s => s.locale == hl && s.device == device && s.geo_location == gl && s.tbm == "nws").SingleOrDefault();

                        seid = sp.seid;
                    
                        JObject obj = JObject.Parse(response);
                        var contents = obj["results"];

                        for (int x = 0; x < contents.Count(); x++)
                        {
                            response = contents[x]["content"].Value<string>();

                            //// Image Links 401 and 402
                            //if (device == "desktop") //401 Desktop Image Links
                            //    arRes = ImagesPatternDesktop(response, "ImageLinks", arRes);
                            //else //402 Mobile Image Links
                            //    arRes = ImagesPatternMobile(response, "ImageLinks", arRes);


                            //Page Links 74 and 382
                            if (device == "desktop") //74 Desktop Page Links
                                arRes = ImagesPatternDesktop(response, "PageLinks", arRes);
                            else //74 Mobile Page Links
                                arRes = ImagesPatternMobile(response, "PageLinks", arRes);
                        }

                        // News
                        //if (device == "desktop")
                        //arRes = NewsPattern(response);

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    if (seid > 0)
                        ProcessResults(arRes, kw, seid, jobid);

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
                string fileName = @"C:\Inetpub\wwwroot\ServerIP_Callback.xml";
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
            if (alRes.Count < 1)
            {
                Console.WriteLine("There is no result for " + kw);
                throw new Exception("No Results");
            }
            else
            {
                Console.WriteLine("Processing the keyword: " + kw);

                string tname = Thread.CurrentThread.Name;

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
                            SendXmlToAPI(path);
                            InsertDashBoardData_Callback(kw, seid, alRes, jobid);
                        }
                        else
                        {
                            InsertDashBoardData_Callback(kw, seid, alRes, jobid, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
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

        private void InsertDashBoardData_Callback(string kw, int seid, ArrayList alRes, string jobid, bool lt20 = false)
        {
            string qry = "insert into dashboard_data(date, name, seid, jobid, count, url) " +
                    "values(Convert(varchar(10),'" + myDate + "',103),N'" + kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "', " + alRes.Count + ", N'" + alRes[0].ToString().Replace("'", "''") + "')";

            try
            {
                using (SqlConnection con = new SqlConnection(strConn()))
                {
                    con.Open();
                    using (SqlCommand comm = new SqlCommand(qry, con))
                    {
                        comm.CommandTimeout = 0;
                        comm.ExecuteNonQuery();

                        if (lt20)
                        {
                            string url = alRes.Count > 0 ? alRes[0].ToString() : "";
                            qry = "exec [InsertLessthan20] '" + myDate + "',N'" + kw.Replace("'", "''") + "'," + seid + ",N'" + url.Replace("'", "''") + "'," + alRes.Count + ",'" + jobid + "'";
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

        private ArrayList NewsPattern(string htmlsource)
        {
            ArrayList googleList = new ArrayList();
            try
            {
                var doc = new HtmlDocument();
                htmlsource = htmlsource.Replace(@"\", "");
                doc.LoadHtml(htmlsource);
                ArrayList alDup = new ArrayList();

                HtmlNodeCollection node = doc.DocumentNode.SelectNodes("//a[@class='top NQHJEb dfhHve']");

                foreach (HtmlNode links in node)
                {
                    try
                    {
                        string urls = links.Attributes["href"].Value;
                        if (urls.StartsWith("http") || urls.StartsWith("https"))
                        {
                            int indx = urls.LastIndexOf("http://");
                            if (indx < 0)
                            {
                                indx = urls.LastIndexOf("https://");
                            }
                            urls = urls.Remove(0, indx);

                            //string links1 = HttpUtility.UrlDecode(urls);

                            alDup.Add(HttpUtility.HtmlDecode(urls.Replace("|", "%7C").Replace("^", "%5E")));
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

        private ArrayList ImagesPatternDesktop(string htmlsource, string urlType, ArrayList googleList)
        {
            //ArrayList googleList = new ArrayList();
            try
            {
                htmlsource = htmlsource.Replace(@"\", "");
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlsource);
                ArrayList alDup = new ArrayList();

                //HtmlNodeCollection node = doc.DocumentNode.SelectNodes("//*[@id=\"rg_s\"]/div/div");//Mw2I7 UkaFJe
                HtmlNodeCollection node = doc.DocumentNode.SelectNodes("//div[@class=\"rg_meta notranslate\"]");
                //HtmlNodeCollection node = doc.DocumentNode.SelectNodes("//div[@class=\"Mw2I7 UkaFJe\"]");

                if (node != null)
                {
                    foreach (HtmlNode links in node)
                    {
                        try
                        {
                            string url = "";

                            if (urlType == "ImageLinks")

                                url = JObject.Parse(links.InnerText)["ou"].Value<string>();
                            else
                                url = JObject.Parse(links.InnerText)["ru"].Value<string>();

                            if (url.StartsWith("http") || url.StartsWith("https"))
                            {
                                int indx = url.LastIndexOf("http://");
                                if (indx < 0)
                                {
                                    indx = url.LastIndexOf("https://");
                                }
                                url = url.Remove(0, indx);
                                alDup.Add(HttpUtility.HtmlDecode(url));
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                else
                {
                    if (urlType == "PageLinks") //SEID=74 Desktop PageLinks //24-06-2022
                    {
                        string pattern1 = "<table class=\\WIkMU6e\\W><tr><td><a href=(.*?)&";
                        //string pattern = @"\]n,\[""http(.*?)\"",";
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
                            alDup.Add(HttpUtility.HtmlDecode(url));
                        }
                    }

                    else  // 74
                    {
                        //string pattern = "x22 targetx3dx22_blankx22 hrefx3dx22(.*?)x22 ";
                        string pattern = "<table class=\\WIkMU6e\\W><tr><td><a href=(.*?)&";
                        Regex rx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection mc = rx.Matches(htmlsource);
                        foreach (Match m in mc)
                        {
                            string url = m.Groups[1].Value;

                            if (url.StartsWith("http") || url.StartsWith("https"))
                            {
                                int indx = url.LastIndexOf("http://");
                                if (indx < 0)
                                {
                                    indx = url.LastIndexOf("https://");
                                }
                                url = url.Remove(0, indx);
                                alDup.Add(HttpUtility.HtmlDecode(url));
                            }
                        }
                    }
                }
                if (urlType == "ImageLinks") //401 Desktop Image links //25-06-2022
                {
                    string pattern1 = "<div class=\\WNZWO1b\\W><img class=\\WyWs4tf\\W alt=\"\" src=(.*?)&amp;s";
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
                        alDup.Add(HttpUtility.HtmlDecode(url));
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
            catch (Exception ex)
            {
                throw new Exception("No pattern match,  " + ex.Message);
            }

            return googleList;
        }

        private ArrayList ImagesPatternMobile(string htmlsource, string urlType, ArrayList googleList)
        {
            //ArrayList googleList = new ArrayList();
            try
            {
                htmlsource = htmlsource.Replace(@"\", "");
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlsource);
                ArrayList alDup = new ArrayList();

                HtmlNodeCollection node;
                if (urlType == "ImageLinks")
                {
                    node = doc.DocumentNode.SelectNodes("//a[@jsname=\"m8x3S\"]/img");
                    if (node == null)
                        node = doc.DocumentNode.SelectNodes("//div[@class=\"rg_meta notranslate\"]");
                }
                else
                {
                    node = doc.DocumentNode.SelectNodes("//a[@class=\"VFACy kGQAp\"]");
                    if (node == null)
                        node = doc.DocumentNode.SelectNodes("//a[@class=\"VFACy\"]");
                    //*03-04-2020
                    if (node == null)
                        node = doc.DocumentNode.SelectNodes("//div[@class=\"rg_meta notranslate\"]");//*03-04-2020  

                }
                if (node != null)
                {
                    foreach (HtmlNode links in node)
                    {
                        try
                        {
                            string url = "";

                            if (urlType == "ImageLinks")
                            {
                                try
                                {
                                    url = links.Attributes["data-iurl"].Value;
                                }
                                catch
                                {
                                    try
                                    {
                                        url = links.Attributes["data-src"].Value;
                                    }
                                    catch
                                    {
                                        url = JObject.Parse(links.InnerText)["ou"].Value<string>();
                                    }

                                }
                            }
                            //*03-04-2020
                            else if (urlType == "PageLinks")
                            {
                                try
                                {
                                    url = JObject.Parse(links.InnerText)["ru"].Value<string>();
                                }
                                catch
                                {
                                    url = links.Attributes["href"].Value;
                                }
                            }//*03-04-2020
                            else
                                url = links.Attributes["href"].Value;

                            if (url.StartsWith("http") || url.StartsWith("https"))
                            {
                                int indx = url.LastIndexOf("http://");
                                if (indx < 0)
                                {
                                    indx = url.LastIndexOf("https://");
                                }
                                url = url.Remove(0, indx);
                                alDup.Add(HttpUtility.HtmlDecode(url));
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    if (urlType == "PageLinks") //SEID=382 mobile image page links //25-06-2022
                    {
                        string pattern = "imgrefurl=(.*?)&amp;"; //25-06-2022
                        Regex rx = new Regex(pattern, RegexOptions.IgnoreCase);
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
                            alDup.Add(HttpUtility.HtmlDecode(url));
                        }
                    }
                    if (urlType == "ImageLinks") //SEID=402 Mobile Image Links //25-06-2022
                    {
                        //string pattern = @"\]n,\[""http(.*?)\"",";
                        string pattern1 = "imgurl=(.*?)&amp;"; //25-06-2022
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
                            alDup.Add(HttpUtility.HtmlDecode(url));
                        }
                    }

                    else
                    {
                        string pattern = "x22 targetx3dx22_blankx22 hrefx3dx22(.*?)x22 ";
                        Regex rx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection mc = rx.Matches(htmlsource);
                        foreach (Match m in mc)
                        {
                            string url = m.Groups[1].Value;

                            if (url.StartsWith("http") || url.StartsWith("https"))
                            {
                                int indx = url.LastIndexOf("http://");
                                if (indx < 0)
                                {
                                    indx = url.LastIndexOf("https://");
                                }
                                url = url.Remove(0, indx);
                                alDup.Add(HttpUtility.HtmlDecode(url));
                            }
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
            catch (Exception ex)
            {
                throw new Exception("No pattern match,  " + ex.Message);
            }

            return googleList;
        }

        private ArrayList MobilePattern(string htmlsource)
        {
            ArrayList googleList = new ArrayList();
            try
            {
                var doc = new HtmlDocument();
                htmlsource = htmlsource.Replace(@"\", "");
                doc.LoadHtml(htmlsource);
                ArrayList alDup = new ArrayList();

                HtmlNodeCollection node = doc.DocumentNode.SelectNodes("//a[@class='C8nzq']|//a[@class='Rk4fgb']|//div[@id='rso']/div/div/div/a[1]|//a[@class='JTuIPc']|//a[@class='C8nzq BmP5tf']|//a[@class='BmP5tf']|//a[@class='sXtWJb']|//a[@class='C8nzq BmP5tf amp_r']|//div[@jsl='$t t-4cfX2GiP_Fk;$x 0;']/a|//g-link[not(contains(@class,'fl'))]/a");

                foreach (HtmlNode links in node)
                {
                    try
                    {
                        var urls = links.Attributes["href"].Value;
                        if (urls.StartsWith("http") || urls.StartsWith("https"))
                        {
                            int indx = urls.LastIndexOf("http://");
                            if (indx < 0)
                            {
                                indx = urls.LastIndexOf("https://");
                            }
                            urls = urls.Remove(0, indx);
                            //string links1 = HttpUtility.UrlDecode(urls);
                            alDup.Add(HttpUtility.HtmlDecode(urls.Replace("|", "%7C").Replace("^", "%5E")));
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
                throw new Exception("No pattern match,   " + ex.Message);
            }

            return googleList;
        }   //MobilePattern

        private ArrayList DesktopPattern(string htmlsource)
        {
            ArrayList googleList = new ArrayList();
            try
            {
                var doc = new HtmlDocument();
                htmlsource = htmlsource.Replace(@"\", "");
                doc.LoadHtml(htmlsource);
                ArrayList alDup = new ArrayList();

                HtmlNodeCollection node = doc.DocumentNode.SelectNodes("//div[@class='srg']//div[@class='r']/a[1]|//div[@class='bkWMgd']/div[@class='g']//div[@class='r']/a[1]|//div[1]/div/h3/div/g-link/a|//div[1]/div/h3/g-link/a");

                foreach (HtmlNode links in node)
                {
                    try
                    {
                        string urls = links.Attributes["href"].Value;
                        if (urls.StartsWith("http") || urls.StartsWith("https"))
                        {
                            int indx = urls.LastIndexOf("http://");
                            if (indx < 0)
                            {
                                indx = urls.LastIndexOf("https://");
                            }
                            urls = urls.Remove(0, indx);

                            //string links1 = HttpUtility.UrlDecode(urls);

                            alDup.Add(HttpUtility.HtmlDecode(urls.Replace("|", "%7C").Replace("^", "%5E")));
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
        }  //DesktopPattern


    }
}

