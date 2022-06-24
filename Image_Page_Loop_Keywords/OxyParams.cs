using System.Collections.Generic;

namespace Image_Page_Loop_Keywords
{
    class OxyParams
    {
        public string source { get; set; }
        public string domain { get; set; }
        public string[] query { get; set; }
        public int limit { get; set; }
        public int pages { get; set; }
        public int start_page { get; set; }
        public string locale { get; set; }
        public string geo_location { get; set; }      
        public bool parse { get; set; }//23-09-2021 changed datatype into "int to bool"
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
