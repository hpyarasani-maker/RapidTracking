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
            //Console.Title = "Running Delete, Download and Job"; //deleting and downloading remaining keywords from dashboard_sending and dashboard_data
            Console.Title = "Running Delete, Download and Job from Dashboard_Data Table"; //deleting and downloading remaining keywords from dashboard_data
            MissingKeywordsJob.ExecuteMissingKeywordsJob(100).Wait();
            //SeeMore.RunSqlJob(10).Wait();
            //SeeMoreLoop.RunSqlJob(10).Wait();

        }
    }
}
