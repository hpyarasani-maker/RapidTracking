using System.Collections.Generic;

namespace Image_Page_Results_Receiving
{
    class SearchParamsPageUrls 
    {

        public static IList<SearchProperties> searches = new List<SearchProperties>()
        {
            new SearchProperties()  //UKDesktopImages_PageURLs
            {
                seid = 74,domain ="co.uk",geo_location="United Kingdom", locale="en-gb", uule="w+CAIQICIOVW5pdGVkIEtpbmdkb20=",device="desktop_chrome",tbm="isch"
            },
            new SearchProperties()  //UKMobileImages_PageURLs
            {
                seid = 382,domain ="co.uk",geo_location="United Kingdom", locale="en-gb", uule="w+CAIQICIOVW5pdGVkIEtpbmdkb20=",device="mobile_android",tbm="isch"
            },

        };
    }
}
