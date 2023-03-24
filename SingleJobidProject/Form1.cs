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

namespace SingleJobidProject
{
    public partial class Form1 : Form
    {
        string xmlPath = "C:\\inetpub\\wwwroot\\NewKeywords_Jobid_GT0.xml";//changes

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        //int count; 

        public Form1()
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

       
        private void Form1_Load(object sender, EventArgs e)
        {

            this.Text = "TrackingData_JobID_Keywords"; //changes
          
            //string kwQry = "Tracking_DB_Keywords_SEID_102_TGBN '" + myDate + "'";

            Thread t = new Thread(new ThreadStart(StartProcess));
            t.SetApartmentState(ApartmentState.STA);
            t.Priority = ThreadPriority.Lowest;
            t.Start();
        }
        public enum stats
        {
            Faulted,
            Pending,
            Empty,
            statuscode

        }
        private void StartProcess()
        {
            while (true)
            {
                string myDate = DateTime.Today.ToString("yyyy-MM-dd");
                //string myDate = "2019-11-20";

                string kwQry = "[GetMissingJobidKeywords] '" + myDate + "'"; // 01-09-2020

                GetKeywords(kwQry);

                if (lstKws.Items.Count <= 0)
                    break;

                int cnt = 0;                
                foreach (string s in lstKws.Items)
                {
                    //26-10-2020 changed character as ':'
                    string seid = s.Split(':')[0];
                    string kw = s.Split(':')[1];
                    string jobid = s.Split(':')[2];
                    //end of 26-10-2020 changed character as ':'


                    bool result = false;
                    try
                    {
                        var doc = new HtmlAgilityPack.HtmlDocument();
                        Task<ArrayList> alresult = GetHTML(kw, Convert.ToInt32(seid),jobid);
                        if (alresult.Status.ToString() == stats.Faulted.ToString() || alresult.Status.ToString()==stats.Pending.ToString() || alresult.Status.ToString() == stats.Empty.ToString() || alresult.Status.ToString() == stats.statuscode.ToString()) //07-02-2022//31-01-2022//04-01-2022
                            throw alresult.Exception.InnerException;//04-01-2022
                        foreach (string[] src in alresult.Result)
                        {
                            string keyword = src[0];
                            JObject obj = JObject.Parse(src[1]);
                            string html = obj["results"][0]["content"].Value<string>();                            
                            string device = src[3];                            
                            result = true;
                            doc = new HtmlAgilityPack.HtmlDocument();
                            doc.LoadHtml(html);
                            //File.WriteAllText(@"C:\inetpub\wwwroot\html\" + jobid + "_" + keyword + ".html", html, Encoding.UTF8);
                            string res = string.Empty;
                            int count = 0;
                            try
                            {
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
                                
                                if (!string.IsNullOrEmpty(res))
                                {
                                    lblCount.Invoke((MethodInvoker)(delegate ()
                                    {
                                        lblCount.Text = "No. of Urls : " + count;
                                    }));

                                    if (count > 20)
                                    {
                                        SendToDB(seid, keyword, res, jobid, count);
                                    }
                                    if (count < 21 && count == 0)
                                    {
                                        SendToDB(seid, keyword, res, jobid, count);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                try //change 03-01-2022
                                {
                                    bool isOldPage = false;
                                    if (ex.Message == "Old page found.")
                                        isOldPage = true;
                                    this.Invoke((MethodInvoker)delegate ()
                                    {
                                        txtError.Text = txtError.Text + seid + ": " + kw + ": " + jobid + Environment.NewLine + ex.Message.ToString() +
                                            Environment.NewLine + Environment.NewLine;
                                        txtError.Refresh();
                                    });
                                    SendToDBFailure(seid, kw, jobid, isOldPage, ex.Message);
                                }
                                finally { }//end 03-01-2022
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        this.Invoke((MethodInvoker)delegate () //change 03-01-2022
                        {
                            txtError.Text = txtError.Text + seid + ": " + kw + ": " + jobid + Environment.NewLine + ex.Message.ToString() +
                                Environment.NewLine + Environment.NewLine;
                            txtError.Refresh();
                        });
                        SendToDBFailure(seid, kw, jobid, false, ex.Message); //end 03-01-2022
                    }
                    finally { }
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        if (result)
                        {
                            textBox1.Text = s;
                            //textBox1.Refresh();
                            label1.Text = ++cnt + " of " + lstKws.Items.Count + " Completed";
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


         private void GetKeywords(string qry)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                lstKws.Items.Clear();
                //lstKWs.Items.Add("58:praivat medicashe insh");
                //lstKWs.Items.Add("106:st.vincent discography");
                //lstKWs.Items.Add("106:romeo and juliet tickets");
                //lstKWs.Items.Add("160:malmö ff");
                //lstKWs.Items.Add("102:terry crews");
                //lstKWs.Items.Add("102:the uninhabitable earth summary");
                //lstKWs.Items.Add("1:rhubarbarone");
                //lstKws.Items.Add("58:best tennis rackets:6947353729158886401");
                //coronavirus rd case	140	6672286477201717249

            });
            //return;

            try
            {
                using (SqlConnection con = new SqlConnection(strConn()))  // 12-05-2020
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
                                    lstKws.Items.Add(dr.GetValue(0) + ":" + dr.GetValue(1) + ":" + dr.GetValue(2)); //26-10-2020 applied ":"
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

        private string GetTextFromXMLFile(string file)
        {
            StreamReader reader = new StreamReader(file);
            string ret = reader.ReadToEnd();
            reader.Close();
            return ret;
        }

        public string ReadAPI()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                //string fileName = @"C:\Inetpub\wwwroot\TrackingData.xml"; //Azure Database
                string fileName = @"C:\Inetpub\wwwroot\TrackingDataFirstServer.xml"; //First Server

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

            using (SqlConnection con = new SqlConnection(strConn()))
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
        public string strConn()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                string fileName = @"C:\Inetpub\wwwroot\TrackingData.xml";

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
        private void SendToDB(string seid, string keyword, string xml, string jobid, int urlcount)
        {
            try
            {
                string myDate = DateTime.Today.ToString("yyyy-MM-dd");               

                using (SqlConnection con = new SqlConnection(strConn()))  // 12-05-2020
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

        public async Task<ArrayList> GetHTML(string keyword, int seid,string jobid)
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

        async Task<ArrayList> GetOxylabsWebDataSources(SearchProperties sp,string jobid)
        {
            JObject obj = null;//07-02-2022
            string username = "gpidatametrics";
            string password = "sdV5X3fcX6";
            string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));
            string[] keyword = { sp.query };                 
            string response;        
            ArrayList lst = new ArrayList();            
                string kw = sp.query;
                string href ="" ;
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
                    Uri uri = new Uri("http://data.oxylabs.io/v1/queries/"+jobid+"/results");
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

    }

}
