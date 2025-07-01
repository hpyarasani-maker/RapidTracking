using System.Collections.Generic;

namespace Bing_Receiving
{ 
    class SearchParams
    {
        public static IList<SearchProperties> searches = new List<SearchProperties>()
        {
            new SearchProperties()
            {
                seid = 5, domain ="com", geo_location="United States", locale="en-us", device="desktop"
            },
            new SearchProperties()
            {
                seid = 6, domain ="co.uk", geo_location="United Kingdom", locale="en-gb", device="desktop"
            },
            new SearchProperties()
            {
                seid = 15, domain ="co.nz", geo_location="New Zealand", locale="en-nz", device="desktop"
            },
            new SearchProperties()
            {
                seid = 17, domain ="co.za", geo_location="South Africa", locale="en-za", device="desktop"
            },
            new SearchProperties()
            {
                seid = 34, domain ="it", geo_location="Italy", locale="en-it", device="desktop"
            },
            new SearchProperties()
            {
                seid = 37, domain ="fr", geo_location="France", locale="en-it", device="desktop"
            },
            new SearchProperties()
            {
                seid = 190, domain ="com", geo_location="United States", locale="en-us", device="mobile"
            },
            new SearchProperties()
            {
                seid = 191, domain ="co.uk", geo_location="United Kingdom", locale="en-gb" , device="mobile"
            },
            new SearchProperties()//01-07-2025
            {
                seid = 1045, domain ="it", geo_location="Italy", locale="it-it" , device="mobile"
            },
            new SearchProperties()
            {
                seid = 1046, domain ="it", geo_location="Italy", locale="en-it" , device="mobile"
            },
            new SearchProperties()
            {
                seid = 1047, domain ="com.ua", geo_location="Ukraine", locale="uk-ua" , device="mobile"
            },
            new SearchProperties()
            {
                seid = 1048, domain ="com.ua", geo_location="Ukraine", locale="ru-ua" , device="mobile"
            },
            new SearchProperties()
            {
                seid = 1049, domain ="hu", geo_location="Hungary", locale="hu-hu" , device="mobile"
            },
            new SearchProperties()
            {
                seid = 1050, domain ="hu", geo_location="Hungary", locale="en-hu" , device="mobile"
            },
            new SearchProperties()
            {
                seid = 1051, domain ="ro", geo_location="Romania", locale="ro-ro" , device="mobile"
            },
            new SearchProperties()
            {
                seid = 1052, domain ="de", geo_location="Germany", locale="de-de" , device="mobile"
            },
            new SearchProperties()
            {
                seid = 1053, domain ="es", geo_location="Spain", locale="es-es" , device="mobile"
            },
            new SearchProperties()
            {
                seid = 1054, domain ="pl", geo_location="Poland", locale="pl-pl" , device="mobile"
            },
            new SearchProperties()
            {
                seid = 1055, domain ="gr", geo_location="Greece", locale="el-gr" , device="mobile"
            },
            new SearchProperties()
            {
                seid = 1056, domain ="gr", geo_location="Greece", locale="el-gr" , device="mobile"
            },
            new SearchProperties()
            {
                seid = 1057, domain ="com.eg", geo_location="Egypt", locale="ar-eg" , device="mobile"
            },
            new SearchProperties()
            {
                seid = 1058, domain ="com.eg", geo_location="Egypt", locale="en-eg" , device="mobile"
            },//01-07-2025
        };
    }
    class SearchProperties
    {
        public int seid { get; set; }
        public string domain { get; set; }
        public string query { get; set; }
        public string geo_location { get; set; }
        public string locale { get; set; }
        public string device { get; set; }
    }
   
    
}
