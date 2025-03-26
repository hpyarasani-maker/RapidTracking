using System.Collections.Generic;

namespace Image_Page_Loop_Keywords
{
    class SearchParams
    {
        public static IList<SearchProperties> searches = new List<SearchProperties>()
        {

            new SearchProperties()  //UKDesktopImages_PageURLs Desktop
            {
                seid=74,domain="co.uk",geo_location="United Kingdom", locale="en-gb", uule="w+CAIQICIOVW5pdGVkIEtpbmdkb20=",device="desktop",tbm="isch"
            },
           new SearchProperties()  //UKMobileImages_PageURLs mobile
            {
                seid=382,domain="co.uk",geo_location="United Kingdom", locale="en-gb", uule="w+CAIQICIOVW5pdGVkIEtpbmdkb20=",device="mobile_android",tbm="isch"
            },
            new SearchProperties() //News 
            {
                seid=140,domain="co.uk",geo_location="United Kingdom", locale="en-gb", uule="w+CAIQICIOVW5pdGVkIEtpbmdkb20=",device="desktop",tbm="nws"
            },

            new SearchProperties() //UKDesktopImages_ImageURLs
            {
                seid=401,domain ="co.uk",geo_location="United Kingdom", locale="en-gb", uule="w+CAIQICIOVW5pdGVkIEtpbmdkb20=",device="desktop",tbm="isch"
            },

            new SearchProperties()   //UKMobileImages_ImageURLs
            {
                seid=402,domain="co.uk",geo_location="United Kingdom", locale="en-gb", uule="w+CAIQICIOVW5pdGVkIEtpbmdkb20=",device="mobile_android",tbm="isch"
            },

        };
    }
}
