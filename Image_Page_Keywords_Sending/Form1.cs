using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace Image_Page_Keywords_Sending
{
    public partial class Form1 : Form
    {
        
        SendingKeywordRequest WOWS = new SendingKeywordRequest();
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        static Random rd = new Random();//29-06-2020
        public Form1()
        {
            InitializeComponent();
            //timerExit();
            //cnt = GetOxyCount();
        }

        void timerExit()
        {
            timer.Interval = 715 * 60000;
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
            Text = "D_Oxylabs_CallbackKwdSending_74_SingleKeyword";
            //Text = "D_Oxylabs_CallbackKwdSending_401_SingleKeyword";
            //Text = "D_Oxylabs_CallbackKwdSending_402_Mobile_SingleKeyword";
            //Text = "D_Oxylabs_CallbackKwdSending_382_Mobile_SingleKeyword";
            //Text = "D_Oxylabs_CallbackKwdSending_140_1";            

            date_picker.Value = DateTime.Today;  

            Thread t = new Thread(new ThreadStart(mainLoop));
            generateWorklist();
            if (getWorklistSize() > 0)
            {
                t.Start();                
            }
            else
            {
                t.Abort();
                Close();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        public void mainLoop()
        {            
            while (getWorklistSize() > 0)
            {
                processWorklist();
                generateWorklist();
            }

            Environment.Exit(Environment.ExitCode);
        }

        public string strConn()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                string fileName = @"C:\Inetpub\wwwroot\ServerIP_Callback.xml";
                // You'll need to put the correct path to your xml file here
                xml.Load(fileName);

                // Select a specific node
                XmlNode node = xml.SelectSingleNode("ConnectionString/con");
                // Get its value
                string name = node.InnerText;

                return name;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
                                                                                   
        public void generateWorklist()
        {
            this.Invoke((MethodInvoker)delegate()
            {
                worklist.Items.Clear();
                //worklist.Items.Add("402:iphone");
               
                worklist.Refresh();
                date_picker.Format = DateTimePickerFormat.Custom;
                date_picker.CustomFormat = "yyyy-MM-dd";
            });

            //return;

            Cursor.Current = Cursors.WaitCursor;
            string myDate = date_picker.Text;

            string strQry = "exec [dbo].[GetKeywords_74] '" + myDate + "'"; 
            //string strQry = "exec [dbo].[GetKeywords_401] '" + myDate + "'";
            //string strQry = "exec [dbo].[GetKeywords_402] '" + myDate + "'"; 
            //string strQry = "exec [dbo].[GetKeywords_382] '" + myDate + "'"; 
            //string strQry = "exec [dbo].[GetBulk_140] '" + myDate + "'";
            //string strQry = "exec [dbo].[GetBulk_Images] '" + myDate + "'";

            //string strQry = "exec [dbo].[GetBulk_381_383] '" + myDate + "'";
            //string strQry = "exec [dbo].[GetKeywords_381-383] '" + myDate + "'";

            SqlConnection objCon = null;
            SqlDataReader objData = null;
            try
            {               
                objCon = new SqlConnection(strConn());
                objCon.Open();
                SqlCommand objCmd = new SqlCommand(strQry, objCon);
                objCmd.CommandTimeout = 0;
                objData = objCmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (objData.Read())
                {
                    this.Invoke((MethodInvoker)delegate()
                    {
                        worklist.Items.Add(objData[0].ToString() + ":" + objData[1].ToString());
                    });
                }
                this.Invoke((MethodInvoker)delegate()
                {
                    worklist.Refresh();
                });
                objData.Close();
            }
            catch (SqlException e)
            {
                string errMsg = "Database Connection is temporarily not working\n" + e.ToString();
                errorList.Invoke((MethodInvoker)delegate()
                {
                    errorList.Items.Add(errMsg);
                });
            }
            catch (Exception ex)
            {
                errorList.Invoke((MethodInvoker)delegate()
                {
                    errorList.Items.Add(ex.ToString());
                });
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

        public void processWorklist()
        {
            string resultsString;
            char sep;
            Array resultsArray;
            string seid;
            string kn;
            int mseconds;

            for (int i = 0; i < worklist.Items.Count; i++)
            {
                mseconds = rd.Next(10, 20) * 1000;
                // get next Project ID, Search Engine ID, Keyword ID and Keyword from worklist
                resultsString = worklist.Items[i].ToString();
                sep = ':';
                resultsArray = resultsString.Split(sep);
                seid = resultsArray.GetValue(0).ToString();
                kn = resultsArray.GetValue(1).ToString();
                TimeSpan ts = new TimeSpan();
                DateTime dt = DateTime.Now;
                try
                {
                    processResults(seid, kn);
                }
                catch (Exception ex)
                {
                    errorList.Invoke((MethodInvoker)delegate ()
                    {
                        errorList.Items.Add(ex.Message);
                    });
                }
                ts = DateTime.Now - dt;

                this.Invoke((MethodInvoker)delegate()
                {
                    progress_lbl.Text = "Completed : " + (i + 1) + " of " + worklist.Items.Count;
                    progress_lbl.Refresh();
                    lblIP.Text = ts.TotalSeconds.ToString();
                    rd_lbl.Text = (mseconds / 1000).ToString() + " " + "seconds";                    
                });
                Thread.Sleep(mseconds);
            }
        }
        
        public void processResults(string seid, string kn)
        {
            try
            {
                WOWS.getTop100(kn, Convert.ToInt32(seid));
            }
            catch(Exception ex)
            {
                throw ex;
            }                    
        }         
    }
}
