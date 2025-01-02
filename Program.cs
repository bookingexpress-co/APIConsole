
using System;

namespace APIConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string trackNo = "963ac701-3c88-443d-a2c7-e430cbee4c57";
            int suplId = 21;
            Console.WriteLine("start");
            Console.WriteLine(DateTime.Now.ToString());
            TravayooBO obj = new TravayooBO();
            var logPath = obj.CreateIfMissing("search");
            //obj.APILog(trackNo, suplId, logPath, "Json");
            obj.SupplierSearchResponse(trackNo, suplId, logPath, "json");
            //obj.RhineResponse(trackNo, suplId, logPath, "xml");

            Console.WriteLine(DateTime.Now.ToString());
            Console.ReadLine();


            //var hb = new HotelBedService("10017");

            //string v = await hb.GetCountryAsync().ConfigureAwait(false);





        }








    }
}
