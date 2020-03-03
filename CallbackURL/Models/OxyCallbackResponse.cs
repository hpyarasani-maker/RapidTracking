using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CallbackURL.Models
{
    public class OxyCallbackResponse
    {
        public string locale { get; set; }
        public int client_id { get; set; }
        public string user_agent_type { get; set; }
        public string source { get; set; }
        public string status { get; set; }
        public string callback_url { get; set; }
        public string domain { get; set; }
        public string geo_location { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string query { get; set; }
        public string parent_uuid { get; set; }
        public string id { get; set; }
        public string results_url { get; set; }  
    }
}