using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace RapidTrackingResSingleThread
{
    public class Client
    {
        public static string username = "respidatametrics";
        public static string password = "SeEx6#^dwuu#6";
        public string session_id = new Random().Next().ToString();

        /*public Client(string country_iso = null)
        {
            this.Proxy = new WebProxy("pr.oxylabs.io:7777");
            var login = $"customer-" + username + (country_iso != null ? "-cc-" + country_iso : "")
                + "-sessid-" + session_id;
            this.Proxy.Credentials = new NetworkCredential(login, password);
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address) as HttpWebRequest;
            request.ConnectionGroupName = session_id;
            //request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36";
            //request.Method = "GET";
            return request;
        }*/
        public async Task DesktopOverView(string url,string country)
        {
            var login = $"customer-" + username + (country != null ? "-cc-" + country : "")
                + "-sessid-" + session_id;
            //string url = "https://www.google.com/search?q=how+does+temperature+affects+baking&oq=how+does+temperature+affect+baki&gs_lcrp=EgZjaHJvbWUqBwgAEAAYgAQyBwgAEAAYgAQyBwgBEAAYgAQyBggCEEUYOTIICAMQABgWGB4yCAgEEAAYFhgeMggIBRAAGBYYHjIICAYQABgWGB4yBggHEEUYPagCCLACAQ&sourceid=chrome&ie=UTF-8";
            HttpClientHandler handler = new HttpClientHandler
            {
                UseCookies = false,
                Proxy = new WebProxy("pr.oxylabs.io:7777")
                {
                    Credentials = new NetworkCredential(login, password)
                }
            };
            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), url))
                {
                    request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36");
                    request.Headers.TryAddWithoutValidation("Cookie", "HSID=Ahh3tWc-_gTdu1jrO; SSID=AV79k0g2Pnb5gt9KJ; APISID=Sd_gAI3XoIpYVeqX/Ast9_otAhP-HLT4lT; SAPISID=BxyJkerdZfvGGbZt/A6ckqjRo0tUZBT-xc; __Secure-1PAPISID=BxyJkerdZfvGGbZt/A6ckqjRo0tUZBT-xc; __Secure-3PAPISID=BxyJkerdZfvGGbZt/A6ckqjRo0tUZBT-xc; SID=g.a000lAiEFe9Co6bNpg2Spi28T-o0hvzW0hV9_fBlQ1Xf_HzDXdXnA0jtmcHszBuBzzCHBCoyWgACgYKAdwSAQASFQHGX2Mi-grB52elHiLQKAuxzW1JoRoVAUF8yKp7Rvqm6qmzZS8jSnxIK3wU0076; __Secure-1PSID=g.a000lAiEFe9Co6bNpg2Spi28T-o0hvzW0hV9_fBlQ1Xf_HzDXdXnz8Zz740A34cp0T4XOYheiQACgYKAeESAQASFQHGX2Mi-KM0IpLyYin_dwUYn0wTahoVAUF8yKrreuIBEjrTOPDDV7yrcMRI0076; __Secure-3PSID=g.a000lAiEFe9Co6bNpg2Spi28T-o0hvzW0hV9_fBlQ1Xf_HzDXdXng9CKPA3xCnW1ujnMGxbbCAACgYKAfQSAQASFQHGX2MifUcOk5FdbN44bURnH5ux4RoVAUF8yKqJ3c6KY_Yc8KPnEtzqRSDQ0076; __Secure-1PSIDTS=sidts-CjQB3EgAEq47jJMbzwPye5K_DDwPB_kLncDJLkbMx2x04By5P-BcaFF98eQruBFfLAdrsCzuEAA; __Secure-3PSIDTS=sidts-CjQB3EgAEq47jJMbzwPye5K_DDwPB_kLncDJLkbMx2x04By5P-BcaFF98eQruBFfLAdrsCzuEAA; AEC=AQTF6HzICwbbYm05h_Hm5i5l88KlyP6JrKmnZDfB7cPL0jQyNxVMjV7dkg; NID=515=UHTazvY94uh8dABvlKNU7ZF8IuW70SbK93ZNG-s2Q5qhb1MmhdfbTI1JYN_ztnlJ8Jwhr4boq7W7qR1Umfa5ShSZEfnTg-t4be3kGTiUHxcljJP0YY0wHwiUaMPUeGcpHuhPMbF9K2tC9uLwJjY_sbdXXkE2Y4c2CcCAOuwkZOgr5jUMK_6a2RiJNPSLlQx-7xL2k15rdNTQ10JvqgTSEMlSzF-h-SUkNlbwhPwjayxhrVU3scTrKdrMoqX4CbAjcO-ynUjkj8_EclKtSRud6LFTaKtxZfUmjI3WSdyywkIwZ0mmf-OYgWdAc9NgSvNJ6W0235-oSs-kQUdJ0aI-Tbob9rRUwo-dGwVY8fES8WobpyHlAC_Y3kCBcw5OfBf0yb0ENVc7n-iOZQFjTI_F0RCLgvJIF97Yd7FFqi3Jx2ROxslfM_HSDkoWHspHqQRDMTxdwsl2S8_YI2OV5Lp7Nb5nfafHG3yt3Rt0AV57YP5moVovJPIaJLI3TU_alMsDOaamhqJnrQPO1il8Wq6bqZg5YQVVFRnseYZsiDKCwzFlSq9Pgl7wdNZsG4-abSsIK5jck-cZk7g; DV=o3SHY7quR1gk0KXi2U5DAWLhDrcCBdljFs55AWIZeAAAAAA; SIDCC=AKEyXzWDlksJ6httDlgBQxnEX130hsNK5vw68mmSy9t4nlazitjb-XMepGlnYkd6kstc_m3v; __Secure-1PSIDCC=AKEyXzU_SBugm1sjIkj0zvbaVZ9OLxgZVBm6IJb4blyDBi-zlGErSq82BbEb7af1yUtvRDT5; __Secure-3PSIDCC=AKEyXzUbtFSokdTtQNKVsq6kHnL-E7KLjx_nnrcH7XZmzIBbdRzEPzFu60UGE8dut7jtWpL");
                    var response = await httpClient.SendAsync(request);
                    var result = await response.Content.ReadAsStringAsync();
                }
            }
        }
        public async Task MobileOverView(string url, string country)
        {
            var login = $"customer-" + username + (country != null ? "-cc-" + country : "")
                + "-sessid-" + session_id;
            //string url = "https://www.google.com/search?q=how+does+temperature+affects+baking&oq=how+does+temperature+affect+baki&gs_lcrp=EgZjaHJvbWUqBwgAEAAYgAQyBwgAEAAYgAQyBwgBEAAYgAQyBggCEEUYOTIICAMQABgWGB4yCAgEEAAYFhgeMggIBRAAGBYYHjIICAYQABgWGB4yBggHEEUYPagCCLACAQ&sourceid=chrome&ie=UTF-8";
            HttpClientHandler handler = new HttpClientHandler
            {
                UseCookies = false,
                Proxy = new WebProxy("pr.oxylabs.io:7777")
                {
                    Credentials = new NetworkCredential(login, password)
                }
            };
            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), url))
                {
                    request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36");
                    request.Headers.TryAddWithoutValidation("Cookie", "HSID=Ahh3tWc-_gTdu1jrO; SSID=AV79k0g2Pnb5gt9KJ; APISID=Sd_gAI3XoIpYVeqX/Ast9_otAhP-HLT4lT; SAPISID=BxyJkerdZfvGGbZt/A6ckqjRo0tUZBT-xc; __Secure-1PAPISID=BxyJkerdZfvGGbZt/A6ckqjRo0tUZBT-xc; __Secure-3PAPISID=BxyJkerdZfvGGbZt/A6ckqjRo0tUZBT-xc; SID=g.a000lAiEFe9Co6bNpg2Spi28T-o0hvzW0hV9_fBlQ1Xf_HzDXdXnA0jtmcHszBuBzzCHBCoyWgACgYKAdwSAQASFQHGX2Mi-grB52elHiLQKAuxzW1JoRoVAUF8yKp7Rvqm6qmzZS8jSnxIK3wU0076; __Secure-1PSID=g.a000lAiEFe9Co6bNpg2Spi28T-o0hvzW0hV9_fBlQ1Xf_HzDXdXnz8Zz740A34cp0T4XOYheiQACgYKAeESAQASFQHGX2Mi-KM0IpLyYin_dwUYn0wTahoVAUF8yKrreuIBEjrTOPDDV7yrcMRI0076; __Secure-3PSID=g.a000lAiEFe9Co6bNpg2Spi28T-o0hvzW0hV9_fBlQ1Xf_HzDXdXng9CKPA3xCnW1ujnMGxbbCAACgYKAfQSAQASFQHGX2MifUcOk5FdbN44bURnH5ux4RoVAUF8yKqJ3c6KY_Yc8KPnEtzqRSDQ0076; __Secure-1PSIDTS=sidts-CjQB3EgAEq47jJMbzwPye5K_DDwPB_kLncDJLkbMx2x04By5P-BcaFF98eQruBFfLAdrsCzuEAA; __Secure-3PSIDTS=sidts-CjQB3EgAEq47jJMbzwPye5K_DDwPB_kLncDJLkbMx2x04By5P-BcaFF98eQruBFfLAdrsCzuEAA; AEC=AQTF6HzICwbbYm05h_Hm5i5l88KlyP6JrKmnZDfB7cPL0jQyNxVMjV7dkg; NID=515=UHTazvY94uh8dABvlKNU7ZF8IuW70SbK93ZNG-s2Q5qhb1MmhdfbTI1JYN_ztnlJ8Jwhr4boq7W7qR1Umfa5ShSZEfnTg-t4be3kGTiUHxcljJP0YY0wHwiUaMPUeGcpHuhPMbF9K2tC9uLwJjY_sbdXXkE2Y4c2CcCAOuwkZOgr5jUMK_6a2RiJNPSLlQx-7xL2k15rdNTQ10JvqgTSEMlSzF-h-SUkNlbwhPwjayxhrVU3scTrKdrMoqX4CbAjcO-ynUjkj8_EclKtSRud6LFTaKtxZfUmjI3WSdyywkIwZ0mmf-OYgWdAc9NgSvNJ6W0235-oSs-kQUdJ0aI-Tbob9rRUwo-dGwVY8fES8WobpyHlAC_Y3kCBcw5OfBf0yb0ENVc7n-iOZQFjTI_F0RCLgvJIF97Yd7FFqi3Jx2ROxslfM_HSDkoWHspHqQRDMTxdwsl2S8_YI2OV5Lp7Nb5nfafHG3yt3Rt0AV57YP5moVovJPIaJLI3TU_alMsDOaamhqJnrQPO1il8Wq6bqZg5YQVVFRnseYZsiDKCwzFlSq9Pgl7wdNZsG4-abSsIK5jck-cZk7g; DV=o3SHY7quR1gk0KXi2U5DAWLhDrcCBdljFs55AWIZeAAAAAA; SIDCC=AKEyXzWDlksJ6httDlgBQxnEX130hsNK5vw68mmSy9t4nlazitjb-XMepGlnYkd6kstc_m3v; __Secure-1PSIDCC=AKEyXzU_SBugm1sjIkj0zvbaVZ9OLxgZVBm6IJb4blyDBi-zlGErSq82BbEb7af1yUtvRDT5; __Secure-3PSIDCC=AKEyXzUbtFSokdTtQNKVsq6kHnL-E7KLjx_nnrcH7XZmzIBbdRzEPzFu60UGE8dut7jtWpL");
                    var response = await httpClient.SendAsync(request);
                    var result = await response.Content.ReadAsStringAsync();
                }
            }
        }
    }

}