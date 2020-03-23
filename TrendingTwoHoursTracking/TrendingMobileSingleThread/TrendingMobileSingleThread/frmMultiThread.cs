using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;


namespace TrendingMobileSingleThread
{
    public partial class frmMultiThread : Form
    {
        string xmlPath1 = "C:\\inetpub\\wwwroot\\tws_oxybulk_remainingstatus_0.xml";
        //string xmlPath1 = "C:\\inetpub\\wwwroot\\tws_oxybulk_remaining.xml";

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        int count;
        int timerVal;
        bool iOS1 = false;       

        //int itmCount1 = 0;
        
        public frmMultiThread()
        {
            InitializeComponent();
            count = 1; // Common.GetOxylabsCount();
            //timerVal = Common.GetOxylabsTime();
            timerVal = 90 * 60000;
            timerExit();
        }
        void timerExit()
        {
            timer.Interval = timerVal;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }
        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            Environment.Exit(Environment.ExitCode);
        }

        private void frmMultiThread_Load(object sender, EventArgs e)
        {
            Text = "Trending Workspace Live IOS Oxylabs tws_oxybulk_remaining status 0";
            //Text = "Trending_Mobile_Remaining_Keywords";             

            Thread t1 = new Thread(new ThreadStart(StartProcess_iOS));
            t1.SetApartmentState(ApartmentState.STA);
            t1.Start();
        }

        private void StartProcess_iOS()
        {
            while (true)
            {
                string myDate = DateTime.Today.ToString("yyyy-MM-dd");

                string kwQry = "[GetBulkTrendingMobile_Status=0] '" + myDate + "'";                 
                //string kwQry = "GetBulkTrendingMobile_Remaining '" + myDate + "'";

                GetiOSKeywords(kwQry);
                if (lstKWs.Items.Count <= 0)
                    break;

                int cnt = 0;
                //this.Invoke((MethodInvoker)delegate ()
                //{
                //    label1.Text = cnt + " of " + itmCount1 + " Completed";
                //    label1.Refresh();
                //});
                foreach (string s in lstKWs.Items)
                {
                    string seid = s.Split(':')[0];
                    string kw = s.Split(':')[1];                   
                    try
                    {
                        string res = string.Empty;
                        iOS clsIOS = new iOS();
                        bool result = false;
                        var doc = new HtmlAgilityPack.HtmlDocument();
                        // HtmlAgilityPack.HtmlDocument doc = GetiOSDocument(seid, kw);
                        Task<ArrayList> alResult = GetHTML(kw, Convert.ToInt32(seid));                       
                        foreach (string[] src in alResult.Result)
                        {
                            string keyword = src[0];
                            JObject obj = JObject.Parse(src[1]);
                            string html = obj["results"][0]["content"].Value<string>();
                            string jobid = src[2];
                            string device = src[3];
                            result = true;
                            doc = new HtmlAgilityPack.HtmlDocument();
                            doc.LoadHtml(html);
                            
                            int count = 0;
                            if (device == "mobile_android")
                            {
                                iOS clsiOS = new iOS();
                                res = clsiOS.ProcessDocument(seid, keyword, doc, out count);
                            }

                            if (!string.IsNullOrEmpty(res))
                            {
                               // if (count > 0)   //condition is for remaining keywords
                               // {
                                    SendiOSToAPI(seid, keyword, res);
                                    SendToDB(seid, keyword, res);
                               // }
                            }
                            

                            }
                        }
                    catch (Exception ex)
                    {
                        this.Invoke((MethodInvoker)delegate ()
                        {
                            txtError.Text = ex.Message.ToString();
                            //string error = ex.Message.ToString() + seid + "=" + kw + Environment.NewLine;
                            //System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\error.txt", error);
                        });
                    }
                    finally { }
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        textBox1.Text = s;
                        textBox1.Refresh();
                        label1.Text = ++cnt + " of " + lstKWs.Items.Count + " Completed";
                        label1.Refresh();
                    });
                }
            }
            iOS1 = true;
            if (iOS1)
                Environment.Exit(Environment.ExitCode);
        }
        
        private void SendiOSToAPI(string seid, string kw, string res)
        {
            XmlDocument xd = new XmlDocument();
            res = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" + res;
            xd.LoadXml(res);
            xd.Save(xmlPath1);

            string submitURL = readAPI();

            string user = "pisoftware";
            string pwd = "r00t123456";
            try
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(submitURL);
                httpWReq.UseDefaultCredentials = true;
                httpWReq.PreAuthenticate = true;
                httpWReq.Credentials = CredentialCache.DefaultCredentials;

                Encoding encoding = new UTF8Encoding();
                string postData = GetTextFromXMLFile(xmlPath1);
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
                ////store into keywordfail table.
                SendToDBFailure(seid, kw);
                                
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
                if(!errorMsg.Contains("Search is not required and will be discarded"))
                    throw new Exception(errorMsg);

            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }

        }
        private void GetiOSKeywords(string qry)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                lstKWs.Items.Clear();
                //itmCount1 = 0;
                //lstKWs.Items.Add("102:lipgloss");
                //lstKWs.Items.Add("322: adidas in glorietta");
                //itmCount1++;
            });
            //return;

            try
            {
                using (SqlConnection con = new SqlConnection(Common.ReadConnection()))
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
                                    lstKWs.Items.Add(dr[0].ToString()+ ":" + dr[1].ToString());                                   
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
        async Task<ArrayList> GetOxylabsWebDataSources(SearchProperties sp)
        {
            Uri queryUri = new Uri("https://data.oxylabs.io/v1/queries/batch");
            string username = "gpidatametrics";
            string password = "sdV5X3fcX6";
            string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));

            string[] kwd = { sp.query };
            OxyParams op = new OxyParams()
            {
                source = "google_search",
                domain = sp.domain,
                query = kwd,
                limit = 100,
                pages = 1,
                //start_page = 1,

                locale = sp.locale,
                geo_location = sp.geo_location,
                //uule = uule,
                parse = 1,
                user_agent_type = sp.device,
                context = new List<Context> {
                    new Context("safe_search", 0)
                }
            };

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(queryUri);
            req.Headers.Clear();

            req.Method = "POST";
            req.ContentType = "application/json";
            req.Headers.Add(HttpRequestHeader.Authorization, "Basic " + authInfo);

            using (var streamWriter = new StreamWriter(req.GetRequestStream()))
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
                using (StreamReader reader = new StreamReader(res.GetResponseStream()))
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
                string[] s = { kw, href, status, "no", jobid, device };    // keyword, url, status, isdownloaded, jobid, device.
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
                    if (cbUrl[2] == "done" && cbUrl[3] == "no")
                    {
                        try
                        {
                            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
                            httpWebRequest.Headers["Authorization"] = "Basic " + authInfo;
                            HttpWebResponse res = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

                            Stream resVal = res.GetResponseStream();
                            StreamReader reader = new StreamReader(resVal, Encoding.UTF8);
                            //** Store all the contents
                            response = reader.ReadToEnd();
                            resVal.Close();
                            res.Close();

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
                    }
                    else if (cbUrl[2] == "pending" && cbUrl[3] == "no")
                    {
                        try
                        {
                            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri.ToString().Replace("/results", ""));
                            httpWebRequest.Headers["Authorization"] = "Basic " + authInfo;
                            HttpWebResponse res = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

                            string doneresp = "";
                            using (StreamReader reader = new StreamReader(res.GetResponseStream()))
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
        private string GetTextFromXMLFile(string file)
        {
            StreamReader reader = new StreamReader(file);
            string ret = reader.ReadToEnd();
            reader.Close();
            return ret;
        }

        public string readAPI()
        {
            string name = string.Empty;
            XmlDocument xml = new XmlDocument();
            try
            {
                string fileName = @"C:\Inetpub\wwwroot\TrendingLiveAPI.xml";
                // You'll need to put the correct path to your xml file here
                xml.Load(fileName);

                // Select a specific node
                XmlNode node = xml.SelectSingleNode("TrendingAPI/submitapi");
                // Get its value
                name = node.InnerText;
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    txtError.Text += ex.Message + "\r\n";
                });
            }
            finally { }
            return name;
        }

        private void SendToDBFailure(string seid, string kw)
        {
            string qry = "Insert into KeywordsFailure(date, seid, keyword, status) values('" + DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") + "', " + seid + ", N'" + kw.Replace("'", "''") + "', '-1')";

            try
            {
                using (SqlConnection con = new SqlConnection(Common.ReadConnection()))
                {
                    con.Open();
                    using (SqlCommand comm = new SqlCommand(qry, con))
                    {
                        comm.CommandTimeout = 0;
                        comm.CommandType = CommandType.Text;
                        comm.ExecuteNonQuery();
                    }
                }
            }
            finally { }
        }

        private void SendToDB(string seid, string keyword, string xml)
        {
            //string qry = "Insert into TrendingXmlResults(date, seid, keyword, xmldata) values('" + DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") + "', " + seid + ", N'" + keyword.Replace("'", "''") + "', N'" + xml.Replace("'", "''") + "') ";
            //qry += "Update [dbo].[Keywords] set status=1 where seid=" + seid + " and keyword=N'" + keyword.Replace("'", "''") + "'; ";

            try
            {
                using (SqlConnection con = new SqlConnection(Common.ReadConnection()))
                {
                    con.Open();

                    using (SqlCommand comm = con.CreateCommand())
                    {
                        comm.CommandTimeout = 0;
                        comm.CommandType = CommandType.StoredProcedure;
                        comm.CommandText = "Insert_TrendingXMLResults";
                        comm.Parameters.Add("Date", SqlDbType.DateTime).Value = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
                        comm.Parameters.Add("Seid", SqlDbType.Int).Value = seid;
                        comm.Parameters.Add("Keyword", SqlDbType.NVarChar).Value = keyword; //.Replace("'", "''");
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
                //this.Invoke((MethodInvoker)delegate ()
                //{
                //    txtError.Text += ex.Message + "\r\n";
                //});
            }
        }

       
    }
}
