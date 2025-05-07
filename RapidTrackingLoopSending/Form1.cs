using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace RapidTrackingLoopSending
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
            //MissingKeywordsJob.ExecuteMissingKeywordsJob(0).Wait();
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
            //Text = "D_Oxylabs_NewSEIDs";
            //Text = "D_Oxylabs_TrackingTrending_KwdSending_106_1";
            //Text = "D_Oxylabs_TrackingTrending_KwdSending_102_1";
            //Text = "D_Oxylabs_TrackingTrending_KwdSending_OtherMobile_2";
            //Text = "D_Oxylabs_TrackingTrending_KwdSending_OtherDesktop_2";
            //Text = "D_Oxylabs_RapidTracking_KwdSending_Desktop_1_2";
            //Text = "D_Oxylabs_TrackingTrending_KwdSending_58_2";
            //Text = "D_Oxylabs_TrackingTrending_KwdSending_All";
            //Text = "D_Oxylabs_Tracking New Keywords_Sending";
            //Text = "D_Oxylabs_RapidTracking_KwdSending_Mobile_Yesterdays";
            //Text = "D_Oxylabs_RapidTracking_KwdSending_CommaKeywordsMobile_Hotels_1";
            //Text = "D_Oxylabs_RapidTracking_KwdSending_Mobile_Hotels_1";
            //Text = "D_Oxylabs_RapidTracking_KwdSending_NotHotelKeywords";
            //Text = "D_Oxylabs_TrackingTrending_SendingCommaKeywords";
            //Text = "D_Oxylabs_TrackingTrending_KwdSending_OtherDesktop_1_P";
            //Text = "D_Oxylabs_RapidTracking_KwdSending_CommaKeywordsMobile_Hotels_1P";
            //Text = "D_Oxylabs_RapidTracking_KwdSending_NonHotelKeywordsP";
            //Text = "Sending KeywordsP-11-14_CommaKeywords_P";
            //Text = "Sending Previous Date Keywords"; //sending previous date keywords
            Text = "D_Oxylabs_TrackingTrending_KwdSendingLoop_All";
            date_picker.Value = DateTime.Today; 

            

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
                string fileName = @"C:\Inetpub\wwwroot\Callback_TrackingTrending.xml";
                
                // You'll need to put the correct path to your xml file here
                xml.Load(fileName);

                // Select a specific node                
                XmlNode node = xml.SelectSingleNode("ConnectionString/con");

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
                //worklist.Items.Add("102:hotel kungsträdgården tripadvisor");
                //worklist.Items.Add("1:sex toys, australia, donald trump, narendra modi");
                //worklist.Items.Add("160:zlatan");
                //worklist.Items.Add("160:messi");
                //worklist.Items.Add("61:cancervårdsförsäkring");
                //worklist.Items.Add("145:kia sportage");
                worklist.Refresh();
                date_picker.Format = DateTimePickerFormat.Custom;
                date_picker.CustomFormat = "yyyy-MM-dd";
            });

           //return;

            Cursor.Current = Cursors.WaitCursor;
            string myDate = date_picker.Text;

            //string strQry = "exec [dbo].[GetBulk_NewSeids] '" + myDate + "'";
            //string strQry = "exec [dbo].[GetBulkUKDesktop_Temp_1] '" + myDate + "'";    // seid: 503
            //string strQry = "exec [dbo].[GetBulk_All] '" + myDate + "'";
            //string strQry = "exec [dbo]. [GetBulkDesktop_NewKeywords] '" + myDate + "'";
            //string strQry = "exec [dbo].[GetBulkDesktop_58_2] '" + myDate + "'";
            //string strQry = "exec [dbo].[GetBulkMobile_102_1] '" + myDate + "'";
            //string strQry = "exec [dbo].[GetBulkDesktop_2] '" + myDate + "'";
            //string strQry = "exec [dbo].[GetBulkMobile_2] '" + myDate + "'";
            //string strQry = "exec [dbo].[GetBulkDesktop_1_2] '" + myDate + "'";
            //string strQry = "exec [dbo].[GetBulkMobile_Hotel] '" + myDate + "'";
            //string strQry = "exec [dbo].[GetCommaKeywords_Hotel] '" + myDate + "'";
            //string strQry = "exec [dbo].[GetBulkMobile_NotHotel] '" + myDate + "'";
            //string strQry = "exec [dbo].[GetCommaKeywords] '" + myDate + "'";
            //string strQry = "exec [dbo].[GetBulk_All] '" + myDate + "'";
            //string strQry = "exec [dbo].[GetBulk_P] '" + myDate + "'";  //sending procedure previous date keywords
            string strQry = "exec [Tracking_DB_Keywords_SEID_102_SeeMore] '" + myDate + "'"; //30-06-2022
            //string strQry = "exec [Tracking_DB_Keywords_SEID_102_TGBN] '" + myDate + "'"; //28-06-2022
            SqlConnection objCon = null;
            SqlDataReader objData = null;

            try
            {               
                objCon = new SqlConnection(await strConn());
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
            int mseconds;


            for (int i = 0; i < worklist.Items.Count; i++)
            {
                //mseconds = rd.Next(30, 50) * 1000; //First Sending app 29-06-2020    //SEID=58-1
               //mseconds = rd.Next(30, 70) * 1000; //First Sending app 29-06-2020    //SEID=58-2
                //mseconds = rd.Next(30, 80) * 1000; //Second Sending app 29-06-2020 //SEID=106-1
                //mseconds = rd.Next(30, 100) * 1000; //Second Sending app 29-06-2020 //SEID=106-2
                //mseconds = rd.Next(30, 110) * 1000; //Third Sending app 29-06-2020 //SEID=OtherDesktop-1
                //mseconds = rd.Next(30, 130) * 1000; //Third Sending app 29-06-2020 //SEID=OtherDesktop-2
                //mseconds = rd.Next(30, 140) * 1000; //Fourth Sending app 29-06-2020 //SEID=OtherMobile-1
                //mseconds = rd.Next(30, 160) * 1000; //Fourth Sending app 29-06-2020 //SEID=OtherMobile-2
                //mseconds = rd.Next(30, 170) * 1000; //Fifith Sending app 29-06-2020 //SEID=1-1
                mseconds = rd.Next(30, 180) * 1000; //Fifith Sending app 29-06-2020 //SEID=1-2
                //mseconds = rd.Next(30, 200) * 1000; //Sixth Sending app 29-06-2020 //SEID=102-1
                //mseconds = rd.Next(30, 210) * 1000; //Sixth Sending app 29-06-2020 //SEID=102-2
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
               });
                this.Invoke((MethodInvoker)delegate ()
                {
                    rnd_lbl.Text = (mseconds / 1000).ToString() + " " + "seconds";
                });
                //Thread.Sleep(mseconds); //if you want to send fast comment this line...
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
