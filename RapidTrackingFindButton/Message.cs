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
            var nodes = doc.DocumentNode.SelectNodes(".//div[@class='tF2Cxc']/div/a");
            if (nodes == null)
                nodes = doc.DocumentNode.SelectNodes(".//div[@class='g tF2Cxc']/div/a");

            return nodes.ToString();
        }
    }
}
