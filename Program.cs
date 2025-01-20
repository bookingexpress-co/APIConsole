
using System;

namespace APIConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string trackNo = "e0094bc1-a5fd-4e79-bfc6-f57d86da7a33";
            int suplId = 21;
            Console.WriteLine("start");
            Console.WriteLine(DateTime.Now.ToString());
            TravayooBO obj = new TravayooBO();

            var path = obj.CreateIfMissing(trackNo);

            obj.Search(trackNo, path, suplId, "Json");
            obj.Search(trackNo, path, 0, "xml");

            obj.Room(trackNo, path, suplId, "Json");
            obj.Room(trackNo, path, 0, "xml");

            obj.PreBook(trackNo, path, suplId, "Json");
            obj.PreBook(trackNo, path, 0, "xml");

            obj.Book(trackNo, path, suplId, "Json");
            obj.Book(trackNo, path, 0, "xml");

            obj.CxlPolicy(trackNo, path, suplId, "xml");

            Console.WriteLine(DateTime.Now.ToString());
            Console.ReadLine();




        }
    }
}
