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

namespace TrendingSending
{
    class SendingKeywordRequest
    {        
        public async Task<string> strConn()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                string fileName = @"C:\Inetpub\wwwroot\TrendingLiveAPI.xml";
                //string fileName = @"C:\Inetpub\wwwroot\ServerIP_Callback.xml";

                // You'll need to put the correct path to your xml file here
                xml.Load(fileName);

                // Select a specific node
                XmlNode node = xml.SelectSingleNode("TrendingAPI/con");
                //XmlNode node = xml.SelectSingleNode("ConnectionString/con");

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

            var datetime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            foreach (JToken link in links)
            {
                string kw = link["query"].Value<string>();
                string jobid = link["id"].Value<string>();

                string qry = "insert into dashboard_data_sending (date, name, seid, jobid) values('" + datetime + "', N'" + kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "'); ";
                //string qry = "insert into dashboard_data_sending (date, name, seid, jobid) values(Convert(varchar(10),'" + date + "',103), N'" + kw.Replace("'", "''") + "', " + seid + ", '" + jobid + "'); ";
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
                            await comm.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task GetOxylabsWebDataSources(SearchProperties sp)
        {
            //ServicePointManager.Expect100Continue = true;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Uri queryUri = new Uri("https://data.oxylabs.io/v1/queries/batch");
            string username = string.Empty;//04-02-2025
            string password = string.Empty;
            if (sp.device == "mobile")
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

            //string callbackURL = "https://seresults.azurewebsites.net/api/callbacktrendingdesktop/";       // Desktop
            string callbackURL = "https://seresults.azurewebsites.net/api/callbacktrendingmobile/";       // Mobile

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
                parse = false,  //23-09-2021 changed datatype into "int to bool"
                user_agent_type = sp.device,   
                context = new List<Context> {
                    new Context("tbm", sp.tbm),
                    new Context("safe_search", 0)
                    //new Context("nfpr", false)                 
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
                    response = await reader.ReadToEndAsync();
                }
                res.Close();

                await SendToDb(sp.seid, response);
            }
            catch(Exception ex)
            {
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


