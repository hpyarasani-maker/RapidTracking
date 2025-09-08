using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;

namespace BingSingleThread
{
    public partial class frmSingleThread : Form
    {
        string xmlPath = "C:\\inetpub\\wwwroot\\bingdesktop_5.xml";

        string myDate = DateTime.Today.ToString("yyyy-MM-dd");

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        
        //int count; 

        public frmSingleThread()
        {
            InitializeComponent();
            //count = 0;   // Common.GetOxylabsCount();          
            //timerExit();

            //MissingKeywordsJob.ExecuteMissingKeywordsJob(0).Wait();
        }
        void TimerExit()
        {
            timer.Interval = 25 * 60000;
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }
        void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            Environment.Exit(Environment.ExitCode);
        }

        private void frmSingleThread_Load(object sender, EventArgs e)
        {
            
            this.Text = "Bing_SingleThread_Desktop_5";
            //this.Text = "Bing_SingleThread_Mobile";
            

            //Thread t = new Thread(new ThreadStart(StartProcess));//16-02-2025
            //t.SetApartmentState(ApartmentState.STA);
            //t.Start();//16-02-2025

            //Task task = Task.Run(() => StartProcess());//16-02-2025
            //task.Wait();//16-02-2025
            Task task = Task.Run(async () =>
            {
                try
                {
                    await StartProcess();
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        txtError.Text = $"Exception: {ex.Message}";
                    });
                    
                }
            });
        }

        private async Task StartProcess()//16-02-2025
        {
            while (true)
            {
                string myDate = DateTime.Today.ToString("yyyy-MM-dd");
                //string myDate = "2019-11-20";


                //string kwQry = "[GetKeywords_bing_Desktop] '" + myDate + "'";  GetKeywords_bing_5
                //string kwQry = "[GetKeywords_bing_Mobile] '" + myDate + "'";
                string kwQry = "[GetKeywords_bing_5] '" + myDate + "'";



                await GetKeywords(kwQry);

                if (lstKWs.Items.Count <= 0)
                    break;

                int cnt = 0;
                foreach (string s in lstKWs.Items)
                {
                    string seid = s.Split(':')[0];
                    string kw = s.Split(':')[1];
                    bool result = false;
                    try
                    {
                        var doc = new HtmlAgilityPack.HtmlDocument();
                        Task<ArrayList> alresult = GetHTML(kw, Convert.ToInt32(seid));
                        StringBuilder sb = new StringBuilder();
                        foreach (string[] src in alresult.Result)
                        {
                            string keyword = src[0];
                            JObject obj = JObject.Parse(src[1]);
                            //string html = obj["results"][0]["content"].Value<string>();
                            try
                            {
                                var cont = obj["results"];
                                foreach (JObject jo in cont)
                                {
                                    sb.Append(jo["content"].Value<string>());
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            string jobid = src[2];
                            string device = src[3];
                            //File.WriteAllText(@"C:\inetpub\wwwroot\html\" + jobid + "_" + keyword + ".html", sb.ToString(), Encoding.UTF8);
                            result = true;
                            doc = new HtmlAgilityPack.HtmlDocument();
                            //doc.LoadHtml(html);
                            string res = string.Empty;                            
                            ArrayList arRes = new ArrayList();

                            try
                            {
                                if (device == "desktop")
                                {
                                    arRes = DesktopPattern(sb.ToString());
                                }
                                else
                                {
                                    arRes = MobilePattern(sb.ToString());
                                }
                                await ProcessResults(arRes, kw, int.Parse(seid), jobid);
                            }
                            catch (Exception ex)
                            {
                                try
                                {
                                    bool isOldPage = false;
                                    if (ex.Message == "Old page found.")
                                        isOldPage = true;
                                    await SendToDBFailure(kw, seid, jobid, isOldPage, "RapidTrackingSingleThread Request Status is faulted");
                                }
                                finally { }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        this.Invoke((MethodInvoker)delegate ()
                        {
                            txtError.Text = ex.Message.ToString();
                            string errorDesk = ex.Message.ToString() + seid + "=" + kw + Environment.NewLine;
                            File.WriteAllText(@"C:\inetpub\wwwroot\errorDesk.txt", errorDesk);
                        });
                    }
                    finally { }
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        if (result)
                        {
                            textBox1.Text = s;
                            //textBox1.Refresh();
                            label1.Text = ++cnt + " of " + lstKWs.Items.Count + " Completed";
                            label1.Refresh();
                        }
                        else
                            textBox1.Text = s + "  -- No result.";
                        textBox1.Refresh();
                    });

                }

            }

            Environment.Exit(Environment.ExitCode);
        }

        public async Task ProcessResults(ArrayList alRes, string kw, int seid, string jobid)
        {
            string qry = "";

            if (alRes.Count < 1)
            {
              

                /*XmlTextWriter writer = new XmlTextWriter(xmlPath, Encoding.UTF8);

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

                writer.WriteEndElement();

                writer.WriteEndDocument();

                writer.Close();

               await SendXmlToAPI(xmlPath);
               await InsertDashBoardData(kw, seid.ToString(), 0, string.Empty, jobid);*/

            }
            else
            {
                Console.WriteLine("Processing the keyword: " + kw);

                string tname = Thread.CurrentThread.Name;

                string result = string.Empty;

                //lblcount.Invoke((MethodInvoker)(delegate ()
                //{
                //    lblcount.Text = "No. of Urls : " + alRes.Count;
                //}));

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
                            //string dURL = CleanInvalidXmlChars(alRes[i].ToString());
                            writer.WriteString(dURL);
                            writer.WriteEndElement();

                            qry += "insert into dashboard_bing(date, name, seid, position, url) values(Convert(varchar(10), '" + myDate + "',103), N'" + kw.Replace("'", "''") + "', " + seid + ", " + k + ", N'" + dURL.ToString().Replace("'", "''") + "')";

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
                        StreamWriter sw = new StreamWriter(xmlPath, false);
                        sw.Write(result);
                        sw.Close();
                    }

                    if (!string.IsNullOrEmpty(result))
                    {
                        lblCount.Invoke((MethodInvoker)(delegate ()
                        {
                            lblCount.Text = "No. of Urls : " + alRes.Count;
                        }));
                        try
                        {
                            if (alRes.Count > 0)
                            {
                               await SendXmlToAPI(xmlPath);
                               await Insert100DashBoardData(qry);
                               await InsertDashBoardData(kw, seid.ToString(), alRes.Count, alRes[0].ToString(), jobid);
                            }
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
       private async Task Insert100DashBoardData(string qry)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strConn()))
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

        private async Task InsertDashBoardData(string kw, string seid, int cnt, string alRes, string jobid)
        {

            string qry = "insert into dashboard_data(date, name, seid, jobid, count, url) " +
                    "values(Convert(varchar(10),'" + myDate + "',103),N'" + kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "', " + cnt + ", N'" + alRes.Replace("'", "''") + "')";

            try
            {
                using (SqlConnection con = new SqlConnection(await Common.ReadConnection()))
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


       
        private async Task SendXmlToAPI(string path)
        {
            string submitURL = Common.readAPI();

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
                string postData = await GetTextFromXMLFile(xmlPath);
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

        private async Task GetKeywords(string qry)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                lstKWs.Items.Clear();
                //lstKWs.Items.Add("5:wifi in warehouse");
            });
            //return;

            try
            {
                using (SqlConnection con = new SqlConnection(await Common.ReadConnection()))  // 12-05-2020
                {
                    con.Open();
                    using (SqlCommand comm = new SqlCommand(qry, con))
                    {
                        comm.CommandTimeout = 0;
                        using (SqlDataReader dr = comm.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (dr.Read())
                            {
                                this.Invoke((MethodInvoker)delegate ()
                                {
                                    lstKWs.Items.Add(dr[0].ToString() + ":" + dr[1].ToString());
                                });
                            }
                        }
                    }
                }
                //this.Invoke((MethodInvoker)delegate ()
                //{
                //    lstKWs.Refresh();
                //});
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    txtError.Text += ex.Message + "\r\n";
                });
            }
            finally { }
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

                // You'll need to put the correct path to your xml file here
                xml.Load(fileName);

                // Select a specific node
                XmlNode node = xml.SelectSingleNode("ConnectionString/apiSubmit");

                // Get its value
                string name = node.InnerText;

                return await Task.FromResult<string>(name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task SendToDBFailure(string seid, string kw, string jobid, bool isOldPage, string errMsg)
        {
            string myDate = DateTime.Today.ToString("yyyy-MM-dd");
            //string myDate = "2019-10-10";

            string qry = "insert into dashboard_dataerrors (date, name, seid, jobid,message) values(Convert(varchar(10),'" + myDate + "',103), N'" +
                    kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "', N'" + errMsg + "')"; //03-01-2022

            string qryOld = "insert into dashboard_oldgooglepage (date, keyword, seid, jobid) values('" + DateTime.Now + "', N'" +
                  kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "')";

            using (SqlConnection con = new SqlConnection(await Common.ReadConnection()))
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

                    //throw new Exception(errorMessage);
                }
                catch (Exception)
                {
                    //throw ex;
                }
            }

        }
       
        private async Task SendToDB(string seid, string keyword, string xml, string jobid, int urlcount)
        {
            try
            {
                string myDate = DateTime.Today.ToString("yyyy-MM-dd");
                //string myDate = "2019-11-20";

                using (SqlConnection con = new SqlConnection(await Common.ReadConnection()))  // 12-05-2020
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
                        comm.Parameters.Add("Received", SqlDbType.VarChar).Value = "Normal Single"; //14-04-2025
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
                throw new Exception(ex.Message.ToString());
            }
        }
        private async Task SendToSendingTable(string kw, string seid, string jobid)
        {
            string date = DateTime.Today.ToString("yyyy-MM-dd");
            StringBuilder sb = new StringBuilder();
            string qry = "insert into dashboard_data_sending (date, name, seid, jobid) values('" + date + "', N'" + kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "'); ";
            sb.Append(qry);
            try
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                {
                    using (SqlConnection con = new SqlConnection(await Common.ReadConnection()))
                    {
                        con.Open();
                        using (SqlCommand comm = new SqlCommand(sb.ToString(), con))
                        {
                            comm.CommandTimeout = 0;
                            await comm.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
        public async Task<ArrayList> GetHTML(string keyword, int seid)
        {
            ArrayList alResult = new ArrayList();
            try
            {
                SearchProperties sp = SearchParams.searches.Where(s => s.seid == seid).SingleOrDefault();
                sp.query = keyword;
                if (sp != null)
                    alResult = GetOxylabsWebDataSources(sp).Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return await Task.FromResult(alResult);
        }
        private ArrayList MobilePattern(string htmlsource)
        {
            ArrayList googleList = new ArrayList();
            try
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlsource);
                ArrayList alDup = new ArrayList();

                HtmlNodeCollection node = doc.DocumentNode.SelectNodes(".//ol[@id='b_results']/li[@class='b_algo']/div[@class='b_algoheader']|//ol[@id='b_results']/li[@class='b_algo']|//div[@class='b_algoheader']|//div[@class='b_algoheader b_removeline12px']|//div[@class='b_algoheader b_removeline8px']");

                if (node != null)
                {
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
                                if (!urls.Contains("www.bing.com/ck/a"))
                                alDup.Add(HttpUtility.HtmlDecode(urls.Trim()));
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
            }
            catch { }
            return googleList;
        }

        private ArrayList DesktopPattern(string htmlsource)
        {
            ArrayList googleList = new ArrayList();
            try
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlsource);
                ArrayList alDup = new ArrayList();

                //HtmlNodeCollection node = doc.DocumentNode.SelectNodes("//ol[@id='b_results']/li[@class='b_algo']|//ol[@id='b_results']/li[@class='b_algo']/h2|//div[@class='b_algoheader']|//ol[@id='b_results']//li[@class='b_algo']/div[@class='b_title']/h2|//li[@class='b_algo']/h2"); //seid = 5
                HtmlNodeCollection node = doc.DocumentNode.SelectNodes(".//div[@class='b_attribution']/cite|.//div[@class='b_adurl']/cite");
                //HtmlNodeCollection node = doc.DocumentNode.SelectNodes(".//ol[@id='b_results']/li[@class='b_algo']/div[@class='b_algoheader']|//ol[@id='b_results']/li[@class='b_algo']|//div[@class='b_algoheader']|//div[@class='b_algoheader b_removeline12px']|//div[@class='b_algoheader b_removeline8px']");

                if (node != null)
                {
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
                                if (!urls.Contains("www.bing.com/ck/a"))
                                    alDup.Add(HttpUtility.HtmlDecode(urls.Trim()));
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
            }
            catch { }
            return googleList;
        }
       

        async Task<ArrayList> GetOxylabsWebDataSources(SearchProperties sp)
        {
            Uri queryUri = new Uri("https://data.oxylabs.io/v1/queries/batch");
            //string username = "gpidatametrics";
            //string password = "sdV5X3fcX6";
            string username = "piapp";
            string password = "b5FCvgkjxx";
            string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));

            //string callbackURL = "https://seresults.azurewebsites.net/api/callbackotherdesktop/"; //Bing desktop
            //string callbackURL = "https://seresults.azurewebsites.net/api/callbackothermobile/"; //Bing mobile



            string[] keyword = { sp.query };
            OxyParams op = new OxyParams()
            {
                source = "bing_search",
                domain = sp.domain,
                query = keyword,
                pages = 10,
                start_page = 1,
                locale = sp.locale,
                //callback_url = callbackURL,
                geo_location = sp.geo_location,
                parse = false,
                user_agent_type = sp.device,
            };


            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(queryUri);
            req.Headers.Clear();

            req.Method = "POST";
            req.ContentType = "application/json";
            req.Headers.Add(HttpRequestHeader.Authorization, "Basic " + authInfo);

            using (var streamWriter = new StreamWriter(await req.GetRequestStreamAsync()))
            {
                var json = JsonConvert.SerializeObject(op, new JsonSerializerSettings
                {
                    Formatting = Newtonsoft.Json.Formatting.Indented,
                });

                streamWriter.Write(json);
            }

            string response;
            try
            {
                HttpWebResponse res = (HttpWebResponse)await req.GetResponseAsync();
                using (StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                {
                    response = reader.ReadToEnd();
                }
                res.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            JObject jo = JObject.Parse(response);
            var links = from p in jo["queries"] select p;
            ArrayList lst = new ArrayList();
            foreach (JToken link in links)
            {
                string kw = link["query"].Value<string>();
                string href = link["_links"][1]["href"].Value<string>();
                string status = link["status"].Value<string>();
                string jobid = link["id"].Value<string>();
                string device = link["user_agent_type"].Value<string>();
                string[] s = { kw, href, status, "no", jobid, device, sp.seid.ToString() }; //13/05/2025   // keyword, url, status, isdownloaded, jobid, device.
                lst.Add(s);
            }

            if (lst.Count <= 0) return lst;
            ArrayList alResult = new ArrayList();
            do
            {
                int cnt = 0;
                foreach (string[] cbUrl in lst)
                {
                    string[] reslt = { "", "", "", "" };
                    response = "";

                    Uri uri = new Uri(cbUrl[1]);
                    //Uri uri = new Uri("http://data.oxylabs.io/v1/queries/7056828476887683073/results");
                    if (cbUrl[2] == "done" && cbUrl[3] == "no")
                    {
                        try
                        {
                            var startTime = System.Diagnostics.Stopwatch.StartNew();//08-11-2023
                            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
                            httpWebRequest.Headers["Authorization"] = "Basic " + authInfo;
                            HttpWebResponse res = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

                            Stream resVal = res.GetResponseStream();
                            StreamReader reader = new StreamReader(resVal, Encoding.UTF8);
                            //** Store all the contents
                            response = reader.ReadToEnd();
                            resVal.Close();
                            res.Close();
                            startTime.Stop();
                            var totalTime = Convert.ToDouble(startTime.ElapsedMilliseconds) / 1000;//08-11-2023
                            lblDownloadedTime.Invoke((MethodInvoker)(delegate ()//08-11-2023
                            {
                                lblDownloadedTime.Text = totalTime.ToString() + " sec";
                            }));//08-11-2023
                            cbUrl[3] = "yes";
                            cnt++;

                            if (!string.IsNullOrEmpty(response))
                            {
                                reslt[0] = cbUrl[0];
                                reslt[1] = response;
                                reslt[2] = cbUrl[4];
                                reslt[3] = cbUrl[5];
                                alResult.Add(reslt);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Result Request: " + ex.Message);
                        }
                    }
                    else if (cbUrl[2] == "faulted" && cbUrl[3] == "no")
                    {
                        cbUrl[3] = "yes";
                        cnt++;
                        if (cbUrl[2] == "faulted")//13-05-2025
                        {
                            this.Invoke((MethodInvoker)delegate ()//13-05-2025
                            {
                                txtError.Text = txtError.Text + cbUrl[6] + ": " + cbUrl[0] + ": " + cbUrl[4] + Environment.NewLine + "Bing Status is faulted" +
                                    Environment.NewLine + Environment.NewLine;
                                txtError.Refresh();
                            });//13-05-2025
                            //await SendToDBFailure(cbUrl[6], cbUrl[0], cbUrl[4], false, "BingSingleThread Request Status is faulted");
                        }//13-05-2025
                    }
                    else if (cbUrl[2] == "pending" && cbUrl[3] == "no")
                    {
                        try
                        {
                            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri.ToString().Replace("/results", ""));
                            httpWebRequest.Headers["Authorization"] = "Basic " + authInfo;
                            HttpWebResponse res = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

                            string doneresp = "";
                            using (StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                            {
                                doneresp = reader.ReadToEnd();
                            }
                            res.Close();

                            JObject job = JObject.Parse(doneresp);
                            string status = job["status"].Value<string>();
                            cbUrl[2] = status;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Status Request: " + ex.Message);
                            txtError.Text = ex.Message.ToString();
                        }
                    }
                    else
                        cnt++;
                    Task.Delay(200).Wait();
                }

                if (lst.Count == cnt) break;

            } while (true);

           return await Task.FromResult<ArrayList>(alResult);

        }

    }
}
