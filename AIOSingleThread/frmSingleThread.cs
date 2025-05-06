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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace AIOSingleThread
{
    public partial class frmSingleThread : Form
    {
        string xmlPath = "C:\\inetpub\\wwwroot\\rapidtracking_singlethread_102_GT20_WC.xml";

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
            
            this.Text = "RapidTracking_SingleThread_102_GT20_WC";
            //this.Text = "RapidTracking_SingleThread_P_A_WOC_10-09-2019";


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


                //string kwQry = "[Tracking_DB_Keywords_Seid_102] '" + myDate + "'";
                //string kwQry = "[Tracking_DB_Keywords_Seid_103p] '" + myDate + "'";               
                //string kwQry = "[GetCommaKeywordsP] '" + myDate + "'";               
                //string kwQry = "[Tracking_DB_Keywords_Seid_102_P] '" + myDate + "'"; //tracking previous date single keywords
                //string kwQry = "Tracking_DB_Keywords_SEID_102_TGBN '" + myDate + "'";
                string kwQry = "[Tracking_DB_Keywords_SEID_102_AIO] '" + myDate + "'";

                await GetKeywords(kwQry);

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
                    bool result = false;
                    try
                    {
                        var doc = new HtmlAgilityPack.HtmlDocument();
                        Task<ArrayList> alresult = GetHTML(kw, Convert.ToInt32(seid));

                        foreach (string[] src in alresult.Result)
                        {
                            string keyword = src[0];
                            JObject obj = JObject.Parse(src[1]);
                            string html = obj["results"][0]["content"].Value<string>();
                            string jobid = src[2];
                            string device = src[3];
                            await SendToSendingTable(kw, seid, jobid);
                            File.WriteAllText(@"C:\inetpub\wwwroot\html\" + jobid + "_" + keyword + ".html", html, Encoding.UTF8);
                            //File.WriteAllText(@"C:\inetpub\wwwroot\"+jobid+"_withOut filter_"+".html", html, Encoding.UTF8);
                            result = true;
                            doc = new HtmlAgilityPack.HtmlDocument();
                            doc.LoadHtml(html);
                            string res = string.Empty;
                            int count = 0;
                            try
                            {
                                if (device == "desktop_chrome")
                                {
                                    Desktop clsDesktop = new Desktop();
                                    res = clsDesktop.ProcessDocument(seid, keyword, doc, out count);
                                }
                                else
                                {
                                    iOS clsiOS = new iOS();
                                    res = clsiOS.ProcessDocument(seid, keyword, doc, out count);
                                }

                                if (!string.IsNullOrEmpty(res))
                                {
                                    lblCount.Invoke((MethodInvoker)(delegate ()
                                    {
                                        lblCount.Text = "No. of Urls : " + count;
                                    }));
                                    if (count > 20)
                                    {
                                        await SendToAPI(seid, keyword, res, jobid);
                                        await SendToDB(seid, keyword, res, jobid, count);
                                    }
                                    bool aio = res.Contains("<block type=\"aiOverview\">");//16-02-2025
                                    if (!aio && device == "mobile_android") // inserting false value //16-02-2025
                                        await InsertAIO_Keyword_False(keyword, seid, aio);
                                }
                                //else
                                //{
                                //    SendToAPI(seid, keyword, res, jobid);
                                //    SendToDB(seid, keyword, res, jobid, count);
                                //}

                            }
                            catch (Exception ex)
                            {
                                try
                                {
                                    bool isOldPage = false;
                                    if (ex.Message == "AIO Old page found")//14-04-2025
                                        isOldPage = true;
                                    await SendToDBFailure(kw, seid, jobid, isOldPage);
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
                            textBox1.Text = s + "  -- AIO No result.";
                        textBox1.Refresh();
                    });

                }

            }

            Environment.Exit(Environment.ExitCode);
        }


        private async Task SendToAPI(string seid, string kw, string res, string jobid)
        {
            //string r = "[\x00-\x08\x0B\x0C\x0E-\x1F\x26]";
            //res = Regex.Replace(res, r, "", RegexOptions.Compiled);
            //if (res == string.Empty)
            //{
            //    XmlDocument xd = new XmlDocument();
            //    res = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            //    res += "<searchResult searchEngine =\"" + seid + "\" keyword=\"" + kw + "\" date =\"" + DateTime.Today.ToString("yyyy-MM-dd") + "\">";
            //    res += "<section col = \"main\" /> <section col=\"right\" /> </searchResult> ";
            //    xd.LoadXml(res);
            //    xd.Save(xmlPath);
            //}
            //else
            //{
            XmlDocument xd = new XmlDocument();
            res = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + res;
            xd.LoadXml(res);
            xd.Save(xmlPath);

            //}
            //SendToURL


            string submitURL = await ReadAPI();
            //return; //03-04-2021
            string user = "pisoftware";
            string pwd = "r00t123456";
            try
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(submitURL);
                httpWReq.UseDefaultCredentials = true;
                httpWReq.PreAuthenticate = true;
                httpWReq.Credentials = CredentialCache.DefaultCredentials;

                Encoding encoding = new UTF8Encoding();
                string postData = await GetTextFromXMLFile(xmlPath);
                byte[] data = encoding.GetBytes(postData);

                httpWReq.ProtocolVersion = HttpVersion.Version11;
                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/x-www-form-urlencoded";


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

                ////store into keywordfail table.
                await SendToDBFailure(seid, kw, jobid, false);


                string errorMsg = string.Empty;
                using (WebResponse response = ex.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    errorMsg = string.Format("API Error: StatusCode {0}", httpResponse.StatusCode);

                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        errorMsg += "\r\n" + reader.ReadToEnd();
                        txtError.Text = errorMsg;
                    }
                }

                throw new Exception(errorMsg);

            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
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
        private async Task GetKeywords(string qry)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                lstKWs.Items.Clear();
                //lstKWs.Items.Add("58:praivat medicashe insh");
                //lstKWs.Items.Add("106:st.vincent discography");
                //lstKWs.Items.Add("106:romeo and juliet tickets");
                //lstKWs.Items.Add("160:malmö ff");
                //lstKWs.Items.Add("102:terry crews");
                //lstKWs.Items.Add("102:the uninhabitable earth summary");
                lstKWs.Items.Add("58:london luton flights");
            });
            return;

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
        private async Task SendToDBFailure(string seid, string kw, string jobid, bool isOldPage)
        {
            string myDate = DateTime.Today.ToString("yyyy-MM-dd");
            //string myDate = "2019-10-10";

            string qry = "insert into dashboard_dataerrors (date, name, seid, jobid) values(Convert(varchar(10),'" + myDate + "',103), N'" +
                  kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "' )";

            string qryOld = "insert into dashboard_oldgooglepage (date, keyword, seid, jobid) values('" + DateTime.Now + "', N'" +
                 kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "' )";

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
        private async Task InsertAIO_Keyword_False(string kw, string seid, bool aio)//16-02-2025
        {
            try
            {
                using (SqlConnection con = new SqlConnection(await Common.ReadConnection()))
                {
                    con.Open();
                    using (SqlCommand comm = con.CreateCommand())
                    {
                        comm.CommandTimeout = 0;
                        comm.CommandType = CommandType.StoredProcedure;
                        comm.CommandText = "Insert_AIO_Keywords_False";
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
                        comm.Parameters.Add("Received", SqlDbType.VarChar).Value = "AIO Single"; //14-04-2025
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
            Uri queryUri = new Uri("http://data.oxylabs.io/v1/queries/batch");
            //string username = "gpidatametrics";
            //string password = "sdV5X3fcX6";

            string username = string.Empty;//04-02-2025
            string password = string.Empty;
            if (sp.device == "mobile_android")
            {
                username = "piapp-aio";
                password = "4gvfnA+aBYpBNs37";
            }
            else if (sp.device == "desktop_chrome")
            {
                username = "piapp-aio";
                password = "4gvfnA+aBYpBNs37";
            }//04-03-2025
            string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));
            string[] keyword = { sp.query };

            OxyParams op = new OxyParams()
            {
                source = "google_search",
                domain = sp.domain,
                query = keyword,
                limit = 100,
                pages = 1,
                locale = sp.locale,
                geo_location = sp.geo_location,
                parse = false, //23-09-2021 changed datatype into "int to bool"
                user_agent_type = sp.device,
                render = "html", //comment for desktop and uncomment for mobile
                browser_instructions = new List<dynamic>
                {
                    new {
                        type = "click",
                        selector = new Selector {
                            type = "xpath",
                            value = "//div[contains(@class,'zNsLfb Jzkafd')]/div/div"
                        }
                    },
                    new
                    {
                        type = "wait",
                        wait_time_s = 10
                    }
                }//comment for desktop and uncomment for mobile
                /*context = new List<Context> { //comment for mobile and uncomment for desktop
                    new Context("tbm", sp.tbm),
                    new Context("safe_search", 0)
                    //,new Context("aomd",1)
                }*/
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
