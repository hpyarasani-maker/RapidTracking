using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace TrendingLoopReceiving
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
            timer.Interval = 15 * 60000;
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            Environment.Exit(Environment.ExitCode);
        }

        public async Task<int> GetOxyCount()
        {
            int ct = 0;
            string strQuery = "exec [dbo].[GetOxyCount]";
            SqlConnection objCon = new SqlConnection(await StrConn());
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
            return await Task.FromResult<int>(ct);
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
                lblDownloadedTime.Text = msg[4] + " sec";
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text = "D_Oxylabs_CallbackTrendingDesktopRecieve_Loop";
           // Text = "D_Oxylabs_CallbackTrendingMobileRecieve_Loop";


            //27th what is bitcoin

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        public async Task<string> StrConn()
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

                return await Task.FromResult<string>(name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public delegate void KeywordDone(string value);
}
