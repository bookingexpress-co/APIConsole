using APIConsole.Helpers;
using System;
using System.IO;

namespace APIConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string trackNo = "6ea80908-efcf-4053-8ff4-2ce1b3e49fac";
            int suplId = 21;
            Console.WriteLine("start");
            Console.WriteLine(DateTime.Now.ToString());
            TravayooBO obj = new TravayooBO();
            var logPath = obj.CreateIfMissing("Case-01");
            obj.APILog(trackNo, suplId, logPath);
            Console.WriteLine(DateTime.Now.ToString());
            Console.ReadLine();
            //Console.r
        }








    }
}
