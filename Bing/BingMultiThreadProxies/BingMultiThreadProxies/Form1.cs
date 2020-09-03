using System;
using System.Collections;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text;
using System.IO;
using System.Threading;
using System.Net;
using System.Data;
using System.Xml;
using System.Web;

namespace BingMultiThreadProxies
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.DateTimePicker date_picker;
		private System.Windows.Forms.Button process_btn;
		private System.Windows.Forms.ListBox worklist1;
		private System.Windows.Forms.ListBox worklist2;
		private System.Windows.Forms.ListBox worklist3;
		private System.Windows.Forms.ListBox results1;
		private System.Windows.Forms.ListBox results2;
		private System.Windows.Forms.ListBox results3;
		private System.Windows.Forms.Label progress_seid2;
		private System.Windows.Forms.Label progress_seid6;
		private System.Windows.Forms.Label progress_seid12;
		private System.Windows.Forms.ListBox errorList;

        Bing server0 = new Bing();


       private TextBox txtError;
       string strCon;
       string liveurl;
       int urlcnt;

        const string xml1 = "Bing(15-17-34-35)_GT50_1.xml";
        const string xml2 = "Bing(15-17-34-35)_GT50_2.xml";
        const string xml3 = "Bing(15-17-34-35)_GT50_3.xml";

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private Label label1;
        private Label label2;
        private Label label3;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
		
        public Form1()
		{            
			InitializeComponent();
            //timerExit();
            strCon = strConn();
            liveurl = readAPI();
            urlcnt = GetTop100Count();
		}
        void timerExit()
        {
            timer.Interval = 30 * 60000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            Environment.Exit(Environment.ExitCode);
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
        public int GetTop100Count()
        {
            int ct = 0;
            string strQuery = "exec [dbo].[Top100ResultsCount]";
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
       
		
		public void generateWorklist2()
		{
            worklist1.Invoke((MethodInvoker)(delegate()
            {
                worklist1.Items.Clear();
                //worklist1.Items.Add("190:edqweqwed");

            }));

			Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
			date_picker.Format = DateTimePickerFormat.Custom;
			date_picker.CustomFormat = "yyyy-MM-dd";
			string myDate = date_picker.Text;
            //return;


            //string strSql = "exec [dbo].[GetKeywords_B_M_1] '" + date_picker.Text + "'";
            string strSql = "exec [dbo].[GetKeywords_B_M_190_1] '" + date_picker.Text + "'";
            //string strSql = "exec [dbo].[GetKeywords_B_M_191_1] '" + date_picker.Text + "'";
            //string strSql = "exec [dbo].[GetKeywords_B_01] '" + date_picker.Text + "'";
            //string strSql = "exec [dbo].[GetKeywords_6_01] '" + date_picker.Text + "'";



            //string strSql = "exec [dbo].[GetKeywords_6_01] '" + date_picker.Text + "'";


            SqlConnection objCon = null; 
			SqlDataReader objData = null;
			try
			{
                objCon = new SqlConnection(strCon);
				objCon.Open();
				SqlCommand objCmd = new SqlCommand(strSql,objCon);
				objCmd.CommandTimeout =0;
				objData = objCmd.ExecuteReader(CommandBehavior.CloseConnection);
				while(objData.Read())
				{
                    worklist1.Invoke((MethodInvoker)(delegate()
                    {
                        worklist1.Items.Add(objData[0].ToString() + ":" + objData[1].ToString());                      
                    }));
				}
                worklist1.Invoke((MethodInvoker)(delegate()
                {
                    worklist1.Refresh();
                }));
                objData.Close();              
			}
			catch(SqlException e)
			{
				string errMsg = "Database Connection is temporarily not working\n"+e.ToString();				
               errorList.Invoke((MethodInvoker)(delegate()
               {
                   errorList.Items.Add(errMsg);
               }));
               
			}
			catch(Exception ex)
			{				
               errorList.Invoke((MethodInvoker)(delegate()
               {
                   errorList.Items.Add(ex.ToString());
               }));
			}			
			finally
			{
                if (objCon.State == ConnectionState.Open)
                    objCon.Close();                 
			}
		}

		public void generateWorklist6()
		{
            worklist2.Invoke((MethodInvoker)(delegate()
            {
                worklist2.Items.Clear();               
            }));
           
			Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
			date_picker.Format = DateTimePickerFormat.Custom;
			date_picker.CustomFormat = "yyyy-MM-dd";
			string myDate = date_picker.Text;
            //return;


            string strSql = "exec [dbo].[GetKeywords_B_M_190_2] '" + date_picker.Text + "'";
            //string strSql = "exec [dbo].[GetKeywords_B_M_191_2] '" + date_picker.Text + "'";
            //string strSql = "exec [dbo].[GetKeywords_B_M_190_2] '" + date_picker.Text + "'";
            //string strSql = "exec [dbo].[GetKeywords_B_02] '" + date_picker.Text + "'";
            //string strSql = "exec [dbo].[GetKeywords_6_02] '" + date_picker.Text + "'";



            SqlConnection objCon = null;
			SqlDataReader objData = null;
			try
			{
                objCon = new SqlConnection(strCon);
				objCon.Open();
				SqlCommand objCmd = new SqlCommand(strSql,objCon);
				objCmd.CommandTimeout =0;
				objData = objCmd.ExecuteReader(CommandBehavior.CloseConnection);
				while(objData.Read())
				{
                    worklist2.Invoke((MethodInvoker)(delegate()
                    {
                        worklist2.Items.Add(objData[0].ToString() + ":" + objData[1].ToString());                        
                    }));
				}
                worklist2.Invoke((MethodInvoker)(delegate()
                {
                    worklist2.Refresh();
                }));
                objData.Close();
                
			}
			catch(SqlException e)
			{
				string errMsg = "Database Connection is temporarily not working\n"+e.ToString();				
               errorList.Invoke((MethodInvoker)(delegate()
               {
                   errorList.Items.Add(errMsg);
               }));
			}
			catch(Exception ex)
			{				
                errorList.Invoke((MethodInvoker)(delegate()
                {
				    errorList.Items.Add(ex.ToString());
                }));
			}			
			finally
			{
                if (objCon.State == ConnectionState.Open)
                    objCon.Close();               			
			}
		}

		public void generateWorklist12()
		{
            worklist3.Invoke((MethodInvoker)(delegate()
            {
                worklist3.Items.Clear();                
            }));
            
			Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
			date_picker.Format = DateTimePickerFormat.Custom;
			date_picker.CustomFormat = "yyyy-MM-dd";
			string myDate = date_picker.Text;
            //return;

            string strSql = "exec [dbo].[GetKeywords_B_M_190_3] '" + date_picker.Text + "'";
            //string strSql = "exec [dbo].[GetKeywords_B_M_191_3] '" + date_picker.Text + "'";
            //string strSql = "exec [dbo].[GetKeywords_B_03] '" + date_picker.Text + "'";
            //string strSql = "exec [dbo].[GetKeywords_6_03] '" + date_picker.Text + "'";


            SqlConnection objCon = null;
			SqlDataReader objData = null;
			try
			{
                objCon = new SqlConnection(strCon);
				objCon.Open();
				SqlCommand objCmd = new SqlCommand(strSql,objCon);
				objCmd.CommandTimeout =0;
				objData = objCmd.ExecuteReader(CommandBehavior.CloseConnection);
				while(objData.Read())
				{
                    worklist3.Invoke((MethodInvoker)(delegate()
                    {
                        worklist3.Items.Add(objData[0].ToString() + ":" + objData[1].ToString());                        
                    }));
				}
                worklist3.Invoke((MethodInvoker)(delegate()
                {
                worklist3.Refresh();
                }));
                objData.Close();               
			}
			catch(SqlException e)
			{
				string errMsg = "Database Connection is temporarily not working\n"+e.ToString();
				
                errorList.Invoke((MethodInvoker)(delegate()
               {
                   errorList.Items.Add(errMsg);
               }));
			}
			catch(Exception ex)
			{				 
                errorList.Invoke((MethodInvoker)(delegate()
               {
                   errorList.Items.Add(ex.ToString());
               }));
			}
			
			finally
			{
                if (objCon.State == ConnectionState.Open)
                    objCon.Close();
			}
		}
		
		public int getWorklistSize2() 
		{
			int worklistSize = worklist1.Items.Count;
			return worklistSize;
		}
		
        public int getWorklistSize6() 
		{
			int worklistSize = worklist2.Items.Count;
			return worklistSize;
		}
		
        public int getWorklistSize12() 
		{
			int worklistSize = worklist3.Items.Count;
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

        public void processResults2(string seid, string kn)
		{  			
			ArrayList seresults = new ArrayList();
			date_picker.Format = DateTimePickerFormat.Custom;
			date_picker.CustomFormat = "yyyy-MM-dd";
			string myDate = date_picker.Text;
            string sIP = string.Empty;
            //IPChanger();

            //seresults = new ArrayList(server0.getTop100(HttpUtility.UrlEncode(kn), seid));
            //Dictionary<string, ArrayList> res = server0.getTop100(HttpUtility.UrlEncode(kn), seid,out sIP);
            //seresults = new ArrayList(server0.getTop100(HttpUtility.UrlEncode(kn), seid, out sIP));

            //string sIP = string.Empty;
            seresults = new ArrayList(server0.getTop100(HttpUtility.UrlEncode(kn), seid));

            // get top 100 results for Keyword and Search Engine ID
            results1.Invoke((MethodInvoker)(delegate()
           {
               results1.Items.Clear();
           }));
           
			// check results are okay
			if (seresults.Count<1)
			{
                results1.Invoke((MethodInvoker)(delegate()
                {
               results1.Items.Add("No Results");
               results1.Refresh();
                }));                              
                               
                //errCount1++;
                //if (errCount1 >= 3)
                //{
                //    cnt = maxCnt;
                //    IPChanger();                   
                //}
			}            
            else if (seresults[0].ToString().Contains("Index was outside the bounds of the array"))
            {
                results1.Invoke((MethodInvoker)(delegate()
                {
                    results1.Items.Add("No result" );
                    results1.Refresh();
                }));
                txtError.Invoke((MethodInvoker)(delegate()
                {
                    txtError.Text += seresults[0].ToString() + "\r\n";
                    txtError.Refresh();
                }));                             
            }
            else
            {      
                results1.Invoke((MethodInvoker)(delegate()
                {
                    results1.Items.Add(seid + " " + kn);
                    label1.Text = "No. of URLs :" + seresults.Count.ToString();

                }));

                string path = @"C:\Inetpub\wwwroot\" + xml1;

                string result = string.Empty;
                MemoryStream stream = new MemoryStream();
                using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8))
                {
                    //XmlTextWriter writer = new XmlTextWriter(path, null);

                    writer.Formatting = Formatting.Indented;
                    writer.Indentation = 2;
                    writer.WriteStartDocument();

                    writer.WriteStartElement("", "searchResults", "");
                    writer.WriteStartElement("", "searchResult", "");
                    writer.WriteStartAttribute("searchEngineId");
                    writer.WriteString(seid);
                    writer.WriteStartAttribute("keyword");
                    writer.WriteString(kn);
                    writer.WriteStartAttribute("date");
                    writer.WriteString(myDate);
                    string c = string.Empty;
                    int k;
                    for (int i = 0; i < seresults.Count; i++)
                    {
                        k = (i + 1);
                        c = k.ToString();
                  
                        writer.WriteStartElement("", "url", "");
                        writer.WriteStartAttribute("position");
                        writer.WriteString(c);
                        writer.WriteEndAttribute();
                        writer.WriteString(seresults[i].ToString());
                        writer.WriteEndElement();
                        //InsertDashBoardData_ServerIPs(myDate, kn, seid, k.ToString(), seresults[i].ToString());
                    }
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();

                    writer.Flush();
                    writer.Flush();

                    //writer.Close();

                    UTF8Encoding utf = new UTF8Encoding();
                    result = utf.GetString(stream.GetBuffer(), 0, (int)stream.Length);
                    stream.Close();
                }

                if (!string.IsNullOrEmpty(result))
                {
                    StreamWriter sw = new StreamWriter(path, false);
                    sw.Write(result);
                    sw.Close();
                }

                if (myDate != "")
                {
                    if (seresults.Count > 50)
                    {
                        sendDatatoURL(liveurl, result);
                        InsertDashBoardData(myDate, kn, seid, seresults[0].ToString(),seresults.Count);
                    }
                }
            }
		}

		public void processResults6(string seid,string kn)
		{     			
			ArrayList seresults = new ArrayList();
			date_picker.Format = DateTimePickerFormat.Custom;
			date_picker.CustomFormat = "yyyy-MM-dd";
			string myDate = date_picker.Text;
            
            //PChanger();

            //seresults = new ArrayList(server0.getTop100(HttpUtility.UrlEncode(kn), seid));

            string sIP = string.Empty;
            //seresults = new ArrayList(server0.getTop100(HttpUtility.UrlEncode(kn), seid, out sIP));
            seresults = new ArrayList(server0.getTop100(HttpUtility.UrlEncode(kn), seid));


            // get top 100 results for Keyword and Search Engine ID
            results2.Invoke((MethodInvoker)(delegate()
            {
                  results2.Items.Clear();
            }));
			
			// check results are okay
			if (seresults.Count<1)
			{
                results2.Invoke((MethodInvoker)(delegate()
               {
                   results2.Items.Add("No Results");
                   results2.Refresh();
               }));                 

                //errCount2++;
                //if (errCount2 >= 3)
                //{
                //    cnt = maxCnt;
                //    IPChanger();
                //}
			}             
            else if (seresults[0].ToString().Contains("Index was outside the bounds of the array"))
            {
                results2.Invoke((MethodInvoker)(delegate()
                {
                    results2.Items.Add("no result");
                    results2.Refresh();
                }));
                txtError.Invoke((MethodInvoker)(delegate()
                {
                    txtError.Text += seresults[0].ToString() + "\r\n";
                    txtError.Refresh();
                }));

                //errCount2++;
                //if (errCount2 >= 1)
                //{
                //    cnt = maxCnt;
                //    IPChanger();
                //}
            }
			else
			{
                results2.Invoke((MethodInvoker)(delegate()
                {
                    results2.Items.Add(seid + " " + kn);
                    label2.Text = "No. of URLs :" + seresults.Count.ToString();

                }));

                string path = @"C:\Inetpub\wwwroot\" + xml2;

                string result = string.Empty;

                MemoryStream stream = new MemoryStream();
                using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8))
                {
                    //XmlTextWriter writer = new XmlTextWriter(path, null);

                    writer.Formatting = Formatting.Indented;
                    writer.Indentation = 2;
                    writer.WriteStartDocument();

                    writer.WriteStartElement("", "searchResults", "");
                    writer.WriteStartElement("", "searchResult", "");
                    writer.WriteStartAttribute("searchEngineId");
                    writer.WriteString(seid);
                    writer.WriteStartAttribute("keyword");
                    writer.WriteString(kn);
                    writer.WriteStartAttribute("date");
                    writer.WriteString(myDate);
                    string c = string.Empty;
                    int k;
                    for (int i = 0; i < seresults.Count; i++)
                    {
                        k = (i + 1);
                        c = k.ToString();

                        writer.WriteStartElement("", "url", "");
                        writer.WriteStartAttribute("position");
                        writer.WriteString(c);
                        writer.WriteEndAttribute();
                        writer.WriteString(seresults[i].ToString());
                        writer.WriteEndElement();
                        //InsertDashBoardData_ServerIPs(myDate, kn, seid, k.ToString(), seresults[i].ToString());
                    }
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();

                    writer.Flush();
                    writer.Flush();

                    //writer.Close();

                    UTF8Encoding utf = new UTF8Encoding();
                    result = utf.GetString(stream.GetBuffer(), 0, (int)stream.Length);
                    stream.Close();
                }

                if (!string.IsNullOrEmpty(result))
                {
                    StreamWriter sw = new StreamWriter(path, false);
                    sw.Write(result);
                    sw.Close();
                }

                if (myDate != "")
                {
                    if (seresults.Count > 50)
                    {
                        sendDatatoURL1(liveurl, result);
                        InsertDashBoardData(myDate, kn, seid, seresults[0].ToString(),seresults.Count);
                    }
                }
			}
		}

        
		public void processResults12(string seid, string kn)
		{
			ArrayList seresults = new ArrayList();
			date_picker.Format = DateTimePickerFormat.Custom;
			date_picker.CustomFormat = "yyyy-MM-dd";
			string myDate = date_picker.Text;

            //IPChanger();                       

            //seresults = new ArrayList(server0.getTop100(HttpUtility.UrlEncode(kn), seid));
            string sIP = string.Empty;
            //seresults = new ArrayList(server0.getTop100(HttpUtility.UrlEncode(kn), seid, out sIP));
            seresults = new ArrayList(server0.getTop100(HttpUtility.UrlEncode(kn), seid));


            // get top 100 results for Keyword and Search Engine ID

            //results.Clear();
            results3.Invoke((MethodInvoker)(delegate()
            {
                results3.Items.Clear();
            }));
			// check results are okay
			if (seresults.Count<1)
			{
                results3.Invoke((MethodInvoker)(delegate()
                {
                results3.Items.Add("No Results");
                results3.Refresh();
                }));

                //errCount3++;
                //if (errCount3 >= 3)
                //{
                //    cnt = maxCnt;
                //    IPChanger();
                //}
			}	
            else if (seresults[0].ToString().Contains("Index was outside the bounds of the array"))
            {
                results3.Invoke((MethodInvoker)(delegate()
                {
                    results3.Items.Add("no result");
                    results3.Refresh();
                }));
                txtError.Invoke((MethodInvoker)(delegate()
                {
                    txtError.Text += seresults[0].ToString() + "\r\n";
                    txtError.Refresh();
                }));
            }
			else
			{                				 
                results3.Invoke((MethodInvoker)(delegate()
                {
                    results3.Items.Add(seid + " " + kn);
                    label3.Text = "No. of URLs :" + seresults.Count.ToString();

                }));

                string path = @"C:\Inetpub\wwwroot\" + xml3;

                string result = string.Empty;
                MemoryStream stream = new MemoryStream();
                using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8))
                {
                   
                    writer.Formatting = Formatting.Indented;
                    writer.Indentation = 2;
                    writer.WriteStartDocument();

                    writer.WriteStartElement("", "searchResults", "");
                    writer.WriteStartElement("", "searchResult", "");
                    writer.WriteStartAttribute("searchEngineId");
                    writer.WriteString(seid);
                    writer.WriteStartAttribute("keyword");
                    writer.WriteString(kn);
                    writer.WriteStartAttribute("date");
                    writer.WriteString(myDate);
                    string c = string.Empty;
                    int k;
                    for (int i = 0; i < seresults.Count; i++)
                    {
                        k = (i + 1);
                        c = k.ToString();
                  
                        writer.WriteStartElement("", "url", "");
                        writer.WriteStartAttribute("position");
                        writer.WriteString(c);
                        writer.WriteEndAttribute();
                        writer.WriteString(seresults[i].ToString());
                        writer.WriteEndElement();
                        //InsertDashBoardData_ServerIPs(myDate, kn, seid, k.ToString(), seresults[i].ToString());
                    }
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();

                    writer.Flush();
                    writer.Flush();

                    //writer.Close();

                    UTF8Encoding utf = new UTF8Encoding();
                    result = utf.GetString(stream.GetBuffer(), 0, (int)stream.Length);
                    stream.Close();
                }

                if (!string.IsNullOrEmpty(result))
                {
                    StreamWriter sw = new StreamWriter(path, false);
                    sw.Write(result);
                    sw.Close();
                }

                if (myDate != "")
                {
                    if (seresults.Count > 50)
                    {
                       sendDatatoURL2(liveurl, result);
                       InsertDashBoardData(myDate, kn, seid, seresults[0].ToString(),seresults.Count);
                    }
                }
			}
		}

        void InsertDashBoardData(string ddate, string kwd, string seid, string url,int count)
        {
            //string strInsert = "insert into dashboard_data(date,name,seid,count,url)values(Convert(varchar(10),'" + ddate + "',103),N'" + kwd.Replace("'", "''") + "'," + seid + ",N'" + url.Replace("'", "''") + "')";
            string strInsert = "insert into dashboard_data(date,name,seid,url,count)values(Convert(varchar(10),'" + ddate + "',103),N'" + kwd.Replace("'", "''") + "'," + seid + ",N'" + url.Replace("'", "''") + "'," + count + ")";
            SqlConnection objCon = null;           
            try
            {
                objCon = new SqlConnection(strCon);
                objCon.Open();
                SqlCommand objCmd = new SqlCommand(strInsert, objCon);
                objCmd.CommandTimeout = 0;
                objCmd.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                string errMsg = "Database Connection is temporarily not working\n" + e.ToString();
                errorList.Invoke((MethodInvoker)(delegate()
                {
                    errorList.Items.Add(errMsg);
                }));
            }
            catch (Exception ex)
            {                
                errorList.Invoke((MethodInvoker)(delegate()
                {
                    errorList.Items.Add(ex.ToString());
                }));
            }
            finally
            {
                if(objCon.State == ConnectionState.Open)
                    objCon.Close();             
            }
        }
        void InsertDashBoardData_ServerIPs(string ddate, string kwd, string seid,string rank, string url)
        {
            string strInsert = "insert into dashboard_data_ServerIPs(date,name,seid,rank,url)values(Convert(varchar(10),'" + ddate + "',103),N'" + kwd.Replace("'", "''") + "'," + seid + ", " + rank + ", N'" + url.Replace("'", "''") + "')";
            SqlConnection objCon = null;
            try
            {
                objCon = new SqlConnection(strCon);
                objCon.Open();
                SqlCommand objCmd = new SqlCommand(strInsert, objCon);
                objCmd.CommandTimeout = 0;
                objCmd.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                string errMsg = "Database Connection is temporarily not working\n" + e.ToString();
                errorList.Invoke((MethodInvoker)(delegate()
                {
                    errorList.Items.Add(errMsg);
                }));
            }
            catch (Exception ex)
            {
                errorList.Invoke((MethodInvoker)(delegate()
                {
                    errorList.Items.Add(ex.ToString());
                }));
            }
            finally
            {
                if (objCon.State == ConnectionState.Open)
                    objCon.Close();
            }
        }
                
		public void processWorklist2()
		{			
			string resultsString;
			char sep;
			Array resultsArray;
			string seid;
			string kn;
			try 
			{
				for (int i=0; i<worklist1.Items.Count; i++)					 
				{
					// get next Project ID, Search Engine ID, Keyword ID and Keyword from worklist
					resultsString = worklist1.Items[i].ToString();
					sep = ':';
					resultsArray = resultsString.Split(sep);
					seid = resultsArray.GetValue(0).ToString();
					kn = resultsArray.GetValue(1).ToString();
                    					
					processResults2(seid,kn);
					
					// update progress label
                    progress_seid2.Invoke((MethodInvoker)(delegate()
                    {
                        progress_seid2.Text = "Completed : " + (i+1) + " of " + worklist1.Items.Count;
                        results1.Refresh();
                        progress_seid2.Refresh();
                    }));
                   
				}
			}
			catch(Exception ex)
			{
				string errorMsg = ex.ToString();
                errorList.Invoke((MethodInvoker)(delegate()
               {
                   errorList.Items.Add(errorMsg);
               }));
			}
		}
		
        public void processWorklist6()
		{
			string resultsString;
			char sep;
			Array resultsArray;
			string seid;
			string kn;
			try 
			{
				for (int i=0; i<worklist2.Items.Count; i++)				 
				{
					// get next Project ID, Search Engine ID, Keyword ID and Keyword from worklist
					resultsString = worklist2.Items[i].ToString();
					sep = ':';
					resultsArray = resultsString.Split(sep);
					seid = resultsArray.GetValue(0).ToString();
					kn = resultsArray.GetValue(1).ToString();
					
					processResults6(seid,kn);
					
					// update progress label
                    progress_seid6.Invoke((MethodInvoker)(delegate()
                    {
                        progress_seid6.Text = "Completed : " + (i + 1) + " of " + worklist2.Items.Count;
                        results2.Refresh();
                        progress_seid6.Refresh();
                    }));
                    
				}
			}
			catch(Exception ex)
			{
				string errorMsg = ex.ToString();                
                errorList.Invoke((MethodInvoker)(delegate()
                {
                    errorList.Items.Add(errorMsg);
                }));                
			}
		}
		
        public void processWorklist12()
		{
			string resultsString;
			char sep;
			Array resultsArray;
			string seid;
			string kn;
			try 
			{
				for (int i=0; i<worklist3.Items.Count; i++)					
				{
					// get next Project ID, Search Engine ID, Keyword ID and Keyword from worklist
					resultsString = worklist3.Items[i].ToString();
					sep = ':';
					resultsArray = resultsString.Split(sep);
					seid = resultsArray.GetValue(0).ToString();
					kn = resultsArray.GetValue(1).ToString();
					
					processResults12( seid,kn);
                    progress_seid12.Invoke((MethodInvoker)(delegate()
                    {
                        progress_seid12.Text = "Completed : " + (i + 1) + " of " + worklist3.Items.Count;
                        results3.Refresh();
                        progress_seid12.Refresh();
                    }));
				}
			}
			catch(Exception ex)
			{
				string errorMsg = ex.ToString();
                 errorList.Invoke((MethodInvoker)(delegate()
                {
				errorList.Items.Add(errorMsg);
                }));
			}
		}
		
        public void mainLoop2()
		{
			//generateWorklist2();
			while (getWorklistSize2() > 0) 
			{
				processWorklist2();
				generateWorklist2();
			}         
		}
		
        public void mainLoop6()
        {		
			//generateWorklist6();
			while (getWorklistSize6() > 0) 
			{
				processWorklist6();
				generateWorklist6();
			}
		}
		
        public void mainLoop12()
		{          
			//generateWorklist12();
			while (getWorklistSize12() > 0) 
			{
				processWorklist12();
				generateWorklist12();
			}          
		}
		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.date_picker = new System.Windows.Forms.DateTimePicker();
            this.process_btn = new System.Windows.Forms.Button();
            this.worklist1 = new System.Windows.Forms.ListBox();
            this.worklist2 = new System.Windows.Forms.ListBox();
            this.worklist3 = new System.Windows.Forms.ListBox();
            this.results1 = new System.Windows.Forms.ListBox();
            this.results2 = new System.Windows.Forms.ListBox();
            this.results3 = new System.Windows.Forms.ListBox();
            this.progress_seid2 = new System.Windows.Forms.Label();
            this.progress_seid6 = new System.Windows.Forms.Label();
            this.progress_seid12 = new System.Windows.Forms.Label();
            this.errorList = new System.Windows.Forms.ListBox();
            this.txtError = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // date_picker
            // 
            this.date_picker.CustomFormat = "dd/mm/yyyy";
            this.date_picker.Location = new System.Drawing.Point(8, 8);
            this.date_picker.Name = "date_picker";
            this.date_picker.Size = new System.Drawing.Size(200, 20);
            this.date_picker.TabIndex = 0;
            // 
            // process_btn
            // 
            this.process_btn.Location = new System.Drawing.Point(224, 8);
            this.process_btn.Name = "process_btn";
            this.process_btn.Size = new System.Drawing.Size(75, 23);
            this.process_btn.TabIndex = 1;
            this.process_btn.Text = "Update UK";
            this.process_btn.Click += new System.EventHandler(this.process_btn_Click);
            // 
            // worklist1
            // 
            this.worklist1.Location = new System.Drawing.Point(8, 43);
            this.worklist1.Name = "worklist1";
            this.worklist1.Size = new System.Drawing.Size(128, 693);
            this.worklist1.TabIndex = 2;
            // 
            // worklist2
            // 
            this.worklist2.Location = new System.Drawing.Point(142, 43);
            this.worklist2.Name = "worklist2";
            this.worklist2.Size = new System.Drawing.Size(128, 693);
            this.worklist2.TabIndex = 3;
            // 
            // worklist3
            // 
            this.worklist3.Location = new System.Drawing.Point(276, 43);
            this.worklist3.Name = "worklist3";
            this.worklist3.Size = new System.Drawing.Size(120, 693);
            this.worklist3.TabIndex = 4;
            // 
            // results1
            // 
            this.results1.Location = new System.Drawing.Point(402, 43);
            this.results1.Name = "results1";
            this.results1.Size = new System.Drawing.Size(354, 199);
            this.results1.TabIndex = 5;
            // 
            // results2
            // 
            this.results2.Location = new System.Drawing.Point(402, 271);
            this.results2.Name = "results2";
            this.results2.Size = new System.Drawing.Size(354, 225);
            this.results2.TabIndex = 6;
            // 
            // results3
            // 
            this.results3.Location = new System.Drawing.Point(402, 530);
            this.results3.Name = "results3";
            this.results3.Size = new System.Drawing.Size(354, 199);
            this.results3.TabIndex = 7;
            // 
            // progress_seid2
            // 
            this.progress_seid2.Location = new System.Drawing.Point(432, 16);
            this.progress_seid2.Name = "progress_seid2";
            this.progress_seid2.Size = new System.Drawing.Size(296, 23);
            this.progress_seid2.TabIndex = 8;
            // 
            // progress_seid6
            // 
            this.progress_seid6.Location = new System.Drawing.Point(402, 245);
            this.progress_seid6.Name = "progress_seid6";
            this.progress_seid6.Size = new System.Drawing.Size(336, 23);
            this.progress_seid6.TabIndex = 9;
            // 
            // progress_seid12
            // 
            this.progress_seid12.Location = new System.Drawing.Point(402, 504);
            this.progress_seid12.Name = "progress_seid12";
            this.progress_seid12.Size = new System.Drawing.Size(320, 23);
            this.progress_seid12.TabIndex = 10;
            // 
            // errorList
            // 
            this.errorList.Location = new System.Drawing.Point(8, 752);
            this.errorList.Name = "errorList";
            this.errorList.Size = new System.Drawing.Size(1016, 108);
            this.errorList.TabIndex = 11;
            // 
            // txtError
            // 
            this.txtError.AcceptsReturn = true;
            this.txtError.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtError.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtError.Location = new System.Drawing.Point(762, 43);
            this.txtError.Multiline = true;
            this.txtError.Name = "txtError";
            this.txtError.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtError.Size = new System.Drawing.Size(242, 686);
            this.txtError.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(658, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "#";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(658, 255);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "#";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(658, 514);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "#";
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(1012, 733);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtError);
            this.Controls.Add(this.errorList);
            this.Controls.Add(this.progress_seid12);
            this.Controls.Add(this.progress_seid6);
            this.Controls.Add(this.progress_seid2);
            this.Controls.Add(this.results3);
            this.Controls.Add(this.results2);
            this.Controls.Add(this.results1);
            this.Controls.Add(this.worklist3);
            this.Controls.Add(this.worklist2);
            this.Controls.Add(this.worklist1);
            this.Controls.Add(this.process_btn);
            this.Controls.Add(this.date_picker);
            this.Name = "Form1";
            this.Text = "D_";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		
        #endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		//static void Main() 
		//{
		//	Application.Run(new Form1());
		//}
        		

		private void process_btn_Click(object sender, System.EventArgs e)
		{
			Thread myThread2 = new Thread(new ThreadStart(mainLoop2));
			generateWorklist2();
			
			if (getWorklistSize2()>0)
			{
				myThread2.Start();				
			}
			else
			{               
				myThread2.Abort();
			}
			Thread myThread6 = new Thread(new ThreadStart(mainLoop6));
			generateWorklist6();
			if (getWorklistSize6()>0)
			{                
				myThread6.Start();				
			}
			else
			{                
                myThread6.Abort();
			}
			Thread myThread12 = new Thread(new ThreadStart(mainLoop12));

			generateWorklist12();
			if (getWorklistSize12()>0)
			{
				myThread12.Start();				
			}
			else
			{
				myThread12.Abort();
			}        
		}

        private void Form1_Load(object sender, System.EventArgs e)
		{
            //liveurl = readAPI();
            /*if (File.Exists("index.txt"))
            {
                StreamReader sw = new StreamReader("index.txt");
                string val = sw.ReadLine();
                sw.Close();

                if (Convert.ToInt32(val) >= 0) server0.x = Convert.ToInt32(val);
            }

            if (server0.x >= server0.dtIPs.Rows.Count)
            {
                server0.x = 0;

                              
                //StreamWriter sw = new StreamWriter("index.txt", false);
                //sw.WriteLine(server0.x);
                //sw.Close();
                //Environment.Exit(Environment.ExitCode);
                
            }*/


            //this.Text = "Bing(15-17-34-37)_1-2-3_Server-5_GT50_"+server0.dtIPs.Rows[server0.x][1].ToString();
            this.Text = "Bing(15-17-34-37)_1-2-3_Proxies_" + server0.dtIPs.Rows[server0.x][1].ToString();


            Thread myThread2 = new Thread(new ThreadStart(mainLoop2));
            generateWorklist2();

            if (getWorklistSize2() > 0)
            {
                myThread2.Start();
            }
            else
            {
                myThread2.Abort();
            }

            Thread myThread6 = new Thread(new ThreadStart(mainLoop6));
            generateWorklist6();
            if (getWorklistSize6() > 0)
            {
                myThread6.Start();
            }
            else
            {
                myThread6.Abort();
            }

            Thread myThread12 = new Thread(new ThreadStart(mainLoop12));
            generateWorklist12();
            if (getWorklistSize12() > 0)
            {
                myThread12.Start();
            }
            else
            {
                myThread12.Abort();
            }            	
		}
        void sendDatatoURL(string api, string result)
        {

            string user = "pisoftware";
            string pwd = "r00t123456";
            try
            {
                Uri uri = new Uri(api);
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(uri);
                httpWReq.UseDefaultCredentials = true;
                httpWReq.PreAuthenticate = true;
                httpWReq.Credentials = CredentialCache.DefaultCredentials;

                Encoding encoding = new UTF8Encoding();
                //string postData = GetTextFromXMLFile(api);
                byte[] data = encoding.GetBytes(result);

                httpWReq.ProtocolVersion = HttpVersion.Version11;
                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/x-www-form-urlencoded"; //charset=UTF-8";  


                string auth = string.Format("{0}:{1}", user, pwd);
                string enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(auth));
                string cred = string.Format("{0} {1}", "Basic", enc);


                httpWReq.Headers[HttpRequestHeader.Authorization] = cred;
                httpWReq.ContentLength = data.Length;

                Stream stream = httpWReq.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                string s = response.ToString();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                //System.Threading.Thread.Sleep(2000);
                String xmlResponse = "";
                String temp = null;
                while ((temp = reader.ReadLine()) != null)
                {
                    xmlResponse += temp;
                }
                //MessageBox.Show(xmlResponse);
                reader.Close();
                response.Close();
            }
            catch (WebException ex)
            {
                string error = "";
                string message = "";

                using (WebResponse response = ex.Response)
                {
                    if (response != null)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)response;
                        error = string.Format("Error:{0}", httpResponse.StatusCode);

                        using (Stream data = response.GetResponseStream())
                        using (var reader = new StreamReader(data))
                        {
                            message = reader.ReadToEnd();
                        }
                    }
                }

                this.Invoke((MethodInvoker)delegate()
                {
                    txtError.Text += ex.Message + "\r\n";
                });

                throw new Exception(ex.Message);

            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        void sendDatatoURL1(string api, string result)
        {


            string user = "pisoftware";
            string pwd = "r00t123456";
            try
            {
                Uri uri = new Uri(api);
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(uri);
                httpWReq.UseDefaultCredentials = true;
                httpWReq.PreAuthenticate = true;
                httpWReq.Credentials = CredentialCache.DefaultCredentials;

                Encoding encoding = new UTF8Encoding();
                //string postData = GetTextFromXMLFile(api);
                byte[] data = encoding.GetBytes(result);

                httpWReq.ProtocolVersion = HttpVersion.Version11;
                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/x-www-form-urlencoded"; //charset=UTF-8";  


                string auth = string.Format("{0}:{1}", user, pwd);
                string enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(auth));
                string cred = string.Format("{0} {1}", "Basic", enc);


                httpWReq.Headers[HttpRequestHeader.Authorization] = cred;
                httpWReq.ContentLength = data.Length;

                Stream stream = httpWReq.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                string s = response.ToString();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                //System.Threading.Thread.Sleep(2000);
                String xmlResponse = "";
                String temp = null;
                while ((temp = reader.ReadLine()) != null)
                {
                    xmlResponse += temp;
                }
                //MessageBox.Show(xmlResponse);
                reader.Close();
                response.Close();
            }
            catch (WebException ex)
            {
                string error = "";
                string message = "";

                using (WebResponse response = ex.Response)
                {
                    if (response != null)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)response;
                        error = string.Format("Error:{0}", httpResponse.StatusCode);

                        using (Stream data = response.GetResponseStream())
                        using (var reader = new StreamReader(data))
                        {
                            message = reader.ReadToEnd();
                        }
                    }
                }

                this.Invoke((MethodInvoker)delegate()
                {
                    txtError.Text += ex.Message + "\r\n";
                });

                throw new Exception(ex.Message);

            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        void sendDatatoURL2(string api, string result)
        {


            string user = "pisoftware";
            string pwd = "r00t123456";
            try
            {
                Uri uri = new Uri(api);
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(uri);
                httpWReq.UseDefaultCredentials = true;
                httpWReq.PreAuthenticate = true;
                httpWReq.Credentials = CredentialCache.DefaultCredentials;

                Encoding encoding = new UTF8Encoding();
                //string postData = GetTextFromXMLFile(api);
                byte[] data = encoding.GetBytes(result);

                httpWReq.ProtocolVersion = HttpVersion.Version11;
                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/x-www-form-urlencoded"; //charset=UTF-8";  


                string auth = string.Format("{0}:{1}", user, pwd);
                string enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(auth));
                string cred = string.Format("{0} {1}", "Basic", enc);


                httpWReq.Headers[HttpRequestHeader.Authorization] = cred;
                httpWReq.ContentLength = data.Length;

                Stream stream = httpWReq.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                string s = response.ToString();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                //System.Threading.Thread.Sleep(2000);
                String xmlResponse = "";
                String temp = null;
                while ((temp = reader.ReadLine()) != null)
                {
                    xmlResponse += temp;
                }
                //MessageBox.Show(xmlResponse);
                reader.Close();
                response.Close();
            }
            catch (WebException ex)
            {
                string error = "";
                string message = "";

                using (WebResponse response = ex.Response)
                {
                    if (response != null)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)response;
                        error = string.Format("Error:{0}", httpResponse.StatusCode);

                        using (Stream data = response.GetResponseStream())
                        using (var reader = new StreamReader(data))
                        {
                            message = reader.ReadToEnd();
                        }
                    }
                }

                this.Invoke((MethodInvoker)delegate()
                {
                    txtError.Text += ex.Message + "\r\n";
                });

                throw new Exception(ex.Message);

            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        private string GetTextFromXMLFile(string file)
        {
            StreamReader reader = new StreamReader(file);
            string ret = reader.ReadToEnd();
            reader.Close();
            return ret;
        }
                      		
	}
}

