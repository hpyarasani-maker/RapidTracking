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
using System.Threading.Tasks;

namespace AIOSending
{
    class SendingKeywordRequest
    {        
        public async Task<string> strConn()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                string fileName = @"C:\Inetpub\wwwroot\Callback_TrackingTrending.xml";
                //string fileName = @"C:\Inetpub\wwwroot\downloadKeywords.xml";


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
           
        private async Task SendToDb(int seid, string response)
        {
            string date = DateTime.Today.ToString("yyyy-MM-dd");
            StringBuilder sb = new StringBuilder();

            JObject jo = JObject.Parse(response);
            var links = from p in jo["queries"] select p;

            var datetime = DateTime.Today.ToString("yyyy-MM-dd");
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
                    using (SqlConnection con = new SqlConnection(await strConn()))
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
        private async Task ProcessError(int seid, string response)
        {
            string date = DateTime.Today.ToString("yyyy-MM-dd");
            StringBuilder sb = new StringBuilder();

            var datetime = DateTime.Today.ToString("yyyy-MM-dd");
            string ErrorQry = "insert into dashboard_dataerrorsSending (date, seid, error) values('" + datetime + "', " + seid + ", N'" + response + "'); ";
            sb.Append(ErrorQry);

            try
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                {
                    using (SqlConnection con = new SqlConnection(await strConn()))
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

        private async Task GetOxylabsWebDataSources(SearchProperties sp)
        {
            //ServicePointManager.Expect100Continue = true;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Uri queryUri = new Uri("https://data.oxylabs.io/v1/queries/batch"); //29-07-2021 changed https:// using https://
            //string username = "gpidatametrics";
            //string password = "sdV5X3fcX6";
            string username = "piapp-aio";
            string password = "4gvfnA+aBYpBNs37";
            string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));



            //string callbackURL = "https://seresults.azurewebsites.net/api/callbackrapidtrackingcommakeywords/";  // rapid tracking comma keywords
            //string callbackURL = "https://seresults.azurewebsites.net/api/callbackuk58desktop/";       //1026,1027,1028,1029 AIO Keyword Tracking Desktop and Mobile
            string callbackURL = "https://seresults.azurewebsites.net/api/callbackuk106mobile/"; //mobile AIO keywords sending desktop and mobile

            OxyParams op = new OxyParams()
            {
                source = "google_search",
                domain = sp.domain,
                query = sp.query.Split(','),
                limit = 100,
                pages = 1,
                locale = sp.locale,
                callback_url = callbackURL,
                geo_location = sp.geo_location,
                parse = false, //23-09-2021 changed datatype into "int to bool"
                user_agent_type = sp.device,
                render = "html", //comment for desktop and uncomment for mobile
                browser_instructions = new List<dynamic>
                {
                    new {
                        type = "click",
                        selector = new Selector {
                            type = "xpath",
                            value = "//div[contains(@class,'zNsLfb Jzkafd')]/div/div"
                        }
                    },
                    new
                    {
                        type = "wait",
                        wait_time_s = 5
                    }
                }//comment for desktop and uncomment for mobile
                /*context = new List<Context> { //comment for mobile and uncomment for desktop
                    new Context("tbm", sp.tbm),
                    new Context("safe_search", 0)
                    //,new Context("aomd",1)
                }*/
            };

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(queryUri);
            req.Headers.Clear();

            req.Method = "POST";
            req.ContentType = "application/json";            
            req.Headers.Add(HttpRequestHeader.Authorization, "Basic " + authInfo);

            using (var streamWriter = new StreamWriter(await req.GetRequestStreamAsync()))
            {
                var json = JsonConvert.SerializeObject(op, new JsonSerializerSettings
                {
                    Formatting = Newtonsoft.Json.Formatting.Indented,
                });
               
                streamWriter.Write(json);
            }

            string response=string.Empty;//22-03-2024

            try
            {
                HttpWebResponse res = (HttpWebResponse)await req.GetResponseAsync();
                using (StreamReader reader = new StreamReader(res.GetResponseStream()))
                {
                    response = reader.ReadToEnd();
                }
                res.Close();

                await SendToDb(sp.seid, response);
                
            }
            catch(Exception ex)
            {
                await ProcessError(sp.seid, ex.Message.ToString()); //03-08-2021 storing error messages
                throw ex;
            }
            finally//22-03-2024
            {
                response = string.Empty;
            }//22-03-2024
        }
        
        public async Task getTop100(string keyword, int seid)
        {
            try
            {
                SearchProperties sp = SearchParams.searches.Where(s => s.seid == seid).SingleOrDefault();
                sp.query = keyword;
                if(sp != null)
                    await GetOxylabsWebDataSources(sp);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}


