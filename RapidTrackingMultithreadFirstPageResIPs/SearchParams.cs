using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapidTrackingMultithreadFirstPageResIPs
{
    public class SearchParams
    {
        public static IList<SearchProperties> searches = new List<SearchProperties>()
        {
            new SearchProperties()
            {
                seid =1, domain ="com", country="cc-US", locale="en-us", uule="w+CAIQICINVW5pdGVkIFN0YXRlcw==",device="desktop_chrome"
            },
            new SearchProperties()
            {
                seid =58, domain ="co.uk", country="cc-UK", locale= "en-gb",uule = "w+CAIQICIOVW5pdGVkIEtpbmdkb20=",device="desktop_chrome"
            },
            new SearchProperties()
            {
                seid =102, domain ="com", country="cc-US", locale="en-us", uule="w+CAIQICINVW5pdGVkIFN0YXRlcw==",device="mobile_android"
            },
            new SearchProperties()
            {
                seid =106, domain ="co.uk", country="cc-UK", locale= "en-gb",uule = "w+CAIQICIOVW5pdGVkIEtpbmdkb20=",device="mobile_android"
            },
        };

        public class SearchProperties
        {
            public int seid { get; set; }
            public string domain { get; set; }
            public string country { get; set; }
            public string locale { get; set; }
            public string uule { get; set; }
            public string device { get; set; }
        }


    }
    
}


