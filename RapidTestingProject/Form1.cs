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
using Newtonsoft.Json.Linq;
using RapidTrackingLibrary;


namespace RapidTestingProject
{
    public partial class Form1 : Form
    {
        string xmlPath = "C:\\inetpub\\wwwroot\\rapidtracking_singlethread_102_GT20_WC.xml";

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        public Form1()
        {
            InitializeComponent();
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
            this.Text = "RapidTracking_SingleThread_102_GT20_WC";
            //this.Text = "RapidTracking_SingleThread_P_A_WOC_10-09-2019";


            Thread t = new Thread(new ThreadStart(StartProcess));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }
        private void StartProcess()
        {
            while (true)
            {
                string myDate = DateTime.Today.ToString("yyyy-MM-dd");
                //string myDate = "2019-11-20";


                string kwQry = "[Tracking_DB_Keywords_Seid_102] '" + myDate + "'";
                //string kwQry = "[Tracking_DB_Keywords_Seid_103p] '" + myDate + "'";               
                //string kwQry = "[GetCommaKeywordsP] '" + myDate + "'";               

                GetKeywords(kwQry);

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
                            File.WriteAllText(@"C:\inetpub\wwwroot\html\" + jobid + "_" + keyword + ".html", html, Encoding.UTF8);
                            //File.WriteAllText(@"C:\inetpub\wwwroot\"+jobid+"_withOut filter_"+".html", html, Encoding.UTF8);
                            result = true;
                            doc = new HtmlAgilityPack.HtmlDocument();
                            doc.LoadHtml(html);
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
                                    if (count > 20)
                                    {
                                        SendToAPI(seid, keyword, res, jobid);
                                        SendToDB(seid, keyword, res, jobid, count);
                                    }
                                }
                               

                            }
                            catch (Exception ex)
                            {
                                try
                                {
                                    bool isOldPage = false;
                                    if (ex.Message == "Old page found.")
                                        isOldPage = true;
                                    SendToDBFailure(kw, seid, jobid, isOldPage);
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
                            textBox1.Text = s + "  -- No result.";
                        textBox1.Refresh();
                    });

                }

            }

            Environment.Exit(Environment.ExitCode);
        }


        private void SendToAPI(string seid, string kw, string res, string jobid)
        {
           
            XmlDocument xd = new XmlDocument();
            res = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + res;
            xd.LoadXml(res);
            xd.Save(xmlPath);

            //}
            //SendToURL


            string submitURL = ReadAPI();

           

        }

        private void GetKeywords(string qry)
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
                lstKWs.Items.Add("139:donald trump");
            });
            return;

            try
            {
                using (SqlConnection con = new SqlConnection(RapidTrackingLibrary.Common.ReadConnection()))  // 12-05-2020
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
        private void SendToDBFailure(string seid, string kw, string jobid, bool isOldPage)
        {
            string myDate = DateTime.Today.ToString("yyyy-MM-dd");
            //string myDate = "2019-10-10";

            string qry = "insert into dashboard_dataerrors (date, name, seid, jobid) values(Convert(varchar(10),'" + myDate + "',103), N'" +
                  kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "' )";

            string qryOld = "insert into dashboard_oldgooglepage (date, keyword, seid, jobid) values('" + DateTime.Now + "', N'" +
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



            //string qry = "Insert into KeywordsFailure(date, seid, keyword, status) values('" + DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") + "', " + seid + ", N'" + kw.Replace("'", "''") + "', '-1')";

            //try
            //{
            //    using (SqlConnection con = new SqlConnection(Common.ReadConnection()))
            //    {
            //        con.Open();
            //        using (SqlCommand comm = new SqlCommand(qry, con))
            //        {
            //            comm.CommandTimeout = 0;
            //            comm.CommandType = CommandType.Text;
            //            comm.ExecuteNonQuery();
            //        }
            //    }
            //}
            //finally { }
        }

        private void SendToDB(string seid, string keyword, string xml, string jobid, int urlcount)
        {
            try
            {
                string myDate = DateTime.Today.ToString("yyyy-MM-dd");
                //string myDate = "2019-11-20";

                using (SqlConnection con = new SqlConnection(Common.ReadConnection()))  // 12-05-2020
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
                    alResult = source.GetOxylabsWebDataSources(sp).Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return await Task.FromResult(alResult);
        }

      
    }
}
