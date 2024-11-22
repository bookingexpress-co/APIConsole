using APIConsole.Helpers;
using System;
using System.IO;

namespace APIConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string trackNo = "ff67e8ef-75dd-4c03-a736-830681c975ac";
            int suplId = 21;

            TravayooBO obj = new TravayooBO();
            obj.APILog(trackNo, suplId);
            //obj.RoomSearch(trackNo, suplId);
            //obj.Prebook(trackNo, suplId);
            //obj.Book(trackNo, suplId);



        }








    }
}
