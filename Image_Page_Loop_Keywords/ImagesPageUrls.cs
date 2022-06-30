using System.Collections.Generic;

namespace Image_Page_Loop_Keywords
{
    class SearchParamsPageUrls 
    {

        public static IList<SearchProperties> searches = new List<SearchProperties>()
        {
            new SearchProperties()  //UKDesktopImages_PageURLs
            {
                seid = 74,domain ="co.uk",geo_location="United Kingdom", locale="en-gb", uule="w+CAIQICIOVW5pdGVkIEtpbmdkb20=",device="desktop",tbm="isch"
            },
            new SearchProperties()  //UKMobileImages_PageURLs
            {
                seid = 382,domain ="co.uk",geo_location="United Kingdom", locale="en-gb", uule="w+CAIQICIOVW5pdGVkIEtpbmdkb20=",device="mobile_android",tbm="isch"
            },

           
        };
    }

    //class SearchProperties
    //{
    //    public int seid { get; set; }
    //    public string domain { get; set; }
    //    public string query { get; set; }
    //    public string geo_location { get; set; }
    //    public string locale { get; set; }
    //    public string uule { get; set; }
    //    public string device { get; set; }

    //    public string tbm { get; set; }
    //}


}
