
using System;

namespace APIConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string trackNo = "a1500093-08d7-488f-8b76-6786d822fae1";
            int suplId = 21;
            Console.WriteLine("start");
            Console.WriteLine(DateTime.Now.ToString());
            TravayooBO obj = new TravayooBO();
            var logPath = obj.CreateIfMissing("search");
            // obj.duplicateResponse(trackNo, suplId, logPath, "json");
            obj.RhineResponse(trackNo, suplId, logPath, "xml");
            obj.APILog(trackNo, suplId, logPath, "Json");
            Console.WriteLine(DateTime.Now.ToString());
            Console.ReadLine();


            //var hb = new HotelBedService("10017");

            //string v = await hb.GetCountryAsync().ConfigureAwait(false);





        }








    }
}
