using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Net;
using System.Xml;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections;

namespace RapidTrackingFromDatabase
{

    public string xmlPath1 = @"C:\inetpub\wwwroot\keywords_1.xml";
    public string xmlPath2 = @"C:\inetpub\wwwroot\keywords_2.xml";
    public string xmlPath3 = @"C:\inetpub\wwwroot\keywords_3.xml";//changes




    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public string strConn()
        {


            XmlDocument xml = new XmlDocument();
            string fileName = @"C:\Inetpub\wwwroot\ServerIP1.xml";
            // You'll need to put the correct path to your xml file here
            xml.Load(fileName);

            // Select a specific node
            XmlNode node = xml.SelectSingleNode("ConnectionString/con");
            // Get its value
            string name = node.InnerText;

            return name;
        }
        public Task<int> getWorklistSize()
        {
            int worklistSize = lstKWs.Items.Count;
            return worklistSize;
        }
        public string readAPI()
        {
            XmlDocument xml = new XmlDocument();
            string fileName = @"C:\Inetpub\wwwroot\ServerIP1.xml";
            // You'll need to put the correct path to your xml file here
            xml.Load(fileName);

            // Select a specific node
            XmlNode node = xml.SelectSingleNode("ConnectionString/apiNew");
            // Get its value
            string name = node.InnerText;

            return name;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "RapidTracking_Errorkeywords_(1-2-3)";//changes

            dtPicker1.Value = DateTime.Today;
            string myDate = dtPicker1.Value.ToString("yyyy-MM-dd");

            Task t1 = Task.Run(async () => //16-02-2025
            {
                try
                {
                    await StartProcess_1();
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        errorList.Text = $"Exception: {ex.Message}";
                    });

                }
            });

            Task t2 = Task.Run(async () =>
            {
                try
                {
                    await StartProcess_2();
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        errorList.Text = $"Exception: {ex.Message}";
                    });

                }
            });

            Task t3 = Task.Run(async () =>
            {
                try
                {
                    await StartProcess_3();
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        errorList.Text = $"Exception: {ex.Message}";
                    });

                }
            });//16-02-2025
        }

        private async Task StartProcess_1()
        {
            lstKWs.Invoke((MethodInvoker)(delegate ()
            {
                lstKWs.Items.Clear();
            }));
            Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            dtPicker1.Format = DateTimePickerFormat.Custom;
            dtPicker1.CustomFormat = "yyyy-MM-dd";
            //string myDate = date_picker.Text;
            string myDate = dtPicker1.Value.ToString("yyyy-MM-dd");
            string strSql = "exec [dbo].[GetKeywords_Bing_1] '" + dtPicker1.Text + "'";
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
                    lstKWs.Invoke((MethodInvoker)(delegate ()
                    {
                        lstKWs.Items.Add(objData[0].ToString() + ":" + objData[1].ToString());
                    }));
                }
                lstKWs.Invoke((MethodInvoker)(delegate ()
                {
                    lstKWs.Refresh();
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
        private async Task StartProcess_2()
        {
            lstKWs2.Invoke((MethodInvoker)(delegate ()
            {
                lstKWs2.Items.Clear();
            }));
            Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            dtPicker1.Format = DateTimePickerFormat.Custom;
            dtPicker1.CustomFormat = "yyyy-MM-dd";
            //string myDate = date_picker.Text;
            string myDate = dtPicker1.Value.ToString("yyyy-MM-dd");
            string strSql = "exec [dbo].[GetKeywords_Bing_2] '" + dtPicker1.Text + "'";
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
                    lstKWs2.Invoke((MethodInvoker)(delegate ()
                    {
                        lstKWs2.Items.Add(objData[0].ToString() + ":" + objData[1].ToString());
                    }));
                }
                lstKWs2.Invoke((MethodInvoker)(delegate ()
                {
                    lstKWs2.Refresh();
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
        private async Task StartProcess_3()
        {
            lstKWs3.Invoke((MethodInvoker)(delegate ()
            {
                lstKWs3.Items.Clear();
            }));
            Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            dtPicker1.Format = DateTimePickerFormat.Custom;
            dtPicker1.CustomFormat = "yyyy-MM-dd";
            //string myDate = date_picker.Text;
            string myDate = dtPicker1.Value.ToString("yyyy-MM-dd");
            string strSql = "exec [dbo].[GetKeywords_Bing_2] '" + dtPicker1.Text + "'";
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
                    lstKWs3.Invoke((MethodInvoker)(delegate ()
                    {
                        lstKWs3.Items.Add(objData[0].ToString() + ":" + objData[1].ToString());
                    }));
                }
                lstKWs3.Invoke((MethodInvoker)(delegate ()
                {
                    lstKWs3.Refresh();
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
        public Task<string> GetXMLDataOneThread(string seid, string kn)
        {
            string xmlString = "";
            string strSql = "select xmldata from temp_table where seid = '" + seid + "' and keyword = N'" + kn.Replace("'", "''") + "'";
            SqlConnection objCon = null;
            SqlDataReader objData = null;
            try
            {
                objCon = new SqlConnection(strConn());
                objCon.Open();
                SqlCommand objCmd = new SqlCommand(strSql, objCon);
                objCmd.CommandTimeout = 0;
                objData = objCmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (objData.Read())
                {
                    xmlString = objData[0].ToString();
                }

                lstKWs.Invoke((MethodInvoker)(delegate ()
                {
                    lstKWs.Refresh();
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

            textBox1.Invoke((MethodInvoker)(delegate ()
            {
                textBox1.Text = xmlString;
            }));
            File.WriteAllText(xmlPath1, xmlString);
            return xmlString;
        }
        public Task<string> GetXMLDataTwoThread(string seid, string kn)
        {
            string xmlString = "";
            string strSql = "select xmldata from temp_table where seid = '" + seid + "' and keyword = N'" + kn.Replace("'", "''") + "'";
            SqlConnection objCon = null;
            SqlDataReader objData = null;
            try
            {
                objCon = new SqlConnection(strConn());
                objCon.Open();
                SqlCommand objCmd = new SqlCommand(strSql, objCon);
                objCmd.CommandTimeout = 0;
                objData = objCmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (objData.Read())
                {
                    xmlString = objData[0].ToString();
                }

                lstKWs2.Invoke((MethodInvoker)(delegate ()
                {
                    lstKWs2.Refresh();
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

            textBox2.Invoke((MethodInvoker)(delegate ()
            {
                textBox2.Text = xmlString;
            }));
            File.WriteAllText(xmlPath2, xmlString);
            return xmlString;
        }
        public Task<string> GetXMLDataThreeThread(string seid, string kn)
        {
            string xmlString = "";
            string strSql = "select xmldata from temp_table where seid = '" + seid + "' and keyword = N'" + kn.Replace("'", "''") + "'";
            SqlConnection objCon = null;
            SqlDataReader objData = null;
            try
            {
                objCon = new SqlConnection(strConn());
                objCon.Open();
                SqlCommand objCmd = new SqlCommand(strSql, objCon);
                objCmd.CommandTimeout = 0;
                objData = objCmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (objData.Read())
                {
                    xmlString = objData[0].ToString();
                }

                lstKWs3.Invoke((MethodInvoker)(delegate ()
                {
                    lstKWs3.Refresh();
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

            textBox3.Invoke((MethodInvoker)(delegate ()
            {
                textBox3.Text = xmlString;
            }));
            File.WriteAllText(xmlPath3, xmlString);
            return xmlString;
        }
        private string GetTextFromXMLFile(string file)
        {
            StreamReader reader = new StreamReader(file);
            string ret = reader.ReadToEnd();
            reader.Close();
            return ret;
        }
        private async Task SendToDBFailure(string seid, string kw, string jobid, bool isOldPage, string errMsg)
        {
            string myDate = DateTime.Today.ToString("yyyy-MM-dd");
            //string myDate = "2019-10-10";

            string qry = "insert into dashboard_dataerrors (date, name, seid, jobid,message) values(Convert(varchar(10),'" + myDate + "',103), N'" +
                    kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "', N'" + errMsg + "')"; //03-01-2022

            string qryOld = "insert into dashboard_oldgooglepage (date, keyword, seid, jobid) values('" + DateTime.Now + "', N'" +
                  kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "')";

            using (SqlConnection con = new SqlConnection(await Common.ReadConnection()))
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
    }
}
