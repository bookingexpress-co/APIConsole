
using APIConsole.Helpers;
using APIConsole.Models;
using APIConsole.Supplier.Models;
using APIConsole.Supplier.Services;
using System;
using System.Configuration;

namespace APIConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string trackNo = "720a3576-2785-4acf-a975-99dc6492e996";
            int suplId = 28;
            string _customerId = "10017";
            Console.WriteLine("start");
            Console.WriteLine(DateTime.Now.ToString());
            //Program.supplierlog(suplId, trackNo);
            Program.HyperGuestData(_customerId, suplId);
            //TravayooBO obj = new TravayooBO();
            //var path = obj.CreateIfMissing(trackNo);
            //obj.Search(trackNo, path, suplId, "Json");
            //obj.Search(trackNo, path, 0, "xml");
            //obj.Room(trackNo, path, suplId, "Json");
            //obj.Room(trackNo, path, 0, "xml");
            //obj.PreBook(trackNo, path, suplId, "Json");
            //obj.PreBook(trackNo, path, 0, "xml");
            //obj.Book(trackNo, path, suplId, "Json");
            //obj.Book(trackNo, path, 0, "xml");
            //obj.CxlPolicy(trackNo, path, suplId, "xml");
            Console.WriteLine("End");
            Console.WriteLine(DateTime.Now.ToString());
            Console.ReadLine();
        }


        public static void supplierlog(int supplierId, string trackNo)
        {

            string _basePath = CommonHelper.BasePath() + @"\" + ConfigurationManager.AppSettings["ClientLogPath"];
            string _constr = ConfigurationManager.ConnectionStrings["INGMContext"].ConnectionString;
            TravayooBO obj = new TravayooBO(_constr, _basePath);
            var path = obj.CreateIfMissing(trackNo);
            obj.CleanDirectory(path);
            obj.APILog(trackNo, path, supplierId, "json");
            obj.APILog(trackNo, path, 0, "xml");
        }


        public static void HyperGuestData(string customerId, int supplierId = 28)
        {
            var _srv = new HYGSTServices(customerId, ApiAction.Hotels);
            //_srv.GetAllHotels();

            _srv.GetHotelInfo("7459");
        }

    }
}
