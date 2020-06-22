using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Net;
using System.Data;
using System.Xml;
using System.Xml.XPath;
using System.Web;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace RapidTrackingMultithread
{
    public partial class Form1 : Form
    {
        ServerIP server0 = new ServerIP();

        string xmlPath1 = "C:\\inetpub\\wwwroot\\RapidTracking_s_1.xml";
        string xmlPath2 = "C:\\inetpub\\wwwroot\\RapidTracking_s_2.xml";
        string xmlPath3 = "C:\\inetpub\\wwwroot\\RapidTracking_s_3.xml";

        string strCon = string.Empty;
        string liveurl = string.Empty;
        public string myDate = string.Empty;

        int kwcnt1 = 0;
        int kwcnt2 = 0;
        int kwcnt3 = 0;

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        public Form1()
        {
            InitializeComponent();
            timerExit();
            strCon = strConn();
            liveurl = readAPI();
        }

        void timerExit()
        {
            timer.Interval = 20 * 60000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            Environment.Exit(Environment.ExitCode);
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


        static int errCount1 = 0;
        static int errCount2 = 0;
        static int errCount3 = 0;

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
                    errCount2 = 0;
                    errCount3 = 0;
                    server0.x++;
                    if (server0.x >= server0.dtIPs.Rows.Count) server0.x = 0;
                    StreamWriter sw = new StreamWriter("index.txt", false);
                    sw.WriteLine(server0.x);
                    sw.Close();

                    StreamWriter sw1 = new StreamWriter("testlog.txt", true);
                    sw1.WriteLine(server0.x + " : " + server0.dtIPs.Rows[server0.x][1].ToString() + " : " + DateTime.Now);
                    sw1.Close();

                    this.Text = "D_TrackingTrending_(1-2-3)_Server-1_" + server0.dtIPs.Rows[server0.x][1].ToString();
                    
                }
            }
        }

        public void generateWorklist2()
        {
            worklist1.Invoke((MethodInvoker)(delegate ()
            {
                worklist1.Items.Clear();
                //worklist1.Items.Add("102:donald trump");
            }));
            Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            date_picker.Format = DateTimePickerFormat.Custom;
            date_picker.CustomFormat = "yyyy-MM-dd";
            myDate = date_picker.Text;
            //return;
            string strSql = "exec [dbo].[GetKeywordsAdult_1] '" + myDate + "'";
       
            SqlConnection objCon = null;
            SqlDataReader objData = null;
            try
            {
                objCon = new SqlConnection(strCon);
                objCon.Open();
                SqlCommand objCmd = new SqlCommand(strSql, objCon);
                objCmd.CommandTimeout = 0;
                objData = objCmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (objData.Read())
                {
                    worklist1.Invoke((MethodInvoker)(delegate ()
                    {
                        worklist1.Items.Add(objData[0].ToString() + ":" + objData[1].ToString());
                    }));
                }
                worklist1.Invoke((MethodInvoker)(delegate ()
                {
                    worklist1.Refresh();
                }));
                objData.Close();
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
                if (objCon.State == ConnectionState.Open)
                    objCon.Close();
            }
        }

        public void generateWorklist6()
        {
            worklist2.Invoke((MethodInvoker)(delegate ()
            {
                worklist2.Items.Clear();
            }));

            Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            date_picker.Format = DateTimePickerFormat.Custom;
            date_picker.CustomFormat = "yyyy-MM-dd";
            myDate = date_picker.Text;
            //return;
            string strSql = "exec [dbo].[GetKeywordsAdult_2] '" + myDate + "'";
           
            SqlConnection objCon = null;
            SqlDataReader objData = null;
            try
            {
                objCon = new SqlConnection(strCon);
                objCon.Open();
                SqlCommand objCmd = new SqlCommand(strSql, objCon);
                objCmd.CommandTimeout = 0;
                objData = objCmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (objData.Read())
                {
                    worklist2.Invoke((MethodInvoker)(delegate ()
                    {
                        worklist2.Items.Add(objData[0].ToString() + ":" + objData[1].ToString());
                    }));
                }
                worklist2.Invoke((MethodInvoker)(delegate ()
                {
                    worklist2.Refresh();
                }));
                objData.Close();

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
                if (objCon.State == ConnectionState.Open)
                    objCon.Close();
            }
        }

        public void generateWorklist12()
        {
            worklist3.Invoke((MethodInvoker)(delegate ()
            {
                worklist3.Items.Clear();
            }));
            Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            date_picker.Format = DateTimePickerFormat.Custom;
            date_picker.CustomFormat = "yyyy-MM-dd";
            myDate = date_picker.Text;
            //return;
            string strSql = "exec [dbo].[GetKeywordsAdult_3] '" + myDate + "'";

            SqlConnection objCon = null;
            SqlDataReader objData = null;
            try
            {
                objCon = new SqlConnection(strCon);
                objCon.Open();
                SqlCommand objCmd = new SqlCommand(strSql, objCon);
                objCmd.CommandTimeout = 0;
                objData = objCmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (objData.Read())
                {
                    worklist3.Invoke((MethodInvoker)(delegate ()
                    {
                        worklist3.Items.Add(objData[0].ToString() + ":" + objData[1].ToString());
                    }));
                }
                worklist3.Invoke((MethodInvoker)(delegate ()
                {
                    worklist3.Refresh();
                }));
                objData.Close();
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
                if (objCon.State == ConnectionState.Open)
                    objCon.Close();
            }
        }

        public int getWorklistSize2()
        {
            int worklistSize = worklist1.Items.Count;
            return worklistSize;
        }

        public int getWorklistSize6()
        {
            int worklistSize = worklist2.Items.Count;
            return worklistSize;
        }

        public int getWorklistSize12()
        {
            int worklistSize = worklist3.Items.Count;
            return worklistSize;
        }

        public void processResults2(string seid, string kn)
        {
            IPChanger();

            string[] seresults = new string[1];
            date_picker.Format = DateTimePickerFormat.Custom;
            date_picker.CustomFormat = "yyyy-MM-dd";
            string myDate = date_picker.Text;

            try
            {
                seresults = server0.GetTop100(kn, int.Parse(seid));
            }
            catch (WebException ex)
            {
                results1.Invoke((MethodInvoker)(delegate ()
                {
                    results1.Items.Add(ex.Message);
                    results1.Refresh();
                }));
                return;
            }

            results1.Invoke((MethodInvoker)(delegate ()
            {
                results1.Items.Clear();
            }));
            if (string.IsNullOrEmpty(seresults[0]) || seresults[0].Trim().StartsWith("Index was outside the bounds of the array"))
            {
                results1.Invoke((MethodInvoker)(delegate ()
                {
                    results1.Items.Add("no result.");
                    results1.Refresh();
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
                results1.Invoke((MethodInvoker)(delegate ()
                {
                    results1.Items.Add("no Results");
                    results1.Refresh();
                }));
            }
            else if (seresults[1].ToString().Contains("Value cannot be null") || seresults[1].ToString().Trim().Contains("index was outside the bounds of the array"))
            {
                results1.Invoke((MethodInvoker)(delegate ()
                {
                    results1.Items.Add("no Results.");
                    results1.Refresh();
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
                results1.Invoke((MethodInvoker)(delegate ()
                {
                    if (kwcnt1 >= 10)
                    {
                        IPChanger();
                        kwcnt1 = 0;
                    }
                }));
                results1.Invoke((MethodInvoker)(delegate ()
                {
                    results1.Items.Add(seid + " " + kn);
                    label1.Text = "No. of URLs : " + (seresults[1]);
                }));


                if (myDate != "")
                {
                    try
                    {
                        int rescount1 = int.Parse(seresults[1]);
                        if (rescount1 > 20)
                        {
                            SendToAPI1(seid, kn, seresults[0]);
                            SendToDB(seid, kn, seresults[0], int.Parse(seresults[1]));
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        public void processResults6(string seid, string kn)
        {
            IPChanger();

            string[] seresults = new string[1];
            date_picker.Format = DateTimePickerFormat.Custom;
            date_picker.CustomFormat = "yyyy-MM-dd";
            string myDate = date_picker.Text;

            try
            {
                seresults = server0.GetTop100(kn, int.Parse(seid));
            }
            catch (WebException ex)
            {
                results2.Invoke((MethodInvoker)(delegate ()
                {
                    results2.Items.Add(ex.Message);
                    results2.Refresh();
                }));
                return;
            }

            results2.Invoke((MethodInvoker)(delegate ()
            {
                results2.Items.Clear();
            }));
            if (string.IsNullOrEmpty(seresults[0]) || seresults[0].Trim().StartsWith("Index was outside the bounds of the array"))
            {
                results1.Invoke((MethodInvoker)(delegate ()
                {
                    results2.Items.Add("no result.");
                    results2.Refresh();
                }));

                errCount2++;
                if (errCount2 >= 10)
                {
                    cnt = maxCnt;
                    IPChanger();
                }
            }

            if (int.Parse(seresults[1]) < 1)
            {
                results2.Invoke((MethodInvoker)(delegate ()
                {
                    results2.Items.Add("no Results");
                    results2.Refresh();
                }));
            }
            else if (seresults[1].ToString().Contains("Value cannot be null") || seresults[1].ToString().Trim().Contains("index was outside the bounds of the array"))
            {
                results2.Invoke((MethodInvoker)(delegate ()
                {
                    results2.Items.Add("no Results.");
                    results2.Refresh();
                }));

                errCount2++;
                if (errCount2 >= 10)
                {
                    cnt = maxCnt;
                    IPChanger();
                }
            }
            else
            {
                kwcnt1++;
                results2.Invoke((MethodInvoker)(delegate ()
                {
                    if (kwcnt2 >= 10)
                    {
                        IPChanger();
                        kwcnt2 = 0;
                    }
                }));

                results2.Invoke((MethodInvoker)(delegate ()
                {
                    results2.Items.Add(seid + " " + kn);
                    label2.Text = "No. of URLs : " + (seresults[1]);

                }));
            }

            if (myDate != "")
            {
                try
                {
                    int rescount2 = int.Parse(seresults[1]);
                    if (rescount2 > 20)
                    {
                        SendToAPI2(seid, kn, seresults[0]);
                        SendToDB(seid, kn, seresults[0], int.Parse(seresults[1]));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        public void processResults12(string seid, string kn)
        {
            IPChanger();
            string[] seresults = new string[1];
            date_picker.Format = DateTimePickerFormat.Custom;
            date_picker.CustomFormat = "yyyy-MM-dd";
            string myDate = date_picker.Text;

            try
            {
                seresults = server0.GetTop100(kn, int.Parse(seid));
            }
            catch (WebException ex)
            {
                results3.Invoke((MethodInvoker)(delegate ()
                {
                    results3.Items.Add(ex.Message);
                    results3.Refresh();
                }));
                return;
            }

            results3.Invoke((MethodInvoker)(delegate ()
            {
                results3.Items.Clear();
            }));
            if (string.IsNullOrEmpty(seresults[0]) || seresults[0].Trim().StartsWith("Index was outside the bounds of the array"))
            {
                results3.Invoke((MethodInvoker)(delegate ()
                {
                    results3.Items.Add("no result.");
                    results3.Refresh();
                }));

                errCount3++;
                if (errCount3 >= 10)
                {
                    cnt = maxCnt;
                    IPChanger();
                }
            }

            if (int.Parse(seresults[1]) < 1)
            {
                results3.Invoke((MethodInvoker)(delegate ()
                {
                    results3.Items.Add("no Results");
                    results3.Refresh();
                }));
            }
            else if (seresults[1].ToString().Contains("Value cannot be null") || seresults[1].ToString().Trim().Contains("index was outside the bounds of the array"))
            {
                results3.Invoke((MethodInvoker)(delegate ()
                {
                    results3.Items.Add("no Results.");
                    results3.Refresh();
                }));

                errCount3++;
                if (errCount3 >= 10)
                {
                    cnt = maxCnt;
                    IPChanger();
                }
            }
            else
            {
                kwcnt3++;
                results3.Invoke((MethodInvoker)(delegate ()
                {
                    if (kwcnt3 >= 10)
                    {
                        IPChanger();
                        kwcnt3 = 0;
                    }
                }));

                results3.Invoke((MethodInvoker)(delegate ()
                {
                    results3.Items.Add(seid + " " + kn);
                    label3.Text = "No. of URLs : " + (seresults[1]);
                }));


                if (myDate != "")
                {
                    try
                    {
                        int rescount3 = int.Parse(seresults[1]);
                        if (rescount3 > 20)
                        {
                            SendToAPI3(seid, kn, seresults[0]);
                            SendToDB(seid, kn, seresults[0], int.Parse(seresults[1]));
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        public void processWorklist2()
        {
            string resultsString;
            char sep;
            Array resultsArray;
            string seid;
            string kn;

            try
            {
                for (int i = 0; i < worklist1.Items.Count; i++)
                {
                    // get next Project ID, Search Engine ID, Keyword ID and Keyword from worklist
                    resultsString = worklist1.Items[i].ToString();
                    sep = ':';
                    resultsArray = resultsString.Split(sep);
                    seid = resultsArray.GetValue(0).ToString();
                    kn = resultsArray.GetValue(1).ToString();
                    try
                    {
                        processResults2(seid, kn);
                    }
                    catch (Exception ex)
                    {
                        errorList.Invoke((MethodInvoker)(delegate ()
                        {
                            errorList.Items.Add(ex.Message.ToString());
                        }));
                    }

                    // update progress label
                    progress_seid2.Invoke((MethodInvoker)(delegate ()
                    {
                        progress_seid2.Text = "Completed : " + (i + 1) + " of " + worklist1.Items.Count;
                        results1.Refresh();
                        progress_seid2.Refresh();
                    }));

                }
            }
            catch (Exception ex)
            {
                string errorMsg = ex.ToString();
                errorList.Invoke((MethodInvoker)(delegate ()
                {
                    errorList.Items.Add(errorMsg);
                }));
            }
        }

        public void processWorklist6()
        {
            string resultsString;
            char sep;
            Array resultsArray;
            string seid;
            string kn;
            try
            {
                for (int i = 0; i < worklist2.Items.Count; i++)
                {
                    // get next Project ID, Search Engine ID, Keyword ID and Keyword from worklist
                    resultsString = worklist2.Items[i].ToString();
                    sep = ':';
                    resultsArray = resultsString.Split(sep);
                    seid = resultsArray.GetValue(0).ToString();
                    kn = resultsArray.GetValue(1).ToString();

                    try
                    {
                        processResults6(seid, kn);
                    }
                    catch (Exception ex)
                    {
                        errorList.Invoke((MethodInvoker)(delegate ()
                        {
                            errorList.Items.Add(ex.Message.ToString());
                        }));
                    }

                    // update progress label
                    progress_seid6.Invoke((MethodInvoker)(delegate ()
                    {
                        progress_seid6.Text = "Completed : " + (i + 1) + " of " + worklist2.Items.Count;
                        results2.Refresh();
                        progress_seid6.Refresh();
                    }));

                }
            }
            catch (Exception ex)
            {
                string errorMsg = ex.ToString();
                errorList.Invoke((MethodInvoker)(delegate ()
                {
                    errorList.Items.Add(errorMsg);
                }));
            }
        }

        public void processWorklist12()
        {
            string resultsString;
            char sep;
            Array resultsArray;
            string seid;
            string kn;
            try
            {
                for (int i = 0; i < worklist3.Items.Count; i++)
                {
                    // get next Project ID, Search Engine ID, Keyword ID and Keyword from worklist
                    resultsString = worklist3.Items[i].ToString();
                    sep = ':';
                    resultsArray = resultsString.Split(sep);
                    seid = resultsArray.GetValue(0).ToString();
                    kn = resultsArray.GetValue(1).ToString();

                    try
                    {
                        processResults12(seid, kn);
                    }
                    catch (Exception ex)
                    {
                        errorList.Invoke((MethodInvoker)(delegate ()
                        {
                            errorList.Items.Add(ex.Message.ToString());
                        }));
                    }
                    progress_seid12.Invoke((MethodInvoker)(delegate ()
                    {
                        progress_seid12.Text = "Completed : " + (i + 1) + " of " + worklist3.Items.Count;
                        results3.Refresh();
                        progress_seid12.Refresh();
                    }));
                }
            }
            catch (Exception ex)
            {
                string errorMsg = ex.ToString();
                errorList.Invoke((MethodInvoker)(delegate ()
                {
                    errorList.Items.Add(errorMsg);
                }));
            }
        }

        public void mainLoop2()
        {
            //generateWorklist2();
            while (getWorklistSize2() > 0)
            {
                processWorklist2();
                generateWorklist2();
            }
        }

        public void mainLoop6()
        {
            //generateWorklist6();
            while (getWorklistSize6() > 0)
            {
                processWorklist6();
                generateWorklist6();
            }
        }

        public void mainLoop12()
        {
            //generateWorklist12();
            while (getWorklistSize12() > 0)
            {
                processWorklist12();
                generateWorklist12();
            }
        }


        //[STAThread]
        //static void Main()
        //{
        //    Application.Run(new Form1());
        //}

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)(delegate ()
            {
                date_picker.Value = DateTime.Today;
            }));

            liveurl = readAPI();
            if (File.Exists("index.txt"))
            {
                StreamReader sw = new StreamReader("index.txt");
                string val = sw.ReadLine();
                sw.Close();

                if (Convert.ToInt32(val) >= 0) server0.x = Convert.ToInt32(val);
            }


            if (server0.x >= server0.dtIPs.Rows.Count)
            {
                server0.x = 0;
            }

            this.Invoke((MethodInvoker)(delegate ()
            {
                this.Text = "D_TrackingTrending_(1-2-3)_Server-1_" + server0.dtIPs.Rows[server0.x][1].ToString();
                
            }));

            Thread myThread2 = new Thread(new ThreadStart(mainLoop2));
            generateWorklist2();

            if (getWorklistSize2() > 0)
            {
                myThread2.Start();
            }
            else
            {
                myThread2.Abort();
            }

            Thread myThread6 = new Thread(new ThreadStart(mainLoop6));
            generateWorklist6();
            if (getWorklistSize6() > 0)
            {
                myThread6.Start();
            }
            else
            {
                myThread6.Abort();
            }

            Thread myThread12 = new Thread(new ThreadStart(mainLoop12));
            generateWorklist12();
            if (getWorklistSize12() > 0)
            {
                myThread12.Start();
            }
            else
            {
                myThread12.Abort();
            }
        }

        private void SendToAPI1(string seid, string kw, string res)
        {
            XmlDocument xd = new XmlDocument();
            res = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + res;
            xd.LoadXml(res);
            xd.Save(xmlPath1);
          
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
                //httpWReq.Timeout = 0;

                Stream stream = httpWReq.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                string s = response.ToString();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                //System.Threading.Thread.Sleep(2000);
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
                string error = "";
                string message = "";

                using (WebResponse response = ex.Response)
                {
                    if (response != null)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)response;
                        error = string.Format("Error:{0}", httpResponse.StatusCode);

                        using (Stream data = response.GetResponseStream())
                        using (var reader = new StreamReader(data))
                        {
                            message = reader.ReadToEnd();
                        }
                    }
                }

                this.Invoke((MethodInvoker)delegate ()
                {
                    txtError.Text += ex.Message + "\r\n";
                });
              
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        private void SendToAPI2(string seid, string kw, string res)
        {
            XmlDocument xd = new XmlDocument();
            res = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + res;
            xd.LoadXml(res);
            xd.Save(xmlPath2);

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
                string s = response.ToString();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                //System.Threading.Thread.Sleep(2000);
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
                string error = "";
                string message = "";

                using (WebResponse response = ex.Response)
                {
                    if (response != null)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)response;
                        error = string.Format("Error:{0}", httpResponse.StatusCode);

                        using (Stream data = response.GetResponseStream())
                        using (var reader = new StreamReader(data))
                        {
                            message = reader.ReadToEnd();
                        }
                    }
                }

                this.Invoke((MethodInvoker)delegate ()
                {
                    txtError.Text += ex.Message + "\r\n";
                });
               
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }

        }
        private void SendToAPI3(string seid, string kw, string res)
        {
            XmlDocument xd = new XmlDocument();
            res = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + res;
            xd.LoadXml(res);
            xd.Save(xmlPath3);
       
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
                //System.Threading.Thread.Sleep(2000);

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
                string error = "";
                string message = "";

                using (WebResponse response = ex.Response)
                {
                    if (response != null)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)response;
                        error = string.Format("Error:{0}", httpResponse.StatusCode);

                        using (Stream data = response.GetResponseStream())
                        using (var reader = new StreamReader(data))
                        {
                            message = reader.ReadToEnd();
                        }
                    }
                }
                this.Invoke((MethodInvoker)delegate ()
                {
                    txtError.Text += ex.Message + "\r\n";
                });

            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }

        }
        private void SendToDB(string seid, string keyword, string xml, int urlcount)
        {
            try
            {
                string myDate = DateTime.Today.ToString("yyyy-MM-dd");
                using (SqlConnection con = new SqlConnection(Common.ReadConnection()))
                {
                    con.Open();
                    using (SqlCommand comm = con.CreateCommand())
                    {
                        comm.CommandTimeout = 0;
                        comm.CommandType = CommandType.StoredProcedure;
                        comm.CommandText = "Insert_Dashboard_Data";
                        comm.Parameters.Add("date", SqlDbType.DateTime).Value = myDate;
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
                this.Invoke((MethodInvoker)delegate ()
                {
                    txtError.Text += ex.Message + "\r\n";
                });
            }
            finally { }
        }


        private string GetTextFromXMLFile(string file)
        {
            StreamReader reader = new StreamReader(file);
            string ret = reader.ReadToEnd();
            reader.Close();
            return ret;
        }

        private void process_btn_Click(object sender, EventArgs e)
        {
            Thread myThread2 = new Thread(new ThreadStart(mainLoop2));
            generateWorklist2();

            if (getWorklistSize2() > 0)
            {
                myThread2.Start();
            }
            else
            {
                myThread2.Abort();
            }

            Thread myThread6 = new Thread(new ThreadStart(mainLoop6));
            generateWorklist6();
            if (getWorklistSize6() > 0)
            {
                myThread6.Start();
            }
            else
            {
                myThread6.Abort();
            }
            Thread myThread12 = new Thread(new ThreadStart(mainLoop12));

            generateWorklist12();
            if (getWorklistSize12() > 0)
            {
                myThread12.Start();
            }
            else
            {
                myThread12.Abort();
            }
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

    }
}
