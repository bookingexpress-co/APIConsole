
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

            //obj.Room(trackNo, suplId, "Json");
            //obj.Room(trackNo, 0, "xml");
   
            obj.PreBook(trackNo, suplId, "Json");
            obj.PreBook(trackNo, 0, "xml");

            //obj.Book(trackNo, suplId, "Json");
            //obj.Book(trackNo, 0, "xml");

            obj.CxlPolicy(trackNo, 0, "xml");
            
            Console.WriteLine(DateTime.Now.ToString());
            Console.ReadLine();


          

        }
    }
}
