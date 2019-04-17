using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrowthZone.Reports
{
    class Program
    {
        static void Main(string[] args)
        {
            var report = new ReportSample("http://localtest.me:12221", "admin", "migteam", "TWljcm9OZXRNZW1iZXJab25lQ2xpZW50", "SmhhOThqamRoO2lma2pKVUFTaGRubW5mamFnLTM4OTRrYTcyamJkOzM4");
            var reportData = report.ExecuteReport("ContactReport", 60);

            Console.Write(reportData);

            Console.Read();
        }
    }
}
