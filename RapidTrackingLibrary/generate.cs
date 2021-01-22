using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace RapidTrackingLibrary
{
    public class Generate
    {
        public void GenerateXml(string seid, string kwd)
        {
            try
            {
                Task<ArrayList> alresult = source.GetHTML(kwd, Convert.ToInt32(seid));

                foreach (string[] src in alresult.Result)
                {
                    string keyword = src[0];
                    JObject obj = JObject.Parse(src[1]);
                    string html = obj["results"][0]["content"].Value<string>();
                    //string jobid = src[2];
                    string device = src[3];
                    //File.WriteAllText(@"C:\inetpub\wwwroot\html\" + jobid + "_" + keyword + ".html", html, Encoding.UTF8);
                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);
                    string res = string.Empty;
                    int count = 0;
                    string resRx = string.Empty;

                    try
                    {
                        if (device == "desktop")
                        {
                            Desktop clsDesktop = new Desktop();
                            res = clsDesktop.ProcessDocument(seid, keyword, doc, out count);
                            resRx = clsDesktop.ProcessClassicLinks(seid, keyword, doc);
                        }
                        else
                        {
                            iOS clsiOS = new iOS();
                            res = clsiOS.ProcessDocument(seid, keyword, doc, out count);
                            resRx = clsiOS.ProcessClassicLinks(seid, keyword, doc);
                        }

                        if (!string.IsNullOrEmpty(res))
                        {
                            if (count > 20)
                            {
                                CreateXml(res, resRx);
                            }
                        }

                    }
                    catch { }
                }
            }
            finally { }
        }

        private void CreateXml(string res, string resRx)
        {
            string xmlPath = @"C:\inetpub\wwwroot\";
            XmlDocument xd = new XmlDocument();
            res = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + res;
            xd.LoadXml(res);
            xd.Save(xmlPath + "xml1.xml");

            resRx = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + resRx;
            xd.LoadXml(resRx);
            xd.Save(xmlPath + "xml2.xml");
        }
    }
}
