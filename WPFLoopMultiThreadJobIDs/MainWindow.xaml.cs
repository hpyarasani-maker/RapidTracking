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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace WPFLoopMultiThreadJobIDs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string xmlPath1 = @"C:\inetpub\wwwroot\MissingJobID_GT20_1.xml";
        string xmlPath2 = @"C:\inetpub\wwwroot\MissingJobID_GT20_2.xml";
        string xmlPath3 = @"C:\inetpub\wwwroot\MissingJobID_GT20_3.xml";//changes

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        bool process1 = false;
        bool process2 = false;
        bool process3 = false;

        string myDate = string.Empty;//21-01-2025
        public MainWindow()
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
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //this.Title = "WPF_RapidTracking_MissingKeywords_(1-2-3)";//changes
            this.Title = "RapidTracking_Errorkeywords_(1-2-3)"; //changes
            dtPicker1.SelectedDate = DateTime.Today;
            myDate = dtPicker1.SelectedDate.Value.Date.ToString("yyyy-MM-dd");//21-01-2025
            //myDate = DateTime.Now.ToString("yyyy-MM-dd");//21-01-2025 commented

            //Thread t1 = new Thread(new ThreadStart(StartProcess_1));
            //t1.SetApartmentState(ApartmentState.STA);
            //t1.Priority = ThreadPriority.Lowest;
            //t1.Start();
            Task t1 = Task.Run(async () => //16-02-2025
            {
                try
                {
                    await StartProcess_1();
                }
                catch (Exception ex)
                {
                    this.txtError.Dispatcher.Invoke((MethodInvoker)delegate ()
                    {
                        txtError.Text = $"Exception: {ex.Message}";
                    });

                }
            });
            //Thread t2 = new Thread(new ThreadStart(StartProcess_2));
            //t2.SetApartmentState(ApartmentState.STA);
            //t2.Priority = ThreadPriority.Lowest;
            //t2.Start();
            Task t2 = Task.Run(async () =>
            {
                try
                {
                    await StartProcess_2();
                }
                catch (Exception ex)
                {
                    this.txtError.Dispatcher.Invoke((MethodInvoker)delegate ()
                    {
                        txtError.Text = $"Exception: {ex.Message}";
                    });

                }
            });

            //Thread t3 = new Thread(new ThreadStart(StartProcess_3));
            //t3.SetApartmentState(ApartmentState.STA);
            //t3.Priority = ThreadPriority.Lowest;
            //t3.Start();
            Task t3 = Task.Run(async () =>
            {
                try
                {
                    await StartProcess_3();
                }
                catch (Exception ex)
                {
                    this.txtError.Dispatcher.Invoke((MethodInvoker)delegate ()
                    {
                        txtError.Text = $"Exception: {ex.Message}";
                    });

                }
            });//16-02-2025
        }
    
    private async Task StartProcess_1() //16-02-2025
    {
        while (true)
        {
                //string myDate = DateTime.Today.ToString("yyyy-MM-dd");
                //string kwQry = "GetErrorKeywords_1 '" + myDate + "'";//changes
                //string kwQry = "GetAllKeywords_1 '" + myDate + "'";     
                string kwQry = " [GetMissingKeywords] '" + myDate + "',1"; //jobids procedure
                //string kwQry = "[GetErrorKeywords] '" + myDate + "',1"; //changes error jobids procedure
                await GetKeywords1(kwQry);

            if (lstKWs.Items.Count <= 0)
                break;

            int cnt = 0;
            foreach (string s in lstKWs.Items)
            {
                //26-10-2020 changed character as ':'
                string seid = s.Split(':')[0];
                string kw = s.Split(':')[1];
                string jobid = s.Split(':')[2];
                //end of 26-10-2020 changed character as ':'
                try
                {
                    var doc = new HtmlAgilityPack.HtmlDocument();
                        Task<ArrayList> alresult = GetHTML(kw, Convert.ToInt32(seid), jobid);
                        if (alresult.Status.ToString() == stats.Faulted.ToString() || alresult.Status.ToString() == stats.Pending.ToString() || alresult.Status.ToString() == stats.Empty.ToString() || alresult.Status.ToString() == stats.statuscode.ToString()) //07-02-2022//31-01-2022//04-01-2022
                            throw alresult.Exception.InnerException;//04-01-2022
                        ArrayList alXml = new ArrayList();
                        int count = 0;
                        string device = string.Empty;
                        string res = string.Empty;
                        foreach (ResultObject r in alresult.Result)
                            foreach (string src in r.Result)
                            {
                                string keyword = r.Keyword;
                                //JObject obj = JObject.Parse(src[1]);
                                //string html = obj["results"][0]["content"].Value<string>();
                                string html = src;
                                device = r.Device;
                                doc = new HtmlAgilityPack.HtmlDocument();
                                doc.LoadHtml(html);
                                //File.WriteAllText(@"C:\inetpub\wwwroot\html\" + jobid + "_" + keyword + ".html", html, Encoding.UTF8);
                                
                                int curCount = 0;

                                if (device == "desktop_chrome")
                                {
                                    Desktop clsDesktop = new Desktop();
                                    alXml.Add(clsDesktop.ProcessDocument(seid, keyword, doc, out curCount));
                                }
                                else
                                {
                                    iOS clsiOS = new iOS();
                                    alXml.Add(clsiOS.ProcessDocument(seid, keyword, doc, out curCount));
                                }
                                count += curCount;
                                doc = null;
                                
                            }
                        this.lblcount1.Dispatcher.Invoke((MethodInvoker)delegate () {
                            lblcount1.Content = "Count: " + count;
                        });
                        int x = 0;
                                XmlDocument xmlDoc = new XmlDocument();
                                foreach (string xml in alXml)
                                {
                                    if (string.IsNullOrEmpty(xml.Trim())) continue;//02-05-2022
                                    if (x == 0)
                                    {
                                        x++;
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
                                res = xmlDoc.InnerXml;
                             if (!string.IsNullOrEmpty(res))
                             {
                            if (count > 0)
                            {
                                await SendToAPI1(seid, kw, res, jobid);
                                await SendToDB(seid, kw, res, jobid, count);
                            }
                            //if (count < 21 && count==0)
                            //{
                            //    await SendToDB(seid, kw, res, jobid, count);
                            //}
                                bool aio = res.Contains("<block type=\"aiOverview\">");//16-02-2025
                                if (aio && device == "mobile_android") // inserting true value //16-02-2025
                                    await InsertAIO_Keyword(kw, seid, aio);
                        }
                    

                }
                catch (Exception ex)
                {
                    try
                    {
                        this.txtError.Dispatcher.Invoke((MethodInvoker)delegate ()
                        {
                            txtError.Text = txtError.Text + seid + ": " + kw + ": " + jobid + Environment.NewLine + ex.Message.ToString() +
                                Environment.NewLine + Environment.NewLine;
                        });
                        await SendToDBFailure(seid, kw, jobid, false, ex.Message);
                    }
                    finally { }
                }
                finally { }
                this.textBox1.Dispatcher.Invoke((MethodInvoker)delegate ()
                {
                    textBox1.Text = s;
                });
                this.label1.Dispatcher.Invoke((MethodInvoker)delegate ()
                {
                    label1.Content = ++cnt + " of " + lstKWs.Items.Count + " Completed";
                    //label1.Refresh();
                });
            }
        }

        process1 = true;
        if (process1 && process2 && process3)
            Environment.Exit(Environment.ExitCode);
    }

    private async Task StartProcess_2()//16-02-2025
    {
        while (true)
        {
                //string myDate = DateTime.Today.ToString("yyyy-MM-dd");
                //string kwQry = "GetErrorKeywords_2 '" + myDate + "'";//changes
                //string kwQry = "GetAllKeywords_2 '" + myDate + "'";     
                string kwQry = " [GetMissingKeywords] '" + myDate + "',2"; //jobids procedure
                //string kwQry = "[GetErrorKeywords] '" + myDate + "',2"; //changes error jobids procedure
                await GetKeywords2(kwQry);
            if (lstKWs2.Items.Count <= 0)
                break;
            int cnt = 0;
            foreach (string s in lstKWs2.Items)
            {
                //26-10-2020 changed character as ':'
                string seid = s.Split(':')[0];
                string kw = s.Split(':')[1];
                string jobid = s.Split(':')[2];
                //end of 26-10-2020 changed character as ':'
                try
                {
                    var doc = new HtmlAgilityPack.HtmlDocument();
                        Task<ArrayList> alresult = GetHTML(kw, Convert.ToInt32(seid), jobid);
                        if (alresult.Status.ToString() == stats.Faulted.ToString() || alresult.Status.ToString() == stats.Pending.ToString() || alresult.Status.ToString() == stats.Empty.ToString() || alresult.Status.ToString() == stats.statuscode.ToString()) //07-02-2022//31-01-2022//04-01-2022
                        throw alresult.Exception.InnerException;//04-01-2022
                        ArrayList alXml = new ArrayList();
                        int count = 0;                        
                        string device = string.Empty;
                        string res = string.Empty;
                        foreach (ResultObject r in alresult.Result)
                            foreach (string src in r.Result)
                            {
                                string keyword = r.Keyword;
                                //JObject obj = JObject.Parse(src[1]);
                                //string html = obj["results"][0]["content"].Value<string>();
                                string html = src;
                                device = r.Device;
                                doc = new HtmlAgilityPack.HtmlDocument();
                                doc.LoadHtml(html);
                                //File.WriteAllText(@"C:\inetpub\wwwroot\html\" + jobid + "_" + keyword + ".html", html, Encoding.UTF8);                                
                                int curCount = 0;

                                if (device == "desktop_chrome")
                                {
                                    Desktop clsDesktop = new Desktop();
                                    alXml.Add(clsDesktop.ProcessDocument(seid, keyword, doc, out curCount));
                                }
                                else
                                {
                                    iOS clsiOS = new iOS();
                                    alXml.Add(clsiOS.ProcessDocument(seid, keyword, doc, out curCount));
                                }
                                count += curCount;
                               doc = null;
                                
                            }
                        this.lblcount2.Dispatcher.Invoke((MethodInvoker)delegate () {
                            lblcount2.Content = "Count: " + count;
                        });
                        int x = 0;
                                XmlDocument xmlDoc = new XmlDocument();
                                foreach (string xml in alXml)
                                {
                                    if (string.IsNullOrEmpty(xml.Trim())) continue;//02-05-2022
                                    if (x == 0)
                                    {
                                        x++;
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

                        res = xmlDoc.InnerXml;
                        if (!string.IsNullOrEmpty(res))
                        {
                            if (count > 0)
                            {
                                await SendToAPI2(seid, kw, res, jobid);
                                await SendToDB(seid, kw, res, jobid, count);
                            }
                            //if (count < 21 && count==0)
                            //{
                            //    await SendToDB(seid, kw, res, jobid, count);
                            //}
                                bool aio = res.Contains("<block type=\"aiOverview\">");//16-02-2025
                                if (aio && device == "mobile_android") // inserting true value //16-02-2025
                                    await InsertAIO_Keyword(kw, seid, aio);
                        }
                    
                }
                catch (Exception ex)
                {
                    try
                    {
                        this.txtError.Dispatcher.Invoke((MethodInvoker)delegate ()
                        {
                            txtError.Text = txtError.Text + seid + ": " + kw + ": " + jobid + Environment.NewLine + ex.Message.ToString() +
                                Environment.NewLine + Environment.NewLine;
                            //txtError.Refresh();
                        });
                        await SendToDBFailure(seid, kw, jobid, false, ex.Message);
                    }
                    finally { }
                }
                finally { }
                this.textBox2.Dispatcher.Invoke((MethodInvoker)delegate ()
                {
                    textBox2.Text = s;
                    //textBox2.Refresh();

                });
                this.label2.Dispatcher.Invoke((MethodInvoker)delegate ()
                {
                    label2.Content = ++cnt + " of " + lstKWs2.Items.Count + " Completed";
                    //label2.Refresh();
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

    private async Task StartProcess_3()//16-02-2025
    {
        while (true)
        {
                //string myDate = DateTime.Today.ToString("yyyy-MM-dd");
                //string kwQry = "GetErrorKeywords_3 '" + myDate + "'";//changes
                //string kwQry = "GetAllKeywords_3 '" + myDate + "'"; 
                string kwQry = " [GetMissingKeywords] '" + myDate + "',3"; //Jobids procedure
                //string kwQry = "[GetErrorKeywords] '" + myDate + "',3"; //changes error jobids procedure
                await GetKeywords3(kwQry);

            if (lstKWs3.Items.Count <= 0)
                break;

            int cnt = 0;
            foreach (string s in lstKWs3.Items)
            {
                //26-10-2020 changed character as ':'
                string seid = s.Split(':')[0];
                string kw = s.Split(':')[1];
                string jobid = s.Split(':')[2];
                //end of 26-10-2020 changed character as ':'
                try
                {
                    var doc = new HtmlAgilityPack.HtmlDocument();
                        Task<ArrayList> alresult = GetHTML(kw, Convert.ToInt32(seid), jobid);
                        if (alresult.Status.ToString() == stats.Faulted.ToString() || alresult.Status.ToString() == stats.Pending.ToString() || alresult.Status.ToString() == stats.Empty.ToString() || alresult.Status.ToString() == stats.statuscode.ToString()) //07-02-2022//31-01-2022//04-01-2022
                            throw alresult.Exception.InnerException;//04-01-2022
                        ArrayList alXml = new ArrayList();
                        int count = 0;
                        string device = string.Empty;
                        string res = string.Empty;
                        foreach (ResultObject r in alresult.Result)
                            foreach (string src in r.Result)
                            {
                                string keyword = r.Keyword;
                                //JObject obj = JObject.Parse(src[1]);
                                //string html = obj["results"][0]["content"].Value<string>();
                                string html = src;
                                device = r.Device;
                                doc = new HtmlAgilityPack.HtmlDocument();
                                doc.LoadHtml(html);
                                //File.WriteAllText(@"C:\inetpub\wwwroot\html\" + jobid + "_" + keyword + ".html", html, Encoding.UTF8);                                
                                int curCount = 0;

                                if (device == "desktop_chrome")
                                {
                                    Desktop clsDesktop = new Desktop();
                                    alXml.Add(clsDesktop.ProcessDocument(seid, keyword, doc, out curCount));
                                }
                                else
                                {
                                    iOS clsiOS = new iOS();
                                    alXml.Add(clsiOS.ProcessDocument(seid, keyword, doc, out curCount));
                                }
                                count += curCount;
                                doc = null;
                                
                            }
                        this.lblcount3.Dispatcher.Invoke((MethodInvoker)delegate () {
                            lblcount3.Content = "Count: " + count;
                        });
                        int x = 0;
                                XmlDocument xmlDoc = new XmlDocument();
                                foreach (string xml in alXml)
                                {
                                    if (string.IsNullOrEmpty(xml.Trim())) continue;//02-05-2022
                                    if (x == 0)
                                    {
                                        x++;
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
                        res = xmlDoc.InnerXml;
                        if (!string.IsNullOrEmpty(res))
                        {
                            if (count > 0)
                            {
                                await SendToAPI3(seid, kw, res, jobid);
                                await SendToDB(seid, kw, res, jobid, count);
                            }
                            //if (count < 21 && count==0)
                            //{
                            //    await SendToDB(seid, kw, res, jobid, count);
                            //}
                                bool aio = res.Contains("<block type=\"aiOverview\">");//16-02-2025
                                if (aio && device == "mobile_android") // inserting true value //16-02-2025
                                    await InsertAIO_Keyword(kw, seid, aio);
                        }
                    
                }
                catch (Exception ex)
                {
                    try
                    {
                        this.txtError.Dispatcher.Invoke((MethodInvoker)delegate ()
                        {
                            txtError.Text = txtError.Text + seid + ": " + kw + ": " + jobid + Environment.NewLine + ex.Message.ToString() +
                                Environment.NewLine + Environment.NewLine;
                            // txtError.Refresh();
                        });
                        await SendToDBFailure(seid, kw, jobid, false, ex.Message);
                    }
                    finally { }
                }
                finally { }
                this.textBox3.Dispatcher.Invoke((MethodInvoker)delegate ()
                {
                    textBox3.Text = s;
                    //textBox3.Refresh();
                });
                this.label3.Dispatcher.Invoke((MethodInvoker)delegate ()
                {
                    label3.Content = ++cnt + " of " + lstKWs3.Items.Count + " Completed";
                    //label2.Refresh();
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


            Stream stream = httpWReq.GetRequestStream();
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

            Stream stream = httpWReq.GetRequestStream();
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

            Stream stream = httpWReq.GetRequestStream();
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

    private async Task SendToDBFailure(string seid, string kw, string jobid, bool isOldPage, string errMsg = "")
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
                    comm.Parameters.Add("Received", SqlDbType.VarChar).Value = "Normal Loop Multithread Jobid"; //14-04-2025
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

    private async Task GetKeywords1(string qry)
    {
        this.lstKWs.Dispatcher.Invoke((MethodInvoker)delegate ()
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
                            this.lstKWs.Dispatcher.Invoke((MethodInvoker)delegate ()
                            {
                                lstKWs.Items.Add(dr.GetValue(0) + ":" + dr.GetValue(1) + ":" + dr.GetValue(2)); //26-10-2020 applied ":"

                            });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            this.txtError.Dispatcher.Invoke((MethodInvoker)delegate ()
            {
                txtError.Text += ex.Message + "\r\n";
            });
        }
        finally { }
    }

    private async Task GetKeywords2(string qry)
    {
        this.lstKWs2.Dispatcher.Invoke((MethodInvoker)delegate ()
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
                            this.lstKWs2.Dispatcher.Invoke((MethodInvoker)delegate ()
                            {
                                lstKWs2.Items.Add(dr.GetValue(0) + ":" + dr.GetValue(1) + ":" + dr.GetValue(2)); //26-10-2020 applied ":"

                            });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            this.txtError.Dispatcher.Invoke((MethodInvoker)delegate ()
            {
                txtError.Text += ex.Message + "\r\n";
            });
        }
        finally { }
    }

    private async Task GetKeywords3(string qry)
    {
        this.lstKWs3.Dispatcher.Invoke((MethodInvoker)delegate ()
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
                            this.lstKWs3.Dispatcher.Invoke((MethodInvoker)delegate ()
                            {
                                lstKWs3.Items.Add(dr.GetValue(0) + ":" + dr.GetValue(1) + ":" + dr.GetValue(2)); //26-10-2020 applied ":"

                            });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            this.txtError.Dispatcher.Invoke((MethodInvoker)delegate ()
            {
                txtError.Text += ex.Message + "\r\n";
            });
        }
        finally { }
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
    public async Task<ArrayList> GetHTML(string keyword, int seid, string jobid)
    {
        ArrayList alResult = new ArrayList();
        try
        {
            SearchProperties sp = SearchParams.searches.Where(s => s.seid == seid).SingleOrDefault();
            sp.query = keyword;
            if (sp != null)
                alResult = GetOxylabsWebDataSources(sp, jobid).Result;
        }
        catch (Exception ex)
        {
            throw ex.InnerException;//03-01-2022
        }

        return await Task.FromResult(alResult);
    }

    async Task<ArrayList> GetOxylabsWebDataSources(SearchProperties sp, string jobid)
    {
        JObject obj = null;//07-02-2022
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
        string response;
        ArrayList lst = new ArrayList();
        string kw = sp.query;
        string href = "";
        string status = "done";
        string device = sp.device;
        string[] s = { kw, href, status, "no", jobid, device };    // keyword, url, status, isdownloaded, jobid, device.
        lst.Add(s);

        if (lst.Count <= 0) return lst;
        ArrayList alResult = new ArrayList();
        do
        {
            int cnt = 0;
            foreach (string[] cbUrl in lst)
            {
                string[] reslt = { "", "", "", "" };
                response = "";
                //Uri uri = new Uri("http://data.oxylabs.io/v1/queries/" + jobid + "/results");
                if (cbUrl[2] == "done" && cbUrl[3] == "no")
                {
                    try
                    {
                            //var _links = jo["queries"]?[0]?["_links"];
                            var _links = await GetJobLinks("http://data.oxylabs.io/v1/queries/" + jobid, authInfo);
                            string[] urls = _links?
                                .Where(link => (string)link["rel"] == "results-content")
                                .SelectMany(link => (link["href_list"] as JArray ?? new JArray())
                                    .Select(item => item.ToString()))
                                .ToArray();
                            List<string> source = new List<string>();
                            foreach (string url in urls)
                            {
                                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
                                httpWebRequest.Headers["Authorization"] = "Basic " + authInfo;
                                HttpWebResponse res = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                                Stream resVal = res.GetResponseStream();
                                StreamReader reader = new StreamReader(resVal, Encoding.UTF8);
                                //** Store all the contents
                                string response1 = reader.ReadToEnd();
                                resVal.Close();
                                res.Close();
                                source.Add(response1);
                            }
                            cbUrl[3] = "yes";
                            cnt++;
                            if (source.Count > 0)
                            {
                                alResult.Add(new ResultObject
                                {
                                    Keyword = cbUrl[0],
                                    JobId = cbUrl[4],
                                    Device = cbUrl[5],
                                    Result = source
                                });
                                string url = "http://data.oxylabs.io/v1/queries/" + jobid;
                                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                                httpWebRequest.Headers["Authorization"] = "Basic " + authInfo;
                                HttpWebResponse res1 = (HttpWebResponse)httpWebRequest.GetResponse();
                                Stream resStream = res1.GetResponseStream();
                                StreamReader reader = new StreamReader(resStream, Encoding.UTF8);
                                response = reader.ReadToEnd();
                                resStream.Close();
                                res1.Close();
                                obj = JObject.Parse(response);
                                status = obj["status"].Value<string>();
                                if (status == "faulted")
                                {
                                    throw new Exception("status is faulted");
                                }
                                if (status == "pending") //31-01-2022
                                {
                                    throw new Exception("status is pending");
                                }
                            }
                            //    else//04-01-2022
                            //    {

                            //        //foreach (string url in urls)
                            //        //{
                            //            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                            //            httpWebRequest.Headers["Authorization"] = "Basic " + authInfo;
                            //            HttpWebResponse res1 = (HttpWebResponse)httpWebRequest.GetResponse();
                            //            Stream resStream = res1.GetResponseStream();
                            //            StreamReader reader = new StreamReader(resStream, Encoding.UTF8);
                            //            response = reader.ReadToEnd();
                            //            resStream.Close();
                            //            res1.Close();

                            //            obj = JObject.Parse(response);
                            //            status = obj["status"].Value<string>();
                            //            if (status == "faulted")
                            //            {
                            //                throw new Exception("status is faulted");
                            //            }
                            //            if (status == "pending") //31-01-2022
                            //            {
                            //                throw new Exception("status is pending");
                            //            }
                            //        //}

                            //}//04-01-2022
                        }

                    catch (Exception ex)
                    {
                        // Console.WriteLine("Result Request: " + ex.Message);//03-01-2022

                        throw new Exception(ex.Message);//31-01-2022//03-01-2022
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
        private async Task<JToken> GetJobLinks(string url, string authInfo)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Headers["Authorization"] = "Basic " + authInfo;
                HttpWebResponse res = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                string response = string.Empty;
                using (StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                {
                    response = reader.ReadToEnd();
                }
                res.Close();
                var jo = JObject.Parse(response);
                return jo["_links"];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        private void LstKWs1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
    {

    }
    private void LstKWs2_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
    {

    }
    private void LstKWs3_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
    {

    }
}
}

