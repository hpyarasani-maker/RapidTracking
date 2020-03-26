using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Xml;

namespace TrendingReceiving
{
    public partial class Form1 : Form
    {
        HTMLParserNewTask WOWS = new HTMLParserNewTask();
        
        Timer timer = new Timer();                

        public Form1()
        {
            InitializeComponent();
            WOWS.OnKeywordDone += WOWS_OnKeywordDone;
            timerExit();
            //cnt = GetOxyCount();
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

        public int GetOxyCount()
        {
            int ct = 0;
            string strQuery = "exec [dbo].[GetOxyCount]";
            SqlConnection objCon = new SqlConnection(strConn());
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
                if (value.StartsWith("Error:"))
                {
                    txtErrors.Text += value + "\r\n\r\n";
                    lblErrors.Text = errors++.ToString();
                }
                else
                    lblCompletedKw.Text = value;

                lblCount.Text = cntr++.ToString();
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text = "D_Oxylabs_CallbackTrendingDesktopRecieve_1";
            //Text = "D_Oxylabs_CallbackTrendingMobileRecieve_2";                
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
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
    }

    public delegate void KeywordDone(string value);
}
