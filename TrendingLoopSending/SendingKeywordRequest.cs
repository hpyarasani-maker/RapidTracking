using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Xml;
using System.Data.SqlClient;
using System.Data;

namespace TrendingLoopSending
{
    class SendingKeywordRequest
    {        
        public string strConn()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                string fileName = @"C:\Inetpub\wwwroot\TrendingLiveAPI.xml";
                //string fileName = @"C:\Inetpub\wwwroot\downloadKeywords.xml";


                // You'll need to put the correct path to your xml file here
                xml.Load(fileName);

                // Select a specific node
                XmlNode node = xml.SelectSingleNode("TrendingAPI/con");

                // Get its value
                string name = node.InnerText;

                return name;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
           
        private void SendToDb(int seid, string response)
        {
            string date = DateTime.Today.ToString("yyyy-MM-dd");
            StringBuilder sb = new StringBuilder();

            JObject jo = JObject.Parse(response);
            var links = from p in jo["queries"] select p;

            var datetime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            foreach (JToken link in links)
            {
                string kw = link["query"].Value<string>();
                string jobid = link["id"].Value<string>();

                string qry = "insert into dashboard_data_sending (date, name, seid, jobid) values('" + datetime + "', N'" + kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "'); ";
                //string qry = "insert into dashboard_data_sendingP (date, name, seid, jobid) values('" + datetime + "', N'" + kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "'); "; //previous date

                sb.Append(qry);
            }

            try
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                {
                    using (SqlConnection con = new SqlConnection(strConn()))
                    {
                        con.Open();
                        using (SqlCommand comm = new SqlCommand(sb.ToString(), con))
                        {
                            comm.CommandTimeout = 0;
                            comm.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
        /// <summary>
        /// //03-08-2021 storing error messages
        /// </summary>
        /// <param name="seid"></param>
        /// <param name="response"></param>
        private void ProcessError(int seid, string response)
        {
            string date = DateTime.Today.ToString("yyyy-MM-dd");
            StringBuilder sb = new StringBuilder();

            var datetime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            string ErrorQry = "insert into dashboard_dataerrorsSending (date, seid, error) values('" + datetime + "', " + seid + ", N'" + response + "'); ";
            sb.Append(ErrorQry);

            try
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                {
                    using (SqlConnection con = new SqlConnection(strConn()))
                    {
                        con.Open();
                        using (SqlCommand comm = new SqlCommand(sb.ToString(), con))
                        {
                            comm.CommandTimeout = 0;
                            comm.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
        ////03-08-2021 storing error messages end

        private void GetOxylabsWebDataSources(SearchProperties sp)
        {
            //ServicePointManager.Expect100Continue = true;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Uri queryUri = new Uri("https://data.oxylabs.io/v1/queries/batch"); //29-07-2021 changed https:// using http://
            string username = "gpidatametrics";
            string password = "sdV5X3fcX6";
            string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));

            string callbackURL = "http://seresults.azurewebsites.net/api/callbacktrendingdesktop/";       // Desktop
           // string callbackURL = "http://seresults.azurewebsites.net/api/callbacktrendingmobile/";       // Mobile

            string[] keyword = { sp.query };
            OxyParams op = new OxyParams()
            {
                source = "google_search",
                domain = sp.domain,
                query = keyword,
                limit = 10,
                pages = 10,
                start_page=1,
                locale = sp.locale,
                callback_url = callbackURL,  
                geo_location = sp.geo_location,
                parse = false, //23-09-2021 changed datatype into "int to bool"
                user_agent_type = sp.device,   
                context = new List<Context> {
                    new Context("safe_search", 0)     
                }
            };                  
            
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(queryUri);
            req.Headers.Clear();

            req.Method = "POST";
            req.ContentType = "application/json";            
            req.Headers.Add(HttpRequestHeader.Authorization, "Basic " + authInfo);

            using (var streamWriter = new StreamWriter(req.GetRequestStream()))
            {
                var json = JsonConvert.SerializeObject(op, new JsonSerializerSettings
                {
                    Formatting = Newtonsoft.Json.Formatting.Indented,
                });
               
                streamWriter.Write(json);
            }

            string response;

            try
            {
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                using (StreamReader reader = new StreamReader(res.GetResponseStream()))
                {
                    response = reader.ReadToEnd();
                }
                res.Close();

                SendToDb(sp.seid, response);
                
            }
            catch(Exception ex)
            {
                ProcessError(sp.seid, ex.Message.ToString()); //03-08-2021 storing error messages
                throw ex;
            }
        }
        
        public void getTop100(string keyword, int seid)
        {
            try
            {
                SearchProperties sp = SearchParams.searches.Where(s => s.seid == seid).SingleOrDefault();
                sp.query = keyword;
                if(sp != null)
                    GetOxylabsWebDataSources(sp);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}


