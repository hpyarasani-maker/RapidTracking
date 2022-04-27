using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapidTrackingFindButton
{
    public class Message
    {

        public string GetSeeMoreText(string seid, string keyword, HtmlDocument doc)
        {
            string msg = string.Empty;
            HtmlNode node = doc.DocumentNode.SelectSingleNode(".//h3/div[@class='GNJvt ipz2Oe']/span[@class='RVQdVd']");
            if (node != null)
            {
                 msg = node.InnerText;
            }
            else
            {
                msg = "No Link button";
            }
            return msg;
        }
    }
}
