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

namespace RapidTrackingMultiThreadRequests
{
    public partial class Form1 : Form
    {

        string xmlPath1 = @"C:\inetpub\wwwroot\keywords_1.xml";
        string xmlPath2 = @"C:\inetpub\wwwroot\keywords_2.xml";
        string xmlPath3 = @"C:\inetpub\wwwroot\keywords_3.xml";//changes

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        bool process1 = false;
        bool process2 = false;
        bool process3 = false;

        string myDate;
        public Form1()
        {
            InitializeComponent();
            //timerExit();
        }

        void timerExit()
        {
            timer.Interval = 110 * 60000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            Environment.Exit(Environment.ExitCode);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "RapidTracking_Errorkeywords_(1-2-3)";//changes

            dtPicker1.Value = DateTime.Today;
            myDate = dtPicker1.Value.ToString("yyyy-MM-dd");

            Task t1 = Task.Run(async () => //16-02-2025
            {
                try
                {
                    await StartProcess_1();
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        txtError.Text = $"Exception: {ex.Message}";
                    });

                }
            });
            
            Task t2 = Task.Run(async () =>
            {
                try
                {
                    await StartProcess_2();
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        txtError.Text = $"Exception: {ex.Message}";
                    });

                }
            });

            Task t3 = Task.Run(async () =>
            {
                try
                {
                    await StartProcess_3();
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        txtError.Text = $"Exception: {ex.Message}";
                    });

                }
            });//16-02-2025
        }

        private async Task StartProcess_1()
        {
            while (true)
            {
                //string myDate = DateTime.Today.ToString("yyyy-MM-dd");
                //string kwQry = "GetErrorKeywords_1 '" + myDate + "'";//changes
                //string kwQry = "GetAllKeywords_1 '" + myDate + "'";     
                //string kwQry = "GetMissingKeywords_1 '" + myDate + "'";     
                string kwQry = "GetAllKeywords_Remaining '" + myDate + "',1";

                await GetKeywords1(kwQry);

                if (lstKWs.Items.Count <= 0)
                    break;

                int cnt = 0;
                foreach (string s in lstKWs.Items)
                {
                    //26-10-2020 changed character as ':'
                    string seid = s.Split(':')[0];
                    string kw = s.Split(':')[1];
                    string jobid = string.Empty;
                    //end of 26-10-2020 changed character as ':'
                    try
                    {
                        var doc = new HtmlAgilityPack.HtmlDocument();
                        Task<ArrayList> alresult = GetHTML(kw, Convert.ToInt32(seid));
                        if (alresult.Status.ToString() == stats.Faulted.ToString() || alresult.Status.ToString() == stats.Pending.ToString() || alresult.Status.ToString() == stats.Empty.ToString() || alresult.Status.ToString() == stats.statuscode.ToString()) //07-02-2022//31-01-2022//04-01-2022
                            throw alresult.Exception.InnerException;//04-01-2022
                        foreach (string[] src in alresult.Result)
                        {
                            string keyword = src[0];
                            JObject obj = JObject.Parse(src[1]);
                            string html = obj["results"][0]["content"].Value<string>();                            
                            string device = src[3];
                            jobid = src[2];
                            await SendToSendingTable(kw, seid, jobid);
                            doc = new HtmlAgilityPack.HtmlDocument();
                            doc.LoadHtml(html);
                            //File.WriteAllText(@"C:\inetpub\wwwroot\html\" + jobid + "_" + keyword + ".html", html, Encoding.UTF8);
                            string res = string.Empty;
                            int count = 0;

                            if (device == "desktop_chrome")
                            {
                                Desktop clsDesktop = new Desktop();
                                (res, count) = await clsDesktop.ProcessDocument(seid, keyword, doc);//08-05-2025
                            }
                            else
                            {
                                iOS clsiOS = new iOS();
                                (res, count) = await clsiOS.ProcessDocument(seid, keyword, doc);//08-05-2025
                            }
                            this.Invoke((MethodInvoker)delegate () {
                            lblcount1.Text = "Count: " + count;
                            });
                            if (!string.IsNullOrEmpty(res))
                            {
                                if (count > 20)
                                {
                                    await SendToAPI1(seid, keyword, res, jobid);
                                    await SendToDB(seid, keyword, res, jobid, count);
                                }
                                if (count <= 20)
                                {
                                    await SendToDB(seid, keyword, res, jobid, count);
                                }
                                bool aio = res.Contains("<block type=\"aiOverview\">");//16-02-2025
                                if (aio && device == "mobile_android") // inserting true value //16-02-2025
                                    await InsertAIO_Keyword(keyword, seid, aio);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            this.Invoke((MethodInvoker)delegate ()
                            {
                                txtError.Text = txtError.Text + seid + ": " + kw + ": " + jobid + Environment.NewLine + ex.Message.ToString() +
                                    Environment.NewLine + Environment.NewLine;
                                txtError.Refresh();
                            });
                            await SendToDBFailure(seid, kw, jobid, false, ex.Message);
                        }
                        finally { }
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

            process1 = true;
            if (process1 && process2 && process3)
                Environment.Exit(Environment.ExitCode);
        }

        private async Task StartProcess_2()
        {
            while (true)
            {
                //string myDate = DateTime.Today.ToString("yyyy-MM-dd");
                //string kwQry = "GetErrorKeywords_2 '" + myDate + "'";//changes
                //string kwQry = "GetAllKeywords_2 '" + myDate + "'";     
                string kwQry = "GetAllKeywords_Remaining '" + myDate + "',2";

                await GetKeywords2(kwQry);

                if (lstKWs2.Items.Count <= 0)
                    break;

                int cnt = 0;
                foreach (string s in lstKWs2.Items)
                {
                    //26-10-2020 changed character as ':'
                    string seid = s.Split(':')[0];
                    string kw = s.Split(':')[1];
                    string jobid = string.Empty;
                    //end of 26-10-2020 changed character as ':'
                    try
                    {
                        var doc = new HtmlAgilityPack.HtmlDocument();
                        Task<ArrayList> alresult = GetHTML(kw, Convert.ToInt32(seid));
                        if (alresult.Status.ToString() == stats.Faulted.ToString() || alresult.Status.ToString() == stats.Pending.ToString() || alresult.Status.ToString() == stats.Empty.ToString() || alresult.Status.ToString() == stats.statuscode.ToString()) //07-02-2022//31-01-2022//04-01-2022
                            throw alresult.Exception.InnerException;//04-01-2022
                        foreach (string[] src in alresult.Result)
                        {
                            string keyword = src[0];
                            JObject obj = JObject.Parse(src[1]);
                            string html = obj["results"][0]["content"].Value<string>();
                            string device = src[3];
                            jobid = src[2];
                            await SendToSendingTable(kw, seid, jobid);
                            doc = new HtmlAgilityPack.HtmlDocument();
                            doc.LoadHtml(html);
                            //File.WriteAllText(@"C:\inetpub\wwwroot\html\" + jobid + "_" + keyword + ".html", html, Encoding.UTF8);
                            string res = string.Empty;
                            int count = 0;

                            if (device == "desktop_chrome")
                            {
                                Desktop clsDesktop = new Desktop();
                                (res, count) = await clsDesktop.ProcessDocument(seid, keyword, doc);//08-05-2025
                            }
                            else
                            {
                                iOS clsiOS = new iOS();
                                (res, count) = await clsiOS.ProcessDocument(seid, keyword, doc);//08-05-2025
                            }
                            this.Invoke((MethodInvoker)delegate () {
                                lblcount2.Text = "Count: " + count;
                            });
                            if (!string.IsNullOrEmpty(res))
                            {
                                if (count > 20)
                                {
                                    await SendToAPI2(seid, keyword, res, jobid);
                                    await SendToDB(seid, keyword, res, jobid, count);
                                }
                                if (count <= 20)
                                {
                                    await SendToDB(seid, keyword, res, jobid, count);
                                }
                                bool aio = res.Contains("<block type=\"aiOverview\">");//16-02-2025
                                if (aio && device == "mobile_android") // inserting true value //16-02-2025
                                    await InsertAIO_Keyword(keyword, seid, aio);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            this.Invoke((MethodInvoker)delegate ()
                            {
                                txtError.Text = txtError.Text + seid + ": " + kw + ": " + jobid + Environment.NewLine + ex.Message.ToString() +
                                    Environment.NewLine + Environment.NewLine;
                                txtError.Refresh();
                            });
                            await SendToDBFailure(seid, kw, jobid, false, ex.Message);
                        }
                        finally { }
                    }
                    finally { }
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        textBox2.Text = s;
                        textBox2.Refresh();
                        label2.Text = ++cnt + " of " + lstKWs2.Items.Count + " Completed";
                        label2.Refresh();
                    });
                }
            }

            process2 = true;
            if (process1 && process2 && process3)
                Environment.Exit(Environment.ExitCode);
        }
        public enum stats
        {
            Faulted,
            Pending,
            Empty,
            statuscode

        }

        private async Task StartProcess_3()
        {
            while (true)
            {
                //string myDate = DateTime.Today.ToString("yyyy-MM-dd");
                //string kwQry = "GetErrorKeywords_3 '" + myDate + "'";//changes
                //string kwQry = "GetAllKeywords_3 '" + myDate + "'";     
                string kwQry = "GetAllKeywords_Remaining '" + myDate + "',3";

                await GetKeywords3(kwQry);

                if (lstKWs3.Items.Count <= 0)
                    break;

                int cnt = 0;
                foreach (string s in lstKWs3.Items)
                {
                    //26-10-2020 changed character as ':'
                    string seid = s.Split(':')[0];
                    string kw = s.Split(':')[1];
                    string jobid = string.Empty;
                    //end of 26-10-2020 changed character as ':'
                    try
                    {
                        var doc = new HtmlAgilityPack.HtmlDocument();
                        Task<ArrayList> alresult = GetHTML(kw, Convert.ToInt32(seid));
                        if (alresult.Status.ToString() == stats.Faulted.ToString() || alresult.Status.ToString() == stats.Pending.ToString() || alresult.Status.ToString() == stats.Empty.ToString() || alresult.Status.ToString() == stats.statuscode.ToString()) //07-02-2022//31-01-2022//04-01-2022
                            throw alresult.Exception.InnerException;//04-01-2022
                        foreach (string[] src in alresult.Result)
                        {
                            string keyword = src[0];
                            JObject obj = JObject.Parse(src[1]);
                            string html = obj["results"][0]["content"].Value<string>();
                            string device = src[3];
                            jobid = src[2];
                            await SendToSendingTable(kw, seid, jobid);
                            doc = new HtmlAgilityPack.HtmlDocument();
                            doc.LoadHtml(html);
                            //File.WriteAllText(@"C:\inetpub\wwwroot\html\" + jobid + "_" + keyword + ".html", html, Encoding.UTF8);
                            string res = string.Empty;
                            int count = 0;

                            if (device == "desktop_chrome")
                            {
                                Desktop clsDesktop = new Desktop();
                                (res, count) = await clsDesktop.ProcessDocument(seid, keyword, doc);//08-05-2025
                            }
                            else
                            {
                                iOS clsiOS = new iOS();
                                (res, count) = await clsiOS.ProcessDocument(seid, keyword, doc);//08-05-2025
                            }
                            this.Invoke((MethodInvoker)delegate () {
                                lblcount3.Text = "Count: " + count;
                            });
                            if (!string.IsNullOrEmpty(res))
                            {
                                if (count > 20)
                                {
                                    await SendToAPI3(seid, keyword, res, jobid);
                                    await SendToDB(seid, keyword, res, jobid, count);
                                }
                                if (count <= 20)
                                {
                                    await SendToDB(seid, keyword, res, jobid, count);
                                }
                                bool aio = res.Contains("<block type=\"aiOverview\">");//16-02-2025
                                if (aio && device == "mobile_android") // inserting true value //16-02-2025
                                    await InsertAIO_Keyword(keyword, seid, aio);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            this.Invoke((MethodInvoker)delegate ()
                            {
                                txtError.Text = txtError.Text + seid + ": " + kw + ": " + jobid + Environment.NewLine + ex.Message.ToString() +
                                    Environment.NewLine + Environment.NewLine;
                                txtError.Refresh();
                            });
                            await SendToDBFailure(seid, kw, jobid, false, ex.Message);
                        }
                        finally { }
                    }
                    finally { }
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        textBox3.Text = s;
                        textBox3.Refresh();
                        label3.Text = ++cnt + " of " + lstKWs3.Items.Count + " Completed";
                        label3.Refresh();
                    });
                }
            }

            process3 = true;
            if (process1 && process2 && process3)
                Environment.Exit(Environment.ExitCode);
        }

        private async Task SendToAPI1(string seid, string kw, string res, string jobid)
        {
            XmlDocument xd = new XmlDocument();
            res = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + res;
            xd.LoadXml(res);
            xd.Save(xmlPath1);
            //File.WriteAllText(@"D:\23-03-2020\" + seid + "_" + kw + jobid + ".xml", res);
            //return;
            //SendToURL

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
                string postData = await GetTextFromXMLFile(xmlPath1);
                byte[] data = encoding.GetBytes(postData);

                httpWReq.ProtocolVersion = HttpVersion.Version11;
                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/x-www-form-urlencoded";


                string auth = string.Format("{0}:{1}", user, pwd);
                string enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(auth));
                string cred = string.Format("{0} {1}", "Basic", enc);


                httpWReq.Headers[HttpRequestHeader.Authorization] = cred;
                httpWReq.ContentLength = data.Length;


                Stream stream = await httpWReq.GetRequestStreamAsync();
                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse response = (HttpWebResponse)await httpWReq.GetResponseAsync();
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
                await SendToDBFailure(seid, kw, jobid, false,ex.Message);


                string errorMsg = string.Empty;
                using (WebResponse response = ex.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    errorMsg = string.Format("API Error: StatusCode {0}", httpResponse.StatusCode);

                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        errorMsg += "\r\n" + await reader.ReadToEndAsync();
                    }
                }

                throw new Exception(errorMsg);

            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }

        }

        private async Task SendToAPI2(string seid, string kw, string res, string jobid)
        {
            XmlDocument xd = new XmlDocument();
            res = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + res;
            xd.LoadXml(res);
            xd.Save(xmlPath2);
            //File.WriteAllText(@"D:\23-03-2020\" + seid + "_" + kw + jobid + ".xml", res);
            //return;
            //SendToURL

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
                string postData = await GetTextFromXMLFile(xmlPath2);
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

                Stream stream = await httpWReq.GetRequestStreamAsync();
                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse response = (HttpWebResponse)await httpWReq.GetResponseAsync();
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
                await SendToDBFailure(seid, kw, jobid, false,ex.Message);


                string errorMsg = string.Empty;
                using (WebResponse response = ex.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    errorMsg = string.Format("API Error: StatusCode {0}", httpResponse.StatusCode);

                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        errorMsg += "\r\n" + await reader.ReadToEndAsync();
                    }
                }

                throw new Exception(errorMsg);

            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }

        }

        private async Task SendToAPI3(string seid, string kw, string res, string jobid)
        {
            XmlDocument xd = new XmlDocument();
            res = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + res;
            xd.LoadXml(res);
            xd.Save(xmlPath3);
            //File.WriteAllText(@"D:\23-03-2020\" + seid + "_" + kw + jobid + ".xml", res);
            //return;
            //SendToURL

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
                string postData = await GetTextFromXMLFile(xmlPath3);
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

                Stream stream = await httpWReq.GetRequestStreamAsync();
                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse response = (HttpWebResponse) await httpWReq.GetResponseAsync();
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
                await SendToDBFailure(seid, kw, jobid, false,ex.Message);


                string errorMsg = string.Empty;
                using (WebResponse response = ex.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    errorMsg = string.Format("API Error: StatusCode {0}", httpResponse.StatusCode);

                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        errorMsg += "\r\n" + await reader.ReadToEndAsync();
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
            string ret = await reader.ReadToEndAsync();
            reader.Close();
            return ret;
        }

        public async Task<string> readAPI()
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
        private async Task InsertAIO_Keyword(string kw, string seid, bool aio)//16-02-2025
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
                //string myDate = DateTime.Today.ToString("yyyy-MM-dd");
                using (SqlConnection con = new SqlConnection(await Common.ReadConnection()))
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
                        comm.Parameters.Add("Received", SqlDbType.VarChar).Value = "Normal Request Oxylabs Multithread Jobid"; //14-04-2025

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
        private async Task GetKeywords1(string qry)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                lstKWs.Items.Clear();

                //lstKWs.Items.Add("58|protective mask|6680028893061066753");
            });
            //return;

            try
            {
                using (SqlConnection con = new SqlConnection(await Common.ReadConnection()))
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
                                    lstKWs.Items.Add(dr.GetValue(0) + ":" + dr.GetValue(1)); //26-10-2020 applied ":"

                                });
                            }
                        }
                    }
                }
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

        private async Task GetKeywords2(string qry)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                lstKWs2.Items.Clear();

                //lstKWs2.Items.Add("58:protective mask:6680028893061066753");
            });
            //return;

            try
            {
                using (SqlConnection con = new SqlConnection(await Common.ReadConnection()))
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
                                    lstKWs2.Items.Add(dr.GetValue(0) + ":" + dr.GetValue(1)); //26-10-2020 applied ":"

                                });
                            }
                        }
                    }
                }
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

        private async Task GetKeywords3(string qry)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                lstKWs3.Items.Clear();
                //lstKWs3.Items.Add("58:protective mask:6680028893061066753");
            });
            //return;

            try
            {
                using (SqlConnection con = new SqlConnection(await Common.ReadConnection()))
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
                                    lstKWs3.Items.Add(dr.GetValue(0) + ":" + dr.GetValue(1)); //26-10-2020 applied ":"

                                });
                            }
                        }
                    }
                }
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
                throw ex.InnerException;//03-01-2022
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
                username = "piapp";
                password = "b5FCvgkjxx";
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
                //query = sp.query.Split(','),
                query = keyword,
                limit = 100,
                pages = 1,
                //start_page = 1,
                locale = sp.locale,
                geo_location = sp.geo_location,
                //uule = uule,
                parse = false, //23-09-2021 changed datatype into "int to bool"
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
                                txtError.Text = txtError.Text + cbUrl[6] + ": " + cbUrl[0] + ": " + cbUrl[4] + Environment.NewLine + "AIO Multithread Request Status is faulted" +
                                    Environment.NewLine + Environment.NewLine;
                                txtError.Refresh();
                            });//13-05-2025
                            await SendToDBFailure(cbUrl[6], cbUrl[0], cbUrl[4], false, "RapidTrackingMultithread Request Status is faulted");
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

        private void txtError_TextChanged(object sender, EventArgs e)
        {

        }
    }

}
