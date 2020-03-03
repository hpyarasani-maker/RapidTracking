using System.Collections.Generic;

namespace TrendingSending
{
    class OxyParams
    {
        public string source { get; set; }
        public string domain { get; set; }
        public string[] query { get; set; }
        public int limit { get; set; }
        public int pages { get; set; }
        public string locale { get; set; }
        public string geo_location { get; set; }        
        public int parse { get; set; }
        public string callback_url { get; set; }
        public string user_agent_type { get; set; }

        public List<Context> context { get; set; }
    }

    class Context
    {
        public string key { get; set; }
        public dynamic value { get; set; }

        public Context(string key, dynamic value)
        {
            this.key = key;
            this.value = value;
        }
    }
}
