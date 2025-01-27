
using System;

namespace APIConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string trackNo = "ab16d572-6069-4e77-b864-4a0ba6784rak-2";
            int suplId = 28;
            Console.WriteLine("start");
            Console.WriteLine(DateTime.Now.ToString());
            Program.supplierlog(suplId, trackNo);

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
            TravayooBO obj = new TravayooBO();
            var path = obj.CreateIfMissing(trackNo);
            obj.CleanDirectory(path);
            obj.APILog(trackNo, path, supplierId, "json");
            obj.APILog(trackNo, path, 0, "xml");
        }

    }
}
