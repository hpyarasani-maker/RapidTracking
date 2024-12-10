using System.Collections.Generic;

namespace AIOSending
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
        public bool parse { get; set; }//23-09-2021 changed datatype into "int to bool"

        public string callback_url { get; set; }
        public string user_agent_type { get; set; }
        public string render { get; set; } //comment for desktop and uncomment to mobile
        public List<browser_instruction> browser_instructions
        {
            get; set;
        }//comment for deskto and uncomment to mobile
        //public List<Context> context { get; set; } uncomment for desktop and comment for mobile
    }
    public class browser_instruction {
        public string type { get; set; }
        public selector selector { get; set; }
    }
    public class selector {
        public string type { get; set; }
        public string value { get; set; }
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
