using System.Collections.Generic;

namespace AIOSending
{
    class SearchParams
    {         
        public static IList<SearchProperties> searches = new List<SearchProperties>()
        {
            new SearchProperties()//Seid's for AI overview testing  1026 to 1029
            {
                seid =1026, domain ="co.uk",geo_location="United Kingdom",locale= "en-gb",uule = "w+CAIQICIOVW5pdGVkIEtpbmdkb20=",device="desktop"
            },
            new SearchProperties()
            {
                seid =1027, domain ="co.uk", geo_location="United Kingdom", locale = "en-gb", uule = "w+CAIQICIOVW5pdGVkIEtpbmdkb20=",device="mobile_android"
            },
            new SearchProperties()
            {
                seid =1028, domain ="com", geo_location="United States", locale="en-us", uule="w+CAIQICINVW5pdGVkIFN0YXRlcw==", device="desktop"
            },
            new SearchProperties()
            {
                seid =1029, domain ="com", geo_location="United States", locale = "en-us", uule = "w+CAIQICINVW5pdGVkIFN0YXRlcw==",device="mobile_android"
            },//Seid's for AI overview testing 1026 to 1029
        };
    }

    class SearchProperties
    {
        public int seid { get; set; }
        public string domain { get; set; }
        public string query { get; set; }
        public string geo_location { get; set; }
        public string locale { get; set; }
        public string uule { get; set; }
        public string tbm { get; set; }
        public string device { get; set; }
    }
   
    
}
