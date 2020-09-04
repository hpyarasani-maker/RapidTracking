using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace RapidTrackingSingleThread
{
    public partial class Form1 : Form
    {
        OxylabsProxies WOWS = new OxylabsProxies();
        //ServerIP WOWS = new ServerIP();

        ArrayList seresults = new ArrayList();

        string xmlPath = "C:\\inetpub\\wwwroot\\Remaining_GT20_All_S1.xml";


        public string myDate = string.Empty;
        string statusCode = string.Empty;
        string liveurl = string.Empty;

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        void TimerExit()
        {
            timer.Interval = 30 * 60000;
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            Environment.Exit(Environment.ExitCode);
        }
        public Form1()
        {
            InitializeComponent();
        }


        public string StrConn()
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

        static int errCount1 = 0;
        int kwcnt1 = 0;

        static int cnt = 0;
        const int maxCnt = 120;

        void IPChanger()
        {
            lock (new Object())
            {
                cnt++;
                if (cnt > maxCnt)
                {
                    cnt = 0;
                    errCount1 = 0;

                    WOWS.x++;
                    if (WOWS.x >= WOWS.dtIPs.Rows.Count) WOWS.x = 0;
                    StreamWriter sw = new StreamWriter("index.txt", false);
                    sw.WriteLine(WOWS.x);
                    sw.Close();

                    //StreamWriter sw1 = new StreamWriter("testlog.txt", true);
                    //sw1.WriteLine(WOWS.x + " : " + WOWS.dtIPs.Rows[WOWS.x][1].ToString() + " : " + DateTime.Now);
                    //sw1.Close();
                    this.Text = "D_RapidTracking_All_1_GT20_ServerIP1_" + WOWS.dtIPs.Rows[WOWS.x][1].ToString();

                }
            }
        }

        public void GenerateWorklist()
        {
            worklist.Invoke((MethodInvoker)(delegate ()
            {
                worklist.Items.Clear();
                //worklist.Items.Add("58:hotel near manila airport terminal 3");
                //worklist.Items.Add("1:#coronopocolypse");
                //worklist.Items.Add("1:@diabetes_101");
                //worklist.Items.Add("1:@fionamartin123");
                //worklist.Items.Add("1:@smith101sam");
                worklist.Items.Add("106:rob beckett tour");
            }));
            Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            date_picker.Format = DateTimePickerFormat.Custom;
            date_picker.CustomFormat = "yyyy-MM-dd";
            string myDate = date_picker.Text;
            return;
            //string strSql = "exec [dbo].[Tracking_DB_Keywords_SEID_102] '" + myDate + "'";       
            string strSql = "exec [dbo].[GetAllKeywords_ServerIps_1] '" + myDate + "'";      

            try
            {
                using (SqlConnection con = new SqlConnection(Common.ReadConnection()))
                {
                    con.Open();
                    using (SqlCommand comm = new SqlCommand(strSql, con))
                    {
                        comm.CommandTimeout = 0;
                        using (SqlDataReader dr = comm.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (dr.Read())
                            {
                                this.Invoke((MethodInvoker)delegate ()
                                {
                                    worklist.Items.Add(dr[0].ToString() + ":" + dr[1].ToString());
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                string errMsg = "Database Connection is temporarily not working\n" + e.ToString();
                errorList.Invoke((MethodInvoker)(delegate ()
                {
                    errorList.Items.Add(errMsg);
                }));

            }
            catch (Exception ex)
            {
                errorList.Invoke((MethodInvoker)(delegate ()
                {
                    errorList.Items.Add(ex.ToString());
                }));
            }
            finally
            {

            }
        }
        public int GetWorklistSize()
        {
            int worklistSize = worklist.Items.Count;
            return worklistSize;
        }
        public string ReadAPI()
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

        public void ProcessResults(string seid, string kn)
        {
            IPChanger();
            string[] seresults = new string[1];
            date_picker.Format = DateTimePickerFormat.Custom;
            date_picker.CustomFormat = "yyyy-MM-dd";
            string myDate = date_picker.Text;
            //string myDate = "2020-02-06";
            //included try catch for capturing error 429 and threading happening up and down included return; has been solved - 20-01-2020
            try
            {
                seresults = WOWS.GetTop100(kn, int.Parse(seid));
            }
            catch (WebException ex)
            {
                results.Invoke((MethodInvoker)(delegate ()
                {
                    results.Items.Add(ex.Message);
                    results.Refresh();
                }));
                return;
            }

            results.Invoke((MethodInvoker)(delegate ()
            {
                results.Items.Clear();
            }));
            if (string.IsNullOrEmpty(seresults[0]) || seresults[0].Trim().StartsWith("Index was outside the bounds of the array"))
            {
                results.Invoke((MethodInvoker)(delegate ()
                {
                    results.Items.Add("no result.");
                    results.Refresh();
                }));

                errCount1++;
                if (errCount1 >= 10)
                {
                    cnt = maxCnt;
                    IPChanger();
                }
            }
            if (int.Parse(seresults[1]) < 1)
            {
                results.Invoke((MethodInvoker)(delegate ()
                {
                    results.Items.Add("No Results");
                    results.Refresh();
                }));

            }
            else if (seresults[1].ToString().Contains("Value cannot be null") || seresults[1].ToString().Trim().Contains("index was outside the bounds of the array"))
            {
                results.Invoke((MethodInvoker)(delegate ()
                {
                    results.Items.Add("No Results.");
                    results.Refresh();
                }));

                errCount1++;
                if (errCount1 >= 10)
                {
                    cnt = maxCnt;
                    IPChanger();
                }
            }
            else
            {
                kwcnt1++;

                results.Invoke((MethodInvoker)(delegate ()
                {
                    if (kwcnt1 >= 10)
                    {
                        IPChanger();
                        kwcnt1 = 0;
                    }
                }));

                results.Invoke((MethodInvoker)(delegate ()
                {
                    results.Items.Add(seid + " " + kn);
                    label1.Text = "Classic Links : " + (seresults[1]);
                }));
            }

            if (myDate != "")
            {
                try
                {
                    int rescount2 = int.Parse(seresults[1]);
                    if (rescount2 > 20)
                    {
                        SendToAPI(seid, kn, seresults[0]);
                        SendToDB(seid, kn, seresults[0], int.Parse(seresults[1]));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void SendToDB(string seid, string keyword, string xml, int urlcount)
        {
            string myDate = DateTime.Now.ToString("yyyy-MM-dd");
            //string myDate = "2020-02-06";

            try
            {
                using (SqlConnection con = new SqlConnection(Common.ReadConnection()))
                {
                    con.Open();
                    using (SqlCommand comm = con.CreateCommand())
                    {
                        comm.CommandTimeout = 0;
                        comm.CommandType = CommandType.StoredProcedure;
                        comm.CommandText = "Insert_dashboard_data";
                        comm.Parameters.Add("date", SqlDbType.Date).Value = myDate;
                        comm.Parameters.Add("name", SqlDbType.NVarChar).Value = keyword;
                        comm.Parameters.Add("Seid", SqlDbType.Int).Value = seid;
                        comm.Parameters.Add("jobid", SqlDbType.NVarChar).Value = string.Empty;
                        comm.Parameters.Add("count", SqlDbType.Int).Value = urlcount;
                        comm.Parameters.Add("XmlData", SqlDbType.Xml).Value = xml.Replace("'", "''");

                        comm.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                string sqlerror = ex.Message.ToString();
            }
            finally { }
        }

        private void SendToAPI(string seid, string kw, string res)
        {
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
            //return;
            string submitURL = ReadAPI();

            string user = "pisoftware";
            string pwd = "r00t123456";
            try
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(submitURL);
                httpWReq.UseDefaultCredentials = true;
                httpWReq.PreAuthenticate = true;
                httpWReq.Credentials = CredentialCache.DefaultCredentials;

                Encoding encoding = new UTF8Encoding();
                string postData = GetTextFromXMLFile(xmlPath);
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
                string s = response.ToString();
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
                    //statusCode = httpResponse.StatusCode.ToString();
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
        int i;
        public void ProcessWorklist()
        {
            string resultsString;
            char sep;
            Array resultsArray;
            string seid;
            string kn;
            try
            {
                for (i = 0; i < worklist.Items.Count; i++)
                {
                    // get next Project ID, Search Engine ID, Keyword ID and Keyword from worklist
                    resultsString = worklist.Items[i].ToString();
                    sep = ':';
                    resultsArray = resultsString.Split(sep);
                    seid = resultsArray.GetValue(0).ToString();
                    kn = resultsArray.GetValue(1).ToString();
                    try
                    {
                        ProcessResults(seid, kn);
                    }
                    catch (Exception ex)
                    {
                        errorList.Invoke((MethodInvoker)(delegate ()
                        {
                            errorList.Items.Add(ex.Message.ToString());
                        }));
                    }
                    progress_lbl.Invoke((MethodInvoker)(delegate ()
                    {
                        progress_lbl.Text = "Completed : " + (i + 1) + " of " + worklist.Items.Count;
                        // results.Refresh();
                        progress_lbl.Refresh();
                    }));
                }
            }
            catch (Exception ex)
            {
                errorList.Invoke((MethodInvoker)(delegate ()
                {
                    errorList.Items.Add(ex.Message.ToString());
                }));

            }
        }

        public void MainLoop()
        {

            GenerateWorklist();
            while (GetWorklistSize() > 0)
            {
                ProcessWorklist();
                GenerateWorklist();
            }
            Environment.Exit(Environment.ExitCode);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)(delegate ()
            {
                date_picker.Value = DateTime.Today;
            }));

            liveurl = ReadAPI();
            if (File.Exists("index.txt"))
            {
                StreamReader sw = new StreamReader("index.txt");
                string val = sw.ReadLine();
                sw.Close();

                if (Convert.ToInt32(val) >= 0) WOWS.x = Convert.ToInt32(val);
            }

            if (WOWS.x >= WOWS.dtIPs.Rows.Count)
            {
                WOWS.x = 0;
            }

            this.Invoke((MethodInvoker)(delegate ()
            {
                //date_picker.Value = DateTime.Today.AddDays(-2);

                this.Text = "D_RapidTracking_All_1_GT20_ServerIP1_" + WOWS.dtIPs.Rows[WOWS.x][1].ToString();

            }));
            Thread myThread = new Thread(new ThreadStart(MainLoop));
            GenerateWorklist();
            if (GetWorklistSize() > 0)
            {
                //mainLoop();
                myThread.Start();
            }
            else
            {
                this.Dispose();
                Application.Exit();
            }
        }
    }
}
