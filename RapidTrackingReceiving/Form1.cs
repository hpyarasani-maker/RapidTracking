using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Xml;

namespace Oxylabs_BulkKeywords
{
    public partial class Form1 : Form
    {
        HTMLParserNewTask WOWS = new HTMLParserNewTask();
        //HTMLParserNewTaskHotels WOWS = new HTMLParserNewTaskHotels();


        Timer timer = new Timer();

        public Form1()
        {
            InitializeComponent();
            WOWS.OnKeywordDone += WOWS_OnKeywordDone;
            TimerExit();
            //cnt = GetOxyCount();
        }

        void TimerExit()
        {
            timer.Interval = 20 * 60 * 60000;
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            Environment.Exit(Environment.ExitCode);
        }

        public int GetOxyCount()
        {
            int ct = 0;
            string strQuery = "exec [dbo].[GetOxyCount]";
            SqlConnection objCon = new SqlConnection(StrConn());
            try
            {
                objCon.Open();
                SqlCommand objCmd = new SqlCommand(strQuery, objCon);
                objCmd.CommandTimeout = 0;
                SqlDataReader objData = null;
                objData = objCmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (objData.Read())
                {
                    ct = Convert.ToInt16(objData[0]);
                }
                objData.Close();
            }
            catch (SqlException e)
            {
                string errMsg = "Database Connection is temporarily not working\n" + e.ToString();
            }
            finally
            {
                if (objCon.State == ConnectionState.Open)
                {
                    objCon.Close();
                }
            }
            return ct;
        }

        int cntr = 1;
        int errors = 1;
        private void WOWS_OnKeywordDone(string value)
        {
            this.Invoke((MethodInvoker)delegate
            {
                string[] msg = value.Split('^');
                if (msg[0].StartsWith("Error:"))
                {
                    txtErrors.Text += msg[0] + "\r\n\tStatusCode: " + msg[1] + "\r\n\r\n";
                    lblErrors.Text = errors++.ToString();
                }
                else
                    lblCompletedKw.Text = msg[0];

                lblStatusCode.Text = msg[1];
                lblCount.Text = cntr++.ToString();
                //31-03-2020
                lblAPITime.Text = msg[2] + " sec";
                lblDBTime.Text = msg[3] + " sec";
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Text = "D_Oxylabs_TrackingTrending_DesktopRecieve_503_10";
            Text = "D_Oxylabs_TrackingTrending_All_1";
            //Text = "D_Oxylabs_RapidTracking_RecieveDesktop_20";
            //Text = "D_Oxylabs_TrackingTrending_MobileRecieve_102_10";
            //Text = "D_Oxylabs_RapidTracking_RecieveMobile_20";
            //Text = "D_Oxylabs_TrackingTrending_RecieveOtherMobile_15";  
            //Text = "D_Oxylabs_TrackingTrending_Recieve_CommaKeywords_3";
            //Text = "D_Oxylabs_TrackingTrending_Recieve_HotelKeywords_4_WC";
            //Text = "D_Oxylabs_TrackingTrending_Yesterdays";
            //Text = "D_Oxylabs_TrackingTrending_ReceiveOtherDesktop_3_P";
            //Text = "D_Oxylabs_TrackingTrending_MobileRecieve_106_10_P_60";
            //Text = "Comma Keywords P Results-11-14_Mobile_2";
            //27th what is bitcoin

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        public string StrConn()
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
    }

    public delegate void KeywordDone(string value);
}
