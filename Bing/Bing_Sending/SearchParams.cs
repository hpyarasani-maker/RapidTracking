using System.Collections.Generic;

namespace Bing_Sending
{ 
    class SearchParams
    {
        public static IList<SearchProperties> searches = new List<SearchProperties>()
        {
            new SearchProperties()
            {
                seid =5, domain ="com", geo_location="United States", language="en", device="desktop"
            },
            new SearchProperties()
            {
                seid =6, domain ="com", geo_location="United Kingdom", language="en", device="desktop"
            },
            new SearchProperties()
            {
                seid =15, domain ="com", geo_location="New Zealand", language="en", device="desktop"
            },
            new SearchProperties()
            {
                seid =17, domain ="com", geo_location="South Africa", language="en", device="desktop"
            },
            new SearchProperties()
            {
                seid =34, domain ="com", geo_location="Italy", language="en", device="desktop"
            },
            new SearchProperties()
            {
                seid =37, domain ="com", geo_location="France", language="en", device="desktop"
            },
            new SearchProperties()
            {
                seid =190, domain ="com", geo_location="United States", language="en", device="mobile"
            },
              new SearchProperties()
            {
                seid =191, domain ="com", geo_location="United Kingdom", language="en" , device="mobile"
            },
        };
    }
    class SearchProperties
    {
        public int seid { get; set; }
        public string domain { get; set; }
        public string query { get; set; }
        public string geo_location { get; set; }
        public string language { get; set; }
        public string device { get; set; }
    }
   
    
}
