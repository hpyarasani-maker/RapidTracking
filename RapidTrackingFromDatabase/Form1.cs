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



    public partial class Form1 : Form
    {
        public string xmlPath1 = @"C:\inetpub\wwwroot\keywords_1.xml";
        public string xmlPath2 = @"C:\inetpub\wwwroot\keywords_2.xml";
        public string xmlPath3 = @"C:\inetpub\wwwroot\keywords_3.xml";//changes

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
            //return worklistSize;
            return Task.FromResult(worklistSize);

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
            this.Text = "RapidTracking_Database_keywords_(1-2-3)";//changes

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
            //string strSql = "exec [dbo].[Tracking_DB_Keywords_SEID_102_Data] '" + dtPicker1.Text + "'";
            string strSql = "Tracking_DB_Keywords_SEID_102_Data '" + dtPicker1.Text + "',1";
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
                        lstKWs.Items.Add(objData[0].ToString() + ":" + objData[1].ToString() + ":" + objData[2].ToString() + ":" + objData[3].ToString());
                    }));
                }
                lstKWs.Invoke((MethodInvoker)(delegate ()
                {
                    lstKWs.Refresh();
                }));
                objData.Close();
                int cnt = 0;
                foreach (string s in lstKWs.Items)
                {
                    string seid = s.Split(':')[0];
                    string keyword = s.Split(':')[1];
                    string jobid = s.Split(':')[2];
                    int count = int.Parse(s.Split(':')[3]);
                    Task<string> x1 = GetXMLDataOneThread(s.Split(':')[0], s.Split(':')[1]);
                    this.Invoke((MethodInvoker)delegate () {
                        lblcount1.Text = "Count: " + count;
                    });
                    //string res = x1.Result;                 
                    await SendToAPI1(seid, keyword, SanitizeXmlString(x1.Result), jobid);
                    await SendToDB(seid, keyword, SanitizeXmlString(x1.Result), jobid, count);
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        textBox1.Text = s;
                        textBox1.Refresh();
                        label1.Text = ++cnt + " of " + lstKWs.Items.Count + " Completed";
                        label1.Refresh();
                    });
                }
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
            //string strSql = "exec [dbo].[Tracking_DB_Keywords_SEID_102_Data] '" + dtPicker1.Text + "'";
            string strSql = "Tracking_DB_Keywords_SEID_102_Data '" + dtPicker1.Text + "',2";
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
                        //lstKWs2.Items.Add(objData[0].ToString() + ":" + objData[1].ToString());
                        lstKWs2.Items.Add(objData[0].ToString() + ":" + objData[1].ToString() + ":" + objData[2].ToString() + ":" + objData[3].ToString());

                    }));
                }
                lstKWs2.Invoke((MethodInvoker)(delegate ()
                {
                    lstKWs2.Refresh();
                }));
                objData.Close();
                int cnt = 0;
                foreach (string s in lstKWs2.Items)
                {
                    string seid = s.Split(':')[0];
                    string keyword = s.Split(':')[1];                    
                    string jobid = s.Split(':')[2];
                    int count = int.Parse(s.Split(':')[3]);
                    Task<string> x2 = GetXMLDataTwoThread(s.Split(':')[0], s.Split(':')[1]);
                    this.Invoke((MethodInvoker)delegate () {
                        lblcount2.Text = "Count: " + count;
                    });
                    await SendToAPI2(seid, keyword, SanitizeXmlString(x2.Result), jobid);
                    await SendToDB(seid, keyword, SanitizeXmlString(x2.Result), jobid, count);
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        textBox2.Text = s;
                        textBox2.Refresh();
                        label2.Text = ++cnt + " of " + lstKWs2.Items.Count + " Completed";
                        label2.Refresh();
                    });
                }
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
            //string strSql = "exec [dbo].[Tracking_DB_Keywords_SEID_102_Data] '" + dtPicker1.Text + "'";
            string strSql = "Tracking_DB_Keywords_SEID_102_Data '" + dtPicker1.Text + "',3";

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
                        //lstKWs3.Items.Add(objData[0].ToString() + ":" + objData[1].ToString());
                        lstKWs3.Items.Add(objData[0].ToString() + ":" + objData[1].ToString() + ":" + objData[2].ToString() + ":" + objData[3].ToString());

                    }));
                }
                lstKWs3.Invoke((MethodInvoker)(delegate ()
                {
                    lstKWs3.Refresh();
                }));
                objData.Close();
                int cnt = 0;
                foreach (string s in lstKWs3.Items)
                {
                    string seid = s.Split(':')[0];
                    string keyword = s.Split(':')[1];
                    string jobid = s.Split(':')[2];
                    int count = int.Parse(s.Split(':')[3]);
                    Task<string> x3 = GetXMLDataThreeThread(s.Split(':')[0], s.Split(':')[1]);
                    this.Invoke((MethodInvoker)delegate () {
                        lblcount3.Text = "Count: " + count;
                    });
                    await SendToAPI3(seid, keyword, SanitizeXmlString(x3.Result), jobid);
                    await SendToDB(seid, keyword, SanitizeXmlString(x3.Result), jobid, count);
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        textBox3.Text = s;
                        textBox3.Refresh();
                        label3.Text = ++cnt + " of " + lstKWs3.Items.Count + " Completed";
                        label3.Refresh();
                    });
                }
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
 
        private async Task SendToDB(string seid, string keyword, string xml, string jobid, int urlcount)
        {
            try
            {
                //string myDate = DateTime.Today.ToString("yyyy-MM-dd");
                using (SqlConnection con = new SqlConnection(await Common.ReadConnection()))
                {
                    con.Open();

                    using (SqlCommand comm = con.CreateCommand())
                    {
                        comm.CommandTimeout = 0;
                        comm.CommandType = CommandType.StoredProcedure;
                        comm.CommandText = "Insert_dashboard_data";
                        comm.Parameters.Add("Date", SqlDbType.DateTime).Value = dtPicker1.Text;
                        comm.Parameters.Add("Name", SqlDbType.NVarChar).Value = keyword; //.Replace("'", "''");
                        comm.Parameters.Add("Seid", SqlDbType.Int).Value = seid;
                        comm.Parameters.Add("JobId", SqlDbType.NVarChar).Value = jobid;
                        comm.Parameters.Add("Count", SqlDbType.Int).Value = urlcount;
                        comm.Parameters.Add("XmlData", SqlDbType.Xml).Value = xml.Replace("'", "''");
                        comm.Parameters.Add("Received", SqlDbType.VarChar).Value = "Normal Request Oxylabs Multithread Jobid"; //14-04-2025

                        await comm.ExecuteNonQueryAsync();
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
        public async Task<string> GetXMLDataOneThread(string seid, string kn)
        {
            string xmlString = "";
            string strSql = "select xmldata from dashboard_data2 where seid = '" + seid + "' and name = N'" + kn.Replace("'", "''") + "'";
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
            return await Task.FromResult(xmlString);
        }
        public async Task<string> GetXMLDataTwoThread(string seid, string kn)
        {
            string xmlString = "";
            string strSql = "select xmldata from dashboard_data2 where seid = '" + seid + "' and name = N'" + kn.Replace("'", "''") + "'";
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
            return await Task.FromResult(xmlString);
        }
        public async Task<string> GetXMLDataThreeThread(string seid, string kn)
        {
            string xmlString = "";
            string strSql = "select xmldata from dashboard_data2 where seid = '" + seid + "' and name = N'" + kn.Replace("'", "''") + "'";
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
            return await Task.FromResult(xmlString);
        }
        private async Task<string> GetTextFromXMLFile(string file)
        {
            StreamReader reader = new StreamReader(file);
            string ret = reader.ReadToEnd();
            reader.Close();
            return await Task.FromResult(ret);
        }
        private async Task SendToAPI1(string seid, string kw, string res, string jobid)
        {
            XmlDocument xd = new XmlDocument();
            res = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + res;
            xd.LoadXml(res);
            xd.Save(xmlPath1);
            //File.WriteAllText(@"D:\23-03-2020\" + seid + "_" + kw + jobid + ".xml", res);
            //return;
            //SendToURL

            string submitURL =  readAPI();

            string user = "pisoftware";
            string pwd = "r00t123456";
            try
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(submitURL);
                httpWReq.UseDefaultCredentials = true;
                httpWReq.PreAuthenticate = true;
                httpWReq.Credentials = CredentialCache.DefaultCredentials;

                Encoding encoding = new UTF8Encoding();
                string postData =  await GetTextFromXMLFile(xmlPath1);
                byte[] data = encoding.GetBytes(postData);

                httpWReq.ProtocolVersion = HttpVersion.Version11;
                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/x-www-form-urlencoded";


                string auth = string.Format("{0}:{1}", user, pwd);
                string enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(auth));
                string cred = string.Format("{0} {1}", "Basic", enc);


                httpWReq.Headers[HttpRequestHeader.Authorization] = cred;
                httpWReq.ContentLength = data.Length;


                Stream stream = await httpWReq.GetRequestStreamAsync();
                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse response = (HttpWebResponse)await httpWReq.GetResponseAsync();
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
                await SendToDBFailure(seid, kw, jobid, false, ex.Message);


                string errorMsg = string.Empty;
                using (WebResponse response = ex.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    errorMsg = string.Format("API Error: StatusCode {0}", httpResponse.StatusCode);

                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        errorMsg += "\r\n" + await reader.ReadToEndAsync();
                    }
                }

                throw new Exception(errorMsg);

            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }

        }

        private async Task SendToAPI2(string seid, string kw, string res, string jobid)
        {
            XmlDocument xd = new XmlDocument();
            res = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + res;
            xd.LoadXml(res);
            xd.Save(xmlPath2);
            //File.WriteAllText(@"D:\23-03-2020\" + seid + "_" + kw + jobid + ".xml", res);
            //return;
            //SendToURL

            string submitURL =  readAPI();

            string user = "pisoftware";
            string pwd = "r00t123456";
            try
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(submitURL);
                httpWReq.UseDefaultCredentials = true;
                httpWReq.PreAuthenticate = true;
                httpWReq.Credentials = CredentialCache.DefaultCredentials;

                Encoding encoding = new UTF8Encoding();
                string postData =  await GetTextFromXMLFile(xmlPath2);
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

                Stream stream = await httpWReq.GetRequestStreamAsync();
                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse response = (HttpWebResponse)await httpWReq.GetResponseAsync();
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
                await SendToDBFailure(seid, kw, jobid, false, ex.Message);


                string errorMsg = string.Empty;
                using (WebResponse response = ex.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    errorMsg = string.Format("API Error: StatusCode {0}", httpResponse.StatusCode);

                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        errorMsg += "\r\n" + await reader.ReadToEndAsync();
                    }
                }

                throw new Exception(errorMsg);

            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }

        }

        private async Task SendToAPI3(string seid, string kw, string res, string jobid)
        {
            XmlDocument xd = new XmlDocument();
            res = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + res;
            xd.LoadXml(res);
            xd.Save(xmlPath3);
            //File.WriteAllText(@"D:\23-03-2020\" + seid + "_" + kw + jobid + ".xml", res);
            //return;
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
                string postData =  await GetTextFromXMLFile(xmlPath3);
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

                Stream stream = await httpWReq.GetRequestStreamAsync();
                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse response = (HttpWebResponse)await httpWReq.GetResponseAsync();
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
                await SendToDBFailure(seid, kw, jobid, false, ex.Message);


                string errorMsg = string.Empty;
                using (WebResponse response = ex.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    errorMsg = string.Format("API Error: StatusCode {0}", httpResponse.StatusCode);

                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        errorMsg += "\r\n" + await reader.ReadToEndAsync();
                    }
                }

                throw new Exception(errorMsg);

            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }

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
        public string SanitizeXmlString(string xml)
        {

            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }

            StringBuilder buffer = new StringBuilder(xml.Length);

            foreach (char c in xml)
            {
                if (XmlSanitizingStream.IsLegalXmlChar(c))
                {
                    buffer.Append(c);
                }
            }

            return buffer.ToString();
        }
        public class XmlSanitizingStream : StreamReader
        {
            public XmlSanitizingStream(Stream streamToSanitize)
            : base(streamToSanitize, true)
            { }

            /// <summary>
            /// Whether a given character is allowed by XML 1.0.
            /// </summary>
            public static bool IsLegalXmlChar(int character)
            {
                return
                (
                     character == 0x9 /* == '\t' == 9   */          ||
                     character == 0xA /* == '\n' == 10  */          ||
                     character == 0xD /* == '\r' == 13  */          ||
                    (character >= 0x20 && character <= 0xD7FF) ||
                    (character >= 0xE000 && character < 0xFFFD) ||       //02-11-2020 changed <= 0xFFFD to < 0xFFFD
                    (character >= 0x10000 && character <= 0x10FFFF)
                );
            }
            private const int EOF = -1;

            public override int Read()
            {
                // Read each char, skipping ones XML has prohibited

                int nextCharacter;

                do
                {
                    // Read a character

                    if ((nextCharacter = base.Read()) == EOF)
                    {
                        // If the char denotes end of file, stop
                        break;
                    }
                }

                // Skip char if it's illegal, and try the next

                while (!XmlSanitizingStream.
                        IsLegalXmlChar(nextCharacter));

                return nextCharacter;
            }

            public override int Peek()
            {
                // Return next legal XML char w/o reading it 

                int nextCharacter;

                do
                {
                    // See what the next character is 
                    nextCharacter = base.Peek();
                }
                while
                (
                    // If it's illegal, skip over 
                    // and try the next.

                    !XmlSanitizingStream.IsLegalXmlChar(nextCharacter) &&
                    (nextCharacter = base.Read()) != EOF
                );

                return nextCharacter;

            }
        }
    }
}
