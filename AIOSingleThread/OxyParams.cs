using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIOSingleThread
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

        public string user_agent_type { get; set; }
        public string render { get; set; } //comment for desktop and uncomment to mobile
        public List<dynamic> browser_instructions;//comment for deskto and uncomment to mobile
        //public List<Context> context { get; set; } uncomment for desktop and comment for mobile
    }
    public class Browser_Instruction
    {
        public string type { get; set; }
        public Selector selector { get; set; }
    }
    public class Selector
    {
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
