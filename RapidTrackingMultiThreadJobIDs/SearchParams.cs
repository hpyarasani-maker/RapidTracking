using System.Collections.Generic;

namespace RapidTrackingMultiThreadJobIDs
{  
    class SearchParams
    {  
        public static IList<SearchProperties> searches = new List<SearchProperties>()
        {
           new SearchProperties()
            {
                seid =1, domain ="com", geo_location="United States", locale="en-us", uule="w+CAIQICINVW5pdGVkIFN0YXRlcw==", device="desktop"
            },
            //new SearchProperties()
            //{
            //    seid =2, domain ="co.uk", geo_location="United Kingdom", locale="en-gb", uule="w+CAIQICIOVW5pdGVkIEtpbmdkb20=",device="desktop"
            //},
            // new SearchProperties()
            //{
            //    seid =7, domain ="com.au", geo_location="Australia", locale="en-au", uule="w+CAIQICIJQXVzdHJhbGlh",device="desktop"
            //},
            new SearchProperties()
            {
                seid =13, domain ="co.nz", geo_location="New Zealand", locale="en-nz", uule="w+CAIQICILTmV3IFplYWxhbmQ=",device="desktop"
            },
            new SearchProperties()
            {
                seid =16, domain ="co.za", geo_location="South Africa", locale="en-za", uule = "w+CAIQICIPU291dGggQWZyaWNhCgoK",device="desktop"
            },

            new SearchProperties()
            {
                seid =21, domain ="ru", geo_location="Russia", locale="ru-ru", uule = "w+CAIQICIGUnVzc2lh",device="desktop"
            },
            //new SearchProperties()
            //{
            //    seid =22, domain ="co.kr", geo_location="South Korea", locale="ko-kr", uule = "w+CAIQICILU291dGggS29yZWE=",device="desktop"
            //},

             new SearchProperties()
            {
                seid =40, domain ="fr", geo_location="France", locale="fr-fr", uule = "w+CAIQICIGRnJhbmNl",device="desktop"
            },

            new SearchProperties()
            {
                seid =44, domain ="be", geo_location="Belgium", locale= "nl-be", uule = "w+CAIQICIHQmVsZ2l1bQ==",device="desktop"
            },

            new SearchProperties()
            {
                seid =57, domain ="nl", geo_location="Netherlands", locale= "nl-nl", uule = "w+CAIQICILTmV0aGVybGFuZHM=",device="desktop"
            },
              new SearchProperties()
            {
                seid =58, domain ="co.uk",geo_location="United Kingdom",locale= "en-gb",uule = "w+CAIQICIOVW5pdGVkIEtpbmdkb20=",device="desktop"
            },
               new SearchProperties()
            {
                seid =59, domain ="es", geo_location="Spain", locale= "es-es", uule = "w+CAIQICIFc3BhaW4=",device="desktop"
            },
                new SearchProperties()
            {
                seid =60, domain ="it", geo_location="Italy", locale= "it-it", uule = "w+CAIQICIFSXRhbHk=",device="desktop"
            },
                 new SearchProperties()
            {
                seid =61, domain ="se", geo_location="Sweden", locale= "sv-se", uule = "w+CAIQICIGc3dlZGVu",device="desktop"
            },

            new SearchProperties()
            {
                seid =63, domain ="com.hk", geo_location="Hong Kong", locale= "zh-TW-HK", uule = "w+CAIQICIJSG9uZyBLb25n",device="desktop"
            },
             new SearchProperties()
            {
                seid =64, domain ="com.sg", geo_location="Singapore", locale= "en-sg", uule = "w+CAIQICIJU2luZ2Fwb3Jl",device="desktop"

            },
             new SearchProperties()
            {
                seid =65, domain ="co.jp", geo_location="Japan", locale= "ja-jp", uule = "w+CAIQICIFSmFwYW4=",device="desktop"

            },
             new SearchProperties()
            {
                seid =66, domain ="com.br", geo_location="Brazil", locale= "pt-br", uule = "w+CAIQICIGQnJhemls",device="desktop"

            },
              new SearchProperties()
            {
                seid =67, domain ="de", geo_location="Germany", locale= "de-de", uule = "w+CAIQICIHR2VybWFueQ==",device="desktop"

            },
               new SearchProperties()
            {
                seid =68, domain ="ch", geo_location="Switzerland", locale= "de-ch", uule = "w+CAIQICIMU3dpdHplcmxhbmQK",device="desktop"

            },
             new SearchProperties()
            {
                seid =69, domain ="lu", geo_location="Luxembourg", locale= "de-lu", uule = "w+CAIQICIKTHV4ZW1ib3VyZw==",device="desktop"

            },
             new SearchProperties()
            {
                seid =70, domain ="at", geo_location="Austria", locale= "de-at", uule = "w+CAIQICIHQXVzdHJpYQ==",device="desktop"

            },
            // new SearchProperties()
            //{
            //    seid =74, domain ="co.uk", geo_location="United Kingdom", locale= "en-gb", uule = "w+CAIQICIOVW5pdGVkIEtpbmdkb20=",tbm="isch",device="desktop"

            //},
              new SearchProperties()
            {
                seid =77, domain ="ae", geo_location="United Arab Emirates", locale= "ar-ae", uule = "w+CAIQICIUVW5pdGVkIEFyYWIgRW1pcmF0ZXM=",device="desktop"

            },
               new SearchProperties()
            {
                seid =79, domain ="com.tr", geo_location="Turkey", locale= "tr-tr", uule ="w+CAIQICIGVHVya2V5",device="desktop"

            },
              new SearchProperties()
            {
                seid =80, domain ="co.ma", geo_location="Morocco", locale= "fr-ma", uule = "w+CAIQICIHTW9yb2Njbw==",device="desktop"

            },
               new SearchProperties()
            {
                seid =81, domain ="com.ly", geo_location="Libya", locale= "ar-ly", uule = "w+CAIQICIFTGlieWE=",device="desktop"

            },
            new SearchProperties()
            {
                seid =82, domain ="dz", geo_location="Algeria", locale= "fr-dz", uule = "w+CAIQICIHQWxnZXJpYQ==",device="desktop"

            },
            new SearchProperties()
            {
                seid =83, domain ="com.ua", geo_location="Ukraine", locale= "uk-ua", uule = "w+CAIQICIHVWtyYWluZQ==",device="desktop"

            },
            new SearchProperties()
            {
                seid =84, domain ="com.eg", geo_location="Egypt", locale= "ar-eg", uule = "w+CAIQICIFRWd5cHQ=",device="desktop"

            },
            new SearchProperties()
            {
                seid =85, domain ="com.bh", geo_location="Bahrain", locale= "ar-bh", uule = "w+CAIQICIHQmFocmFpbg==",device="desktop"

            },
            new SearchProperties()
            {
                seid =86, domain ="com.qa", geo_location="Qatar", locale= "ar-qa", uule = "w+CAIQICIFUWF0YXI=",device="desktop"

            },
            new SearchProperties()
            {
                seid =87, domain ="com.sa", geo_location="Saudi Arabia", locale= "ar-sa", uule = "w+CAIQICIMU2F1ZGkgQXJhYmlh",device="desktop"

            },
            new SearchProperties()
            {
                seid =88, domain ="com.kw", geo_location="Kuwait", locale= "ar-kw", uule = "w+CAIQICIGS3V3YWl0",device="desktop"

            },
            new SearchProperties()
            {
                seid =89, domain ="com.vn", geo_location="Vietnam", locale= "vi-vn", uule = "w+CAIQICIHVmlldG5hbQ==",device="desktop"

            },
            new SearchProperties()
            {
                seid =90, domain ="ga", geo_location="Gabon", locale= "fr-ga", uule = "w+CAIQICIFR2Fib24=",device="desktop"

            },
            new SearchProperties()
            {
                seid =93, domain ="co.in", geo_location="India", locale= "en-in", uule = "w+CAIQICIFSW5kaWE=",device="desktop"

            },
            new SearchProperties()
            {
                seid =98, domain ="co.il", geo_location="Israel", locale= "iw-il", uule = "w+CAIQICIGSXNyYWVs",device="desktop"

            },
            /* new SearchProperties()
            {
                seid =99, domain ="com", geo_location="United States", locale= "en-us", uule = "w+CAIQICINVW5pdGVkIFN0YXRlcw==",device="desktop"

            },*/
            new SearchProperties()
            {
            seid =102, domain ="com", geo_location="United States", locale = "en-us", uule = "w+CAIQICINVW5pdGVkIFN0YXRlcw==",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =104, domain ="it", geo_location="Italy", locale= "it-IT", uule = "w+CAIQICIFSXRhbHk=",device="mobile_android"
             },
            new SearchProperties()
            {
            seid =105, domain ="ie", geo_location="Ireland", locale= "en-ie", uule = "w+CAIQICIHSXJlbGFuZA==",device="desktop"
            },
            new SearchProperties()
            {
            seid =106, domain ="co.uk", geo_location="United Kingdom", locale = "en-gb", uule = "w+CAIQICIOVW5pdGVkIEtpbmdkb20=",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =109, domain ="com.my", geo_location="Malaysia", locale= "en-my", uule = "w+CAIQICIITWFsYXlzaWE=",device="desktop"
            },
            new SearchProperties()
            {
            seid =111, domain ="co.th", geo_location="Thailand", locale= "th-th", uule = "w+CAIQICIIVGhhaWxhbmQ=",device="desktop"
            },
            new SearchProperties()
            {
            seid =113, domain ="com.ng", geo_location="Nigeria", locale= "en-ng", uule = "w+CAIQICIHTmlnZXJpYQ==",device="desktop"
            },
            new SearchProperties()
            {
            seid =114, domain ="ch", geo_location="Switzerland", locale= "it-ch", uule = "w+CAIQICIMU3dpdHplcmxhbmQK",device="desktop"
            },
            new SearchProperties()
            {
            seid =115, domain ="ch", geo_location="Switzerland", locale= "fr-ch", uule = "w+CAIQICIMU3dpdHplcmxhbmQK",device="desktop"
            },

            new SearchProperties()
            {
            seid =117, domain ="pt", geo_location="Portugal", locale= "pt-pt", uule = "w+CAIQICIIUG9ydHVnYWw=",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =118, domain ="nl", geo_location="Netherlands", locale= "nl-nl", uule = "w+CAIQICILTmV0aGVybGFuZHM=",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =119, domain ="com.mx", geo_location="Mexico", locale= "es-419-mx", uule = "w+CAIQICIGTWV4aWNv",device="desktop"
            },
            new SearchProperties()
            {
            seid =120, domain ="es", geo_location="Spain", locale= "es-es", uule = "w+CAIQICIFc3BhaW4=",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =121, domain ="com.au", geo_location="Australia", locale= "en-au", uule = "w+CAIQICIJQXVzdHJhbGlh",device="desktop"
            },
            new SearchProperties()
            {
            seid =122, domain ="pl", geo_location="Poland", locale= "pl-pl", uule = "w+CAIQICIGUG9sYW5k",device="desktop"
            },
            new SearchProperties()
            {
            seid =123, domain ="ro", geo_location="Romania", locale= "ro-ro", uule = "w+CAIQICIHUm9tYW5pYQ==",device="desktop"
            },
            new SearchProperties()
            {
            seid =124, domain ="bg", geo_location="Bulgaria", locale= "bg-bg", uule = "w+CAIQICIIQnVsZ2FyaWE=",device="desktop"
            },
            new SearchProperties()
            {
            seid =125, domain ="si", geo_location="Slovenia", locale= "sl-si", uule = "w+CAIQICIIU2xvdmVuaWE=",device="desktop"
            },
            new SearchProperties()
            {
            seid =126, domain ="hu", geo_location="Hungary", locale= "hu-hu", uule = "w+CAIQICIHSHVuZ2FyeQ==",device="desktop"
            },
            new SearchProperties()
            {
            seid =127, domain ="cz", geo_location="Czech Republic", locale = "cs-cz", uule = "w+CAIQICIOQ3plY2ggUmVwdWJsaWM=",device="desktop"
            },
            new SearchProperties()
            {
            seid =128, domain ="sk", geo_location="Slovakia", locale= "sk-sk", uule = "w+CAIQICIIU2xvdmFraWE=",device="desktop"
            },
            new SearchProperties()
            {
            seid =129, domain ="com.ar", geo_location="Argentina", locale= "es-419-ar", uule = "w+CAIQICIJQXJnZW50aW5h",device="desktop"
            },
            new SearchProperties()
            {
            seid =130, domain ="com.lb", geo_location="Lebanon", locale= "ar-lb", uule = "w+CAIQICIHTGViYW5vbg==",device="desktop"
            },
            new SearchProperties()
            {
            seid =131, domain ="jo", geo_location="Jordan", locale= "ar-jo", uule = "w+CAIQICIGSm9yZGFu",device="desktop"
            },
            new SearchProperties()
            {
            seid =132, domain ="fi", geo_location="Finland", locale= "fi-fi", uule = "w+CAIQICIHRmlubGFuZA==",device="desktop"
            },
            new SearchProperties()
            {
            seid =133, domain ="gr", geo_location="Greece", locale= "el-gr", uule = "w+CAIQICIGR3JlZWNl",device="desktop"
            },
            new SearchProperties()
            {
            seid =134, domain ="no", geo_location="Norway", locale= "no-no", uule = "w+CAIQICIGTm9yd2F5",device="desktop"
            },
            new SearchProperties()
            {
            seid =135, domain ="be", geo_location="Belgium", locale= "fr-be", uule = "w+CAIQICIHQmVsZ2l1bQ==",device="desktop"
            },
            new SearchProperties()
            {
            seid =136, domain ="dk", geo_location="Denmark", locale= "da-dk", uule = "w+CAIQICIHRGVubWFyaw==",device="desktop"
            },
            new SearchProperties()
            {
            seid =137, domain ="cl", geo_location="Chile", locale= "es-419-cl", uule = "w+CAIQICIFQ2hpbGU=",device="desktop"
            },
            new SearchProperties()
            {
            seid =139, domain ="com.au", geo_location="Australia", locale= "en-au", uule = "w+CAIQICIJQXVzdHJhbGlh",device="mobile_android"
            },
             new SearchProperties()
            {
            seid =141, domain ="ch", geo_location="Switzerland", locale= "it-ch", uule = "w+CAIQICIMU3dpdHplcmxhbmQK",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =142, domain ="ch", geo_location="Switzerland", locale= "fr-ch", uule = "w+CAIQICIMU3dpdHplcmxhbmQK",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =143, domain ="ch", geo_location="Switzerland", locale= "de-ch", uule = "w+CAIQICIMU3dpdHplcmxhbmQK",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =144, domain ="pt", geo_location="Portugal", locale= "pt-pt", uule = "w+CAIQICIIUG9ydHVnYWw=",device="desktop"
            },
            new SearchProperties()
            {
            seid =145, domain ="de", geo_location="Germany", locale= "de-de", uule = "w+CAIQICIHR2VybWFueQ==",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =146, domain ="ae", geo_location="United Arab Emirates", locale= "ar-ae", uule = "w+CAIQICIUVW5pdGVkIEFyYWIgRW1pcmF0ZXM=",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =147, domain ="at", geo_location="Austria", locale= "de-at", uule = "w+CAIQICIHQXVzdHJpYQ==",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =148, domain ="be", geo_location="Belgium", locale= "nl-be", uule = "w+CAIQICIHQmVsZ2l1bQ==",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =149, domain ="be", geo_location="Belgium", locale= "fr-be", uule = "w+CAIQICIHQmVsZ2l1bQ==",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =150, domain ="com.br", geo_location="Brazil", locale= "pt-br", uule = "w+CAIQICIGQnJhemls",device="mobile_android"
            },

            new SearchProperties()
            {
            seid =152, domain ="dk", geo_location="Denmark", locale= "da-dk", uule ="w+CAIQICIHRGVubWFyaw==",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =153, domain ="fi", geo_location="Finland", locale= "fi-fi", uule = "w+CAIQICIHRmlubGFuZA==",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =154, domain ="fr", geo_location="France", locale= "fr-fr", uule = "w+CAIQICIGRnJhbmNl",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =155, domain ="gr", geo_location="Greece", locale= "el-gr", uule = "w+CAIQICIGR3JlZWNl",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =156, domain ="com.hk", geo_location="Hong Kong", locale = "zh-TW-HK", uule = "w+CAIQICIJSG9uZyBLb25n",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =157, domain ="ie", geo_location="Ireland", locale= "en-ie", uule = "w+CAIQICIHSXJlbGFuZA==",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =158, domain ="co.jp", geo_location="Japan", locale= "ja-jp", uule = "w+CAIQICIFSmFwYW4=",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =159, domain ="no", geo_location="Norway", locale= "no-no", uule = "w+CAIQICIGTm9yd2F5",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =160, domain ="se", geo_location="Sweden", locale= "sv-se", uule = "w+CAIQICIGTm9yd2F5",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =167, domain ="hu", geo_location="Hungary", locale= "hu-hu", uule = "w+CAIQICIHSHVuZ2FyeQ==",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =168, domain ="pl", geo_location="Poland", locale= "pl-pl", uule = "w+CAIQICIGUG9sYW5k",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =169, domain ="com.tr", geo_location="Turkey", locale= "tr-tr", uule = "w+CAIQICIGVHVya2V5",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =172, domain ="ru", geo_location="Russia", locale= "ru-ru", uule = "w+CAIQICIGUnVzc2lh",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =173, domain ="com.tw", geo_location="Taiwan", locale= "zh-tw", uule = "w+CAIQICIGVGFpd2Fu",device="desktop"
            },
            new SearchProperties()
            {
            seid =174, domain ="co.il", geo_location="Israel", locale= "en-il", uule = "w+CAIQICIGSXNyYWVs",device="desktop"
            },
            new SearchProperties()
            {
            seid =176, domain ="rs", geo_location="Serbia", locale= "sr-rs", uule = "w+CAIQICIGU2VyYmlh",device="desktop"
            },
            new SearchProperties()
            {
            seid =177, domain ="com", geo_location="New York,United States", locale= "en-us", uule = "w+CAIQICIWTmV3IFlvcmssVW5pdGVkIFN0YXRlcw==",device="desktop"
            },
            new SearchProperties()
            {
            seid =178, domain ="com", geo_location="New York,United States", locale= "en-us", uule = "w+CAIQICIWTmV3IFlvcmssVW5pdGVkIFN0YXRlcw==",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =179, domain ="com", geo_location="Los Angeles,United States", locale= "en-us", uule = "w+CAIQICIkTG9zIEFuZ2VsZXMsQ2FsaWZvcm5pYSxVbml0ZWQgU3RhdGVz",device="desktop"
            },
            new SearchProperties()
            {
            seid =180, domain ="com", geo_location="Los Angeles,United States", locale= "en-us", uule = "w+CAIQICIkTG9zIEFuZ2VsZXMsQ2FsaWZvcm5pYSxVbml0ZWQgU3RhdGVz",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =181, domain ="co.za", geo_location="South Africa", locale= "en-za", uule = "w+CAIQICIPU291dGggQWZyaWNhCgoK",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =182, domain ="com.sa", geo_location="Saudi Arabia", locale= "ar-sa", uule = "w+CAIQICIMU2F1ZGkgQXJhYmlh",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =183, domain ="ca", geo_location="Canada", locale= "fr-ca", uule = "w+CAIQICIGQ2FuYWRh",device="desktop"
            },
            new SearchProperties()
            {
            seid =184, domain ="com", geo_location="Chicago,United States", locale= "en-us", uule = "w+CAIQICIVQ2hpY2FnbyxVbml0ZWQgU3RhdGVz",device="desktop"
            },
            new SearchProperties()
            {
            seid =185, domain ="com", geo_location="Chicago,United States", locale= "en-us", uule = "w+CAIQICIVQ2hpY2FnbyxVbml0ZWQgU3RhdGVz",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =186, domain ="com", geo_location="San Francisco,United States", locale= "en-us", uule = "w+CAIQICImU2FuIEZyYW5jaXNjbyxDYWxpZm9ybmlhLFVuaXRlZCBTdGF0ZXM",device="desktop"
            },
            new SearchProperties()
            {
            seid =187, domain ="com", geo_location="San Francisco,United States", locale= "en-us", uule = "w+CAIQICImU2FuIEZyYW5jaXNjbyxDYWxpZm9ybmlhLFVuaXRlZCBTdGF0ZXM",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =188, domain ="com.pk", geo_location="Pakistan", locale= "en-pk", uule = "w+CAIQICIIUGFraXN0YW4=",device="desktop"
            },
            new SearchProperties()
            {
            seid =189, domain ="com.pk", geo_location="Pakistan", locale= "en-pk", uule = "w+CAIQICIIUGFraXN0YW4=",device="mobile_android"
            },
             new SearchProperties()
            {
                seid =204, domain ="co.in", geo_location="India", locale= "en-in", uule = "w+CAIQICIFSW5kaWE=",device="mobile_android"

            },
             new SearchProperties()
            {
                seid =206, domain ="ca", geo_location="Canada", locale= "fr-ca", uule = "w+CAIQICIGQ2FuYWRh",device="mobile_android"

            },

            new SearchProperties()
            {
                seid =207, domain ="com.my", geo_location="Malaysia", locale= "en-my", uule = "w+CAIQICIITWFsYXlzaWE=",device="mobile_android"

            },

            new SearchProperties()
            {
                seid =208, domain ="com.sg", geo_location="Singapore", locale= "zh-sg", uule = "w+CAIQICIJU2luZ2Fwb3Jl",device="mobile_android"

            },

            new SearchProperties()
            {
                seid =209, domain ="ca", geo_location="Canada", locale= "en-ca", uule = "w+CAIQICIGQ2FuYWRh",device="desktop"

            },

            new SearchProperties()
            {
                seid =210, domain ="ca", geo_location="Canada", locale= "en-ca", uule = "w+CAIQICIGQ2FuYWRh",device="mobile_android"

            },

            new SearchProperties()
            {
                seid =211, domain ="lk", geo_location="Sri Lanka", locale= "en-lk", uule = "w+CAIQICIJU3JpIExhbmth",device="mobile_android"

            },

            new SearchProperties()
            {
                seid =212, domain ="lk", geo_location="Sri Lanka", locale= "en-lk", uule = "w+CAIQICIJU3JpIExhbmth",device="desktop"

            },

            new SearchProperties()
            {
                seid =213, domain ="hr", geo_location="Croatia", locale= "hr-hr", uule = "w+CAIQICIHQ3JvYXRpYQ==",device="desktop"

            },

            new SearchProperties()
            {
                seid =214, domain ="hr", geo_location="Croatia", locale= "hr-hr", uule = "w+CAIQICIHQ3JvYXRpYQ==",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =215, domain ="cz", geo_location="Czech Republic", locale= "cs-cz", uule = "w+CAIQICIHQ3plY2hpYQ==",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =219, domain ="co.th", geo_location="Thailand", locale= "th-th", uule = "w+CAIQICIIVGhhaWxhbmQ=",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =223, domain ="com", geo_location="Houston,United States", locale= "en-us", uule = "w+CAIQICIbSG91c3RvbixUZXhhcyxVbml0ZWQgU3RhdGVz",device="desktop"

            },
            new SearchProperties()
            {
                seid =224, domain ="com", geo_location="Houston,United States", locale= "en-us", uule = "w+CAIQICIbSG91c3RvbixUZXhhcyxVbml0ZWQgU3RhdGVz",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =225, domain ="com", geo_location="Miami,United States", locale= "en-us", uule = "w+CAIQICIbTWlhbWksRmxvcmlkYSxVbml0ZWQgU3RhdGVz",device="desktop"

            },
            new SearchProperties()
            {
                seid =226, domain ="com", geo_location="Miami,United States", locale= "en-us", uule = "w+CAIQICIbTWlhbWksRmxvcmlkYSxVbml0ZWQgU3RhdGVz",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =233, domain ="ae", geo_location="United Arab Emirates", locale= "en-ae", uule = "w+CAIQICIUVW5pdGVkIEFyYWIgRW1pcmF0ZXM=",device="desktop"

            },
            new SearchProperties()
            {
                seid =234, domain ="com.hk", geo_location="Hong Kong", locale= "en-hk", uule = "w+CAIQICIJSG9uZyBLb25n",device="desktop"

            },
            new SearchProperties()
            {
                seid =235, domain ="com.mx", geo_location="Mexico", locale= "es-419-mx", uule = "w+CAIQICIGTWV4aWNv",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =236, domain ="com.mx", geo_location="Mexico City,Mexico", locale= "es-419-mx", uule = "w+CAIQICIeTWV4aWNvIENpdHksTWV4aWNvIENpdHksTWV4aWNv",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =237, domain ="cl", geo_location="Chile", locale= "es-419-cl", uule = "w+CAIQICIFQ2hpbGU=",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =238, domain ="com.pe", geo_location="Peru", locale= "es-419-pe", uule = "w+CAIQICIEUGVydQ==",device="desktop"

            },
            new SearchProperties()
            {
                seid =239, domain ="com.pe", geo_location="Peru", locale= "es-419-pe", uule = "w+CAIQICIEUGVydQ==",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =240, domain ="com.co", geo_location="Columbia", locale= "es-419-co", uule = "w+CAIQICIIQ29sb21iaWE=",device="desktop"

            },
            new SearchProperties()
            {
                seid =241, domain ="com.co", geo_location="Columbia", locale= "es-419-co", uule = "w+CAIQICIIQ29sb21iaWE=",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =242, domain ="com.mx", geo_location="Mexico City,Mexico", locale= "es-419-mx", uule = "w+CAIQICIeTWV4aWNvIENpdHksTWV4aWNvIENpdHksTWV4aWNv",device="desktop"

            },
            new SearchProperties()
            {
                seid =243, domain ="co.id", geo_location="Indonesia", locale= "id-id", uule ="w+CAIQICIJSW5kb25lc2lh",device="desktop"

            },
            new SearchProperties()
            {
                seid =244, domain ="co.id", geo_location="Indonesia", locale= "id-id", uule = "w+CAIQICIJSW5kb25lc2lh",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =245, domain ="com.ph", geo_location="Philippines", locale= "fil-ph", uule = "w+CAIQICILUGhpbGlwcGluZXM=",device="desktop"

            },
            new SearchProperties()
            {
                seid =246, domain ="com.ph", geo_location="Philippines", locale= "fil-ph", uule = "w+CAIQICILUGhpbGlwcGluZXM=",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =247, domain ="com.sg", geo_location="Singapore", locale= "en-sg", uule = "w+CAIQICIJU2luZ2Fwb3Jl",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =248, domain ="co.th", geo_location="Thailand", locale= "en-th", uule = "w+CAIQICIIVGhhaWxhbmQ=",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =249, domain ="com.hk", geo_location="Hong Kong", locale= "en-hk", uule = "w+CAIQICIJSG9uZyBLb25n",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =250, domain ="com.my", geo_location="Malaysia", locale= "ms-my", uule = "w+CAIQICIITWFsYXlzaWE=",device="desktop"

            },
            new SearchProperties()
            {
                seid =251, domain ="com.my", geo_location="Malaysia", locale= "ms-my", uule = "w+CAIQICIITWFsYXlzaWE=",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =252, domain ="co.th", geo_location="Thailand", locale= "en-th", uule = "w+CAIQICIIVGhhaWxhbmQ=",device="desktop"

            },
            new SearchProperties()
            {
                seid =253, domain ="com.sg", geo_location="Singapore", locale= "zh-sg", uule = "w+CAIQICIJU2luZ2Fwb3Jl",device="desktop"

            },

            new SearchProperties()
            {
                seid =254, domain ="com.hk", geo_location="Hong Kong", locale= "zh-cn-hk", uule = "w+CAIQICIJSG9uZyBLb25n",device="desktop"

            },
            new SearchProperties()
            {
                seid =255, domain ="com.hk", geo_location="Hong Kong", locale= "zh-cn-hk", uule = "w+CAIQICIJSG9uZyBLb25n",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =257, domain ="co.nz", geo_location="New Zealand", locale= "en-nz", uule = "w+CAIQICILTmV3IFplYWxhbmQ=",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =258, domain ="com.tw", geo_location="Taiwan", locale= "zh-tw", uule = "w+CAIQICIGVGFpd2Fu",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =259, domain ="ae", geo_location="United Arab Emirates", locale= "en-ae", uule = "w+CAIQICIUVW5pdGVkIEFyYWIgRW1pcmF0ZXM=",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =260, domain ="co.jp", geo_location="Japan", locale= "en-jp", uule = "w+CAIQICIFSmFwYW4=",device="desktop"

            },
            new SearchProperties()
            {
                seid =261, domain ="co.jp", geo_location="Japan", locale= "en-jp", uule = "w+CAIQICIFSmFwYW4=",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =262, domain ="dk", geo_location="Denmark", locale= "en-dk", uule = "w+CAIQICIHRGVubWFyaw==",device="desktop"

            },
            new SearchProperties()
            {
                seid =263, domain ="dk", geo_location="Denmark", locale= "en-dk", uule = "w+CAIQICIHRGVubWFyaw==",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =264, domain ="se", geo_location="Sweden", locale= "en-se", uule = "w+CAIQICIGU3dlZGV",device="desktop"

            },
            new SearchProperties()
            {
                seid =265, domain ="se", geo_location="Sweden", locale= "en-se", uule = "w+CAIQICIGU3dlZGV",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =266, domain ="no", geo_location="Norway", locale= "en-no", uule = "w+CAIQICIGTm9yd2F5",device="desktop"

            },
            new SearchProperties()
            {
                seid =267, domain ="no", geo_location="Norway", locale= "en-no", uule = "w+CAIQICIGTm9yd2F5",device="mobile_android"

            },

            new SearchProperties()
            {
                seid =268, domain ="fi", geo_location="Finland", locale= "en-fi", uule = "w+CAIQICIHRmlubGFuZA==",device="desktop"

            },

            new SearchProperties()
            {
                seid =269, domain ="fi", geo_location="Finland", locale= "en-fi", uule = "w+CAIQICIHRmlubGFuZA==",device="mobile_android"

            },

            new SearchProperties()
            {
                seid =270, domain ="de", geo_location="Germany", locale= "en-de", uule = "w+CAIQICIHR2VybWFueQ==",device="desktop"

            },

            new SearchProperties()
            {
                seid =271, domain ="de", geo_location="Germany", locale= "en-de", uule = "w+CAIQICIHR2VybWFueQ==",device="mobile_android"

            },

            new SearchProperties()
            {
                seid =272, domain ="fr", geo_location="France", locale= "en-fr", uule = "w+CAIQICIGRnJhbmNl",device="desktop"

            },

            new SearchProperties()
            {
                seid =273, domain ="fr", geo_location="France", locale= "en-fr", uule = "w+CAIQICIGRnJhbmNl",device="mobile_android"

            },

            new SearchProperties()
            {
                seid =274, domain ="it", geo_location="Italy", locale= "en-it", uule = "w+CAIQICIFSXRhbHk=",device="desktop"

            },

            new SearchProperties()
            {
                seid =275, domain ="it", geo_location="Italy", locale= "en-it", uule = "w+CAIQICIFSXRhbHk=",device="mobile_android"

            },

            new SearchProperties()
            {
                seid =279, domain ="co.uk", geo_location="London,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIdTG9uZG9uLEVuZ2xhbmQsVW5pdGVkIEtpbmdkb20=",device="desktop"

            },

            new SearchProperties()
            {
                seid =280, domain ="co.uk", geo_location="London,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIdTG9uZG9uLEVuZ2xhbmQsVW5pdGVkIEtpbmdkb20=",device="mobile_android"

            },

            new SearchProperties()
            {
                seid =281, domain ="co.uk", geo_location="Birmingham,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIhQmlybWluZ2hhbSxFbmdsYW5kLFVuaXRlZCBLaW5nZG9t",device="desktop"

            },

            new SearchProperties()
            {
                seid =282, domain ="co.uk", geo_location="Birmingham,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIhQmlybWluZ2hhbSxFbmdsYW5kLFVuaXRlZCBLaW5nZG9t",device="mobile_android"

            },

            new SearchProperties()
            {
                seid =283, domain ="co.uk", geo_location="Leeds,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIcTGVlZHMsRW5nbGFuZCxVbml0ZWQgS2luZ2RvbQ==",device="desktop"

            },

            new SearchProperties()
            {
                seid =284, domain ="co.uk", geo_location="Leeds,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIcTGVlZHMsRW5nbGFuZCxVbml0ZWQgS2luZ2RvbQ==",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =285, domain ="co.uk", geo_location="Sheffield,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIgU2hlZmZpZWxkLEVuZ2xhbmQsVW5pdGVkIEtpbmdkb20=",device="desktop"

            },
            new SearchProperties()
            {
                seid =286, domain ="co.uk", geo_location="Sheffield,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIgU2hlZmZpZWxkLEVuZ2xhbmQsVW5pdGVkIEtpbmdkb20=",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =287, domain ="co.uk", geo_location="Bradford,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIfQnJhZGZvcmQsRW5nbGFuZCxVbml0ZWQgS2luZ2RvbQ==",device="desktop"

            },
            new SearchProperties()
            {
                seid =288, domain ="co.uk", geo_location="Bradford,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIfQnJhZGZvcmQsRW5nbGFuZCxVbml0ZWQgS2luZ2RvbQ==",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =289, domain ="co.uk", geo_location="Manchester,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIhTWFuY2hlc3RlcixFbmdsYW5kLFVuaXRlZCBLaW5nZG9t",device="desktop"

            },
            new SearchProperties()
            {
                seid =290, domain ="co.uk", geo_location="Manchester,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIhTWFuY2hlc3RlcixFbmdsYW5kLFVuaXRlZCBLaW5nZG9t",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =291, domain ="co.uk", geo_location="Liverpool,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIgTGl2ZXJwb29sLEVuZ2xhbmQsVW5pdGVkIEtpbmdkb20=",device="desktop"

            },
            new SearchProperties()
            {
                seid =292, domain ="co.uk", geo_location="Liverpool,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIgTGl2ZXJwb29sLEVuZ2xhbmQsVW5pdGVkIEtpbmdkb20=",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =293, domain ="co.uk", geo_location="Bristol,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIeQnJpc3RvbCxFbmdsYW5kLFVuaXRlZCBLaW5nZG9t",device="desktop"

            },
            new SearchProperties()
            {
                seid =294, domain ="co.uk", geo_location="Bristol,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIeQnJpc3RvbCxFbmdsYW5kLFVuaXRlZCBLaW5nZG9t",device="mobile_android"

            },
             new SearchProperties()
            {
                seid =295, domain ="co.uk", geo_location="Newcastle,Northern Ireland,United Kingdom", locale= "en-gb", uule = "w+CAIQICIpTmV3Y2FzdGxlLE5vcnRoZXJuIElyZWxhbmQsVW5pdGVkIEtpbmdkb20=",device="desktop"

            },
            new SearchProperties()
            {
                seid =296, domain ="co.uk", geo_location="Newcastle,Northern Ireland,United Kingdom", locale= "en-gb", uule = "w+CAIQICIpTmV3Y2FzdGxlLE5vcnRoZXJuIElyZWxhbmQsVW5pdGVkIEtpbmdkb20=",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =297, domain ="co.uk", geo_location="Sunderland,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIhU3VuZGVybGFuZCxFbmdsYW5kLFVuaXRlZCBLaW5nZG9t",device="desktop"

            },
            new SearchProperties()
            {
                seid =298, domain ="co.uk", geo_location="Sunderland,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIhU3VuZGVybGFuZCxFbmdsYW5kLFVuaXRlZCBLaW5nZG9t",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =299, domain ="co.uk", geo_location="Wolverhampton,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIkV29sdmVyaGFtcHRvbixFbmdsYW5kLFVuaXRlZCBLaW5nZG9t",device="desktop"

            },
          new SearchProperties()
            {
                seid =300, domain ="co.uk", geo_location="Wolverhampton,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIkV29sdmVyaGFtcHRvbixFbmdsYW5kLFVuaXRlZCBLaW5nZG9t",device="mobile_android"

            },
           new SearchProperties()
            {
                seid =301, domain ="co.uk", geo_location="Plymouth,England,United Kingdom", locale= "en-gb", uule ="w+CAIQICIfUGx5bW91dGgsRW5nbGFuZCxVbml0ZWQgS2luZ2RvbQ==",device="desktop"

            },
           new SearchProperties()
            {
                seid =302, domain ="co.uk", geo_location="Plymouth,England,United Kingdom", locale= "en-gb", uule ="w+CAIQICIfUGx5bW91dGgsRW5nbGFuZCxVbml0ZWQgS2luZ2RvbQ==",device="mobile_android"

            },
         new SearchProperties()
            {
                seid =303, domain ="co.uk", geo_location="Cardiff,Wales,United Kingdom", locale= "en-gb", uule ="w+CAIQICIcQ2FyZGlmZixXYWxlcyxVbml0ZWQgS2luZ2RvbQ==",device="desktop"

            },
           new SearchProperties()
            {
                seid =304, domain ="co.uk", geo_location="Cardiff,Wales,United Kingdom", locale= "en-gb", uule ="w+CAIQICIcQ2FyZGlmZixXYWxlcyxVbml0ZWQgS2luZ2RvbQ==",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =305, domain ="co.uk", geo_location="Oxford,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIdT3hmb3JkLEVuZ2xhbmQsVW5pdGVkIEtpbmdkb20=",device="desktop"

            },
           new SearchProperties()
            {
                seid =306, domain ="co.uk", geo_location="Oxford,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIdT3hmb3JkLEVuZ2xhbmQsVW5pdGVkIEtpbmdkb20=",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =307, domain ="co.uk", geo_location="Cambridge,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIgQ2FtYnJpZGdlLEVuZ2xhbmQsVW5pdGVkIEtpbmdkb20=",device="desktop"

            },
            new SearchProperties()
            {
                seid =308, domain ="co.uk", geo_location="Cambridge,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIgQ2FtYnJpZGdlLEVuZ2xhbmQsVW5pdGVkIEtpbmdkb20=",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =309, domain ="co.uk", geo_location="Belfast,Northern Ireland,United Kingdom", locale= "en-gb", uule ="w+CAIQICInQmVsZmFzdCxOb3J0aGVybiBJcmVsYW5kLFVuaXRlZCBLaW5nZG9t",device="desktop"

            },
            new SearchProperties()
            {
                seid =310, domain ="co.uk", geo_location="Belfast,Northern Ireland,United Kingdom", locale= "en-gb", uule = "w+CAIQICInQmVsZmFzdCxOb3J0aGVybiBJcmVsYW5kLFVuaXRlZCBLaW5nZG9t",device="mobile_android"
            },

            new SearchProperties()
            {
                seid =311, domain ="co.uk", geo_location="Glasgow,Scotland,United Kingdom", locale= "en-gb", uule = "w+CAIQICIfR2xhc2dvdyxTY290bGFuZCxVbml0ZWQgS2luZ2RvbQ==",device="desktop"

            },
            new SearchProperties()
            {
                seid =312, domain ="co.uk", geo_location="Glasgow,Scotland,United Kingdom", locale= "en-gb", uule = "w+CAIQICIfR2xhc2dvdyxTY290bGFuZCxVbml0ZWQgS2luZ2RvbQ==",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =313, domain ="co.uk", geo_location="Edinburgh,Scotland,United Kingdom", locale= "en-gb", uule = "w+CAIQICIhRWRpbmJ1cmdoLFNjb3RsYW5kLFVuaXRlZCBLaW5nZG9t",device="desktop"

            },
            new SearchProperties()
            {
                seid =314, domain ="co.uk", geo_location="Edinburgh,Scotland,United Kingdom", locale= "en-gb", uule = "w+CAIQICIhRWRpbmJ1cmdoLFNjb3RsYW5kLFVuaXRlZCBLaW5nZG9t",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =315, domain ="co.uk", geo_location="Brighton,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIfQnJpZ2h0b24sRW5nbGFuZCxVbml0ZWQgS2luZ2RvbQ==",device="desktop"

            },
            new SearchProperties()
            {
                seid =316, domain ="co.uk", geo_location="Brighton,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIfQnJpZ2h0b24sRW5nbGFuZCxVbml0ZWQgS2luZ2RvbQ==",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =317, domain ="co.uk", geo_location="Hove,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIoQnJpZ2h0b24gYW5kIEhvdmUsRW5nbGFuZCxVbml0ZWQgS2luZ2RvbQ==",device="desktop"

            },
            new SearchProperties()
            {
                seid =318, domain ="co.uk", geo_location="Hove,England,United Kingdom", locale= "en-gb", uule ="w+CAIQICIoQnJpZ2h0b24gYW5kIEhvdmUsRW5nbGFuZCxVbml0ZWQgS2luZ2RvbQ==",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =319, domain ="co.uk", geo_location="Southampton,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIiU291dGhhbXB0b24sRW5nbGFuZCxVbml0ZWQgS2luZ2RvbQ==",device="desktop"

            },
           new SearchProperties()
            {
                seid =320, domain ="co.uk", geo_location="Southampton,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIiU291dGhhbXB0b24sRW5nbGFuZCxVbml0ZWQgS2luZ2RvbQ==",device="mobile_android"

            },
           new SearchProperties()
            {
                seid =321, domain ="com.ph", geo_location="Philippines", locale= "en-ph", uule = "w+CAIQICILUGhpbGlwcGluZXM=",device="desktop"

            },

            new SearchProperties()
            {
                seid =322, domain ="com.ph", geo_location="Philippines", locale= "en-ph", uule = "w+CAIQICILUGhpbGlwcGluZXM=",device="mobile_android"

            },

            new SearchProperties()
            {
                seid =323, domain ="bs", geo_location="Bahamas", locale= "en-bs", uule = "w+CAIQICIHQmFoYW1hcw==",device="desktop"

            },

            new SearchProperties()
            {
                seid =324, domain ="bs", geo_location="Bahamas", locale= "en-bs", uule ="w+CAIQICIHQmFoYW1hcw==",device="mobile_android"

            },

            new SearchProperties()
            {
                seid =325, domain ="com.jm", geo_location="Jamaica", locale= "en-jm", uule = "w+CAIQICIHSmFtYWljYQ==",device="desktop"

            },
            new SearchProperties()
            {
                seid =326, domain ="com.jm", geo_location="Jamaica", locale= "en-jm", uule = "w+CAIQICIHSmFtYWljYQ==",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =327, domain ="com.mx", geo_location="Mexico", locale= "en-mx", uule = "w+CAIQICIGTWV4aWNv",device="desktop"

            },
            new SearchProperties()
            {
                seid =328, domain ="com.mx", geo_location="Mexico", locale= "en-mx", uule = "w+CAIQICIGTWV4aWNv",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =329, domain ="com.pr", geo_location="Puerto Rico", locale= "en-pr", uule = "w+CAIQICILUHVlcnRvIFJpY28=",device="desktop"

            },
            new SearchProperties()
            {
                seid =330, domain ="com.pr", geo_location="Puerto Rico", locale= "en-pr", uule = "w+CAIQICILUHVlcnRvIFJpY28=",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =331, domain ="com.pr", geo_location="Puerto Rico", locale= "es-419-pr", uule = "w+CAIQICILUHVlcnRvIFJpY28=",device="desktop"

            },
            new SearchProperties()
            {
                seid =332, domain ="com.pr", geo_location="Puerto Rico", locale= "es-419-pr", uule = "w+CAIQICILUHVlcnRvIFJpY28=",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =333, domain ="ca", geo_location="Toronto,canada", locale= "en-ca", uule = "w+CAIQICIWVG9yb250byxPbnRhcmlvLENhbmFkYQ==",device="desktop"

            },
            new SearchProperties()
            {
                seid =334, domain ="ca", geo_location="Toronto,canada", locale= "en-ca", uule = "w+CAIQICIWVG9yb250byxPbnRhcmlvLENhbmFkYQ==",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =335, domain ="co.ve", geo_location="Venezuela", locale= "es-419-ve", uule = "w+CAIQICIJVmVuZXp1ZWxh",device="desktop"

            },
            new SearchProperties()
            {
                seid =336, domain ="co.ve", geo_location="Venezuela", locale= "es-419-ve", uule = "w+CAIQICIJVmVuZXp1ZWxh",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =337, domain ="com.ar", geo_location="Argentina", locale= "es-419-ar", uule =  "w+CAIQICIJQXJnZW50aW5h",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =338, domain ="com", geo_location="Dallas,Texas,United States", locale= "en-us", uule = "w+CAIQICIaRGFsbGFzLFRleGFzLFVuaXRlZCBTdGF0ZXM=",device="desktop"

            },
            new SearchProperties()
            {
                seid =339, domain ="com", geo_location="Dallas,Texas,United States", locale= "en-us", uule = "w+CAIQICIaRGFsbGFzLFRleGFzLFVuaXRlZCBTdGF0ZXM=",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =350, domain ="com.ng", geo_location="Nigeria", locale= "en-ng", uule = "w+CAIQICIHTmlnZXJpYQ==",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =351, domain ="co.ke", geo_location="Kenya", locale= "en-ke", uule = "w+CAIQICIFS2VueWE=",device="desktop"

            },
            new SearchProperties()
            {
                seid =352, domain ="co.ke", geo_location="Kenya", locale= "en-ke", uule = "w+CAIQICIFS2VueWE=",device="mobile_android"

            },

            new SearchProperties()
            {
                seid =353, domain ="com.au", geo_location="Sydney,Australia", locale= "en-au", uule = "w+CAIQICIgU3lkbmV5LE5ldyBTb3V0aCBXYWxlcyxBdXN0cmFsaWE=",device="desktop"

            },
            new SearchProperties()
            {
                seid =354, domain ="com.au", geo_location="Melbourne,Australia", locale= "en-au", uule = "w+CAIQICIcTWVsYm91cm5lLFZpY3RvcmlhLEF1c3RyYWxpYQ==",device="desktop"

            },
            new SearchProperties()
            {
                seid =355, domain ="com.au", geo_location="Brisbane,Australia", locale= "en-au", uule = "w+CAIQICIdQnJpc2JhbmUsUXVlZW5zbGFuZCxBdXN0cmFsaWE=",device="desktop"

            },
            new SearchProperties()
            {
                seid =356, domain ="com.au", geo_location="Perth,Australia", locale= "en-au", uule = "w+CAIQICIhUGVydGgsV2VzdGVybiBBdXN0cmFsaWEsQXVzdHJhbGlh",device="desktop"

            },
            new SearchProperties()
            {
                seid =357, domain ="com.au", geo_location="Adelaide,Australia", locale= "en-au", uule = "w+CAIQICIiQWRlbGFpZGUsU291dGggQXVzdHJhbGlhLEF1c3RyYWxpYQ==",device="desktop"

            },
            new SearchProperties()
            {
                seid =358, domain ="com.au", geo_location="Sydney,Australia", locale= "en-au", uule = "w+CAIQICIgU3lkbmV5LE5ldyBTb3V0aCBXYWxlcyxBdXN0cmFsaWE=",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =359, domain ="com.au", geo_location="Melbourne,Australia", locale= "en-au", uule = "w+CAIQICIcTWVsYm91cm5lLFZpY3RvcmlhLEF1c3RyYWxpYQ==",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =360, domain ="com.au", geo_location="Brisbane,Australia", locale= "en-au", uule = "w+CAIQICIdQnJpc2JhbmUsUXVlZW5zbGFuZCxBdXN0cmFsaWE=",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =361, domain ="com.au", geo_location="Perth,Australia", locale= "en-au", uule = "w+CAIQICIhUGVydGgsV2VzdGVybiBBdXN0cmFsaWEsQXVzdHJhbGlh",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =362, domain ="com.au", geo_location="Adelaide,Australia", locale= "en-au", uule = "w+CAIQICIiQWRlbGFpZGUsU291dGggQXVzdHJhbGlhLEF1c3RyYWxpYQ==",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =363, domain ="com.bd", geo_location="Bangladesh", locale= "en-bd", uule =  "w+CAIQICIKQmFuZ2xhZGVzaA",device="desktop"

            },
            new SearchProperties()
            {
                seid =364, domain ="com.bd", geo_location="Bangladesh", locale= "en-bd", uule = "w+CAIQICIKQmFuZ2xhZGVzaA",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =365, domain ="lv", geo_location="Latvia", locale= "lv-lv", uule = "w+CAIQICIGTGF0dmlh",device="desktop"

            },
            new SearchProperties()
            {
                seid =366, domain ="lv", geo_location="Latvia", locale= "lv-lv", uule = "w+CAIQICIGTGF0dmlh",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =367, domain ="com", geo_location="Aspen,Colorado,United States", locale=  "en-us", uule = "w+CAIQICIcQXNwZW4sQ29sb3JhZG8sVW5pdGVkIFN0YXRlcw==",device="desktop"

            },
            new SearchProperties()
            {
                seid =368, domain ="com", geo_location="Aspen,Colorado,United States", locale=  "en-us", uule = "w+CAIQICIcQXNwZW4sQ29sb3JhZG8sVW5pdGVkIFN0YXRlcw==",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =369, domain ="com", geo_location="Napa,California,United States", locale=  "en-us", uule = "w+CAIQICIfbmFwYSwgY2FsaWZvcm5pYSwgdW5pdGVkIHN0YXRlcw==",device="desktop"

            },
            new SearchProperties()
            {
                seid =370, domain ="com", geo_location="Napa,California,United States", locale=  "en-us", uule = "w+CAIQICIfbmFwYSwgY2FsaWZvcm5pYSwgdW5pdGVkIHN0YXRlcw==",device="mobile_android"

            },
             new SearchProperties()
            {
                seid =371, domain ="com.om", geo_location="Oman", locale= "ar-om", uule = "w+CAIQICIET21hbg",device="desktop"

            },
              new SearchProperties()
            {
                seid =372, domain ="com.om", geo_location="Oman", locale= "ar-om", uule = "w+CAIQICIET21hbg",device="mobile_android"

            },
              new SearchProperties()
            {
                seid =373, domain ="com.om", geo_location="Oman", locale= "en-om", uule = "w+CAIQICIET21hbg",device="desktop"

            },
              new SearchProperties()
            {
                seid =374, domain ="com.om", geo_location="Oman", locale= "en-om", uule = "w+CAIQICIET21hbg",device="mobile_android"

            },
            // new SearchProperties()
            //{
            //    seid =381, domain ="co.uk", geo_location="United Kingdom", locale= "en-gb", uule ="w+CAIQICIOVW5pdGVkIEtpbmdkb20=",device="desktop"

            //},
            // new SearchProperties()
            //{
            //    seid =382, domain ="co.uk", geo_location="United Kingdom", locale= "en-gb", uule ="w+CAIQICIOVW5pdGVkIEtpbmdkb20=",device="mobile_android"

            //},
            //  new SearchProperties()
            //{
            //    seid =383, domain ="co.uk", geo_location="United Kingdom", locale= "en-gb", uule ="w+CAIQICIOVW5pdGVkIEtpbmdkb20=",device="mobile_android"
            //},
              new SearchProperties()
            {
                seid =384, domain ="co.uk", geo_location="Hemel Hempstead,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIfSGVtZWwgSGVtcHN0ZWFkLCBVbml0ZWQgS2luZ2RvbQ==",device="desktop"

            },
              new SearchProperties()
            {
                seid =385, domain ="co.uk", geo_location="Hemel Hempstead,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIfSGVtZWwgSGVtcHN0ZWFkLCBVbml0ZWQgS2luZ2RvbQ==",device="mobile_android"

            },
             new SearchProperties()
            {
                seid =386, domain ="co.uk", geo_location="Leicester,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIZTGVpY2VzdGVyLCBVbml0ZWQgS2luZ2RvbQ==",device="desktop"

            },
             new SearchProperties()
            {
                seid =387, domain ="co.uk", geo_location="Leicester,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIZTGVpY2VzdGVyLCBVbml0ZWQgS2luZ2RvbQ==",device="mobile_android"

            },
             new SearchProperties()
            {
                seid =388, domain ="co.uk", geo_location="Nottingham,England,United Kingdom", locale= "en-gb", uule ="w+CAIQICIaTm90dGluZ2hhbSwgVW5pdGVkIEtpbmdkb20=",device="desktop"

            },
             new SearchProperties()
            {
                seid =389, domain ="co.uk", geo_location="Nottingham,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIaTm90dGluZ2hhbSwgVW5pdGVkIEtpbmdkb20=",device="mobile_android"

            },
             new SearchProperties()
            {
                seid =390, domain ="co.uk", geo_location="Portsmouth,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIaUG9ydHNtb3V0aCwgVW5pdGVkIEtpbmdkb20=",device="desktop"

            },
             new SearchProperties()
            {
                seid =391, domain ="co.uk", geo_location="Portsmouth,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIaUG9ydHNtb3V0aCwgVW5pdGVkIEtpbmdkb20=",device="mobile_android"

            },
             new SearchProperties()
            {
                seid =392, domain ="co.uk", geo_location="Reading,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIXUmVhZGluZywgVW5pdGVkIEtpbmdkb20=",device="desktop"

            },

             new SearchProperties()
            {
                seid =393, domain ="co.uk", geo_location="Reading,England,United Kingdom", locale= "en-gb", uule ="w+CAIQICIXUmVhZGluZywgVW5pdGVkIEtpbmdkb20=",device="mobile_android"

            },
             new SearchProperties()
            {
                seid =394, domain ="co.uk", geo_location="Stoke-on-Trent,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIeU3Rva2Utb24tVHJlbnQsIFVuaXRlZCBLaW5nZG9t",device="desktop"

            },
              new SearchProperties()
            {
                seid =395, domain ="co.uk", geo_location="Stoke-on-Trent,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIeU3Rva2Utb24tVHJlbnQsIFVuaXRlZCBLaW5nZG9t",device="mobile_android"

            },
              new SearchProperties()
            {
                seid =396, domain ="co.uk", geo_location="Swansea,Wales,United Kingdom", locale= "en-gb", uule = "w+CAIQICIXU3dhbnNlYSwgVW5pdGVkIEtpbmdkb20=",device="desktop"

            },
              new SearchProperties()
            {
                seid =397, domain ="co.uk", geo_location="Swansea,Wales,United Kingdom", locale= "en-gb", uule = "w+CAIQICIXU3dhbnNlYSwgVW5pdGVkIEtpbmdkb20=",device="mobile_android"

            },
             new SearchProperties()
            {
                seid =398, domain ="com.vn", geo_location="Vietnam", locale= "vi-vn", uule =  "w+CAIQICIHVmlldG5hbQ==",device="mobile_android"

            },
             new SearchProperties()
            {
                seid =399, domain ="com.mm", geo_location="Myanmar", locale= "my-mm", uule = "w+CAIQICIHTXlhbm1hcg==",device="desktop"

            },
              new SearchProperties()
            {
                seid =400, domain ="com.mm", geo_location="Myanmar", locale= "my-mm", uule ="w+CAIQICIHTXlhbm1hcg==",device="mobile_android"

            },
              new SearchProperties()
            {
                seid =403, domain ="ro", geo_location="Romania", locale= "ro-ro", uule = "w+CAIQICIHUm9tYW5pYQ==",device="mobile_android"
            },
              new SearchProperties()
            {
                seid =404, domain ="ch", geo_location="Switzerland", locale= "en-ch", uule = "w+CAIQICILU3dpdHplcmxhbmQ=",device="desktop"
            },
               new SearchProperties()
            {
                seid =405, domain ="ch", geo_location="Switzerland", locale= "en-ch", uule = "w+CAIQICILU3dpdHplcmxhbmQ=",device="mobile_android"
            },
                new SearchProperties()
            {
                seid =406, domain ="com.bh", geo_location="Bahrain", locale= "en-bh", uule = "w+CAIQICIHQmFocmFpbg==",device="mobile_android"

            },
             new SearchProperties()
            {
                seid =407, domain ="com.eg", geo_location="Egypt", locale= "en-eg", uule = "w+CAIQICIFRWd5cHQ=",device="mobile_android"

            },
             new SearchProperties()
            {
                seid =408, domain ="jo", geo_location="Jordan", locale= "en-jo", uule = "w+CAIQICIGSm9yZGFu",device="mobile_android"
            },
             new SearchProperties()
            {
                seid =409, domain ="com.kw", geo_location="Kuwait", locale= "en-kw", uule = "w+CAIQICIGS3V3YWl0",device="mobile_android"

            },
             new SearchProperties()
            {
            seid =410, domain ="com.lb", geo_location="Lebanon", locale= "en-lb", uule = "w+CAIQICIHTGViYW5vbg==",device="mobile_android"
            },
              new SearchProperties()
            {
                seid =411, domain ="com.qa", geo_location="Qatar", locale= "en-qa", uule = "w+CAIQICIFUWF0YXI=",device="mobile_android"

            },
              new SearchProperties()
            {
                seid =412, domain ="com.sa", geo_location="Saudi Arabia", locale= "en-sa", uule = "w+CAIQICIMU2F1ZGkgQXJhYmlh",device="mobile_android"

            },
              new SearchProperties()
            {
                seid =413, domain ="com.bh", geo_location="Bahrain", locale= "en-bh", uule = "w+CAIQICIHQmFocmFpbg==",device="desktop"

            },
             new SearchProperties()
            {
                seid =414, domain ="com.eg", geo_location="Egypt", locale= "en-eg", uule = "w+CAIQICIFRWd5cHQ=",device="desktop"

            },
             new SearchProperties()
            {
                seid =415, domain ="jo", geo_location="Jordan", locale= "en-jo", uule = "w+CAIQICIGSm9yZGFu",device="desktop"
            },
             new SearchProperties()
            {
                seid =416, domain ="com.kw", geo_location="Kuwait", locale= "en-kw", uule = "w+CAIQICIGS3V3YWl0",device="desktop"

            },
             new SearchProperties()
            {
            seid =417, domain ="com.lb", geo_location="Lebanon", locale= "en-lb", uule = "w+CAIQICIHTGViYW5vbg==",device="desktop"
            },
              new SearchProperties()
            {
                seid =418, domain ="com.qa", geo_location="Qatar", locale= "en-qa", uule = "w+CAIQICIFUWF0YXI=",device="desktop"

            },
              new SearchProperties()
            {
                seid =419, domain ="com.sa", geo_location="Saudi Arabia", locale= "en-sa", uule = "w+CAIQICIMU2F1ZGkgQXJhYmlh",device="desktop"

            },
             new SearchProperties()
            {
                seid =420, domain ="com.bh", geo_location="Bahrain", locale= "ar-bh", uule = "w+CAIQICIHQmFocmFpbg==",device="mobile_android"

            },
             new SearchProperties()
            {
                seid =421, domain ="com.eg", geo_location="Egypt", locale= "ar-eg", uule = "w+CAIQICIFRWd5cHQ=",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =422, domain ="jo", geo_location="Jordan", locale= "ar-jo", uule = "w+CAIQICIGSm9yZGFu",device="mobile_android"
            },
            new SearchProperties()
            {
                seid =423, domain ="com.kw", geo_location="Kuwait", locale= "ar-kw", uule = "w+CAIQICIGS3V3YWl0",device="mobile_android"

            },
            new SearchProperties()
            {
            seid =424, domain ="com.lb", geo_location="Lebanon", locale= "ar-lb", uule = "w+CAIQICIHTGViYW5vbg==",device="mobile_android"
            },
            new SearchProperties()
            {
             seid =425, domain ="com.qa", geo_location="Qatar", locale= "ar-qa", uule = "w+CAIQICIFUWF0YXI=",device="mobile_android"

            },
            new SearchProperties()
            {
                seid =426, domain ="co.kr", geo_location="South Korea", locale="ko-kr", uule = "w+CAIQICILU291dGggS29yZWE=",device="mobile_android"
            },
             new SearchProperties()
            {
                seid =427, domain ="co.kr", geo_location="South Korea", locale="ko-kr", uule = "w+CAIQICILU291dGggS29yZWE=",device="desktop"
            },
            new SearchProperties()
            {
            seid =428, domain ="cz", geo_location="Czech Republic", locale = "en-cz", uule = "w+CAIQICIOQ3plY2ggUmVwdWJsaWM=",device="desktop"
            },
            new SearchProperties()
            {
            seid =429, domain ="cz", geo_location="Czech Republic", locale = "en-cz", uule = "w+CAIQICIOQ3plY2ggUmVwdWJsaWM=",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =430, domain ="com.mt", geo_location="Malta", locale = "en-mt", uule = "w+CAIQICIFbWFsdGE=",device="desktop"
            },
            new SearchProperties()
            {
            seid =431, domain ="com.mt", geo_location="Malta", locale = "en-mt", uule = "w+CAIQICIFbWFsdGE=",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =432, domain ="is", geo_location="Iceland", locale = "is-is", uule = "w+CAIQICIHSWNlbGFuZA==",device="desktop"
            },
              new SearchProperties()
            {
            seid =433, domain ="is", geo_location="Iceland", locale = "is-is", uule = "w+CAIQICIHSWNlbGFuZA==",device="mobile_android"
            },
               new SearchProperties()
            {
            seid =434, domain ="is", geo_location="Iceland", locale = "en-is", uule = "w+CAIQICIHSWNlbGFuZA==",device="desktop"
            },
                new SearchProperties()
            {
            seid =435, domain ="is", geo_location="Iceland", locale = "en-is", uule = "w+CAIQICIHSWNlbGFuZA==",device="mobile_android"
            },
            new SearchProperties()
            {
                seid =436, domain ="co.uk", geo_location="Peterborough,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIjUGV0ZXJib3JvdWdoLEVuZ2xhbmQsVW5pdGVkIEtpbmdkb20=",device="desktop"
            },
            new SearchProperties()
            {
                seid =437, domain ="co.uk", geo_location="Peterborough,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIjUGV0ZXJib3JvdWdoLEVuZ2xhbmQsVW5pdGVkIEtpbmdkb20=",device="mobile_android"
            },
            new SearchProperties()
            {
            seid =438, domain ="com.tw", geo_location="Taiwan", locale= "en-tw", uule = "w+CAIQICIGVGFpd2Fu",device="desktop"
            },
            new SearchProperties()
            {
            seid =439, domain ="com.tw", geo_location="Taiwan", locale= "en-tw", uule = "w+CAIQICIGVGFpd2Fu",device="mobile_android"
            },
            new SearchProperties()
            {
                seid =441, domain ="co.uk", geo_location="Newcastle upon Tyne,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIqTmV3Y2FzdGxlIHVwb24gVHluZSxFbmdsYW5kLFVuaXRlZCBLaW5nZG9t", device="desktop"

            },
            new SearchProperties()
            {
                seid =442, domain ="co.uk", geo_location="Newcastle upon Tyne,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIqTmV3Y2FzdGxlIHVwb24gVHluZSxFbmdsYW5kLFVuaXRlZCBLaW5nZG9t", device="mobile_android"

            },
             new SearchProperties()
            {
                seid =443, domain ="co.uk", geo_location="E1,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIZRTEsRW5nbGFuZCxVbml0ZWQgS2luZ2RvbQ==", device="desktop"
            },
            new SearchProperties()
            {
                seid =444, domain ="co.uk", geo_location="E1,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIZRTEsRW5nbGFuZCxVbml0ZWQgS2luZ2RvbQ==", device="mobile_android"
            },

            new SearchProperties()
            {
                seid =445, domain ="co.uk", geo_location="NW1,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIaTlcxLEVuZ2xhbmQsVW5pdGVkIEtpbmdkb20=", device="desktop"
            },
            new SearchProperties()
            {
                seid =446, domain ="co.uk", geo_location="NW1,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIaTlcxLEVuZ2xhbmQsVW5pdGVkIEtpbmdkb20=", device="mobile_android"
            },

            new SearchProperties()
            {
                seid =447, domain ="co.uk", geo_location="SE1,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIaU0UxLEVuZ2xhbmQsVW5pdGVkIEtpbmdkb20=", device="desktop"
            },
            new SearchProperties()
            {
                seid =448, domain ="co.uk", geo_location="SE1,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIaU0UxLEVuZ2xhbmQsVW5pdGVkIEtpbmdkb20=", device="mobile_android"
            },
            new SearchProperties()
            {
                seid =449, domain ="co.uk", geo_location="Aberdeen,Scotland,United Kingdom", locale= "en-gb", uule = "w+CAIQICIgQWJlcmRlZW4sU2NvdGxhbmQsVW5pdGVkIEtpbmdkb20=", device="desktop"

            },
            new SearchProperties()
            {
                seid =450, domain ="co.uk", geo_location="Aberdeen,Scotland,United Kingdom", locale= "en-gb", uule = "w+CAIQICIgQWJlcmRlZW4sU2NvdGxhbmQsVW5pdGVkIEtpbmdkb20=", device="mobile_android"

            },
            new SearchProperties()
            {
                seid =451, domain ="co.uk", geo_location="Bolton,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIdQm9sdG9uLEVuZ2xhbmQsVW5pdGVkIEtpbmdkb20=", device="desktop"

            },
            new SearchProperties()
            {
                seid =452, domain ="co.uk", geo_location="Bolton,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIdQm9sdG9uLEVuZ2xhbmQsVW5pdGVkIEtpbmdkb20=", device="mobile_android"

            },
            new SearchProperties()
            {
                seid =453, domain ="co.uk", geo_location="Bournemouth,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIiQm91cm5lbW91dGgsRW5nbGFuZCxVbml0ZWQgS2luZ2RvbQ==", device="desktop"

            },
            new SearchProperties()
            {
                seid =454, domain ="co.uk", geo_location="Bournemouth,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIiQm91cm5lbW91dGgsRW5nbGFuZCxVbml0ZWQgS2luZ2RvbQ==", device="mobile_android"

            },
            new SearchProperties()
            {
                seid =455, domain ="co.uk", geo_location="Coventry,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIfQ292ZW50cnksRW5nbGFuZCxVbml0ZWQgS2luZ2RvbQ==", device="desktop"

            },
            new SearchProperties()
            {
                seid =456, domain ="co.uk", geo_location="Coventry,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIfQ292ZW50cnksRW5nbGFuZCxVbml0ZWQgS2luZ2RvbQ==", device="mobile_android"

            },
            new SearchProperties()
            {
                seid =457, domain ="co.uk", geo_location="Croydon,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIeQ3JveWRvbixFbmdsYW5kLFVuaXRlZCBLaW5nZG9t", device="desktop"

            },
            new SearchProperties()
            {
                seid =458, domain ="co.uk", geo_location="Croydon,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIeQ3JveWRvbixFbmdsYW5kLFVuaXRlZCBLaW5nZG9t", device="mobile_android"

            },
            new SearchProperties()
            {
                seid =459, domain ="co.uk", geo_location="Northampton,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIiTm9ydGhhbXB0b24sRW5nbGFuZCxVbml0ZWQgS2luZ2RvbQ==", device="desktop"
            },
            new SearchProperties()
            {
                seid =460, domain ="co.uk", geo_location="Northampton,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIiTm9ydGhhbXB0b24sRW5nbGFuZCxVbml0ZWQgS2luZ2RvbQ==", device="mobile_android"
            },
            new SearchProperties()
            {
                seid =461, domain ="co.uk", geo_location="Norwich,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIeTm9yd2ljaCxFbmdsYW5kLFVuaXRlZCBLaW5nZG9t", device="desktop"

            },
            new SearchProperties()
            {
                seid =462, domain ="co.uk", geo_location="Norwich,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIeTm9yd2ljaCxFbmdsYW5kLFVuaXRlZCBLaW5nZG9t", device="mobile_android"

            },
            new SearchProperties()
            {
                seid =463, domain ="co.uk", geo_location="Slough,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIdU2xvdWdoLEVuZ2xhbmQsVW5pdGVkIEtpbmdkb20=", device="desktop"

            },
            new SearchProperties()
            {
                seid =464, domain ="co.uk", geo_location="Slough,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIdU2xvdWdoLEVuZ2xhbmQsVW5pdGVkIEtpbmdkb20=", device="mobile_android"

            },
            new SearchProperties()
            {
                seid =465, domain ="co.uk", geo_location="Watford,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIeV2F0Zm9yZCxFbmdsYW5kLFVuaXRlZCBLaW5nZG9t", device="desktop"

            },
            new SearchProperties()
            {
                seid =466, domain ="co.uk", geo_location="Watford,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIeV2F0Zm9yZCxFbmdsYW5kLFVuaXRlZCBLaW5nZG9t", device="mobile_android"

            },
            new SearchProperties()
            {
                seid =467, domain ="co.uk", geo_location="SW1V,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIbU1cxVixFbmdsYW5kLFVuaXRlZCBLaW5nZG9t", device="desktop"
            },
            new SearchProperties()
            {
                seid =468, domain ="co.uk", geo_location="SW1V,England,United Kingdom", locale= "en-gb", uule = "w+CAIQICIbU1cxVixFbmdsYW5kLFVuaXRlZCBLaW5nZG9t", device="mobile_android"
            },
            new SearchProperties()
            {
                seid =469, domain ="com.cy", geo_location="Cyprus", locale= "EL-CY", uule = "w+CAIQICIGQ3lwcnVz", device="desktop"
            },
            new SearchProperties()
            {
                seid =470, domain ="com.cy", geo_location="Cyprus", locale= "EL-CY", uule = "w+CAIQICIGQ3lwcnVz", device="mobile_android"
            },
            new SearchProperties()
            {
                seid =471, domain ="ee", geo_location="Estonia", locale= "ET-EE", uule = "w+CAIQICIHRXN0b25pYQ==", device="desktop"
            },
            new SearchProperties()
            {
                seid =472, domain ="ee", geo_location="Estonia", locale= "ET-EE", uule = "w+CAIQICIHRXN0b25pYQ==", device="mobile_android"
            },
            new SearchProperties()
            {
                seid =473, domain ="lt", geo_location="Lithuania", locale= "LT-LT", uule = "w+CAIQICIJTGl0aHVhbmlh", device="desktop"
            },
            new SearchProperties()
            {
                seid =474, domain ="lt", geo_location="Lithuania", locale= "LT-LT", uule = "w+CAIQICIJTGl0aHVhbmlh", device="mobile_android"
            }

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
