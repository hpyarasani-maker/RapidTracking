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

namespace AIOMultiThread
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

        string myDate = DateTime.Now.ToString("yyyy-MM-dd");
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
            //myDate = dtPicker1.SelectedDate.ToString("yyyy-MM-dd");
            myDate = DateTime.Now.ToString("yyyy-MM-dd");

            Thread t1 = new Thread(new ThreadStart(StartProcess_1));
            t1.SetApartmentState(ApartmentState.STA);
            t1.Priority = ThreadPriority.Lowest;
            t1.Start();

            Thread t2 = new Thread(new ThreadStart(StartProcess_2));
            t2.SetApartmentState(ApartmentState.STA);
            t2.Priority = ThreadPriority.Lowest;
            t2.Start();

            Thread t3 = new Thread(new ThreadStart(StartProcess_3));
            t3.SetApartmentState(ApartmentState.STA);
            t3.Priority = ThreadPriority.Lowest;
            t3.Start();
        }
    
    private void StartProcess_1()
    {
        while (true)
        {
                //string myDate = DateTime.Today.ToString("yyyy-MM-dd");
                //string kwQry = "GetErrorKeywords_1 '" + myDate + "'";//changes
                //string kwQry = "GetAllKeywords_1 '" + myDate + "'";     
                string kwQry = " [GetMissingKeywords] '" + myDate + "',1"; //jobids procedure
                //string kwQry = "[GetErrorKeywords] '" + myDate + "',1"; //changes error jobids procedure
                GetKeywords1(kwQry);

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
                    foreach (string[] src in alresult.Result)
                    {
                        string keyword = src[0];
                        JObject obj = JObject.Parse(src[1]);
                        string html = obj["results"][0]["content"].Value<string>();
                        string device = src[3];
                        doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(html);
                        //File.WriteAllText(@"C:\inetpub\wwwroot\html\" + jobid + "_" + keyword + ".html", html, Encoding.UTF8);
                        string res = string.Empty;
                        int count = 0;

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
                            doc = null;
                        this.lblcount1.Dispatcher.Invoke((MethodInvoker)delegate () {
                            lblcount1.Content = "Count: " + count;
                        });
                        if (!string.IsNullOrEmpty(res))
                        {
                            if (count > 20)
                            {
                                SendToAPI1(seid, keyword, res, jobid);
                                SendToDB(seid, keyword, res, jobid, count);
                            }
                            if (count < 21 && count==0)
                            {
                                SendToDB(seid, keyword, res, jobid, count);
                            }
                        }
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
                        SendToDBFailure(seid, kw, jobid, false, ex.Message);
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

    private void StartProcess_2()
    {
        while (true)
        {
                //string myDate = DateTime.Today.ToString("yyyy-MM-dd");
                //string kwQry = "GetErrorKeywords_2 '" + myDate + "'";//changes
                //string kwQry = "GetAllKeywords_2 '" + myDate + "'";     
                string kwQry = " [GetMissingKeywords] '" + myDate + "',2"; //jobids procedure
                //string kwQry = "[GetErrorKeywords] '" + myDate + "',2"; //changes error jobids procedure
                GetKeywords2(kwQry);

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
                    foreach (string[] src in alresult.Result)
                    {
                        string keyword = src[0];
                        JObject obj = JObject.Parse(src[1]);
                        string html = obj["results"][0]["content"].Value<string>();
                        string device = src[3];
                        doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(html);
                        //File.WriteAllText(@"C:\inetpub\wwwroot\html\" + jobid + "_" + keyword + ".html", html, Encoding.UTF8);
                        string res = string.Empty;
                        int count = 0;

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
                            doc = null;
                            this.lblcount2.Dispatcher.Invoke((MethodInvoker)delegate () {
                            lblcount2.Content = "Count: " + count;
                        });
                        if (!string.IsNullOrEmpty(res))
                        {
                            if (count > 20)
                            {
                                SendToAPI2(seid, keyword, res, jobid);
                                SendToDB(seid, keyword, res, jobid, count);
                            }
                            if (count < 21 && count==0)
                            {
                                SendToDB(seid, keyword, res, jobid, count);
                            }
                        }
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
                        SendToDBFailure(seid, kw, jobid, false, ex.Message);
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

    private void StartProcess_3()
    {
        while (true)
        {
                //string myDate = DateTime.Today.ToString("yyyy-MM-dd");
                //string kwQry = "GetErrorKeywords_3 '" + myDate + "'";//changes
                //string kwQry = "GetAllKeywords_3 '" + myDate + "'"; 
                string kwQry = " [GetMissingKeywords] '" + myDate + "',3"; //Jobids procedure
                //string kwQry = "[GetErrorKeywords] '" + myDate + "',3"; //changes error jobids procedure
                GetKeywords3(kwQry);

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
                    foreach (string[] src in alresult.Result)
                    {
                        string keyword = src[0];
                        JObject obj = JObject.Parse(src[1]);
                        string html = obj["results"][0]["content"].Value<string>();
                        string device = src[3];
                        doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(html);
                        //File.WriteAllText(@"C:\inetpub\wwwroot\html\" + jobid + "_" + keyword + ".html", html, Encoding.UTF8);
                        string res = string.Empty;
                        int count = 0;

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
                            doc = null;
                            this.lblcount3.Dispatcher.Invoke((MethodInvoker)delegate () {
                            lblcount3.Content = "Count: " + count;
                        });
                        if (!string.IsNullOrEmpty(res))
                        {
                            if (count > 20)
                            {
                                SendToAPI3(seid, keyword, res, jobid);
                                SendToDB(seid, keyword, res, jobid, count);
                            }
                            if (count < 21 && count==0)
                            {
                                SendToDB(seid, keyword, res, jobid, count);
                            }
                        }
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
                        SendToDBFailure(seid, kw, jobid, false, ex.Message);
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

    private void SendToAPI1(string seid, string kw, string res, string jobid)
    {
        XmlDocument xd = new XmlDocument();
        res = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + res;
        xd.LoadXml(res);
        xd.Save(xmlPath1);
        //File.WriteAllText(@"D:\23-03-2020\" + seid + "_" + kw + jobid + ".xml", res);
        //return;
        //SendToURL

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
            httpWReq.ContentType = "application/x-www-form-urlencoded";


            string auth = string.Format("{0}:{1}", user, pwd);
            string enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(auth));
            string cred = string.Format("{0} {1}", "Basic", enc);


            httpWReq.Headers[HttpRequestHeader.Authorization] = cred;
            httpWReq.ContentLength = data.Length;


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
            SendToDBFailure(seid, kw, jobid, false);


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

    private void SendToAPI2(string seid, string kw, string res, string jobid)
    {
        XmlDocument xd = new XmlDocument();
        res = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + res;
        xd.LoadXml(res);
        xd.Save(xmlPath2);
        //File.WriteAllText(@"D:\23-03-2020\" + seid + "_" + kw + jobid + ".xml", res);
        //return;
        //SendToURL

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
            string postData = GetTextFromXMLFile(xmlPath2);
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
            SendToDBFailure(seid, kw, jobid, false);


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

    private void SendToAPI3(string seid, string kw, string res, string jobid)
    {
        XmlDocument xd = new XmlDocument();
        res = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + res;
        xd.LoadXml(res);
        xd.Save(xmlPath3);
        //File.WriteAllText(@"D:\23-03-2020\" + seid + "_" + kw + jobid + ".xml", res);
        //return;
        //SendToURL

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
            string postData = GetTextFromXMLFile(xmlPath3);
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
            SendToDBFailure(seid, kw, jobid, false);


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

    private void SendToDBFailure(string seid, string kw, string jobid, bool isOldPage, string errMsg = "")
    {
        string myDate = DateTime.Today.ToString("yyyy-MM-dd");
        //string myDate = "2019-10-10";

        string qry = "insert into dashboard_dataerrors (date, name, seid, jobid,message) values(Convert(varchar(10),'" + myDate + "',103), N'" +
                kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "', N'" + errMsg + "')"; //03-01-2022

        string qryOld = "insert into dashboard_oldgooglepage (date, keyword, seid, jobid) values('" + DateTime.Now + "', N'" +
              kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "')";

        using (SqlConnection con = new SqlConnection(Common.ReadConnection()))
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

    private void SendToDB(string seid, string keyword, string xml, string jobid, int urlcount)
    {
        try
        {
            //string myDate = DateTime.Today.ToString("yyyy-MM-dd");
            using (SqlConnection con = new SqlConnection(Common.ReadConnection()))
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

    private void GetKeywords1(string qry)
    {
        this.lstKWs.Dispatcher.Invoke((MethodInvoker)delegate ()
        {
            lstKWs.Items.Clear();

            //lstKWs.Items.Add("58|protective mask|6680028893061066753");
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

    private void GetKeywords2(string qry)
    {
        this.lstKWs2.Dispatcher.Invoke((MethodInvoker)delegate ()
        {
            lstKWs2.Items.Clear();

            //lstKWs2.Items.Add("58:protective mask:6680028893061066753");
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

    private void GetKeywords3(string qry)
    {
        this.lstKWs3.Dispatcher.Invoke((MethodInvoker)delegate ()
        {
            lstKWs3.Items.Clear();
            //lstKWs3.Items.Add("58:protective mask:6680028893061066753");
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
        //string username = "gpidatametrics";
        //string password = "sdV5X3fcX6";
        string username = "piapp";
        string password = "b5FCvgkjxx";
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
                Uri uri = new Uri("http://data.oxylabs.io/v1/queries/" + jobid + "/results");
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
                        if (response == "")//31-01-2022
                        {
                            throw new Exception("empty");
                        }//31-01-2022
                        obj = JObject.Parse(response);//07-02-2022
                        string statuscode = obj["results"][0]["status_code"].Value<string>();//07-02-2022
                        if (statuscode != "200")
                        {
                            throw new Exception("Status code : " + statuscode);
                        }//07-02-2022 end

                        if (!string.IsNullOrEmpty(response))
                        {
                            reslt[0] = cbUrl[0];
                            reslt[1] = response;
                            reslt[2] = cbUrl[4];
                            reslt[3] = cbUrl[5];
                            alResult.Add(reslt);
                        }
                        else//04-01-2022
                        {
                            string resURL = "http://data.oxylabs.io/v1/queries/" + jobid;
                            httpWebRequest = (HttpWebRequest)WebRequest.Create(resURL);
                            httpWebRequest.Headers["Authorization"] = "Basic " + authInfo;
                            HttpWebResponse res1 = (HttpWebResponse)httpWebRequest.GetResponse();
                            Stream resStream = res1.GetResponseStream();
                            reader = new StreamReader(resStream, Encoding.UTF8);
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

                        }//04-01-2022
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

