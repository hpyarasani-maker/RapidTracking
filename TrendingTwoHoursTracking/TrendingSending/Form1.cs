using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace TrendingSending
{
    public partial class Form1 : Form
    {        
        SendingKeywordRequest WOWS = new SendingKeywordRequest();
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();       

        public Form1()
        {
            InitializeComponent();
            //timerExit();
            //cnt = GetOxyCount();
            //Comment below code for First sending file for fresh hour keyword for every 2 hours
            //Uncomment below code for 2nd sending file for faulted keywords for every two hours
            //DateTime dt = DateTime.Now;
            //int hour = dt.Hour;
            //string curDate = DateTime.Now.ToString("yyyy-MM-dd");
            ////string delQuery = "delete from dashboard_dataerrors where convert(varchar(10),date,127)='"+curDate+"' and datepart(hh,date)=" + hour;
            //string delQuery = "Exec [dbo].[Delete_Faulted_Keywords] '" + curDate + "'";
            //DeleteFaulted(delQuery).Wait();
        }
        private async Task DeleteFaulted(string qry)
        {
            using (SqlConnection con = new SqlConnection(await strConn()))
            {
                con.Open();
                using (SqlCommand comm = con.CreateCommand())
                {
                    comm.CommandType = CommandType.Text;
                    comm.CommandText = qry;
                    comm.CommandTimeout = 0;
                    comm.ExecuteNonQuery();
                }
            }
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
               
        private async void Form1_Load(object sender, EventArgs e)
        {
            Text = "D_Oxylabs_CallbackTrendingDesktopSending_1";
            //Text = "D_Oxylabs_CallbackTrendingMobileSending_1";            

            date_picker.Value = DateTime.Today; //.AddDays(-1);

            Thread t = new Thread(new ThreadStart(mainLoop));
            await generateWorklist();
            if (await getWorklistSize() > 0)
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

        public async void mainLoop()
        {            
            while (await getWorklistSize() > 0)
            {
                await processWorklist();
                await generateWorklist();
            }

            Environment.Exit(Environment.ExitCode);
        }

        public async Task<string> strConn()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                string fileName = @"C:\Inetpub\wwwroot\TrendingLiveAPI.xml";
                //string fileName = @"C:\Inetpub\wwwroot\ServerIP_Callback.xml";
                // You'll need to put the correct path to your xml file here
                xml.Load(fileName);

                // Select a specific node
                XmlNode node = xml.SelectSingleNode("TrendingAPI/con");
                //XmlNode node = xml.SelectSingleNode("ConnectionString/con");

                // Get its value
                string name = node.InnerText;

                return await Task.FromResult<string>(name);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
                                                                                   
        public async Task generateWorklist()
        {
            this.Invoke((MethodInvoker)delegate()
            {
                worklist.Items.Clear();
                //worklist.Items.Add("1:donald trump, narendra modi");
                //worklist.Items.Add("1:sex toys, australia, donald trump, narendra modi");
               
                worklist.Refresh();
                date_picker.Format = DateTimePickerFormat.Custom;
                date_picker.CustomFormat = "yyyy-MM-dd";
            });

            //return;

            Cursor.Current = Cursors.WaitCursor;
            string myDate = date_picker.Text;
            //string myDate = "2019-10-26";

            string strQry = "exec [dbo].[GetBulkTrendingDesktop_1] '" + myDate + "'";
            //string strQry = "exec [dbo].[GetBulkTrendingMobile_1] '" + myDate + "'"; 
            //string strQry = "[GetBulkTrendingMobile_Status=0] '" + myDate + "'";

            SqlConnection objCon = null;
            SqlDataReader objData = null;

            try
            {               
                objCon = new SqlConnection(await strConn());
                objCon.Open();
                SqlCommand objCmd = new SqlCommand(strQry, objCon);
                objCmd.CommandTimeout = 0;
                objData = await objCmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
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

        public async Task<int> getWorklistSize()
        {
            int worklistSize = worklist.Items.Count;
            return await Task.FromResult<int>(worklistSize);
        }

        public async Task processWorklist()
        {
            string resultsString;
            char sep;
            Array resultsArray;
            string seid;
            string kn;
            
            for (int i = 0; i < worklist.Items.Count; i++)
            {
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
                   await processResults(seid, kn);
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
                });
                Thread.Sleep(50000);
                //Thread.Sleep(30000);
            }
        }
        
        public async Task processResults(string seid, string kn)
        {
            try
            {
                await WOWS.getTop100(kn, Convert.ToInt32(seid));
            }
            catch(Exception ex)
            {
                throw ex;
            }                    
        }         
    }
}
