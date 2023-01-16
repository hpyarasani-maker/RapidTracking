using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DownloadKeywords
{
    class ExecuteJob
    {

        static void Main(string[] args)
        {
            Console.Title = "Running Delete, Download and Job";
            MissingKeywordsJob.ExecuteMissingKeywordsJob(100).Wait();
            //SeeMore.RunSqlJob(10).Wait();
            //SeeMoreLoop.RunSqlJob(10).Wait();

        }
    }
}
