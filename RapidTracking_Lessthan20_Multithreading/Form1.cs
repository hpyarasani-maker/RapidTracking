using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TrackingTrending
{
    public partial class Form1 : Form
    {

        string xmlPath1 = @"C:\inetpub\wwwroot\trackingtrending_lt20_1.xml";
        string xmlPath2 = @"C:\inetpub\wwwroot\trackingtrending_lt20_2.xml";
        string xmlPath3 = @"C:\inetpub\wwwroot\trackingtrending_lt20_3.xml";

        //string xmlPath1 = @"C:\inetpub\wwwroot\trackingtrending_remaining_1.xml";
        //string xmlPath2 = @"C:\inetpub\wwwroot\trackingtrending_remaining_2.xml";
        //string xmlPath3 = @"C:\inetpub\wwwroot\trackingtrending_remaining_3.xml";

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                
        bool process1 = false;
        bool process2 = false;
        bool process3 = false;

        int id1 = 0;
        int id2 = 0;
        int id3 = 0;
        string myDate;
        public Form1()
        {
            InitializeComponent();
            timerExit();
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

        private void Form1_Load_1(object sender, EventArgs e)
        {
            this.Text = "TrackingTrending Lessthan20_(1-2-3)";

            dtPicker1.Value = DateTime.Today;
            myDate = dtPicker1.Value.ToString("yyyy-MM-dd");

            Thread t1 = new Thread(new ThreadStart(StartProcess_1));
            t1.SetApartmentState(ApartmentState.STA);
            t1.Start();

            Thread t2 = new Thread(new ThreadStart(StartProcess_2));
            t2.SetApartmentState(ApartmentState.STA);
            t2.Start();

            Thread t3 = new Thread(new ThreadStart(StartProcess_3));
            t3.SetApartmentState(ApartmentState.STA);
            t3.Start();
        }

        private void StartProcess_1()
        {
            int id1 = 0;
            while (true)
            {
                //string myDate = DateTime.Today.ToString("yyyy-MM-dd");
                string kwQry = "GetLessthan20Keywords_1 '" + myDate + "'";
                //string kwQry = "GetAllKeywords_1 '" + myDate + "'";     
                id1++;
                GetKeywords1(kwQry);
                //14-10-2020
                this.Invoke((MethodInvoker)delegate ()
                {
                    SendToDBTable(id1, "Thread1", lstKWs.Items.Count);
                });
                //end 14-10-2020
                if (lstKWs.Items.Count <= 0 )
                    break;

                int cnt = 0; 
                foreach (string s in lstKWs.Items)
                {
                    string seid = s.Split(':')[0];
                    string kw = s.Split(':')[1];

                    try
                    {
                        var doc = new HtmlAgilityPack.HtmlDocument(); 
                        ArrayList alresult = getHTML(kw, Convert.ToInt32(seid));
                        foreach (string[] src in alresult)
                        { 
                            string keyword = src[0];
                            JObject obj = JObject.Parse(src[1]);
                            string html = obj["results"][0]["content"].Value<string>();
                            string jobid = src[2];
                            string device = src[3];
                            doc = new HtmlAgilityPack.HtmlDocument();
                            doc.LoadHtml(html);
                            string res = string.Empty;
                            int count = 0;

                            if (device == "desktop")
                            {
                                Desktop clsDesktop = new Desktop();
                                res = clsDesktop.ProcessDocument(seid, keyword, doc, out count);
                            }
                            else
                            {
                                iOS clsiOS = new iOS();
                                res = clsiOS.ProcessDocument(seid, keyword, doc, out count);
                            }
                            this.Invoke((MethodInvoker)delegate () { //13-05-2020
                                lblcount1.Text = "Count: " + count;
                            });
                            if (!string.IsNullOrEmpty(res))
                            {
                                if (count > 20)
                                {
                                    SendToAPI1(seid, keyword, res, jobid);
                                    SendToDB(seid, keyword,res, jobid, count);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Invoke((MethodInvoker)delegate ()
                        {
                            txtError.Text = ex.Message.ToString();
                            //string errorDesk = ex.Message.ToString() + seid + "=" + kw + Environment.NewLine;
                            //System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\errorDesk.txt", errorDesk);
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

            process1 = true;
            if (process1 && process2 && process3)
                Environment.Exit(Environment.ExitCode);
        }

        private void StartProcess_2()
        {
            int id2 = 0;
            while (true)
            {
                //string myDate = DateTime.Today.ToString("yyyy-MM-dd");
                string kwQry = "GetLessthan20Keywords_2 '" + myDate + "'";
                //string kwQry = "GetAllKeywords_2 '" + myDate + "'";     
                id2++;
                GetKeywords2(kwQry);
                //14-10-2020
                this.Invoke((MethodInvoker)delegate ()//13-10-2020
                {
                    SendToDBTable(id2, "Thread2", lstKWs2.Items.Count);
                });
                //end 14-10-2020
                if (lstKWs2.Items.Count <= 0)
                    break;

                int cnt = 0;
                foreach (string s in lstKWs2.Items)
                {
                    string seid = s.Split(':')[0];
                    string kw = s.Split(':')[1];

                    try
                    {
                        var doc = new HtmlAgilityPack.HtmlDocument();
                        ArrayList alresult = getHTML(kw, Convert.ToInt32(seid));
                        foreach (string[] src in alresult)
                        {
                            string keyword = src[0];
                            string html = src[1].Replace(@"\", "");
                            string jobid = src[2];
                            string device = src[3];

                            doc = new HtmlAgilityPack.HtmlDocument();
                            doc.LoadHtml(html);
                            string res = string.Empty;
                            int count = 0;
                            if (device == "desktop")
                            {
                                Desktop clsDesktop = new Desktop();
                                res = clsDesktop.ProcessDocument(seid, keyword, doc, out count);
                            }
                            else
                            {
                                iOS clsiOS = new iOS();
                                res = clsiOS.ProcessDocument(seid, keyword, doc, out count);
                            }
                            this.Invoke((MethodInvoker)delegate () { //13-05-2020
                                lblcount2.Text = "Count: " + count;
                            });
                            if (!string.IsNullOrEmpty(res))
                            {
                                if (count > 20)
                                {
                                    SendToAPI2(seid, keyword, res, jobid);
                                    SendToDB(seid, keyword,res, jobid, count);
                                }                                
                            }                           
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Invoke((MethodInvoker)delegate ()
                        {
                            txtError.Text = ex.Message.ToString();
                            //string errorDesk = ex.Message.ToString() + seid + "=" + kw + Environment.NewLine;
                            //System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\errorDesk.txt", errorDesk);
                        });
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

        private void StartProcess_3()
        {
            int id3 = 0;
            while (true)
            {
                //string myDate = DateTime.Today.ToString("yyyy-MM-dd");
                string kwQry = "GetLessthan20Keywords_3 '" + myDate + "'";
                //string kwQry = "GetAllKeywords_3 '" + myDate + "'";     
                id3++;
                GetKeywords3(kwQry);
                //14-10-2020
                this.Invoke((MethodInvoker)delegate ()//13-10-2020
                {
                    SendToDBTable(id3, "Thread3", lstKWs3.Items.Count);
                });
                //end 14-10-2020
                if (lstKWs3.Items.Count <= 0)
                    break;

                int cnt = 0;
                foreach (string s in lstKWs3.Items)
                {
                    string seid = s.Split(':')[0];
                    string kw = s.Split(':')[1];

                    try
                    {
                        var doc = new HtmlAgilityPack.HtmlDocument();
                        ArrayList alresult = getHTML(kw, Convert.ToInt32(seid)); 
                        foreach (string[] src in alresult)
                        {
                            string keyword = src[0];
                            string html = src[1].Replace(@"\", "");
                            string jobid = src[2];
                            string device = src[3];

                            doc = new HtmlAgilityPack.HtmlDocument();
                            doc.LoadHtml(html);
                            string res = string.Empty;
                            int count = 0;
                            if (device == "desktop")
                            {
                                Desktop clsDesktop = new Desktop();
                                res = clsDesktop.ProcessDocument(seid, keyword, doc, out count);
                            }
                            else
                            {
                                iOS clsiOS = new iOS();
                                res = clsiOS.ProcessDocument(seid, keyword, doc, out count);
                            }
                            this.Invoke((MethodInvoker)delegate () { //13-05-2020
                                lblcount3.Text = "Count: " + count;
                            });
                            if (!string.IsNullOrEmpty(res))
                            {
                                if (count > 20)
                                {
                                    SendToAPI3(seid, keyword, res, jobid);
                                    SendToDB(seid, keyword,res, jobid, count);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Invoke((MethodInvoker)delegate ()
                        {
                            txtError.Text = ex.Message.ToString();
                            //string errorDesk = ex.Message.ToString() + seid + "=" + kw + Environment.NewLine;
                            //System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\errorDesk.txt", errorDesk);
                        });
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

        private void SendToAPI1(string seid, string kw, string res, string jobid)
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
                SendToDBFailure(seid, kw, jobid);


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
                SendToDBFailure(seid, kw, jobid);


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
                SendToDBFailure(seid, kw, jobid);


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

        //14-10-2020 New method to updated number of threads repeating and count the keywords
        private void SendToDBTable(int id, string threadname, int threadcount)//13-10-2020
        {

            try
            {
                String query = "";
                string myDate = DateTime.Today.ToString("yyyy-MM-dd");

                using (SqlConnection con = new SqlConnection(Common.ReadConnection()))
                {
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    SqlCommand comm = new SqlCommand("Select count(*) from Lessthen20Table_Threads where id= @id and date=@date", con);
                    comm.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    comm.Parameters.Add("@date", SqlDbType.DateTime).Value = myDate;
                    var result = comm.ExecuteScalar();
                    int count = int.Parse(result.ToString());
                    if (count > 0)
                    {
                        query = "UPDATE Lessthen20Table_Threads SET " + threadname + "=" + threadcount + " Where id=" + id + " and date='" + myDate + "' ";
                        comm = new SqlCommand(query, con);
                        comm.ExecuteNonQuery();
                    }
                    else
                    {
                        query = "INSERT INTO dbo.Lessthen20Table_Threads (Date,id," + threadname + ") VALUES (@date,@id,@tcount)";
                        comm = new SqlCommand(query, con);
                        comm.Parameters.Add("@date", SqlDbType.DateTime).Value = myDate;
                        comm.Parameters.Add("@id", SqlDbType.Int).Value = id;
                        comm.Parameters.Add("@tcount", SqlDbType.Int).Value = threadcount;
                        comm.ExecuteNonQuery();
                    }
                    if (con.State != ConnectionState.Closed)
                        con.Close();
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
        //end 14-10-2020
        private void SendToDBFailure(string seid, string kw, string jobid)
        {
            //string myDate = DateTime.Today.ToString("yyyy-MM-dd");
            string qry = "insert into dashboard_dataerrors (date, name, seid, jobid) values(Convert(varchar(10),'" + myDate + "',103), N'" +
                  kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "' )";

            using (SqlConnection con = new SqlConnection(Common.ReadConnection()))
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

                    //throw new Exception(errorMessage);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
            }            
        }

        private void SendToDB(string seid, string keyword,string xml, string jobid, int urlcount)
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

                        comm.CommandType = CommandType.Text;
                        comm.CommandText = "Insert into dashboard_data2 (date, name, seid, jobid, count) values(Convert(varchar(10),'" + myDate + "',103), N'" +
                                            keyword.Replace("'", "''") + "', " + seid + ", '" + jobid + "', " + urlcount + " )";
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
            this.Invoke((MethodInvoker)delegate ()
            {
                lstKWs.Items.Clear();                

                //lstKWs.Items.Add("106:dyson");
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
                                    lstKWs.Items.Add(dr.GetValue(0) + ":" + dr.GetValue(1));
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

        private void GetKeywords2(string qry)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                lstKWs2.Items.Clear();

                //lstKWs2.Items.Add("106:harrods");
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
                                    lstKWs2.Items.Add(dr.GetValue(0) + ":" + dr.GetValue(1));
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

        private void GetKeywords3(string qry)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                lstKWs3.Items.Clear();

                //lstKWs3.Items.Add("67:borussia dortmund");
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
                                    lstKWs3.Items.Add(dr.GetValue(0) + ":" + dr.GetValue(1));
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

        public ArrayList getHTML(string keyword, int seid)
        {
            ArrayList alResult = new ArrayList();
            try
            {
                SearchProperties sp = SearchParams.searches.Where(s => s.seid == seid).SingleOrDefault();
                sp.query = keyword;
                if (sp != null)
                    alResult = GetOxylabsWebDataSources(sp);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return alResult;
        }

        ArrayList GetOxylabsWebDataSources(SearchProperties sp)
        {
            Uri queryUri = new Uri("https://data.oxylabs.io/v1/queries/batch");
            string username = "gpidatametrics";
            string password = "sdV5X3fcX6";
            string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));
            string[] kwd = { sp.query };  //13-05-2020

            OxyParams op = new OxyParams()
            {
                source = "google_search",
                domain = sp.domain,
                //query = sp.query.Split(','),
                query=kwd,   //13-05-2020
                limit = 100,
                pages = 1,
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
                HttpWebResponse res = (HttpWebResponse) req.GetResponse();
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
                            HttpWebResponse res = (HttpWebResponse) httpWebRequest.GetResponse();

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
                            HttpWebResponse res = (HttpWebResponse) httpWebRequest.GetResponse();

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

            return alResult;
        }
        
    }
}
