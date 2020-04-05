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

        ArrayList seresults = new ArrayList();

        string xmlPath = "C:\\inetpub\\wwwroot\\Remaining_103_WC_Proxies.xml";


        public string myDate = string.Empty;
        string statusCode = string.Empty;

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        void timerExit()
        {
            timer.Interval = 30 * 60000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            Environment.Exit(Environment.ExitCode);
        }
        public Form1()
        {
            InitializeComponent();
        }


        public string strConn()
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

        public void generateWorklist()
        {
            worklist.Invoke((MethodInvoker)(delegate ()
            {
                worklist.Items.Clear();
                //worklist.Items.Add("106:what time is it in uk");
            }));
            Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            date_picker.Format = DateTimePickerFormat.Custom;
            date_picker.CustomFormat = "yyyy-MM-dd";
            string myDate = date_picker.Text;
            //return;
            string strSql = "exec [dbo].[Tracking_DB_Keywords_SEID_102] '" + myDate + "'";//changes        

            SqlConnection objCon = null;
            SqlDataReader objData = null;
            try
            {
                objCon = new SqlConnection(strConn());
                objCon.Open();
                SqlCommand objCmd = new SqlCommand(strSql, objCon);
                objCmd.CommandTimeout = 0;
                objData = objCmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (objData.Read())
                {
                    worklist.Invoke((MethodInvoker)(delegate ()
                    {
                        worklist.Items.Add(objData[0].ToString() + ":" + objData[1].ToString());
                    }));
                }

                worklist.Invoke((MethodInvoker)(delegate ()
                {
                    worklist.Refresh();
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
                objCon.Dispose();
                objCon.Close();
            }
        }
        public int getWorklistSize()
        {
            int worklistSize = worklist.Items.Count;
            return worklistSize;
        }
        public string readAPI()
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

        public void processResults(string seid, string kn)
        {
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
            catch (Exception ex)
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

            if (int.Parse(seresults[1]) < 1)
            {
                results.Invoke((MethodInvoker)(delegate ()
                {
                    results.Items.Add("No Results");
                    results.Refresh();
                }));

            }
            else if (seresults[0].ToString().Contains("e100") && seresults[0].ToString().Trim().StartsWith("e100"))
            {
                results.Invoke((MethodInvoker)(delegate ()
                {
                    results.Items.Add("e100: no result.");
                    results.Refresh();
                }));
                errorList.Invoke((MethodInvoker)(delegate ()
                {
                    errorList.Text += seresults[0].ToString() + "\r\n";
                    errorList.Refresh();
                }));
            }
            else
            {
                results.Invoke((MethodInvoker)(delegate ()
                {
                    results.Items.Add(seid + " " + kn);
                    label1.Text = "Item URLs Count : " + (seresults[1]);
                    //results.Refresh();
                }));
            }

            if (!string.IsNullOrEmpty(seresults[0]))
            {
                StreamWriter sw = new StreamWriter(xmlPath, false);
                sw.Write(seresults[0]); //updated to string array [0] //20-01-2020
                sw.Close();
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
                    //else
                    //{
                    //    SendToAPI(seid, kn, seresults[0]);
                    //    SendToDB(seid, kn, seresults[0], int.Parse(seresults[1]));
                    //}
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
        public void processWorklist()
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
                    processResults(seid, kn);
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
                    errorList.Items.Add(ex.ToString());
                }));

            }
        }

        public void mainLoop()
        {

            generateWorklist();
            while (getWorklistSize() > 0)
            {
                processWorklist();
                generateWorklist();
            }
            Environment.Exit(Environment.ExitCode);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)(delegate ()
            {
                //date_picker.Value = DateTime.Today.AddDays(-2);

                //this.Text = "D_RapidTracking_102_Remaining_Proxies_GT0";//changes
                this.Text = "D_RapidTracking_103_WOC_Remaining_Proxies";
            }));
            Thread myThread = new Thread(new ThreadStart(mainLoop));
            generateWorklist();
            if (getWorklistSize() > 0)
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
