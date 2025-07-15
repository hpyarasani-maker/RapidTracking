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

namespace RapidTrackingLoopSending
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
            string username = string.Empty;//04-02-2025
            string password = string.Empty;
            if (sp.device == "mobile_android")
            {
                username = "piapp";
                password = "b5FCvgkjxx";
            }
            else if (sp.device == "desktop_chrome")
            {
                username = "piapp-aio";
                password = "4gvfnA+aBYpBNs37";
            }//04-03-2025
            string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));

            //string callbackURL = "https://seresults.azurewebsites.net/api/callbackrapidtrackingdesktop/";  // rapid tracking other desktop
            //string callbackURL = "https://seresults.azurewebsites.net/api/callbackrapidtrackingmobile/";  // rapid tracking other mobile
            //string callbackURL = "https://seresults.azurewebsites.net/api/callbackrapidtrackingcommakeywords/";  // rapid tracking comma keywords
            //string callbackURL = "https://seresults.azurewebsites.net/api/callbackrapidtrackingmobilehotel/";
            //string callbackURL = "https://seresults.azurewebsites.net/api/callbackrapidTrackingnewdesktop/"; //Desktop new keywords
            //string callbackURL = "https://seresults.azurewebsites.net/api/callbackrapidTrackingnewmobile/"; //mobile new keywords
            // string callbackURL = "https://seresults.azurewebsites.net/api/callbackrapidtrackingnewcommakeywords/";  // new comma keywords

            //string callbackURL = "https://seresults.azurewebsites.net/api/callbackuk503desktoptemp"; //sending new SEIDs
            //string callbackURL = "https://seresults.azurewebsites.net/api/trackingtrending/";

            //string callbackURL = "https://seresults.azurewebsites.net/api/callbackuk58desktop/";       // 58
            //string callbackURL = "https://seresults.azurewebsites.net/api/callbackuk106mobile/";       // 106
            //string callbackURL = "https://seresults.azurewebsites.net/api/callbackus1desktop/";     // 1            
            //string callbackURL = "https://seresults.azurewebsites.net/api/callbackus102mobile/";     // 102
            string callbackURL = "https://seresults.azurewebsites.net/api/callbackotherdesktop/";  // Loop other desktop
            //string callbackURL = "https://seresults.azurewebsites.net/api/callbackothermobile/";  // other mobile

            //string callbackURL = "https://seresults.azurewebsites.net/api/callbackimages/"; // images
            //string callbackURL = "https://seresults.azurewebsites.net/api/callbacknews/"; // news

            //string callbackURL = "https://previous.azurewebsites.net/api/callbackrapidtrackingdesktop/";  // rapid tracking other desktop
            //string callbackURL = "https://previous.azurewebsites.net/api/callbackrapidtrackingcommakeywords/";  // rapid tracking comma keywords
            //string callbackURL = "https://previous.azurewebsites.net/api/callbackrapidtrackingmobilehotel/";
            //string callbackURL = "https://previous.azurewebsites.net/api/callbackrapidtrackingmobile/";
            //string callbackURL = "https://previous.azurewebsites.net/api/callbackrapidtrackingdesktop/";
            // string callbackURL = "https://previous.azurewebsites.net/api/callbackus1desktop/";     // 1 2019-10-10
            //string callbackURL = "https://previous.azurewebsites.net/api/callbackuk58desktop/";       // 58 sending for previous date
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

            using (var streamWriter = new StreamWriter(await req.GetRequestStreamAsync()))
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


